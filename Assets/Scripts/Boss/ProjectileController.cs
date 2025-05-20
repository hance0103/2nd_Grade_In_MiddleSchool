using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class ProjectileController : MonoBehaviour
{
    private ObjectPool<GameObject> projectilePool;
    private ProjectileScriptableObject projectileData;
    private Transform playerTransform;
    private GameObject projectilePrefab;
    private bool isEnraged = false;

    private void Awake()
    {
        projectilePool = new ObjectPool<GameObject>(
            createFunc: CreateProjectile,
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            defaultCapacity: 20,
            maxSize: 100
        );
    }

    // 프로젝타일 생성 함수
    private GameObject CreateProjectile()
    {
        GameObject proj = Instantiate(projectilePrefab);
        ProjectileBehaviour behaviour = proj.GetComponent<ProjectileBehaviour>();
        if (behaviour == null)
        {
            behaviour = proj.AddComponent<ProjectileBehaviour>();
        }
        behaviour.Initialize(projectileData.Damage, projectilePool);
        
        return proj;
    }

    public static ProjectileController Create(ProjectileScriptableObject data, Transform bossTransform, Transform player, GameObject prefab, bool enraged)
    {
        GameObject controllerObj = new GameObject("ProjectileController");
        ProjectileController controller = controllerObj.AddComponent<ProjectileController>();
        controller.projectileData = data;
        controller.playerTransform = player;
        controller.projectilePrefab = prefab;
        controller.isEnraged = enraged;
        controllerObj.transform.position = bossTransform.position;
        return controller;
    }

    private IEnumerator MoveProjectile(GameObject projectile, Vector2 direction, List<bool> flagList = null)
    {
        float timer = 0f;

        while (projectile != null && projectile.activeInHierarchy)
        {
            projectile.transform.position += (Vector3)(direction * projectileData.ProjectileSpeed * Time.deltaTime);
            timer += Time.deltaTime;

            yield return null;
        }
        if (flagList != null)
        {
            flagList.Add(true);
        }
    }

    public IEnumerator ExecutePattern(Transform bossTransform)
    {
        float nextFireTime = 0f;
        float verticalSpacing = projectileData.VerticalSpacing;
        int currentRow = 0;  // 시작 위치: 상단
        int direction = 1;   // 1: 하강, -1: 상승
        int projectilesFired = 0;

        // 플레이어 방향 계산
        float targetX = playerTransform.position.x;
        float directionX = (targetX > bossTransform.position.x) ? 1f : -1f;
        float angleToPlayer = (directionX > 0) ? 0f : 180f;
        Debug.Log(angleToPlayer);

        while (projectilesFired < projectileData.ProjectileCount)
        {
            if (Time.time >= nextFireTime)
            {
                Vector3 basePosition = bossTransform.position + new Vector3(0, 1.8f, 0);

                if (isEnraged)
                {
                    //광폭화 패턴: 02 1 02 1 ...
                    if (currentRow == 1)
                    {
                        // Middle row - fire single projectile
                        SpawnProjectile(basePosition, angleToPlayer);
                        projectilesFired++;
                    }
                    else
                    {
                        // Top and bottom rows - fire simultaneously
                        SpawnProjectile(basePosition + new Vector3(0, verticalSpacing, 0), angleToPlayer);
                        SpawnProjectile(basePosition + new Vector3(0, -verticalSpacing, 0), angleToPlayer);
                        projectilesFired += 2;
                    }
                    currentRow = (currentRow == 1) ? 0 : 1;
                }
                else
                {
                    // 기본패턴: 0 1 2 1 0 ...
                    float yOffset = currentRow == 0 ? verticalSpacing :
                                  currentRow == 1 ? 0f : -verticalSpacing;

                    SpawnProjectile(basePosition + new Vector3(0, yOffset, 0), angleToPlayer);
                    projectilesFired++;

                    currentRow += direction;
                    if (currentRow >= 2 || currentRow <= 0)
                    {
                        direction *= -1;
                    }
                }

                nextFireTime = Time.time + (1f / projectileData.FireRate);
            }
            yield return null;
        }

        yield return StartCoroutine(WaitForProjectilesOffScreen());
        yield return new WaitForSeconds(projectileData.AfterFireDelay);
        Destroy(gameObject);
    }

    public IEnumerator ExecuteRadialPattern(Transform bossTransform, float startAngle, float endAngle, int countOffset = 0, List<bool> flagList = null)
    {
        float angleRange = endAngle - startAngle;
        float actualProjectileCount = projectileData.ProjectileCount + countOffset; // countOffset으로 발사 개수 조정
        float angleStep = angleRange / (actualProjectileCount - 1);

        
        List<bool> coroutineFlagList = new();


        // 모든 탄환을 한번에 발사
        for (int i = 0; i < actualProjectileCount; i++)
        {
            Vector3 basePosition = bossTransform.position;
            float angle = startAngle + (i * angleStep); // 200도에서 340도 사이로 발사
            float radians = angle * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(
                Mathf.Cos(radians),
                Mathf.Sin(radians)
            );

            GameObject projectile = projectilePool.Get();
            projectile.transform.parent = bossTransform;
            projectile.transform.position = basePosition;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
            projectile.transform.localScale = projectileData.ProjectileScale;

            StartCoroutine(MoveProjectile(projectile, direction, coroutineFlagList));

        }
        bool isAllCoroutineEnd = false;
        while (!isAllCoroutineEnd)
        {
            if (coroutineFlagList.Count == actualProjectileCount)
                isAllCoroutineEnd = true;
            yield return null;
        }
        if (flagList != null)
            flagList.Add(true);

        yield return new WaitForSeconds(projectileData.AfterFireDelay);

    }

    #region 보스2 패턴5 독비 패턴
    public IEnumerator ExecuteWeakPattern5Rain(Transform bossTransform, float mapWidth, float mapCenter, float safeZoneWidth, float leftBound, float rightBound)
    {
        // 맵을 14개의 구역으로 나눔
        int divisions = 14;
        float sectionWidth = mapWidth / divisions;
        List<GameObject> activeProjectiles = new List<GameObject>();

        for (int iteration = 0; iteration < 5; iteration++)
        {
            // 비활성화된 투사체 제거
            activeProjectiles.RemoveAll(p => p == null || !p.activeInHierarchy);

            #region 안전구역 위치 계산

            // 왼쪽 영역의 안전구역 위치들 계산
            List<float> leftPossibleSafeZones = new List<float>();
            int leftSections = divisions / 2;
            for (int i = 0; i < leftSections; i++)
            {
                float xPos = leftBound + (i * sectionWidth);
                leftPossibleSafeZones.Add(xPos);
            }
            

            // 오른쪽 영역의 안전구역 위치들 계산
            List<float> rightPossibleSafeZones = new List<float>();
            for (int i = 0; i < leftSections; i++)
            {
                float xPos = mapCenter + (i * sectionWidth);
                rightPossibleSafeZones.Add(xPos);
            }
            #endregion

            #region 안전구역 선택 로직
            // 왼쪽 안전구역 2개 선택
            List<float> leftSelectedSafeZones = new List<float>();
            if (leftPossibleSafeZones.Count >= 2)
            {
                // 첫 번째 안전구역 선택
                int firstIndex = Random.Range(0, leftPossibleSafeZones.Count);
                leftSelectedSafeZones.Add(leftPossibleSafeZones[firstIndex]);
                float firstZone = leftPossibleSafeZones[firstIndex];

                leftPossibleSafeZones.RemoveAll(x => Mathf.Abs(x - firstZone) <= safeZoneWidth);

                // 남은 위치들 중에서 두 번째 안전구역 선택
                if (leftPossibleSafeZones.Count > 0)
                {
                    int secondIndex = Random.Range(0, leftPossibleSafeZones.Count);
                    leftSelectedSafeZones.Add(leftPossibleSafeZones[secondIndex]);
                }
            }

            // 오른쪽 안전구역 2개 선택
            List<float> rightSelectedSafeZones = new List<float>();
            if (rightPossibleSafeZones.Count >= 2)
            {
                int firstIndex = Random.Range(0, rightPossibleSafeZones.Count);
                rightSelectedSafeZones.Add(rightPossibleSafeZones[firstIndex]);
                float firstZone = rightPossibleSafeZones[firstIndex];

                rightPossibleSafeZones.RemoveAll(x => Mathf.Abs(x - firstZone) <= safeZoneWidth);

                if (rightPossibleSafeZones.Count > 0)
                {
                    int secondIndex = Random.Range(0, rightPossibleSafeZones.Count);
                    rightSelectedSafeZones.Add(rightPossibleSafeZones[secondIndex]);
                }
            }
            #endregion

            #region safeZoneWidth 간격으로 투사체 생성
            
            // 안전구역 내부가 아닌 곳에만 투사체 생성
            for (float x = leftBound; x <= rightBound; x += safeZoneWidth)
            {
                bool isInSafeZone = false;
                if (x < mapCenter)
                {
                    // 왼쪽 영역에서는 왼쪽 안전구역 체크
                    isInSafeZone = leftSelectedSafeZones.Exists(zone => Mathf.Abs(x - zone) <= safeZoneWidth);
                }
                else
                {
                    // 오른쪽 영역에서는 오른쪽 안전구역 체크
                    isInSafeZone = rightSelectedSafeZones.Exists(zone => Mathf.Abs(x - zone) <= safeZoneWidth);
                }

                // 안전구역이 아닌 경우에만 투사체 생성
                if (!isInSafeZone)
                {
                    try
                    {
                        GameObject projectile = projectilePool.Get();
                        projectile.transform.parent = bossTransform;
                        if (projectile != null)
                        {
                            float offsetY = Random.Range(-1.5f, 1.5f);
                            projectile.SetActive(true);
                            Vector3 spawnPosition = new Vector3(x, bossTransform.position.y + 4f + offsetY, 0);
                            projectile.transform.position = spawnPosition;
                            projectile.transform.rotation = Quaternion.identity;
                            projectile.transform.localScale = projectileData.ProjectileScale;
                            activeProjectiles.Add(projectile);
                            StartCoroutine(MoveRainProjectile(projectile));
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning($"Failed to spawn projectile: {e.Message}");
                    }
                }
            }
            yield return new WaitForSeconds(projectileData.FireRate);
            #endregion
        }

        // 모든 투사체가 사라질 때까지 대기
        while (activeProjectiles.Count > 0)
        {
            activeProjectiles.RemoveAll(p => p == null || !p.activeInHierarchy);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(projectileData.AfterFireDelay);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    #endregion

    public IEnumerator ExecuteContinuousRainPattern(Transform bossTransform, float mapWidth, float startX, float endX, float safeZoneWidth, System.Func<bool> shouldContinue)
    {
        // 맵을 8등분
        int divisions = 8;
        float sectionWidth = mapWidth / divisions;
        List<GameObject> activeProjectiles = new List<GameObject>();

        while (shouldContinue())  // 함수를 호출하여 bool 값 확인
        {
            // 이전 프로젝타일 정리
            activeProjectiles.RemoveAll(p => p == null || !p.activeInHierarchy);

            #region 안전구역 위치 계산

            // 왼쪽 4개 구역의 안전구역 위치들 계산
            List<float> leftPossibleSafeZones = new List<float>();
            int leftSections = divisions / 2; // 4개 구역 (0 ~ 3)
            for (int i = 0; i < leftSections; i++)
            {
                float xPos = startX + (i * sectionWidth);
                leftPossibleSafeZones.Add(xPos);
            }

            // 오른쪽 4개 구역의 안전구역 위치들 계산
            List<float> rightPossibleSafeZones = new List<float>();
            for (int i = 4; i < divisions; i++)
            {
                float xPos = startX + (i * sectionWidth);
                rightPossibleSafeZones.Add(xPos);
            }
            #endregion

            #region 안전구역 선택 로직
            // 왼쪽 안전구역 2개 선택
            List<float> leftSelectedSafeZones = new List<float>();
            if (leftPossibleSafeZones.Count >= 2)
            {
                int firstIndex = Random.Range(0, leftPossibleSafeZones.Count);
                leftSelectedSafeZones.Add(leftPossibleSafeZones[firstIndex]);
                float firstZone = leftPossibleSafeZones[firstIndex];

                leftPossibleSafeZones.RemoveAll(x => Mathf.Abs(x - firstZone) <= safeZoneWidth);

                if (leftPossibleSafeZones.Count > 0)
                {
                    int secondIndex = Random.Range(0, leftPossibleSafeZones.Count);
                    leftSelectedSafeZones.Add(leftPossibleSafeZones[secondIndex]);
                }
            }

            // 오른쪽 안전구역 2개 선택
            List<float> rightSelectedSafeZones = new List<float>();
            if (rightPossibleSafeZones.Count >= 2)
            {
                int firstIndex = Random.Range(0, rightPossibleSafeZones.Count);
                rightSelectedSafeZones.Add(rightPossibleSafeZones[firstIndex]);
                float firstZone = rightPossibleSafeZones[firstIndex];

                rightPossibleSafeZones.RemoveAll(x => Mathf.Abs(x - firstZone) <= safeZoneWidth);

                if (rightPossibleSafeZones.Count > 0)
                {
                    int secondIndex = Random.Range(0, rightPossibleSafeZones.Count);
                    rightSelectedSafeZones.Add(rightPossibleSafeZones[secondIndex]);
                }
            }
            #endregion

            #region 안전구역 간격으로 투사체 생성
            // 안전구역 내부가 아닌 곳에만 투사체 생성
            for (float x = startX; x <= endX; x += sectionWidth)
            {
                bool isInSafeZone = false;
                if (x < startX + sectionWidth * 4)
                {
                    // 왼쪽 구역에서는 왼쪽 안전구역 체크
                    isInSafeZone = leftSelectedSafeZones.Exists(zone => Mathf.Abs(x - zone) <= safeZoneWidth);
                }
                else
                {
                    // 오른쪽 구역에서는 오른쪽 안전구역 체크
                    isInSafeZone = rightSelectedSafeZones.Exists(zone => Mathf.Abs(x - zone) <= safeZoneWidth);
                }

                // 안전구역이 아닌 곳에만 투사체 생성
                if (!isInSafeZone)
                {
                    try
                    {
                        GameObject projectile = projectilePool.Get();
                        if (projectile != null)
                        {
                            float offsetX = Random.Range(-1.5f, 1.5f);
                            float offsetY = Random.Range(-1.5f, 1.5f);
                            projectile.SetActive(true);
                            Vector3 spawnPosition = new Vector3(x + offsetX, bossTransform.position.y + 4f + offsetY, 0);
                            projectile.transform.position = spawnPosition;
                            projectile.transform.rotation = Quaternion.identity;
                            projectile.transform.localScale = projectileData.ProjectileScale;
                            activeProjectiles.Add(projectile);
                            StartCoroutine(MoveRainProjectile(projectile));
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning($"Failed to spawn projectile: {e.Message}");
                    }
                }
            }
            yield return new WaitForSeconds(projectileData.FireRate);
            #endregion
        }

        // 모든 투사체가 사라질 때까지 대기
        while (activeProjectiles.Count > 0)
        {
            activeProjectiles.RemoveAll(p => p == null || !p.activeInHierarchy);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(projectileData.AfterFireDelay);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }


    private IEnumerator MoveRainProjectile(GameObject projectile)
    {
        if (projectile == null) yield break;

        float destroyY = -10f;
        Vector2 moveDirection = Vector2.down;
        float moveSpeed = projectileData.ProjectileSpeed;
        float rotationSpeed = 180f; // 초당 회전 각도 (시계방향은 양수)

        while (projectile != null && projectile.activeInHierarchy)
        {
            if (projectile == null) break;  // 추가 null 체크
            // 아래로 이동
            projectile.transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);

            // 시계방향으로 회전 (Z축 기준 음수 회전이 시계방향)
            projectile.transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);

            if (projectile.transform.position.y <= destroyY)
            {
                if (projectile != null && projectilePool != null)  // 추가 null 체크
                {
                    projectile.SetActive(false);  // 비활성화 후
                    projectilePool.Release(projectile);  // 풀에 반환
                }
                break;
            }

            yield return null;
        }
    }

    private void SpawnProjectile(Vector3 position, float angle)
    {
        GameObject projectile = projectilePool.Get();
        projectile.transform.position = position;
        projectile.transform.rotation = Quaternion.Euler(0, -angle, 0);
        projectile.transform.localScale = projectileData.ProjectileScale;

        // 수평 방향으로만 발사
        Vector2 direction = new Vector2((angle == 0f) ? 1f : -1f, 0f);
        StartCoroutine(MoveProjectile(projectile, direction));
    }

    private IEnumerator WaitForProjectilesOffScreen()
    {
        while (projectilePool.CountActive > 0)
        {
            yield return null;
        }
    }
    #region 안쓰는 OnDestroy
    //private void OnDestroy()
    //{
    //    StopAllCoroutines();
    //    projectilePool.Clear();
    //    foreach (GameObject obj in FindObjectsOfType<GameObject>())
    //    {
    //        if (obj.name.Contains("(Clone)"))
    //        {
    //            Destroy(obj);
    //        }
    //    }
    //}
    #endregion
    private void OnDestroy()
    {
        StopAllCoroutines();
        if (projectilePool != null)
        {
            projectilePool.Clear();
        }
    }
}
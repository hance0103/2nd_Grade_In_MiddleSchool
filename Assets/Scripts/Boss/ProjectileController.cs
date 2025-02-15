using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

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

    private IEnumerator MoveProjectile(GameObject projectile, Vector2 direction)
    {
        float timer = 0f;

        while (projectile != null && projectile.activeInHierarchy)
        {
            projectile.transform.position += (Vector3)(direction * projectileData.ProjectileSpeed * Time.deltaTime);
            timer += Time.deltaTime;

            yield return null;
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

        while (projectilesFired < projectileData.ProjectileCount)
        {
            if (Time.time >= nextFireTime)
            {
                Vector3 basePosition = bossTransform.position + new Vector3(0, 1.5f, 0);

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

    public IEnumerator ExecuteRadialPattern(Transform bossTransform, bool isSecondLayer = false, float countOffset = 0f, bool isWeak4 = false)
    {
        // 발사 각도 제한 (양쪽 끝 제외)
        float angleStart = isSecondLayer ? 205f : 192f; ; // 시작 각도
        float angleEnd = isSecondLayer ? 335f : 348f;   // 끝 각도
        float angleRange = angleEnd - angleStart;
        float angleStep = angleRange / (projectileData.ProjectileCount - 1);

        float actualProjectileCount = projectileData.ProjectileCount - countOffset; // countOffset으로 발사 개수 조정
        float radiusOffset = isSecondLayer ? 1.5f : 0f;

        // 모든 탄환을 한번에 발사
        for (int i = 0; i < actualProjectileCount; i++)
        {
            Vector3 basePosition = bossTransform.position;
            float angle = angleStart + (i * angleStep); // 200도에서 340도 사이로 발사
            float radians = angle * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(
                Mathf.Cos(radians),
                Mathf.Sin(radians)
            );

            if (isSecondLayer)
            {
                basePosition += new Vector3(
                    radiusOffset * Mathf.Cos(radians),
                    radiusOffset * Mathf.Sin(radians),
                    0
                );
            }

            GameObject projectile = projectilePool.Get();
            projectile.transform.position = basePosition;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
            projectile.transform.localScale = projectileData.ProjectileScale;

            StartCoroutine(MoveProjectile(projectile, direction));
        }

        //yield return StartCoroutine(WaitForProjectilesOffScreen());
        yield return new WaitForSeconds(projectileData.AfterFireDelay);
        if (isSecondLayer)
        {
            yield return new WaitForSeconds(projectileData.AfterFireDelay);
        }

        /*if (isWeak4 == false)
        {
            Destroy(gameObject);
        }*/ 
      
    }

    public IEnumerator ExecuteParallelRadialPattern(Transform bossTransform)
    {
        // 두 패턴을 병렬로 실행
        SoundManager.Instance.EffectSoundOn("23-1");
        Coroutine firstLayer = StartCoroutine(ExecuteRadialPattern(bossTransform, false, 0f, true)); // 첫 번째 층
        SoundManager.Instance.EffectSoundOn("23-1");
        Coroutine secondLayer = StartCoroutine(ExecuteRadialPattern(bossTransform, true, 0f, true)); // 두 번째 층

        // 두 코루틴이 모두 끝날 때까지 대기
        yield return firstLayer;
        yield return secondLayer;

        // 이후 패턴의 종료 지연 처리
        yield return new WaitForSeconds(projectileData.AfterFireDelay);
        Destroy(gameObject);
    }


    public IEnumerator ExecuteWeakPattern5Rain(Transform bossTransform, float mapWidth, float mapCenter, float safeZoneWidth, float leftBound, float rightBound)
    {
        // 맵을 14등분
        int divisions = 14;
        float sectionWidth = mapWidth / divisions;
        List<GameObject> activeProjectiles = new List<GameObject>();

        // 5회 반복
        for (int iteration = 0; iteration < 5; iteration++)
        {
            // 이전 프로젝타일 정리
            activeProjectiles.RemoveAll(p => p == null || !p.activeInHierarchy);

            // 왼쪽/오른쪽 랜덤 선택
            bool isLeftSide = Random.value > 0.5f;
            float sideStart = isLeftSide ? leftBound : mapCenter;
            float sideEnd = isLeftSide ? mapCenter : rightBound;

            // 선택된 방향에서 가능한 안전구역 위치들 계산
            List<float> possibleSafeZones = new List<float>();
            int sideSections = divisions / 2;

            for (int i = 0; i < sideSections; i++)
            {
                float xPos = isLeftSide ?
                    leftBound + (i * sectionWidth) :
                    mapCenter + (i * sectionWidth);
                possibleSafeZones.Add(xPos);
            }

            // 2개의 안전구역 랜덤 선택 (최소 간격 보장)
            List<float> selectedSafeZones = new List<float>();
            if (possibleSafeZones.Count >= 2)
            {
                // 첫 번째 안전구역 선택
                int firstIndex = Random.Range(0, possibleSafeZones.Count);
                selectedSafeZones.Add(possibleSafeZones[firstIndex]);
                float firstZone = possibleSafeZones[firstIndex];

                // 첫 번째 안전구역과 인접한 위치 제거
                possibleSafeZones.RemoveAll(x => Mathf.Abs(x - firstZone) <= safeZoneWidth * 2);

                // 두 번째 안전구역 선택
                if (possibleSafeZones.Count > 0)
                {
                    int secondIndex = Random.Range(0, possibleSafeZones.Count);
                    selectedSafeZones.Add(possibleSafeZones[secondIndex]);
                }
            }

            // 비 발사
            for (float x = leftBound; x <= rightBound; x += safeZoneWidth)
            {
                // 선택된 방향의 안전구역이 아닌 곳에만 발사
                bool isInSafeZone = false;
                if (isLeftSide && x < mapCenter)
                {
                    isInSafeZone = selectedSafeZones.Exists(zone => Mathf.Abs(x - zone) <= safeZoneWidth);
                }
                else if (!isLeftSide && x >= mapCenter)
                {
                    isInSafeZone = selectedSafeZones.Exists(zone => Mathf.Abs(x - zone) <= safeZoneWidth);
                }

                if (!isInSafeZone)
                {
                    try
                    {
                        GameObject projectile = projectilePool.Get();
                        if (projectile != null)
                        {
                            projectile.SetActive(true);
                            Vector3 spawnPosition = new Vector3(x, bossTransform.position.y + 10f, 0);
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
        }

        // 모든 프로젝타일이 사라질 때까지 대기
        while (activeProjectiles.Count > 0)
        {
            activeProjectiles.RemoveAll(p => p == null || !p.activeInHierarchy);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(projectileData.AfterFireDelay);
        yield return new WaitForSeconds(1f); // 모든 프로젝타일이 사라질 때까지 추가 대기
        Destroy(gameObject); // 컨트롤러 제거
    }

    public IEnumerator ExecuteContinuousRainPattern(Transform bossTransform, float mapWidth, float safeZoneWidth, System.Func<bool> shouldContinue)
    {
        float startX = -mapWidth / 2;
        float endX = mapWidth / 2;
        float spawnSpacing = safeZoneWidth * 1.5f;
        List<GameObject> activeProjectiles = new List<GameObject>();

        while (shouldContinue())  // 함수를 호출하여 bool 값 확인
        {
            // 이전 프로젝타일 정리
            activeProjectiles.RemoveAll(p => p == null || !p.activeInHierarchy);

            // 독비 발사
            for (float x = startX; x <= endX; x += spawnSpacing)
            {
                if (projectilePool != null)
                {
                    try
                    {
                        GameObject projectile = projectilePool.Get();
                        if (projectile != null)
                        {
                            projectile.SetActive(true);
                            Vector3 spawnPosition = new Vector3(x, bossTransform.position.y + 10f, 0);
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
        }

        // 모든 프로젝타일이 사라질 때까지 대기
        while (activeProjectiles.Count > 0)
        {
            activeProjectiles.RemoveAll(p => p == null || !p.activeInHierarchy);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator MoveRainProjectile(GameObject projectile)
    {
        if (projectile == null) yield break;

        float destroyY = -10f;
        Vector2 moveDirection = Vector2.down;
        float moveSpeed = projectileData.ProjectileSpeed;

        while (projectile != null && projectile.activeInHierarchy)
        {
            if (projectile == null) break;  // 추가 null 체크

            projectile.transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);

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
        //projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
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
    //public IEnumerator ExecuteRainPattern(Transform bossTransform, float mapWidth, float safeZoneWidth, Transform[] safePositions)
    //{
    //    float playerWidth = safeZoneWidth / 1.5f;
    //    float spawnSpacing = playerWidth * 1.5f;

    //    float startX = -mapWidth / 2;
    //    float endX = mapWidth / 2;
    //    float centerX = 0f;

    //    List<GameObject> activeProjectiles = new List<GameObject>();

    //    // 5번 반복
    //    for (int iteration = 0; iteration < 5; iteration++)
    //    {
    //        // 이전 프로젝타일 정리
    //        activeProjectiles.RemoveAll(p => p == null || !p.activeInHierarchy);

    //        // 왼쪽 또는 오른쪽 랜덤 선택
    //        bool isLeftSide = Random.value > 0.5f;
    //        float sideStartX = isLeftSide ? startX : centerX;
    //        float sideEndX = isLeftSide ? centerX : endX;

    //        // 선택된 방향에서 가능한 안전구역 위치들 수집
    //        List<float> possibleSafeZones = new List<float>();
    //        for (float x = sideStartX; x <= sideEndX; x += spawnSpacing)
    //        {
    //            possibleSafeZones.Add(x);
    //        }

    //        // 2개의 안전구역 랜덤 선택 (최소 간격 보장)
    //        List<float> selectedSafeZones = new List<float>();
    //        if (possibleSafeZones.Count >= 2)
    //        {
    //            // 첫 번째 안전구역 선택
    //            int firstIndex = Random.Range(0, possibleSafeZones.Count);
    //            selectedSafeZones.Add(possibleSafeZones[firstIndex]);
    //            float firstZone = possibleSafeZones[firstIndex];

    //            // 첫 번째 안전구역과 너무 가까운 위치 제거
    //            possibleSafeZones.RemoveAll(x => Mathf.Abs(x - firstZone) <= safeZoneWidth * 2);

    //            // 남은 위치 중에서 두 번째 안전구역 선택
    //            if (possibleSafeZones.Count > 0)
    //            {
    //                int secondIndex = Random.Range(0, possibleSafeZones.Count);
    //                selectedSafeZones.Add(possibleSafeZones[secondIndex]);
    //            }
    //        }

    //        Debug.Log($"Iteration {iteration + 1}: Safe zones on {(isLeftSide ? "Left" : "Right")} side at positions: {string.Join(", ", selectedSafeZones)}");

    //        // 비 발사
    //        for (float x = startX; x <= endX; x += spawnSpacing)
    //        {
    //            bool isInSafeZone = false;

    //            // 현재 x가 선택된 방향의 안전구역에 있는지 확인
    //            if (isLeftSide && x < centerX) // 왼쪽이 선택된 경우
    //            {
    //                isInSafeZone = selectedSafeZones.Exists(zone => Mathf.Abs(x - zone) <= safeZoneWidth);
    //            }
    //            else if (!isLeftSide && x > centerX) // 오른쪽이 선택된 경우
    //            {
    //                isInSafeZone = selectedSafeZones.Exists(zone => Mathf.Abs(x - zone) <= safeZoneWidth);
    //            }

    //            if (!isInSafeZone)
    //            {
    //                try
    //                {
    //                    GameObject projectile = projectilePool.Get();
    //                    if (projectile != null)
    //                    {
    //                        projectile.SetActive(true);  // 명시적으로 활성화
    //                        Vector3 spawnPosition = new Vector3(x, bossTransform.position.y + 10f, 0);
    //                        projectile.transform.position = spawnPosition;
    //                        projectile.transform.rotation = Quaternion.identity;
    //                        projectile.transform.localScale = projectileData.ProjectileScale;

    //                        activeProjectiles.Add(projectile);
    //                        StartCoroutine(MoveRainProjectile(projectile));
    //                    }
    //                }
    //                catch (System.Exception e)
    //                {
    //                    Debug.LogWarning($"Failed to spawn projectile: {e.Message}");
    //                }
    //            }
    //        }

    //        yield return new WaitForSeconds(projectileData.FireRate);
    //    }

    //    // 모든 프로젝타일이 사라질 때까지 대기
    //    while (activeProjectiles.Count > 0)
    //    {
    //        activeProjectiles.RemoveAll(p => p == null || !p.activeInHierarchy);
    //        yield return new WaitForSeconds(0.1f);
    //    }

    //    yield return new WaitForSeconds(projectileData.AfterFireDelay);
    //    yield return new WaitForSeconds(1f); // 모든 프로젝타일이 사라질 때까지 추가 대기
    //    Destroy(gameObject); // 컨트롤러 제거
    //}
}
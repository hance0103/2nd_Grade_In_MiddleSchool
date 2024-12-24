using System.Collections;
using System.Collections.Generic;
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
                Vector3 basePosition = bossTransform.position;

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

    private void SpawnProjectile(Vector3 position, float angle)
    {
        GameObject projectile = projectilePool.Get();
        projectile.transform.position = position;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
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

    private void OnDestroy()
    {
        projectilePool.Clear();
    }
}
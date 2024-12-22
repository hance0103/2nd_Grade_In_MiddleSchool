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

    public static ProjectileController Create(ProjectileScriptableObject data, Transform bossTransform, Transform player, GameObject prefab)
    {
        GameObject controllerObj = new GameObject("ProjectileController");
        ProjectileController controller = controllerObj.AddComponent<ProjectileController>();
        controller.projectileData = data;
        controller.playerTransform = player;
        controller.projectilePrefab = prefab;
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

            //Vector3 screenPos = Camera.main.WorldToViewportPoint(projectile.transform.position);
            //if (screenPos.x < -0.1f || screenPos.x > 1.1f || screenPos.y < -0.1f || screenPos.y > 1.1f)
            //{
            //    // 화면 밖으로 벗어나면 투사체 풀로 반환
            //    projectilePool.Release(projectile);
            //    yield break;
            //}

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
                // 기본 위치 설정
                Vector3 basePosition = bossTransform.position;

                // 높이 계산
                float yOffset = 0f;
                switch (currentRow)
                {
                    case 0: // 상단
                        yOffset = verticalSpacing;
                        break;
                    case 1: // 중단
                        yOffset = 0f;
                        break;
                    case 2: // 하단
                        yOffset = -verticalSpacing;
                        break;
                }

                // 발사체 생성 및 발사
                Vector3 spawnPosition = basePosition + new Vector3(0, yOffset, 0);
                SpawnProjectile(spawnPosition, angleToPlayer);

                Debug.Log($"Row: {currentRow}, Direction: {(direction == 1 ? "Down" : "Up")}");

                projectilesFired++;

                // 다음 행 위치 계산 (0->1->2->1->0)
                currentRow += direction;

                // 방향 전환
                if (currentRow >= 2 || currentRow <= 0)
                {
                    direction *= -1;
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
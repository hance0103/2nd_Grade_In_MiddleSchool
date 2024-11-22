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

            Vector3 screenPos = Camera.main.WorldToViewportPoint(projectile.transform.position);
            if (screenPos.x < -0.1f || screenPos.x > 1.1f || screenPos.y < -0.1f || screenPos.y > 1.1f)
            {
                // 화면 밖으로 벗어나면 투사체 풀로 반환
                projectilePool.Release(projectile);
                yield break;
            }

            yield return null;
        }
    }

    public IEnumerator ExecutePattern(Transform bossTransform)
    {
        float nextFireTime = 0f;
        float verticalSpacing = 10f;
        int currentRow = 0;  // 0: 상단, 1: 중단, 2: 하단
        bool isDescending = true;
        int projectilesFired = 0;

        // 패턴 시작 시 플레이어 방향 한 번만 계산
        Vector2 directionToPlayer = (playerTransform.position - bossTransform.position).normalized;
        float angleToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        while (projectilesFired < projectileData.ProjectileCount)
        {
            if (Time.time >= nextFireTime)
            {
                // 지그재그 패턴의 위치 계산
                Vector3 basePosition = bossTransform.position;
                float xOffset = (currentRow % 2) * 2f;

                // 각 행의 높이 설정
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
                Vector3 spawnPosition = basePosition + new Vector3(xOffset, yOffset, 0);
                SpawnProjectile(spawnPosition, angleToPlayer);

                projectilesFired++;

                // 다음 행 위치 계산 (0->1->2->1->0 패턴)
                if (isDescending)
                {
                    currentRow++;
                    if (currentRow == 2)
                    {
                        isDescending = false;
                    }
                }
                else
                {
                    currentRow--;
                    if (currentRow == 0)
                    {
                        isDescending = true;
                    }
                }

                nextFireTime = Time.time + (1f / projectileData.FireRate);
            }
            yield return null;
        }

        // 모든 투사체가 발사되면 마지막 투사체가 화면을 벗어날 때까지 대기
        yield return StartCoroutine(WaitForProjectilesOffScreen());// 투사체가 화면을 완전히 벗어날 때까지의 시간
        yield return new WaitForSeconds(projectileData.AfterFireDelay);
        Destroy(gameObject);
    }


    private void SpawnProjectile(Vector3 position, float angle)
    {
        GameObject projectile = projectilePool.Get();
        projectile.transform.position = position;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
        projectile.transform.localScale = projectileData.ProjectileScale;

        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
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
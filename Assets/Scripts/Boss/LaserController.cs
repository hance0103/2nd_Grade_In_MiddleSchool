using UnityEngine;
using System.Collections;

public class LaserController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private LaserScriptableObject laserData;
    private Camera mainCamera;
    private Transform playerTransform;

    public static LaserController Create(LaserScriptableObject data, Transform boss, Transform player)
    {
        GameObject laserObj = new GameObject("Laser");
        LaserController controller = laserObj.AddComponent<LaserController>();
        controller.Initialize(data, player);
        return controller;
    }

    private void Initialize(LaserScriptableObject data, Transform target)
    {
        mainCamera = Camera.main;
        playerTransform = target;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 0f;
        lineRenderer.endWidth = 0f;

        laserData = data;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = laserData.LaserColor;
        lineRenderer.endColor = laserData.LaserColor;
    }

    private Vector2 GetMapEndPoint(Vector2 startPos, Vector2 direction)
    {
        float vertExtent = mainCamera.orthographicSize;
        float horizExtent = vertExtent * Screen.width / Screen.height;

        Vector2 cameraPos = mainCamera.transform.position;
        Rect mapBounds = new Rect(
            cameraPos.x - horizExtent,
            cameraPos.y - vertExtent,
            horizExtent * 2,
            vertExtent * 2
        );

        float maxDistance = Mathf.Max(mapBounds.width, mapBounds.height) * 2;
        return startPos + (direction * maxDistance);
    }

    // 일반 레이저 발사 (약공격용)
    public IEnumerator FireLaser(Transform bossTransform)
    {
        float fadeInTime = 0.1f;
        float fadeOutTime = 0.1f;

        Vector2 startPosition = bossTransform.position;
        Vector2 direction = (playerTransform.position - bossTransform.position).normalized;
        Vector2 endPosition = GetMapEndPoint(startPosition, direction);

        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);

        // 페이드 인
        float elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            float width = Mathf.Lerp(0, laserData.LaserWidth, elapsed / fadeInTime);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            yield return null;
        }

        // 데미지 체크
        //RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, Vector2.Distance(startPosition, endPosition), laserData.TargetLayer);
        //if (hit.collider != null && hit.collider.CompareTag("Player"))
        //{
        //    PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
        //    if (playerHealth != null)
        //    {
        //        playerHealth.TakeDamage(laserData.Damage);
        //    }
        //}

        // 레이저 지속
        yield return new WaitForSeconds(laserData.LaserDuration);

        // 페이드 아웃
        elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            float width = Mathf.Lerp(laserData.LaserWidth, 0, elapsed / fadeOutTime);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            yield return null;
        }

        Destroy(gameObject);
    }

    // 시간 정지와 함께하는 레이저 발사 (강공격용)
    public IEnumerator FireStrongLaser(Transform bossTransform)
    {
        float fadeInTime = 0.1f;
        float fadeOutTime = 0.1f;

        Vector2 startPosition = bossTransform.position;
        Vector2 direction = (playerTransform.position - bossTransform.position).normalized;
        Vector2 endPosition = GetMapEndPoint(startPosition, direction);

        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);

        // 페이드 인 (실제 시간 기준)
        float elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float width = Mathf.Lerp(0, laserData.LaserWidth, elapsed / fadeInTime);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            yield return null;
        }

        // 데미지 체크
        //RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, Vector2.Distance(startPosition, endPosition), laserData.TargetLayer);
        //if (hit.collider != null && hit.collider.CompareTag("Player"))
        //{
        //    PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
        //    if (playerHealth != null)
        //    {
        //        playerHealth.TakeDamage(laserData.Damage);
        //    }
        //}

        // 레이저 지속 (실제 시간 기준)
        yield return new WaitForSecondsRealtime(laserData.LaserDuration);

        // 페이드 아웃 (실제 시간 기준)
        elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float width = Mathf.Lerp(laserData.LaserWidth, 0, elapsed / fadeOutTime);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            yield return null;
        }

        Destroy(gameObject);
    }
}
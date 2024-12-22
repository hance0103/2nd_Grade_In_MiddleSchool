using UnityEngine;
using System.Collections;

public class LaserController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private LaserScriptableObject laserData;
    private Camera mainCamera;
    private Transform playerTransform;

    public static LaserController Create(LaserScriptableObject data, Vector2 startPosition, Transform player)
    {
        GameObject laserObj = new GameObject("Laser");
        laserObj.transform.position = startPosition; // 시작 위치 설정
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

    // 일반 레이저 발사 (약공격용)
    public IEnumerator FireLaser(Vector2 startPosition, Vector2 staticPlayerPosition)
    {
        float fadeInTime = 0.1f;
        float fadeOutTime = 0.1f;

        Vector2 direction = GetHorizontalDirection(startPosition, staticPlayerPosition);
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

        yield return new WaitForSeconds(laserData.LaserDuration);   // 레이저 지속

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

    public IEnumerator FireStrongLaser(Vector2 startPosition, Vector2 staticPlayerPosition)
    {
        float fadeInTime = 0.1f;
        float fadeOutTime = 0.1f;

        Vector2 direction = GetHorizontalDirection(startPosition, staticPlayerPosition);
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

        yield return new WaitForSecondsRealtime(laserData.LaserDuration);// 레이저 지속 (실제 시간 기준)

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

    public Vector2 GetMapEndPoint(Vector2 startPos, Vector2 direction)
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

    
    public Vector2 GetHorizontalDirection(Vector2 from, Vector2 to)  // 방향을 수평으로만 계산하는 새로운 메서드
    {
        return new Vector2(to.x > from.x ? 1 : -1, 0).normalized;
    }

}
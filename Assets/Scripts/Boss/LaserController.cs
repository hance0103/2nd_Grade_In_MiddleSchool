using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class LaserController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private LaserScriptableObject laserData;
    private Transform playerTransform;
    private LayerMask targetLayer;
    private Animator BossAnimator;
    //[SerializeField] private Animator LaserAnimator;
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
        playerTransform = target;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 0f;
        lineRenderer.endWidth = 0f;

        laserData = data;
        lineRenderer.material = new Material(Shader.Find(laserData.LaserSprite));
        lineRenderer.startColor = laserData.LaserColor;
        lineRenderer.endColor = laserData.LaserColor;
    }

    public IEnumerator FireLaser(Vector2 startPosition, Vector2 playerPosition)
    {
        float fadeInTime = 0.1f;
        float fadeOutTime = 0.1f;

        // 시작점에서 플레이어 방향으로의 벡터 계산
        Vector2 direction = (playerPosition - startPosition).normalized;
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
        float maxDistance = 100f; // 레이저의 최대 사정거리
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, maxDistance, targetLayer);

        if (hit.collider != null)
        {
            // 벽에 부딪힌 지점을 레이저의 끝점으로 사용
            return hit.point;
        }

        // 벽과 충돌하지 않았을 경우 최대 사정거리까지 발사
        return startPos + (direction * maxDistance);
    }


    public Vector2 GetHorizontalDirection(Vector2 from, Vector2 to)  // 방향을 수평으로만 계산하는 새로운 메서드
    {
        return new Vector2(to.x > from.x ? 1 : -1, 0).normalized;
    }

    public IEnumerator FireVerticalLaserWithoutFade(LaserController laser, Vector2 startPosition, Vector2 endPosition, float width)
    {
        if (laser != null)
        {
            LineRenderer lineRenderer = laser.GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                // 레이저 초기 설정
                lineRenderer.startWidth = width;
                lineRenderer.endWidth = width;
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, startPosition);
                lineRenderer.SetPosition(1, startPosition); // 처음엔 시작점에서 멈춤

                // 레이저가 점진적으로 나가는 시간
                float laserMoveDuration = 0.5f; // 0.5초 동안 이동
                float elapsedTime = 0f;

                while (elapsedTime < laserMoveDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float progress = elapsedTime / laserMoveDuration;

                    // 끝점으로 점진적으로 이동
                    Vector2 currentEndPosition = Vector2.Lerp(startPosition, endPosition, progress);
                    lineRenderer.SetPosition(1, currentEndPosition);

                    yield return null; // 다음 프레임까지 대기
                }

                // 이동 완료 후 최종 위치 설정
                lineRenderer.SetPosition(1, endPosition);

                // 초기 알파값 설정
                Color startColor = lineRenderer.startColor;
                Color endColor = lineRenderer.endColor;
                startColor.a = 1f;
                endColor.a = 1f;
                lineRenderer.startColor = startColor;
                lineRenderer.endColor = endColor;
            }
        }
        yield return null;
    }
    public void UpdateLaserPosition(Vector2 startPosition, Vector2 endPosition, float width)
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, endPosition);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }
    }

    public void SetTargetLayer(LayerMask targetLayer)
    {
        this.targetLayer = targetLayer;
    }

}
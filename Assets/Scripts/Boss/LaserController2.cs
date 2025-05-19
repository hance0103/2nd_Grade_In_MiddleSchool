using UnityEngine;
using System.Collections;

public class LaserController2 : MonoBehaviour
{
    private Transform laserTransform;
    private SpriteRenderer spriteRenderer;
    private LaserScriptableObject laserData;
    private Transform playerTransform;
    private LayerMask targetLayer;
    private Animator animator;
    private Rigidbody2D rb;

    public static LaserController2 Create(LaserScriptableObject data, Vector2 startPosition, Transform player)
    {
        GameObject laserObj = new GameObject("Laser");
        laserObj.transform.position = startPosition;

        LaserController2 controller = laserObj.AddComponent<LaserController2>();
        controller.Initialize(data, player);
        return controller;
    }

    public void Initialize(LaserScriptableObject data, Transform target)
    {
        playerTransform = target;
        laserData = data;

        // SpriteRenderer 추가 및 설정
        gameObject.transform.SetParent(transform);
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>(laserData.LaserSprite);
        spriteRenderer.color = laserData.LaserColor;

        // 크기 조절을 위한 Transform 저장
        laserTransform = gameObject.transform;
        laserTransform.localScale = new Vector3(0, laserData.LaserWidth, 1);

        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;

        BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
        boxCollider2D.isTrigger = true;
    }

    public IEnumerator FireLaser(Vector2 startPosition, Vector2 playerPosition)
    {
        float fadeInTime = 0.1f;
        float fadeOutTime = 0.1f;

        // 방향 및 거리 설정
        Vector2 direction = (playerPosition - startPosition).normalized;
        float distance = Vector2.Distance(startPosition, playerPosition);
        transform.position = startPosition;
        transform.right = direction; // 회전

        // 페이드 인
        float elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(0, laserData.LaserWidth, elapsed / fadeInTime);
            laserTransform.localScale = new Vector3(distance, scale, 1);
            yield return null;
        }

        yield return new WaitForSeconds(laserData.LaserDuration);

        // 페이드 아웃
        elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(laserData.LaserWidth, 0, elapsed / fadeOutTime);
            laserTransform.localScale = new Vector3(distance, scale, 1);
            yield return null;
        }

        Destroy(gameObject);
    }

    public IEnumerator FireStrongLaser(Vector2 startPosition, Vector2 staticPlayerPosition)
    {
        float fadeInTime = 0.1f;
        float fadeOutTime = 0.1f;

        Vector2 direction = GetHorizontalDirection(startPosition, staticPlayerPosition);
        float distance = 15f; // 맵 끝까지 길이 설정

        transform.position = startPosition;
        transform.right = direction;

        // 페이드 인
        float elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(0, laserData.LaserWidth, elapsed / fadeInTime);
            laserTransform.localScale = new Vector3(distance, scale, 1);
            yield return null;
        }

        yield return new WaitForSeconds(laserData.LaserDuration);

        // 페이드 아웃
        elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(laserData.LaserWidth, 0, elapsed / fadeOutTime);
            laserTransform.localScale = new Vector3(distance, scale, 1);
            yield return null;
        }

        Destroy(gameObject);
    }

    public Vector2 GetHorizontalDirection(Vector2 from, Vector2 to)
    {
        return new Vector2(to.x > from.x ? 1 : -1, 0).normalized;
    }

    public void UpdateLaserPosition(Vector2 startPosition, Vector2 endPosition, float width)
    {
        if (laserTransform != null)
        {
            transform.position = startPosition;
            transform.right = (endPosition - startPosition).normalized;
            float distance = Vector2.Distance(startPosition, endPosition);
            laserTransform.localScale = new Vector3(distance, width, 1);
        }
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

    public void SetTargetLayer(LayerMask targetLayer)
    {
        this.targetLayer = targetLayer;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerHPManager hpManager= collision.gameObject.GetComponent<PlayerHPManager>();
            hpManager.TakeDamage(laserData.Damage);
        }
    }
    public void DeactivateLaser()
    {
        gameObject.SetActive(false);
    }
}


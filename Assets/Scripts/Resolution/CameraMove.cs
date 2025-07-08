using System;
using System.Collections;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public static CameraMove Instance { get; private set; }
    [Header("Camera Follow")]
    [SerializeField] private float smoothing = 0.2f; // 부드럽게 따라가는 보간 상수
    [SerializeField] private Vector2 minCameraBoundary;
    [SerializeField] private Vector2 maxCameraBoundary;

    [Header("Boss Zoom Event")]
    [SerializeField] private Transform boss;         // 보스 오브젝트의 Transform
    [SerializeField] private float zoomSizeBoss = 3f;    // 보스에게 줌인할 때의 Orthographic Size
    [SerializeField] private float zoomDurationBoss = 0.4f;// 줌인/아웃에 걸리는 시간(초)
    [SerializeField] private float pauseDurationBoss = 5f;// 줌인 상태로 멈춰있는 시간(초)
    [Header("Finish Boss Zoom Event")]
    [SerializeField] private Transform Finishboss;
    [SerializeField] private float zoomSizeBossF = 3f;    // 보스에게 줌인할 때의 Orthographic Size
    [SerializeField] private float zoomDurationBossF = 0.4f;// 줌인/아웃에 걸리는 시간(초)
    [SerializeField] private float pauseDurationBossF = 4f;// 줌인 상태로 멈춰있는 시간(초)
    [SerializeField] private GameObject FinishBossSprite;
    [Header("카메라 흔들기 옵션")]
    public float shakeMagnitude = 0.1f;   // 흔들림 정도
    public float shakeFrequency = 20f;    // 흔들림 빈도(1초당 진동 횟수 정도)

    [Header("Player Zoom Event")]
    [SerializeField] private Transform Player;         // 플레이어 오브젝트의 Transform
    [SerializeField] private float zoomSizePlayer = 3f;    // 플레이어에게 줌인할 때의 Orthographic Size
    [SerializeField] private float zoomDurationPlayer = 1f;// 줌인/아웃에 걸리는 시간(초) 
    [SerializeField] private float minCamY = -2f; // 내려갈 수 있는 최저 높이
    [SerializeField] private float maxCamY = 3f; // 올라갈 수 있는 최고 높이
    [SerializeField] private float minCamX = -5f;    // 왼쪽 끝
    [SerializeField] private float maxCamX = 5f;    // 오른쪽 끝

    private bool isZooming = false;
    private Camera cam;                // 메인 카메라
    private GameObject player;         // 플레이어
    private float originalTimeScale;   // 시간 복원용
    private bool isEnrageEventTriggered = false; // HP 50% 이하 이벤트가 이미 실행됐는지
    private bool isDieEventTriggered = false; // 사망 이벤트가 이미 실행됐는지
    private float originalSize;        // 카메라 원래 Orthographic Size
    private Vector3 originalPos;       // 카메라 원래 위치

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        // 혹시 스크립트가 카메라와 별개 오브젝트에 붙어있다면 밑에처럼 대체할 수 있음
        if (cam == null)
        {
            cam = Camera.main;
        }

        // 최초에 플레이어를 찾는다(만약 Player가 이미 비활성화되어있으면 찾지 못할 수도 있으므로 이후 LateUpdate에서 다시 재시도)
        // 보통은 씬 시작할때 비활성화 되어있어서 지금 당장은 찾지 못함
        player = GameObject.Find("Player");
    }

    private void LateUpdate()
    {
        // ============ 플레이어 활성화 여부 재확인 로직 ============
        // player가 null이면(씬에 없거나 아직 못찾았으면) 다시 찾기 시도
        //찾게 된다면 카메라가 플레이어를 쫒아가는 로직 실행
        if (player == null)
        {
            player = GameObject.Find("Player");
        }
        // 그래도 없다면(또는 아직 비활성화상태라면) 카메라 이동 로직은 생략
        if (player == null || !player.activeInHierarchy)
        {
            // 여기서 return하면 아래 카메라 이동/줌 로직이 실행 안 됨
            return;
        }

        // ============ 보스 HP 50% 이하 체크 → 한 번만 이벤트 실행 ============
        //if (!isEnrageEventTriggered &&
        //    BossHPManager.Instance.GetCurrentHP() <= BossHPManager.Instance.GetMaxHP() * 0.5f)
        //{
        //    isEnrageEventTriggered = true;
        //    StartCoroutine(ZoomToBossCoroutine());
        //}
        // ============ 보스 사망 체크 → 한 번만 이벤트 실행 ============
        //if (!isDieEventTriggered &&
        //    BossHPManager.Instance.GetCurrentHP() == BossHPManager.Instance.GetMaxHP() * 0f)
        //{
        //    isDieEventTriggered = true;
        //    StartCoroutine(ZoomToPlayerCoroutine());
        //}
        // 이벤트 중이면(줌 연출 코루틴 진행중) 평소의 카메라 따라가기 로직 중단
        if (isZooming)
        {
            return;
        }
        if (GameManager.isFinishBossZoominAllowed)
        {
            StartCoroutine(ZoomToFinishBossCoroutine());
        }
        // ============ 평상시 카메라 이동(플레이어 추적) ============
        //광폭화 연출로 줌 이벤트가 끝나면 다시 여기로 돌아오게 됨
        Vector3 targetPos = new Vector3(
            player.transform.position.x,
            player.transform.position.y,
            transform.position.z
        );
        
        // 화면 경계 제한
        targetPos.x = Mathf.Clamp(targetPos.x, minCameraBoundary.x, maxCameraBoundary.x);
        targetPos.y = Mathf.Clamp(targetPos.y, minCameraBoundary.y, maxCameraBoundary.y);

        // 부드럽게 이동 (보간)
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);
    }
    public void EnrageBoss()
    {
        StartCoroutine(ZoomToBossCoroutine());
    }
    private IEnumerator ZoomToBossCoroutine()
    {
        isZooming = true;  // 줌 이벤트 시작
        yield return new WaitForSeconds(0.7f);
        // 1) 게임 시간을 멈춤
        
        Time.timeScale = 0f;

        // 카메라 현 상태 저장
        originalSize = cam.orthographicSize;
        originalPos = transform.position;
        // 보스 스프라이트 원본 스케일
        Vector3 bossOriginalScale = boss.localScale;

        // 보스 위치 (z는 그대로 유지)
        Vector3 bossPos = boss.position;
        bossPos.z = transform.position.z;

        // 2) 카메라 줌인
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / zoomDurationBoss;
            cam.orthographicSize = Mathf.Lerp(originalSize, zoomSizeBoss, t);
            transform.position = Vector3.Lerp(originalPos, bossPos, t);
            

            yield return null;
        }

        //보스 스프라이트 흔들기 (너무 짜치면 없애는걸로...)
        StartCoroutine(DistortBossSpriteCoroutine(pauseDurationBoss));

        // 3) 줌된 상태로 잠시 대기(pauseDuration 초)
        yield return new WaitForSecondsRealtime(pauseDurationBoss);

        // 4) 카메라 원상 복귀 (줌 아웃)
        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / zoomDurationBoss;
            cam.orthographicSize = Mathf.Lerp(zoomSizeBoss, originalSize, t);
            transform.position = Vector3.Lerp(bossPos, originalPos, t);
            yield return null;
        }

        // 5) 시간 복원
        
        Time.timeScale = 1f;
        // 스프라이트 스케일 원상복귀
        boss.localScale = bossOriginalScale;
        // 줌 이벤트 종료
        isZooming = false;
    }
    private IEnumerator DistortBossSpriteCoroutine(float duration) // 보스 Transform으로 왜곡 효과 주기
    {
        Vector3 bossPos = boss.position;
        bossPos.z = transform.position.z;
        float elapsed = 0f;
        Vector3 bossOriginalScale = boss.localScale;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float wave = Mathf.Sin(Time.unscaledTime * 20f) * 0.05f;
            boss.localScale = new Vector3(bossOriginalScale.x * (1 + wave),
                                          bossOriginalScale.y * (1 - wave),
                                          bossOriginalScale.z);
            // (b) 카메라 흔들기
            // 흔들림은 매 프레임마다 약간의 랜덤 오프셋을 추가해 주는 식으로
            // Time.unscaledTime * shakeFrequency 로 흔들 빈도를 조절하거나
            // Random.insideUnitCircle를 쓰는 방법 등 다양하게 가능
            // 여기서는 간단히 sin/cos를 이용한 흔들림 예시
            float shakeX = Mathf.Sin(Time.unscaledTime * shakeFrequency) * shakeMagnitude;
            float shakeY = Mathf.Cos(Time.unscaledTime * shakeFrequency) * shakeMagnitude;
            // 보스 위치 기준으로 흔들린 위치
            transform.position = bossPos + new Vector3(shakeX, shakeY, 0f);
            yield return null;
        }
        // 원상 복구
        boss.localScale = bossOriginalScale;
    }

    private object WaitForSeconds(float v)
    {
        throw new NotImplementedException();
    }
    public void PlayerDie()
    {
        StartCoroutine(ZoomToPlayerCoroutine());
    }
    private IEnumerator ZoomToPlayerCoroutine()
    {
        isZooming = true;
        Time.timeScale = 0f;

        originalSize = cam.orthographicSize;
        originalPos = transform.position;

        Vector3 targetPos = Player.position;
        targetPos.z = originalPos.z;
        targetPos.y = Mathf.Clamp(targetPos.y, minCamY, maxCamY); // Y 하한
        targetPos.x = Mathf.Clamp(targetPos.x, minCamX, maxCamX); // X 범위

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / zoomDurationPlayer;

            cam.orthographicSize = Mathf.Lerp(originalSize, zoomSizePlayer, t);
            transform.position = Vector3.Lerp(originalPos, targetPos, t);

            yield return null;
        }

        while (!GameManager.isPlayerZoomOutAllowed)
            yield return null;

        Vector3 zoomOutStartPos = transform.position;
        zoomOutStartPos.y = Mathf.Max(zoomOutStartPos.y, minCamY);
        zoomOutStartPos.x = Mathf.Clamp(zoomOutStartPos.x, minCamX, maxCamX);

        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / zoomDurationPlayer;

            cam.orthographicSize = Mathf.Lerp(zoomSizePlayer, originalSize, t);
            transform.position = Vector3.Lerp(zoomOutStartPos, originalPos, t);

            yield return null;
        }

        /* 5) 시간 복원 */
        Time.timeScale = 1f;
        isZooming = false;
    }
    private IEnumerator ZoomToFinishBossCoroutine()
    {
        isZooming = true;  // 줌 이벤트 시작

        // 카메라 현 상태 저장
        originalSize = cam.orthographicSize;
        originalPos = transform.position;

        // 보스 위치 (z는 그대로 유지)
        Vector3 bossPos = Finishboss.position;
        bossPos.z = transform.position.z;

        // 카메라 줌인
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / zoomDurationBossF;
            cam.orthographicSize = Mathf.Lerp(originalSize, zoomSizeBossF, t);
            transform.position = Vector3.Lerp(originalPos, bossPos, t);
            yield return null;
        }
        if (FinishBossSprite != null)
            StartCoroutine(FadeOutSprite(FinishBossSprite, 2f));
        // 줌된 상태로 잠시 대기(pauseDuration 초) 이 동안 보스 쓰러지는 애니메이션 재생
        yield return new WaitForSecondsRealtime(pauseDurationBossF);
        // 카메라 원상 복귀 (줌 아웃)
        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / zoomDurationBossF;
            cam.orthographicSize = Mathf.Lerp(zoomSizeBossF, originalSize, t);
            transform.position = Vector3.Lerp(bossPos, originalPos, t);
            yield return null;
        }
        isZooming = false;

        // 줌 이벤트 종료

    }
    IEnumerator FadeOutSprite(GameObject target, float duration)
    {
        
        var sprites = target.GetComponentsInChildren<SpriteRenderer>();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            foreach (var sr in sprites)
            {
                Color c = sr.color;
                sr.color = new Color(c.r, c.g, c.b, alpha);
            }

            yield return null;
        }

        // 완전히 0으로 맞춰주고 비활성화까지
        foreach (var sr in sprites)
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);

        target.SetActive(false);
    }
}

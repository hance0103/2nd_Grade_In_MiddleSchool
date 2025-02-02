using System.Collections;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [Header("Camera Follow")]
    [SerializeField] private float smoothing = 0.2f; // 부드럽게 따라가는 보간 상수
    [SerializeField] private Vector2 minCameraBoundary;
    [SerializeField] private Vector2 maxCameraBoundary;

    [Header("Boss Zoom Event")]
    [SerializeField] private Transform boss;         // 보스 오브젝트의 Transform
    [SerializeField] private float zoomSize = 3f;    // 보스에게 줌인할 때의 Orthographic Size
    [SerializeField] private float zoomDuration = 1f;// 줌인/아웃에 걸리는 시간(초)
    [SerializeField] private float pauseDuration = 2f;// 줌인 상태로 멈춰있는 시간(초)

    private Camera cam;                // 메인 카메라
    private GameObject player;         // 플레이어
    private float originalTimeScale;   // 시간 복원용
    private bool isZooming = false;    // 줌 연출 중인지 여부
    private bool isEventTriggered = false; // HP 50% 이하 이벤트가 이미 실행됐는지
    private float originalSize;        // 카메라 원래 Orthographic Size
    private Vector3 originalPos;       // 카메라 원래 위치

    private void Awake()
    {
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
        if (!isEventTriggered &&
            BossHPManager.Instance.GetCurrentHP() <= BossHPManager.Instance.GetMaxHP() * 0.5f)
        {
            isEventTriggered = true;
            StartCoroutine(ZoomToBossCoroutine());
        }

        // 이벤트 중이면(줌 연출 코루틴 진행중) 평소의 카메라 따라가기 로직 중단
        if (isZooming)
        {
            return;
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

    private IEnumerator ZoomToBossCoroutine()
    {
        isZooming = true;  // 줌 이벤트 시작

        // 1) 게임 시간을 멈춤
        originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        // 카메라 현 상태 저장
        originalSize = cam.orthographicSize;
        originalPos = transform.position;

        // 보스 위치 (z는 그대로 유지)
        Vector3 bossPos = boss.position;
        bossPos.z = transform.position.z;

        // 2) 카메라 줌인
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / zoomDuration;
            cam.orthographicSize = Mathf.Lerp(originalSize, zoomSize, t);
            transform.position = Vector3.Lerp(originalPos, bossPos, t);
            yield return null;
        }

        // 3) 줌된 상태로 잠시 대기(pauseDuration 초)
        yield return new WaitForSecondsRealtime(pauseDuration);

        // 4) 카메라 원상 복귀 (줌 아웃)
        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / zoomDuration;
            cam.orthographicSize = Mathf.Lerp(zoomSize, originalSize, t);
            transform.position = Vector3.Lerp(bossPos, originalPos, t);
            yield return null;
        }

        // 5) 시간 복원
        Time.timeScale = originalTimeScale;
        Time.timeScale = 1f;
        // 줌 이벤트 종료
        isZooming = false;
    }
}

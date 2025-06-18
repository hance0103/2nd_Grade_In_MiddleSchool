using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCamera : MonoBehaviour
{
    private Camera cam;                // 메인 카메라
    private GameObject player;
    [Header("Camera Follow")]
    [SerializeField] private float smoothing = 0; // 부드럽게 따라가는 보간 상수
    [SerializeField] private Vector2 minCameraBoundary;
    [SerializeField] private Vector2 maxCameraBoundary;
    public static ColorCamera Instance { get; private set; }
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
}

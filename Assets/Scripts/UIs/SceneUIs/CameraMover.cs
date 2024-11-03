using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public Vector3 targetPosition = new Vector3(9.5f, 0f, -10f); // 이동할 목표 위치 (Z는 카메라 깊이)
    public float smoothTime = 0.5f;  // 감속 이동에 걸리는 시간
    private Vector3 velocity = Vector3.zero; // 현재 속도 (SmoothDamp에 사용)

    private bool shouldMove = false; // 카메라가 이동 중인지 여부를 체크

    void Update()
    {
        if (shouldMove)
        {
            // SmoothDamp를 사용해 카메라를 목표 위치로 이동
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            // 목표 위치에 거의 도달하면 이동을 멈춤
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                shouldMove = false;
            }
        }
    }

    // 버튼을 통해 호출할 함수
    public void MoveCamera()
    {
        velocity = Vector3.zero; // 새로운 목표로 이동할 때 속도를 초기화
        shouldMove = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public void FireLaser(LaserScriptableObject laserData, Transform bossTransform)
    {
        Debug.Log("레이저 발사 스크립트 실행!");

        // 레이저 발사 위치 설정
        Vector3 startPosition = bossTransform.position + laserData.LaserOffset;
        GameObject laser = new GameObject(laserData.LaserType);
        laser.transform.position = startPosition;

        // TrailRenderer 추가
        TrailRenderer trail = laser.AddComponent<TrailRenderer>();
        trail.time = 0.5f; // 트레일이 남아있는 시간 (이 값을 조정하면 잔상이 얼마나 오래 보일지 결정)
        trail.startWidth = 0.1f; // 트레일의 시작 너비
        trail.endWidth = 0.05f; // 트레일의 끝 너비
        trail.material = new Material(Shader.Find("Sprites/Default")); // 트레일의 머티리얼을 설정 (임시로 기본 머티리얼 사용)

        // 레이저 이동 시작 코루틴
        StartCoroutine(LaserMove(laser, laserData.LaserSpeed, laserData.LaserDuration, trail));
    }

    private IEnumerator LaserMove(GameObject laser, float speed, float duration, TrailRenderer trail)
    {
        float elapsedTime = 0f;
        Vector3 targetPosition = laser.transform.position + Vector3.forward * 10f; // 임의의 목표 지점 설정

        // 레이저의 트레일이 보이도록 설정 (이동할 때마다 트레일을 따라가게 됨)
        while (elapsedTime < duration)
        {
            laser.transform.position = Vector3.MoveTowards(laser.transform.position, targetPosition, speed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 레이저가 목표에 도달한 후, 트레일을 없앰
        Destroy(laser);
    }
}

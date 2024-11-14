using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public GameObject laserPrefab; // 레이저 프리팹을 에디터에서 할당

    public void FireLaser(LaserScriptableObject laserData, Transform bossTransform)
    {
        Debug.Log("레이저 발사 스크립트 실행!");

        // 레이저 발사 위치 설정
        Vector3 startPosition = bossTransform.position + laserData.LaserOffset;

        // 프리팹을 인스턴스화하여 레이저 생성
        GameObject laser = Instantiate(laserPrefab, startPosition, Quaternion.identity);
        laser.name = laserData.LaserType;
        Debug.Log("레이저 생성");

        // 레이저에 TrailRenderer 설정 (프리팹에 설정되어 있다면 생략 가능)
        TrailRenderer trail = laser.GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.time = 2f; // 트레일이 남아있는 시간
            trail.startWidth = 0.1f;
            trail.endWidth = 0.05f;
        }

        // 레이저 이동 시작 코루틴
        StartCoroutine(LaserMove(laser, laserData.LaserSpeed, laserData.LaserDuration, trail));
    }

    private IEnumerator LaserMove(GameObject laser, float speed, float duration, TrailRenderer trail)
    {
        float elapsedTime = 0f;
        Vector3 targetPosition = laser.transform.position + Vector3.forward * 10f; // 임의의 목표 지점 설정

        // 레이저의 트레일이 보이도록 설정
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
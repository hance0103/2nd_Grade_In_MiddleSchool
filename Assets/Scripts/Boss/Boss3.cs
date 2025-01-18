using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Boss3 : MonoBehaviour
{
    enum BossState
    {
        None,
        Idle,
        WeakPattern1,
        WeakPattern2,
        WeakPattern3,
        WeakPattern4,
        WeakPattern5,
        EnragedPattern,
        DesperatePattern1,
        DesperatePattern2,
        DesperatePattern3,
        Groggy,
    }

    enum PatternState
    {
        Teleporting,
        PreAttack,
        Attacking,
        PostAttack,
    }

    private Coroutine currentCoroutine = null;
    private Dictionary<int, BossState[]> patternDic = new();
    private BossState currentState;
    public Player player;
    private LaserController laserController;
    private ProjectileController projectileController;

    [Header("보스 기본 설정")]
    [Tooltip("광폭화 설정")]
    [SerializeField] private bool isEnraged = false; // Inspector에서 설정 가능
    [Tooltip("발악 설정")]
    [SerializeField] private bool isDesperate = false;
    [Tooltip("그로기 시간 설정")]
    [SerializeField] private float groggyTime = 5f;
    [Tooltip("맵 너비 계산")]
    [SerializeField] private Transform[] mapWidthPositions;
  
    [Header("약공격1 데이터")]
    [SerializeField] private BossScriptableObject weakPattern1Data;
    [SerializeField] private LaserScriptableObject weak1LaserData;

    [Header("약공격2 데이터")]
    [SerializeField] private BossScriptableObject weakPattern2Data;
    [SerializeField] private LaserScriptableObject weak2LaserData;

    [Header("약공격3 데이터")]
    [SerializeField] private BossScriptableObject weakPattern3Data;
    [SerializeField] private LaserScriptableObject weak3LaserData;

    [Header("약공격4 데이터")]
    [SerializeField] private BossScriptableObject weakPattern4Data;
    [SerializeField] private LaserScriptableObject weak4LaserData;

    [Header("약공격5 데이터")]
    [SerializeField] private BossScriptableObject weakPattern5Data;
    [SerializeField] private LaserScriptableObject weak5LaserData;

    [Header("광폭화 패턴 데이터")]
    [SerializeField] private BossScriptableObject enragedPatternData;
    [SerializeField] private LaserScriptableObject enragedLaserData;

    [Header("발악패턴1 데이터")]
    [SerializeField] private BossScriptableObject desperatePattern1Data;
    [SerializeField] private LaserScriptableObject desperate1LaserData;

    [Header("발악패턴2 데이터")]
    [SerializeField] private BossScriptableObject desperatePattern2Data;
    [SerializeField] private LaserScriptableObject desperate2LaserData;

    [Header("발악패턴3 데이터")]
    [SerializeField] private BossScriptableObject desperatePattern3Data;
    [SerializeField] private LaserScriptableObject desperate3LaserData;



    void Start()
    {
        patternDic.Add(0, new BossState[] {
            BossState.WeakPattern1,
            BossState.WeakPattern2,
            BossState.WeakPattern3,
            BossState.WeakPattern4,
            BossState.WeakPattern5,
            BossState.EnragedPattern,
            BossState.DesperatePattern1,
            BossState.DesperatePattern2,
            BossState.DesperatePattern3
        });

        StartCoroutine(Idle());
    }

    void Update()
    {
        if (currentState == BossState.WeakPattern1 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(WeakPattern1());
        }
        if (currentState == BossState.WeakPattern2 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(WeakPattern2());
        }
        if (currentState == BossState.WeakPattern3 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(WeakPattern3());
        }
        if (currentState == BossState.WeakPattern4 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(WeakPattern4());
        }
        if (currentState == BossState.WeakPattern5 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(WeakPattern5());
        }
        if (currentState == BossState.EnragedPattern && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(EnragedPattern());
        }
        if (currentState == BossState.DesperatePattern1 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(DesperatePattern1());
        }
        if (currentState == BossState.DesperatePattern2 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(DesperatePattern2());
        }
        if (currentState == BossState.DesperatePattern3 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(DesperatePattern3());
        }
        if (currentState == BossState.Idle && currentCoroutine == null)
        {
            StartCoroutine(Idle());
        }
    }

    public IEnumerator Idle()
    {
        int patternNum = Random.Range(0, patternDic.Count);
        BossState[] currentPattern = patternDic[patternNum];
        for (int i = 0; i < currentPattern.Length; i++)
        {
            currentState = currentPattern[i];
            yield return new WaitUntil(() => currentState == BossState.None);
        }
        yield return null;
    }

    public IEnumerator WeakPattern1()
    {
        Debug.Log("약공격1");
        currentState = BossState.WeakPattern1;
 
        transform.position = new Vector2 (mapWidthPositions[1].position.x, mapWidthPositions[0].position.y);
        FacePlayer();

        float belowBound = mapWidthPositions[0].position.y;
        float topBound = mapWidthPositions[1].position.y;
        float mapHeight = topBound - belowBound;

        // Calculate the height of each layer (3등분)
        float layerHeight = mapHeight / 3f;

        // Define the y-positions for each layer
        float[] layerPositions = new float[3];
        for (int i = 0; i < 3; i++)
        {
            layerPositions[i] = belowBound + (layerHeight * (i + 0.5f)); // Center of each layer
        }

        Vector2 leftPosition = new Vector2(mapWidthPositions[0].position.x - 1, 0);
        Vector2 rightPosition = new Vector2(mapWidthPositions[1].position.x + 1, 0);

        // 레이저 공격 5회 반복
        for (int attackCount = 0; attackCount < 5; attackCount++)
        {
            Debug.Log($"레이저 {attackCount + 1}회 공격 시작");

            // 랜덤하게 층 선택 (1~3층)
            int randomLayer = Random.Range(0, 3);
            float targetY = layerPositions[randomLayer];

            // 시작점과 목표점 설정
            Vector2 startPosition = new Vector2(leftPosition.x, targetY);
            Vector2 targetPosition = new Vector2(rightPosition.x, targetY);

            // 1. 경고선 생성
            LineRenderer warningLine = CreateDangerZone(weak1LaserData);
            StartCoroutine(BlinkDangerZone(warningLine));

            // 경고선 위치 설정
            warningLine.SetPosition(0, startPosition);
            warningLine.SetPosition(1, targetPosition);

            // 발사 전 대기 시간
            yield return new WaitForSeconds(weak1LaserData.LaserLockDuration);
            Destroy(warningLine.gameObject);

            // 2. 레이저 발사
            Debug.Log("레이저 발사!");
            LaserController laser = LaserController.Create(
                weak1LaserData,
                startPosition,
                null  // No target transform as we're using fixed positions
            );
            laser.SetTargetLayer(weak1LaserData.TargetLayer);

            // 레이저 발사
            yield return StartCoroutine(laser.FireLaser(startPosition, targetPosition));

            // 다음 공격 전 대기
            if (attackCount < 4) // 마지막 공격이 아닐 경우에만 대기
            {
                yield return new WaitForSeconds(weak1LaserData.LaserLockDuration);
            }
        }

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern2()
    {
        Debug.Log("약공격2");
        currentState = BossState.WeakPattern2;

        float belowBound = mapWidthPositions[0].position.y - 1;
        float topBound = mapWidthPositions[1].position.y + 1;
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        float centerBound = (leftBound + rightBound) / 2;
        float mapWidth = rightBound - leftBound;

        transform.position = new Vector2(centerBound, belowBound + 1);
        FacePlayer();

        // 7등분 위치 계산 (가로 위치)
        float sectionWidth = mapWidth / 7f;
        float[] sectionPositions = new float[7];
        for (int i = 0; i < 7; i++)
        {
            sectionPositions[i] = leftBound + (sectionWidth * i) + (sectionWidth * 0.5f); // 왼쪽에서 오른쪽으로
        }

        // 시작 위치 랜덤 선택 (왼쪽 or 오른쪽)
        bool startFromLeft = Random.value > 0.5f;
        float startX = startFromLeft ? leftBound : rightBound;

        // 첫 번째 레이저 경고 및 발사
        LineRenderer firstWarningLine = CreateDangerZone(weak2LaserData);
        StartCoroutine(BlinkDangerZone(firstWarningLine));

        firstWarningLine.SetPosition(0, new Vector2(startX, topBound));
        firstWarningLine.SetPosition(1, new Vector2(startX, belowBound));

        yield return new WaitForSeconds(weak2LaserData.LaserLockDuration);

        // 첫 번째 레이저 발사
        LaserController firstLaser = LaserController.Create(
            weak2LaserData,
            new Vector2(startX, topBound),
            null
        );
        firstLaser.SetTargetLayer(weak2LaserData.TargetLayer);
        yield return StartCoroutine(firstLaser.FireLaser(
            new Vector2(startX, topBound),
            new Vector2(startX, belowBound)
        ));

        Destroy(firstWarningLine.gameObject);

        // 7개의 경고선 순차 생성
        //float oppositeX = startFromLeft ? rightBound : leftBound;
        List<LineRenderer> warningLines = new List<LineRenderer>();
        for (int i = 0; i < 7; i++)
        {
            LineRenderer warningLine = CreateDangerZone(weak2LaserData);
            StartCoroutine(BlinkDangerZone(warningLine));

            warningLine.SetPosition(0, new Vector2(sectionPositions[i], topBound));
            warningLine.SetPosition(1, new Vector2(sectionPositions[i], belowBound));

            warningLines.Add(warningLine);
            yield return new WaitForSeconds(0.2f); // 경고선 생성 간격
        }

        // 잠깐의 대기 시간
        yield return new WaitForSeconds(weak2LaserData.LaserLockDuration);

        // 경고선 모두 제거
        foreach (var line in warningLines)
        {
            Destroy(line.gameObject);
        }

        // 7개 레이저 순차 발사 (모두 위에서 아래로)
        for (int i = 0; i < 7; i++)
        {
            LaserController laser = LaserController.Create(
                weak2LaserData,
                new Vector2(sectionPositions[i], topBound),
                null
            );
            laser.SetTargetLayer(weak2LaserData.TargetLayer);

            StartCoroutine(laser.FireLaser(
                new Vector2(sectionPositions[i], topBound),
                new Vector2(sectionPositions[i], belowBound)
            ));

            yield return new WaitForSeconds(0.2f); // 레이저 발사 간격
        }

        // 패턴 종료 대기
        yield return new WaitForSeconds(weak2LaserData.LaserLockDuration);

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern3()
    {
        Debug.Log("약공격3");
        currentState = BossState.WeakPattern3;

        // 총 4번 반복
        for (int attackCount = 0; attackCount < 4; attackCount++)
        {
            Debug.Log($"레이저 {attackCount + 1}회 공격 시작");

            // 두 가지 패턴 중 랜덤 선택 (0: 관통 십자가, 1: 감싸는 십자가)
            bool isPenetratingCross = Random.value > 0.5f;

            // 경고선 4개 생성 (십자가 모양)
            List<LineRenderer> warningLines = new List<LineRenderer>();
            for (int i = 0; i < 4; i++)
            {
                LineRenderer warningLine = CreateDangerZone(weak3LaserData);
                StartCoroutine(BlinkDangerZone(warningLine));
                warningLines.Add(warningLine);
            }

            // 플레이어 추적 단계
            float trackingTime = 0f;
            float trackingDuration = weak3LaserData.LaserFollowDuration;
            Vector2 fixedPosition = Vector2.zero; // 고정될 위치

            while (trackingTime < trackingDuration)
            {
                Vector2 playerPosition = player.transform.position;
                float offset = isPenetratingCross ? 0f : 2f; // 감싸는 십자가일 경우 offset 적용

                // 수직 레이저 경고선 (위, 아래)
                warningLines[0].SetPosition(0, new Vector2(playerPosition.x, playerPosition.y + offset));
                warningLines[0].SetPosition(1, new Vector2(playerPosition.x, playerPosition.y + 10f));

                warningLines[1].SetPosition(0, new Vector2(playerPosition.x, playerPosition.y - offset));
                warningLines[1].SetPosition(1, new Vector2(playerPosition.x, playerPosition.y - 10f));

                // 수평 레이저 경고선 (왼쪽, 오른쪽)
                warningLines[2].SetPosition(0, new Vector2(playerPosition.x + offset, playerPosition.y));
                warningLines[2].SetPosition(1, new Vector2(playerPosition.x + 10f, playerPosition.y));

                warningLines[3].SetPosition(0, new Vector2(playerPosition.x - offset, playerPosition.y));
                warningLines[3].SetPosition(1, new Vector2(playerPosition.x - 10f, playerPosition.y));

                if (trackingTime >= trackingDuration - 0.45f && fixedPosition == Vector2.zero)
                {
                    fixedPosition = playerPosition; // 발사 0.45초 전에 위치 고정
                }

                trackingTime += Time.deltaTime;
                yield return null;
            }

            // 경고선 제거
            foreach (var line in warningLines)
            {
                Destroy(line.gameObject);
            }

            // 레이저 4방향 발사
            float laserOffset = isPenetratingCross ? 0f : 2f;
            Vector2[] laserDirections = new Vector2[]
            {
            Vector2.up,
            Vector2.down,
            Vector2.right,
            Vector2.left
            };

            foreach (Vector2 direction in laserDirections)
            {
                Vector2 startPos = fixedPosition;
                if (!isPenetratingCross)
                {
                    startPos += direction * laserOffset;
                }

                LaserController laser = LaserController.Create(
                    weak3LaserData,
                    startPos,
                    null
                );
                laser.SetTargetLayer(weak3LaserData.TargetLayer);

                StartCoroutine(laser.FireLaser(
                    startPos,
                    startPos + direction * 20f
                ));
            }

            // 다음 공격 전 대기
            yield return new WaitForSeconds(weak3LaserData.LaserLockDuration);
        }

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern4()
    {
        Debug.Log("약공격4");
        currentState = BossState.WeakPattern4;

        // 패턴 구현

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern5()
    {
        Debug.Log("약공격5");
        currentState = BossState.WeakPattern5;

        // 패턴 구현

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator EnragedPattern()
    {
        Debug.Log("광폭화 패턴");
        currentState = BossState.EnragedPattern;

        // 패턴 구현

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator DesperatePattern1()
    {
        Debug.Log("발악패턴1");
        currentState = BossState.DesperatePattern1;

        // 패턴 구현

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator DesperatePattern2()
    {
        Debug.Log("발악패턴2");
        currentState = BossState.DesperatePattern2;

        // 패턴 구현

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator DesperatePattern3()
    {
        Debug.Log("발악패턴3");
        currentState = BossState.DesperatePattern3;

        // 패턴 구현

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator GroggyState()
    {
        Debug.Log("그로기 상태");
        currentState = BossState.Groggy;

        for (float i = groggyTime; i > 0; i--)
        {
            Debug.Log("카운트다운: " + i);
            yield return new WaitForSeconds(1f);
        }

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }
    private LineRenderer CreateDangerZone(LaserScriptableObject laserData)
    {
        GameObject dangerZoneObj = new GameObject("DangerZone");
        LineRenderer lineRenderer = dangerZoneObj.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = laserData.LaserWidth;
        lineRenderer.endWidth = laserData.LaserWidth;

        Color warningColor = new Color(1f, 0f, 0f, 0.5f);
        // 빨간색 반투명 material 설정
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = warningColor;
        lineRenderer.endColor = warningColor;

        return lineRenderer;
    }

    private IEnumerator BlinkDangerZone(LineRenderer dangerZone)
    {
        float blinkSpeed = 0.5f; // 깜빡임 속도

        while (dangerZone != null && dangerZone.gameObject != null) // null 체크 추가
        {
            // 알파값 조절로 깜빡임 효과
            if (dangerZone == null) yield break; // 안전 장치 추가

            // Fade out
            for (float t = 0; t < blinkSpeed; t += Time.deltaTime)
            {
                if (dangerZone == null) yield break; // 안전 장치 추가
                float alpha = Mathf.Lerp(0.5f, 0.1f, t / blinkSpeed);
                dangerZone.startColor = new Color(1f, 0f, 0f, alpha);
                dangerZone.endColor = new Color(1f, 0f, 0f, alpha);
                yield return null;
            }

            // Fade in
            for (float t = 0; t < blinkSpeed; t += Time.deltaTime)
            {
                if (dangerZone == null) yield break; // 안전 장치 추가
                float alpha = Mathf.Lerp(0.1f, 0.5f, t / blinkSpeed);
                dangerZone.startColor = new Color(1f, 0f, 0f, alpha);
                dangerZone.endColor = new Color(1f, 0f, 0f, alpha);
                yield return null;
            }
        }
    }
    private void FacePlayer() // 시선
    {
        if (player != null)
        {
            float direction = transform.position.x - player.transform.position.x;
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x) * (direction > 0 ? -1 : 1),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }
}
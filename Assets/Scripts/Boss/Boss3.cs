using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
   
    [Header("약공격5 데이터")]
    [SerializeField] private BossScriptableObject weakPattern5Data;
    [SerializeField] private LaserScriptableObject weak5LaserData;

    [Header("약공 4, 5 공용 데이터")]
    [SerializeField] private ProjectileScriptableObject weak4ProjData;
    [SerializeField] private GameObject MusicProjectile;

    [Header("광폭화 패턴 데이터")]
    [SerializeField] private BossScriptableObject enragedPatternData;
    [SerializeField] private LaserScriptableObject enragedLaserData;
    [Tooltip("초기 레이저 3개의 페이드인/아웃 속도")]
    [SerializeField] private float initialLasersFadeTime = 0.2f;

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
            //BossState.WeakPattern1,
            //BossState.WeakPattern2,
            //BossState.WeakPattern3,
            //BossState.WeakPattern4,
            //BossState.WeakPattern5,
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

        float bottomBound = mapWidthPositions[0].position.y;
        float topBound = mapWidthPositions[1].position.y;
        float mapHeight = topBound - bottomBound;

        // Calculate the height of each layer (3등분)
        float layerHeight = mapHeight / 3f;

        // Define the y-positions for each layer
        float[] layerPositions = new float[3];
        for (int i = 0; i < 3; i++)
        {
            layerPositions[i] = bottomBound + (layerHeight * (i + 0.5f)); // Center of each layer
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

        float bottomBound = mapWidthPositions[0].position.y - 1;
        float topBound = mapWidthPositions[1].position.y + 1;
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        float centerBound = (leftBound + rightBound) / 2;
        float mapWidth = rightBound - leftBound;

        transform.position = new Vector2(centerBound, bottomBound + 1);
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
        firstWarningLine.SetPosition(1, new Vector2(startX, bottomBound));

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
            new Vector2(startX, bottomBound)
        ));

        Destroy(firstWarningLine.gameObject);

        // 7개의 경고선 순차 생성
        List<LineRenderer> warningLines = new List<LineRenderer>();
        for (int i = 0; i < 7; i++)
        {
            LineRenderer warningLine = CreateDangerZone(weak2LaserData);
            StartCoroutine(BlinkDangerZone(warningLine));

            warningLine.SetPosition(0, new Vector2(sectionPositions[i], topBound));
            warningLine.SetPosition(1, new Vector2(sectionPositions[i], bottomBound));

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
                new Vector2(sectionPositions[i], bottomBound)
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

        float topBound = mapWidthPositions[1].position.y + 1;
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        float centerBound = (leftBound + rightBound) / 2;

        transform.position = new Vector2(centerBound, topBound -4);
        FacePlayer();

        // 총 4번 반복
        for (int attackCount = 0; attackCount < 4; attackCount++)
        {
            Debug.Log($"레이저 {attackCount + 1}회 공격 시작");

            // 랜덤 레이저
            bool isPenetratingCross = Random.value > 0.5f;
            List<LineRenderer> warningLines = new List<LineRenderer>();
            for (int i = 0; i < 4; i++)
            {
                LineRenderer warningLine = CreateDangerZone(weak3LaserData);
                StartCoroutine(BlinkDangerZone(warningLine));
                warningLines.Add(warningLine);
            }

            float trackingTime = 0f;
            float trackingDuration = weak3LaserData.LaserFollowDuration;
            Vector2 fixedPosition = Vector2.zero;
            bool isPositionFixed = false; // 위치 고정 상태를 추적하는 변수 추가

            while (trackingTime < trackingDuration)
            {
                // 추적 중일 때만 플레이어 위치 업데이트
                Vector2 currentPosition = isPositionFixed ? fixedPosition : (Vector2)player.transform.position;
                float offset = isPenetratingCross ? 0f : 2f;

                // 수직 레이저 경고선 (위, 아래)
                warningLines[0].SetPosition(0, new Vector2(currentPosition.x, currentPosition.y + offset));
                warningLines[0].SetPosition(1, new Vector2(currentPosition.x, currentPosition.y + 10f));

                warningLines[1].SetPosition(0, new Vector2(currentPosition.x, currentPosition.y - offset));
                warningLines[1].SetPosition(1, new Vector2(currentPosition.x, currentPosition.y - 10f));

                // 수평 레이저 경고선 (왼쪽, 오른쪽)
                warningLines[2].SetPosition(0, new Vector2(currentPosition.x + offset, currentPosition.y));
                warningLines[2].SetPosition(1, new Vector2(currentPosition.x + 10f, currentPosition.y));

                warningLines[3].SetPosition(0, new Vector2(currentPosition.x - offset, currentPosition.y));
                warningLines[3].SetPosition(1, new Vector2(currentPosition.x - 10f, currentPosition.y));

                // 발사 0.45초 전에 위치 고정
                if (trackingTime >= trackingDuration - 0.45f && !isPositionFixed)
                {
                    fixedPosition = (Vector2)player.transform.position;
                    isPositionFixed = true;
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

        float bottomBound = mapWidthPositions[0].position.y;
        float topBound = mapWidthPositions[1].position.y;
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        float centerBound = (leftBound + rightBound) / 2;

        transform.position = new Vector2(leftBound + 10, bottomBound + 4);
        FacePlayer();

        // 6번 반복
        for (int attackCount = 0; attackCount < 6; attackCount++)
        {
            Debug.Log($"폭발 {attackCount + 1}회 공격 시작");

            // 현재 플레이어 위치 저장
            Vector2 targetPosition = player.transform.position;

            // 경고 표시 생성
            GameObject warningObj = new GameObject($"Warning_{attackCount}");
            SpriteRenderer warningRenderer = warningObj.AddComponent<SpriteRenderer>();

            // 원형 스프라이트 직접 생성
            Texture2D circleTexture = new Texture2D(128, 128);
            for (int y = 0; y < circleTexture.height; y++)
            {
                for (int x = 0; x < circleTexture.width; x++)
                {
                    float dx = x - circleTexture.width / 2;
                    float dy = y - circleTexture.height / 2;
                    float distance = Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = distance < circleTexture.width / 2 ? 1f : 0f;
                    circleTexture.SetPixel(x, y, new Color(1f, 0f, 0f, alpha));
                }
            }
            circleTexture.Apply();

            Sprite circleSprite = Sprite.Create(circleTexture,
                new Rect(0, 0, circleTexture.width, circleTexture.height),
                new Vector2(0.5f, 0.5f));

            // 경고 표시 설정
            warningRenderer.sprite = circleSprite;
            warningRenderer.color = new Color(1f, 0f, 0f, 0.7f); // 더 진한 빨간색
            warningRenderer.transform.position = targetPosition;
            warningRenderer.transform.localScale = new Vector3(4f, 4f, 1f); // 더 큰 경고 크기
            warningRenderer.sortingOrder = 10; // 레이어 순서를 높여서 확실히 보이게 함

            // 경고 표시 깜빡임
            float warningDuration = 1f;
            float currentTime = 0f;

            while (currentTime < warningDuration)
            {
                float alpha = Mathf.PingPong(currentTime * 5f, 0.7f) + 0.3f; // 최소 알파값 증가
                warningRenderer.color = new Color(1f, 0f, 0f, alpha);
                currentTime += Time.deltaTime;
                yield return null;
            }

            // 나머지 코드는 동일...
            Destroy(warningObj);

            // 폭발 프로젝타일 생성
            ProjectileController projectileController = ProjectileController.Create(
                weak4ProjData,
                transform,
                player.transform,
                MusicProjectile,
                false
            );

            GameObject projectile = Instantiate(MusicProjectile, targetPosition, Quaternion.identity);
            ProjectileBehaviour behaviour = projectile.GetComponent<ProjectileBehaviour>();
            if (behaviour == null)
            {
                behaviour = projectile.AddComponent<ProjectileBehaviour>();
            }
            behaviour.Initialize(weak4ProjData.Damage, null);

            float explosionDuration = 0.5f;
            float startScale = 0.5f;
            float endScale = 2f;
            float elapsed = 0f;

            while (elapsed < explosionDuration)
            {
                float scale = Mathf.Lerp(startScale, endScale, elapsed / explosionDuration);
                projectile.transform.localScale = new Vector3(scale, scale, 1f);
                float alpha = 1f - (elapsed / explosionDuration);
                SpriteRenderer projRenderer = projectile.GetComponent<SpriteRenderer>();
                if (projRenderer != null)
                {
                    projRenderer.color = new Color(1f, 1f, 1f, alpha);
                }
                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(projectile);

            if (attackCount < 5)
            {
                yield return new WaitForSeconds(1f);
            }
        }

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern5()
    {
        Debug.Log("약공격5");
        currentState = BossState.WeakPattern5;

        float bottomBound = mapWidthPositions[0].position.y;
        float topBound = mapWidthPositions[1].position.y;
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        float centerBound = (leftBound + rightBound) / 2;

        transform.position = new Vector2(rightBound - 10, bottomBound + 4);
        FacePlayer();

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator EnragedPattern()
    {
        Debug.Log("광폭화 패턴");
        currentState = BossState.EnragedPattern;

        float bottomBound = mapWidthPositions[0].position.y - 1;
        float topBound = mapWidthPositions[1].position.y + 1;
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        float mapWidth = rightBound - leftBound;

        // 맵을 7등분하여 레이저 위치 계산
        float sectionWidth = mapWidth / 7f;
        float[] sectionPositions = new float[7];
        for (int i = 0; i < 7; i++)
        {
            sectionPositions[i] = leftBound + (sectionWidth * i) + (sectionWidth * 0.5f);
        }

        // 가운데 3개의 레이저 생성 (3,4,5번째 구역)
        LaserController[] lasers = new LaserController[3];
        Vector2[] currentPositions = new Vector2[3];

        for (int i = 0; i < 3; i++)
        {
            currentPositions[i] = new Vector2(sectionPositions[i + 2], 0);

            lasers[i] = LaserController.Create(
                enragedLaserData,
                new Vector2(currentPositions[i].x, topBound),
                null
            );
            lasers[i].SetTargetLayer(enragedLaserData.TargetLayer);
        }

        // 초기 레이저 크기로 페이드 인
        float fadeInTime = initialLasersFadeTime;
        float elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            float width = Mathf.Lerp(0, enragedLaserData.LaserWidth, elapsed / fadeInTime);

            for (int i = 0; i < 3; i++)
            {
                lasers[i].UpdateLaserPosition(
                    new Vector2(currentPositions[i].x, topBound),
                    new Vector2(currentPositions[i].x, bottomBound),
                    width
                );
            }
            yield return null;
        }

        // 레이저 이동 (스크립터블 오브젝트의 LaserSpeed 사용)
        bool reachedEdges = false;

        while (!reachedEdges)
        {
            // 왼쪽 레이저 이동
            currentPositions[0].x -= enragedLaserData.LaserSpeed * Time.deltaTime;
            lasers[0].UpdateLaserPosition(
                new Vector2(currentPositions[0].x, topBound),
                new Vector2(currentPositions[0].x, bottomBound),
                enragedLaserData.LaserWidth
            );

            // 가운데 레이저는 고정
            lasers[1].UpdateLaserPosition(
                new Vector2(currentPositions[1].x, topBound),
                new Vector2(currentPositions[1].x, bottomBound),
                enragedLaserData.LaserWidth
            );

            // 오른쪽 레이저 이동
            currentPositions[2].x += enragedLaserData.LaserSpeed * Time.deltaTime;
            lasers[2].UpdateLaserPosition(
                new Vector2(currentPositions[2].x, topBound),
                new Vector2(currentPositions[2].x, bottomBound),
                enragedLaserData.LaserWidth
            );

            // 맵 끝에 도달했는지 확인
            if (currentPositions[0].x <= leftBound && currentPositions[2].x >= rightBound)
            {
                reachedEdges = true;
            }

            yield return null;
        }

        // 페이드 아웃
        float fadeOutTime = initialLasersFadeTime;
        elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            float width = Mathf.Lerp(enragedLaserData.LaserWidth, 0, elapsed / fadeOutTime);

            for (int i = 0; i < 3; i++)
            {
                lasers[i].UpdateLaserPosition(
                    new Vector2(currentPositions[i].x, topBound),
                    new Vector2(currentPositions[i].x, bottomBound),
                    width
                );
            }
            yield return null;
        }

        // 레이저 제거
        foreach (var laser in lasers)
        {
            if (laser != null && laser.gameObject != null)
            {
                Destroy(laser.gameObject);
            }
        }

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
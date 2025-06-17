using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class Boss3 : MonoBehaviour
{
    #region enum 선언
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
    #endregion

    #region 변수 영역
    private Coroutine currentCoroutine = null;
    private Dictionary<int, BossState[]> patternDic = new();
    private bool isDesEnr = false;
    private BossState currentState;
    public GameObject player;
    private LaserController2 laserController;
    private ProjectileController projectileController;
    public bool EndPattern = false;

    [SerializeField]
    private BossState[] currentBossStateArray = null;

    [Header("보스 기본 설정")]
    [Tooltip("광폭화 설정")]
    [SerializeField] private bool isEnraged = false; // Inspector에서 설정 가능
    [Tooltip("발악 설정")]
    [SerializeField] private bool isDesperate = false;
    [Tooltip("그로기 시간 설정")]
    [SerializeField] private float groggyTime = 5f;
    [Header("시작 전 카운트다운")]
    [SerializeField] private float countDownBeforeStart = 2f;
    [Tooltip("맵 너비 계산")]
    [SerializeField] private Transform[] mapWidthPositions;
    [Tooltip("약공격 5 위치1")]
    [SerializeField] private Transform[] pattern51Positions;
    [Tooltip("약공격 5 위치2")]
    [SerializeField] private Transform[] pattern52Positions;
    [Tooltip("약공격 5 위치3")]
    [SerializeField] private Transform[] pattern53Positions;
    [Tooltip("약공격 5 위치4")]
    [SerializeField] private Transform[] pattern54Positions;

    [Header("약공격1 데이터")]
    [SerializeField] private BossScriptableObject weakPattern1Data;
    [SerializeField] private LaserScriptableObject weak1LaserData;

    [Header("약공격2 데이터")]
    [SerializeField] private BossScriptableObject weakPattern2Data;
    [SerializeField] private LaserScriptableObject weak2LaserData;
    [SerializeField]
    private float _wp2LaserDelay;

    [Header("약공격3 데이터")]
    [SerializeField] private BossScriptableObject weakPattern3Data;
    [SerializeField] private LaserScriptableObject weak3LaserData;
    [SerializeField] private LaserScriptableObject weak3Laser_E_Data;

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
    [SerializeField] private ProjectileScriptableObject desperate2ProjData;

    [Header("발악패턴3 데이터")]
    [SerializeField] private BossScriptableObject desperatePattern3Data;
    [SerializeField] private LaserScriptableObject desperate3LaserData;


    private BossHPManager bossHPManager; // BossHPManager ����
    private bool shouldTriggerEnrage = false;
    private bool isEnrageTriggered = false;
    private bool isDead = false;
    Animator animator;
    #endregion


    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        if (isEnraged == true)
            animator.SetBool("isEnraged", true);

        patternDic.Add(0, new BossState[] {
            //BossState.WeakPattern1,
            //BossState.WeakPattern2,
            BossState.WeakPattern3,
            //BossState.WeakPattern4,
            //BossState.WeakPattern5,
            //BossState.EnragedPattern,
            //BossState.DesperatePattern1,
            //BossState.DesperatePattern2,
            //BossState.DesperatePattern3
        });

        StartCoroutine(BeforeIdle());
    }
    [Header("광폭화 팝업")]
    [SerializeField] private GameObject BossEnragePopup;
    [SerializeField] private BossEnragePopup BossEnragePopupScript;

    private bool Enrageactive = true;
    private void BossEnrage()
    {
        player.GetComponent<PlayerController>().PlayerStop();
        BossEnragePopup.SetActive(true);
        BossEnragePopupScript.OnEnrage();
    }
    void Update()
    {
        // 사망 조건 체크 - 최우선으로 처리
        if (BossHPManager.Instance.GetCurrentHP() <= 0 && !isDead)
        {
            isDead = true;
            StartCoroutine(DeathEffect());
            return; // 다른 업데이트 로직 실행 방지
        }
        if (Enrageactive && BossHPManager.Instance.GetCurrentHP() <= BossHPManager.Instance.GetMaxHP() * 0.5f && EndPattern)
        {
            Debug.Log(EndPattern);
            Enrageactive = false;
            BossEnrage();
        }
        #region 보스 상태 체크
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
            if (currentBossStateArray == null)
            {
                Debug.Log("새로운 패턴 리스트 배정");
                StartCoroutine(Idle());
            }
        }
        #endregion

        #region 광폭화 체크
        if (!isEnraged && BossHPManager.Instance.GetCurrentHP() <= BossHPManager.Instance.GetMaxHP() * 0.5f)
        {
            isEnraged = true;
            //animator.SetBool("isEnraged", true);
            // ����ȭ ȿ��
        }

        // 광폭화 조건 확인 - 체력이 50% 이하일 때
        if (!isEnraged && !isEnrageTriggered && BossHPManager.Instance.GetCurrentHP() <= BossHPManager.Instance.GetMaxHP() * 0.5f)
        {
            isEnrageTriggered = true; // 한 번만 트리거되도록 설정
            shouldTriggerEnrage = true;
            Debug.Log("광폭화 준비됨: 현재 패턴 완료 후 광폭화 시작");
        }

        // 현재 패턴 상태가 None으로 변경되었을 때(패턴이 완료됨) 광폭화 체크
        if (shouldTriggerEnrage && currentState == BossState.None && currentCoroutine == null)
        {
            shouldTriggerEnrage = false;
            StartCoroutine(EnrageEffect(transform.position, player.transform.position));
        }
        #endregion

    }
    private const float PATTERN_GAP = 0.3f;
    private bool isDelayed = false;
    private IEnumerator FinishPattern()
    {
        EndPattern = true;
        yield return new WaitForSeconds(PATTERN_GAP);

        currentState = BossState.None;
        currentCoroutine = null;
    }
    public IEnumerator Idle()
    {
        int patternNum = Random.Range(0, patternDic.Count);
        currentBossStateArray = patternDic[patternNum];
        for (int i = 0; i < currentBossStateArray.Length; i++)
        {
            yield return new WaitForSeconds(PATTERN_GAP);
            currentState = currentBossStateArray[i];
            yield return new WaitUntil(() => currentState == BossState.None); // 패턴이 모두 실행되길 기다림

            currentState = BossState.Idle; // Idle에서 다시 새로운 패턴 받아오기
            currentCoroutine = null; // Idle 실행 조건
            if (BossHPManager.Instance.GetCurrentHP() <= BossHPManager.Instance.GetMaxHP() * 0.5f && !isDelayed)
            {
                yield return new WaitForSeconds(2f);
                isDelayed = true;
            }
        }
        currentBossStateArray = null;
    }
    public IEnumerator BeforeIdle()
    {
        // ī��Ʈ�ٿ�
        yield return new WaitForSeconds(countDownBeforeStart);

        int patternNum = Random.Range(0, patternDic.Count);
        currentBossStateArray = patternDic[patternNum];
        for (int i = 0; i < currentBossStateArray.Length; i++)
        {
            currentState = currentBossStateArray[i];
            yield return new WaitUntil(() => currentState == BossState.None); // currentState�� None�� �Ǳ� ������ ����
            currentState = BossState.Idle;
            currentCoroutine = null; // �̰� ������ �����ؼ� update������ ����� �����ϵ���
        }

        currentBossStateArray = null;
    }

    #region 약공격1
    public IEnumerator WeakPattern1()
    {
        yield return new WaitForSeconds(0.5f);
        EndPattern = false;
        Debug.Log(isEnraged ? "약공격1 - 광폭화" : "약공격1 - 기본");
        currentState = BossState.WeakPattern1;

        transform.position = new Vector2(mapWidthPositions[1].position.x, mapWidthPositions[0].position.y + 2f);
        FacePlayer();

        #region 맵 데이터
        float bottomBound = mapWidthPositions[0].position.y;
        float topBound = mapWidthPositions[1].position.y;
        float mapHeight = topBound - bottomBound;
        float layerHeight = mapHeight / 3f;
        #endregion

        float[] layerPositions = new float[3];
        for (int i = 0; i < 3; i++)
        {
            layerPositions[i] = bottomBound + (layerHeight * (i + 0.5f));
        }

        Vector2 leftPosition = new Vector2(mapWidthPositions[0].position.x - 15, 0);
        Vector2 rightPosition = new Vector2(mapWidthPositions[1].position.x + 15, 0);

        int attackCount = isEnraged ? 4 : 3;
        if (isDesEnr == true)
        {
            attackCount = 7;
        }

        for (int i = 0; i < attackCount; i++)
        {
            #region 강화약공1
            if (isEnraged)
            {
                List<int> availableLayers = new List<int> { 0, 1, 2 };
                int firstLayer = availableLayers[Random.Range(0, availableLayers.Count)];
                availableLayers.Remove(firstLayer);
                int secondLayer = availableLayers[Random.Range(0, availableLayers.Count)];

                int[] selectedLayers = { firstLayer, secondLayer };
                List<LineRenderer> warningLines = new List<LineRenderer>();

                foreach (int layer in selectedLayers)
                {
                    float targetY = layerPositions[layer];
                    Vector2 startPosition = new Vector2(leftPosition.x, targetY);
                    Vector2 targetPosition = new Vector2(rightPosition.x, targetY);

                    LineRenderer warningLine = CreateDangerZone(weak1LaserData);
                    StartCoroutine(BlinkDangerZone(warningLine));
                    warningLine.SetPosition(0, startPosition);
                    warningLine.SetPosition(1, targetPosition);
                    warningLines.Add(warningLine);
                }

                yield return new WaitForSeconds(weak1LaserData.LaserLockDuration);

                foreach (var line in warningLines)
                {
                    Destroy(line.gameObject);
                }

                // 두 레이저 동시 발사
                foreach (int layer in selectedLayers)
                {
                    float targetY = layerPositions[layer];
                    Vector2 startPosition = new Vector2(leftPosition.x, targetY);
                    Vector2 targetPosition = new Vector2(rightPosition.x, targetY);

                    LaserController2 laser = LaserController2.Create(
                        weak1LaserData,
                        startPosition,
                        null
                    );
                    laser.SetTargetLayer(weak1LaserData.TargetLayer);
                    animator.SetTrigger("isNormal");
                    StartCoroutine(laser.FireLaser(startPosition, targetPosition));
                }

                // 발사 후 레이저 지속시간만큼 대기
                yield return new WaitForSeconds(weak1LaserData.LaserDuration);
            }
            #endregion

            #region 기본약공1
            else
            {
                // 기본 상태: 1개 레이어 공격 (기존 코드와 동일)
                int randomLayer = Random.Range(0, 3);
                float targetY = layerPositions[randomLayer];

                Vector2 startPosition = new Vector2(leftPosition.x, targetY);
                Vector2 targetPosition = new Vector2(rightPosition.x, targetY);

                LineRenderer warningLine = CreateDangerZone(weak1LaserData);
                StartCoroutine(BlinkDangerZone(warningLine));
                warningLine.SetPosition(0, startPosition);
                warningLine.SetPosition(1, targetPosition);

                yield return new WaitForSeconds(weak1LaserData.LaserLockDuration);
                Destroy(warningLine.gameObject);

                LaserController2 laser = LaserController2.Create(
                    weak1LaserData,
                    startPosition,
                    null
                );
                laser.SetTargetLayer(weak1LaserData.TargetLayer);
                animator.SetTrigger("isNormal");
                yield return StartCoroutine(laser.FireLaser(startPosition, targetPosition));
            }
            #endregion
            yield return new WaitForSeconds(weak1LaserData.LaserFollowDuration);
        }


        StartCoroutine(FinishPattern());
    }
    #endregion

    #region 약공격2
    public IEnumerator WeakPattern2()
    {
        yield return new WaitForSeconds(0.5f);
        EndPattern = false;
        Debug.Log(isEnraged ? "약공격2 - 광폭화" : "약공격2");
        currentState = BossState.WeakPattern2;

        #region 맵 데이터
        float bottomBound = mapWidthPositions[0].position.y - 1;
        float topBound = mapWidthPositions[1].position.y + 1;
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        float centerBound = (leftBound + rightBound) / 2;
        #endregion

        transform.position = new Vector2(centerBound, bottomBound + 8.9f);
        FacePlayer();

        #region 강화약공2
        if (isEnraged)
        {
            float mapWidth = rightBound - leftBound;
            float sectionWidth = mapWidth / 7f;
            float[] sectionPositions = new float[7];
            for (int i = 0; i < 7; i++)
            {
                sectionPositions[i] = leftBound + (sectionWidth * i) + (sectionWidth * 0.5f);
            }

            int[][] firePairs = new int[][] {
                new int[] {0, 6},
                new int[] {1, 5},
                new int[] {2, 4},
                new int[] {3}
            };

            // 먼저 모든 경고선 표시
            List<LineRenderer> allWarnings = new List<LineRenderer>();
            foreach (var pair in firePairs)
            {
                foreach (int pos in pair)
                {
                    LineRenderer warning = CreateDangerZone(weak2LaserData);
                    StartCoroutine(BlinkDangerZone(warning));
                    warning.SetPosition(0, new Vector2(sectionPositions[pos], topBound));
                    warning.SetPosition(1, new Vector2(sectionPositions[pos], bottomBound));
                    allWarnings.Add(warning);
                }
                yield return new WaitForSeconds(0.3f);
            }

            yield return new WaitForSeconds(weak2LaserData.LaserLockDuration);

            foreach (var warning in allWarnings)
            {
                Destroy(warning.gameObject);
            }

            // 그 다음 레이저 발사
            foreach (var pair in firePairs)
            {
                foreach (int pos in pair)
                {
                    LaserController2 laser = LaserController2.Create(
                        weak2LaserData,
                        new Vector2(sectionPositions[pos], topBound),
                        null
                    );
                    laser.SetTargetLayer(weak2LaserData.TargetLayer);
                    StartCoroutine(laser.FireLaser(
                        new Vector2(sectionPositions[pos], topBound),
                        new Vector2(sectionPositions[pos], bottomBound)
                    ));
                }
                yield return new WaitForSeconds(_wp2LaserDelay);
            }
        }
        #endregion

        #region 기본약공2
        else
        {
            float mapWidth = rightBound - leftBound;
            float sectionWidth = mapWidth / 7f;
            float[] sectionPositions = new float[7];
            for (int i = 0; i < 7; i++)
            {
                sectionPositions[i] = leftBound + (sectionWidth * i) + (sectionWidth * 0.5f);
            }

            // 시작 위치 랜덤 선택 (왼쪽 or 오른쪽)
            bool startFrom = Random.value > 0.5f;
            float startX = startFrom ? rightBound : leftBound;

            // 7개의 경고선 순차 생성
            List<LineRenderer> warningLines = new List<LineRenderer>();
            for (int i = 0; i < 7; i++)
            {
                LineRenderer warningLine = CreateDangerZone(weak2LaserData);
                StartCoroutine(BlinkDangerZone(warningLine));

                warningLine.SetPosition(0, new Vector2(sectionPositions[i], topBound));
                warningLine.SetPosition(1, new Vector2(sectionPositions[i], bottomBound));

                warningLines.Add(warningLine);
                yield return new WaitForSeconds(0.3f);
            }

            yield return new WaitForSeconds(weak2LaserData.LaserLockDuration);

            foreach (var line in warningLines)
            {
                Destroy(line.gameObject);
            }

            // 7개 레이저 순차 발사
            for (int i = 0; i < 7; i++)
            {
                LaserController2 laser = LaserController2.Create(
                    weak2LaserData,
                    new Vector2(sectionPositions[i], topBound),
                    null
                );
                laser.SetTargetLayer(weak2LaserData.TargetLayer);

                StartCoroutine(laser.FireLaser(
                    new Vector2(sectionPositions[i], topBound),
                    new Vector2(sectionPositions[i], bottomBound)
                ));

                yield return new WaitForSeconds(_wp2LaserDelay);
            }
            yield return new WaitForSeconds(weak2LaserData.LaserLockDuration);
        }
        #endregion

        StartCoroutine(FinishPattern());
        yield return new WaitForSeconds(weakPattern2Data.AfterAttackDelay);

    }
    #endregion

    #region 약공격3
    public IEnumerator WeakPattern3()
    {
        yield return new WaitForSeconds(0.5f);
        EndPattern = false;
        Debug.Log(isEnraged ? "약공격3 - 광폭화" : "약공격3");
        currentState = BossState.WeakPattern3;

        LaserScriptableObject selectedData = !isEnraged ? weak3LaserData : weak3Laser_E_Data;
        Debug.Log(selectedData);

        #region 맵데이터
        float topBound = mapWidthPositions[1].position.y + 1;
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        float centerBound = (leftBound + rightBound) / 2;
        #endregion

        transform.position = new Vector2(centerBound, topBound - 4);
        FacePlayer();

        // 총 4번 반복
        for (int attackCount = 0; attackCount < 4; attackCount++)
        {
            int rand = Random.Range(1, 5);
            Debug.Log(rand);


            for (int j = 0; j < rand; j++)
            {
                if (j % 2 == 1)
                {
                    Debug.Log($"우물정자 레이저");

                    // 우물 정자 레이저 생성 (세로 2개, 가로 2개)
                    List<LineRenderer> warningLines = new List<LineRenderer>();
                    float offset = 2f; // 오프셋 거리

                    // 경고선 생성
                    for (int i = 0; i < 4; i++)
                    {
                        LineRenderer warningLine = CreateDangerZone(weak3LaserData);
                        StartCoroutine(BlinkDangerZone(warningLine));
                        warningLines.Add(warningLine);
                    }

                    float trackingTime = 0f;
                    float trackingDuration = weak3LaserData.LaserFollowDuration * 0.8f;

                    while (trackingTime < trackingDuration)
                    {
                        Vector2 playerPos = (Vector2)player.transform.position;

                        // 세로 레이저 경고선 (왼쪽, 오른쪽)
                        warningLines[0].SetPosition(0, new Vector2(playerPos.x + offset, topBound));
                        warningLines[0].SetPosition(1, new Vector2(playerPos.x + offset, leftBound));

                        warningLines[1].SetPosition(0, new Vector2(playerPos.x - offset, topBound));
                        warningLines[1].SetPosition(1, new Vector2(playerPos.x - offset, leftBound));

                        // 수평 레이저 경고선 (위, 아래)
                        warningLines[2].SetPosition(0, new Vector2(leftBound, playerPos.y + offset));
                        warningLines[2].SetPosition(1, new Vector2(rightBound, playerPos.y + offset));

                        warningLines[3].SetPosition(0, new Vector2(leftBound, playerPos.y - offset));
                        warningLines[3].SetPosition(1, new Vector2(rightBound, playerPos.y - offset));

                        trackingTime += Time.deltaTime;
                        yield return null;
                    }

                    // 경고선 제거
                    foreach (var line in warningLines)
                    {
                        Destroy(line.gameObject);
                    }

                    // 우물 정자 패턴 레이저 발사
                    // 경고선 위치에 맞춰 레이저 발사
                    foreach (var warningLine in warningLines)
                    {
                        Vector2 startPos = warningLine.GetPosition(0); // 경고선의 시작 위치를 가져옵니다.
                        Vector2 endPos = warningLine.GetPosition(1); // 경고선의 끝 위치를 가져옵니다.

                        LaserController2 laser = LaserController2.Create(
                            weak3LaserData,
                            startPos,
                            null
                        );
                        laser.SetTargetLayer(weak3LaserData.TargetLayer);
                        animator.SetTrigger("isNormal");
                        StartCoroutine(laser.FireLaser(
                            startPos,
                            endPos // 경고선의 끝 위치로 발사
                        ));
                    }

                    // 다음 공격 전 대기
                    yield return new WaitForSeconds(weak3LaserData.LaserLockDuration * 0.8f);
                }
                else
                {
                    Debug.Log($"십자 레이저");

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
                        //float offset = isPenetratingCross ? 0f : 2f;
                        float offset = 0f;
                        // 수직 레이저 경고선 (위, 아래)
                        warningLines[0].SetPosition(0, new Vector2(currentPosition.x, currentPosition.y));
                        warningLines[0].SetPosition(1, new Vector2(currentPosition.x, currentPosition.y + 50f));

                        warningLines[1].SetPosition(0, new Vector2(currentPosition.x, currentPosition.y));
                        warningLines[1].SetPosition(1, new Vector2(currentPosition.x, currentPosition.y - 50f));

                        // 수평 레이저 경고선 (왼쪽, 오른쪽)
                        warningLines[2].SetPosition(0, new Vector2(currentPosition.x, currentPosition.y));
                        warningLines[2].SetPosition(1, new Vector2(currentPosition.x + 50f, currentPosition.y));

                        warningLines[3].SetPosition(0, new Vector2(currentPosition.x, currentPosition.y));
                        warningLines[3].SetPosition(1, new Vector2(currentPosition.x - 50f, currentPosition.y));

                        // 발사 0.45초 전에 위치 고정
                        if (trackingTime >= trackingDuration - 0.45f && !isPositionFixed)
                        {
                            fixedPosition = (Vector2)player.transform.position;
                            isPositionFixed = true;
                        }

                        trackingTime += Time.deltaTime;
                        yield return null;
                    }

                    // 먼저 경고선 위치 정보를 저장
                    List<Vector2[]> laserPaths = new List<Vector2[]>();
                    foreach (var line in warningLines)
                    {
                        laserPaths.Add(new Vector2[] { line.GetPosition(0), line.GetPosition(1) });
                    }

                    // 경고선 제거
                    foreach (var line in warningLines)
                    {
                        Destroy(line.gameObject);
                    }

                    // 저장된 정확한 위치로 레이저 발사
                    for (int i = 0; i < laserPaths.Count; i++)
                    {
                        Vector2 exactStartPos = laserPaths[i][0];
                        Vector2 exactEndPos = laserPaths[i][1];

                        LaserController2 laser = LaserController2.Create(
                            weak3LaserData,
                            exactStartPos,
                            null
                        );
                        laser.SetTargetLayer(weak3LaserData.TargetLayer);
                        animator.SetTrigger("isNormal");
                        StartCoroutine(laser.FireLaser(
                            exactStartPos,
                            exactEndPos
                        ));
                    }
                    // 다음 공격 전 대기
                    yield return new WaitForSeconds(weak3LaserData.LaserLockDuration);
                }
            }
        }


        StartCoroutine(FinishPattern());
    }
    #endregion

    #region 약공격4
    public IEnumerator WeakPattern4()
    {
        yield return new WaitForSeconds(0.5f);
        EndPattern = false;
        Debug.Log("약공격4");
        currentState = BossState.WeakPattern4;

        #region 맵데이터
        float bottomBound = mapWidthPositions[0].position.y;
        float topBound = mapWidthPositions[1].position.y;
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        float centerBound = (leftBound + rightBound) / 2;
        #endregion

        transform.position = new Vector2(leftBound + 10, bottomBound + 4);
        FacePlayer();

        // 공격 반복
        int totalAttack = isEnraged ? 8 : 4;
        for (int attackCount = 0; attackCount < totalAttack; attackCount++)
        {
            Debug.Log($"폭발 {attackCount + 1}회 공격 시작");

            // 현재 플레이어 위치 저장
            Vector2 targetPosition = player.transform.position;

            // 경고 표시 생성
            GameObject warningObj = new GameObject($"Warning_{attackCount}");
            SpriteRenderer warningRenderer = warningObj.AddComponent<SpriteRenderer>();

            #region 원형 스프라이트 직접 생성
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
            #endregion

            #region 경고 관련
            // 경고 표시 설정
            warningRenderer.sprite = circleSprite;
            warningRenderer.color = new Color(1f, 0f, 0f, 0.7f); // 더 진한 빨간색
            warningRenderer.transform.position = targetPosition;
            warningRenderer.transform.localScale = new Vector3(3f, 3f, 1f); // 더 큰 경고 크기
            warningRenderer.sortingOrder = 10; // 레이어 순서를 높여서 확실히 보이게 함

            // 경고 표시 깜빡임
            float warningDuration = isEnraged ? 0.5f : 0.7f;
            float currentTime = 0f;

            while (currentTime < warningDuration)
            {
                float alpha = Mathf.PingPong(currentTime * 5f, 0.7f) + 0.3f; // 최소 알파값 증가
                warningRenderer.color = new Color(1f, 0f, 0f, alpha);
                currentTime += Time.deltaTime;
                yield return null;
            }

            Destroy(warningObj);
            #endregion
            animator.SetTrigger("isNormal");
            #region 폭발 프로젝타일 생성
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

            float explosionDuration = isEnraged ? 0.25f : 0.5f;
            float startScale = 0.5f;
            float endScale = isEnraged ? 3f: 3f;
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

            if (attackCount < totalAttack - 1)
            {
                float timeBetweenAttacks = isEnraged ? 0.5f : 1f;
                yield return new WaitForSeconds(timeBetweenAttacks);
            }
            #endregion
        }


        StartCoroutine(FinishPattern());
    }
    #endregion

    #region 약공격5
    public IEnumerator WeakPattern5()
    {
        yield return new WaitForSeconds(0.5f);
        EndPattern = false;
        Debug.Log("약공격5");
        currentState = BossState.WeakPattern5;
        #region 맵데이터
        float bottomBound = mapWidthPositions[0].position.y;
        float topBound = mapWidthPositions[1].position.y;
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        float centerBound = (leftBound + rightBound) / 2;
        #endregion

        transform.position = new Vector2(rightBound - 10, bottomBound + 4);
        FacePlayer();

        // 3번의 공격 패턴 실행
        int totalPatternCount = 3;
        for (int patternCount = 0; patternCount < totalPatternCount; patternCount++)
        {
            Debug.Log($"약공격5 - {patternCount + 1}번째 패턴");

            // 4개의 패턴 위치 배열 중 하나를 선택
            Transform[] patternPositions;
            int randomPatternIndex = Random.Range(0, 4);

            switch (randomPatternIndex)
            {
                case 0:
                    patternPositions = pattern51Positions;
                    break;
                case 1:
                    patternPositions = pattern52Positions;
                    break;
                case 2:
                    patternPositions = pattern53Positions;
                    break;
                case 3:
                default:
                    patternPositions = pattern54Positions;
                    break;
            }

            // 선택한 패턴 위치 배열 사용
            List<Transform> selectedPositions = new List<Transform>(patternPositions);

            // 모든 위치에 대한 경고 표시 동시 생성
            List<GameObject> warningObjects = new List<GameObject>();
            foreach (Transform position in selectedPositions)
            {
                GameObject warningObj = new GameObject($"Warning_{patternCount}_{selectedPositions.IndexOf(position)}");
                SpriteRenderer warningRenderer = warningObj.AddComponent<SpriteRenderer>();

                #region 원형 스프라이트 생성

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
                #endregion

                // 경고 표시 설정
                warningRenderer.sprite = circleSprite;
                warningRenderer.color = new Color(1f, 0f, 0f, 0.7f);
                warningRenderer.transform.position = position.position;
                warningRenderer.transform.localScale = new Vector3(4f, 4f, 1f);
                warningRenderer.sortingOrder = 10;

                warningObjects.Add(warningObj);
            }

            // 경고 표시 깜빡임
            float warningDuration = isEnraged ? 0.7f : 1f;
            float currentTime = 0f;

            while (currentTime < warningDuration)
            {
                float alpha = Mathf.PingPong(currentTime * 5f, 0.7f) + 0.3f;
                foreach (GameObject warningObj in warningObjects)
                {
                    SpriteRenderer warningRenderer = warningObj.GetComponent<SpriteRenderer>();
                    warningRenderer.color = new Color(1f, 0f, 0f, alpha);
                }
                currentTime += Time.deltaTime;
                yield return null;
            }

            // 경고 표시 제거
            foreach (GameObject warningObj in warningObjects)
            {
                Destroy(warningObj);
            }

            // 모든 위치에 동시에 폭발 생성
            List<GameObject> projectiles = new List<GameObject>();
            List<ProjectileController> projectileControllers = new List<ProjectileController>();

            foreach (Transform position in selectedPositions)
            {
                ProjectileController projectileController = ProjectileController.Create(
                    weak4ProjData,
                    transform,
                    player.transform,
                    MusicProjectile,
                    false
                );
                projectileControllers.Add(projectileController);

                GameObject projectile = Instantiate(MusicProjectile, position.position, Quaternion.identity);
                ProjectileBehaviour behaviour = projectile.GetComponent<ProjectileBehaviour>();
                if (behaviour == null)
                {
                    behaviour = projectile.AddComponent<ProjectileBehaviour>();
                }
                behaviour.Initialize(weak4ProjData.Damage, null);
                projectiles.Add(projectile);
            }

            // 폭발 애니메이션
            float explosionDuration = isEnraged ? 0.25f : 0.5f;
            float startScale = 0.5f;
            float endScale = isEnraged ? 3f : 2f;
            float elapsed = 0f;

            while (elapsed < explosionDuration)
            {
                float scale = Mathf.Lerp(startScale, endScale, elapsed / explosionDuration);
                float alpha = 1f - (elapsed / explosionDuration);

                foreach (GameObject projectile in projectiles)
                {
                    projectile.transform.localScale = new Vector3(scale, scale, 1f);
                    SpriteRenderer projRenderer = projectile.GetComponent<SpriteRenderer>();
                    if (projRenderer != null)
                    {
                        projRenderer.color = new Color(1f, 1f, 1f, alpha);
                    }
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            // 프로젝타일과 ProjectileController 정리
            foreach (GameObject projectile in projectiles)
            {
                Destroy(projectile);
            }

            foreach (ProjectileController controller in projectileControllers)
            {
                if (controller != null && controller.gameObject != null)
                {
                    Destroy(controller.gameObject);
                }
            }

            // 다음 패턴 전 대기
            if (patternCount < totalPatternCount - 1)
            {
                float timeBetweenPatterns = isEnraged ? 1f : 1.5f;
                yield return new WaitForSeconds(timeBetweenPatterns);
            }
        }

        yield return new WaitForSeconds(weakPattern5Data.AfterAttackDelay);

        yield return StartCoroutine(FinishPattern());
    }
    #endregion

    #region 보스강화
    public IEnumerator EnragedPattern()
    {
        Debug.Log("광폭화 패턴");
        currentState = BossState.EnragedPattern;
        #region 맵 데이터
        float bottomBound = mapWidthPositions[0].position.y - 1;
        float topBound = mapWidthPositions[1].position.y + 1;
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        float mapWidth = rightBound - leftBound;
        #endregion

        #region 레이저 위치 계산 및 생성
        // 맵을 7등분하여 레이저 위치 계산
        float sectionWidth = mapWidth / 7f;
        float[] sectionPositions = new float[7];
        for (int i = 0; i < 7; i++)
        {
            sectionPositions[i] = leftBound + (sectionWidth * i) + (sectionWidth * 0.5f);
        }

        // 가운데 3개의 레이저 생성 (3,4,5번째 구역)
        LaserController2[] lasers = new LaserController2[3];
        Vector2[] currentPositions = new Vector2[3];

        for (int i = 0; i < 3; i++)
        {
            currentPositions[i] = new Vector2(sectionPositions[i + 2], 0);

            lasers[i] = LaserController2.Create(
                enragedLaserData,
                new Vector2(currentPositions[i].x, topBound),
                null
            );
            lasers[i].SetTargetLayer(enragedLaserData.TargetLayer);
        }
        #endregion

        #region 초기 레이저 크기로 페이드 인
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
        #endregion

        #region 레이저 이동 (스크립터블 오브젝트의 LaserSpeed 사용)
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
        #endregion

        #region 레이저 페이드 아웃
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
        #endregion

        #region 레이저 제거
        foreach (var laser in lasers)
        {
            if (laser != null && laser.gameObject != null)
            {
                Destroy(laser.gameObject);
            }
        }
        #endregion

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }
    #endregion

    #region 발악패턴1
    public IEnumerator DesperatePattern1()
    {
        yield return new WaitForSeconds(0.5f);
        EndPattern = false;
        Debug.Log("발악패턴1");
        currentState = BossState.DesperatePattern1;

        #region 맵데이터
        float bottomBound = mapWidthPositions[0].position.y;
        float topBound = mapWidthPositions[1].position.y;
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        float centerBound = (leftBound + rightBound) / 2;
        #endregion

        transform.position = new Vector2(centerBound, bottomBound + 4);
        FacePlayer();

        bool weakPattern1Complete = false;
        bool weakPattern2Complete = false;

        // WeakPattern1 (not enraged)
        StartCoroutine(WeakPattern1Wrapper(() => {
            weakPattern1Complete = true;
        }));

        // WeakPattern2 (enraged, 7 times)
        StartCoroutine(WeakPattern2Wrapper(() => {
            weakPattern2Complete = true;
        }));

        // Wait until both patterns complete
        yield return new WaitUntil(() => weakPattern1Complete && weakPattern2Complete);


        StartCoroutine(FinishPattern());
    }
    #endregion

    #region 발악1의 약공1
    public IEnumerator WeakPattern1Des()
    {
        yield return new WaitForSeconds(0.5f);
        EndPattern = false;
        Debug.Log("약공격1 - 기본(발악1)");
        currentState = BossState.WeakPattern1;

        #region 맵데이터
        float bottomBound = mapWidthPositions[0].position.y;
        float topBound = mapWidthPositions[1].position.y;
        float mapHeight = topBound - bottomBound;
        float layerHeight = mapHeight / 3f;
        #endregion

        float[] layerPositions = new float[3];
        for (int i = 0; i < 3; i++)
        {
            layerPositions[i] = bottomBound + (layerHeight * (i + 0.5f));
        }

        Vector2 leftPosition = new Vector2(mapWidthPositions[0].position.x - 1, 0);
        Vector2 rightPosition = new Vector2(mapWidthPositions[1].position.x + 1, 0);

        
        for (int i = 0; i < 7; i++)
        {
            
                // 기본 상태: 1개 레이어 공격 
                int randomLayer = Random.Range(0, 3);
                float targetY = layerPositions[randomLayer];

                Vector2 startPosition = new Vector2(leftPosition.x, targetY);
                Vector2 targetPosition = new Vector2(rightPosition.x, targetY);

                LineRenderer warningLine = CreateDangerZone(weak1LaserData);
                StartCoroutine(BlinkDangerZone(warningLine));
                warningLine.SetPosition(0, startPosition);
                warningLine.SetPosition(1, targetPosition);

                yield return new WaitForSeconds(weak1LaserData.LaserLockDuration);
                Destroy(warningLine.gameObject);

                LaserController2 laser = LaserController2.Create(
                    weak1LaserData,
                    startPosition,
                    null
                );
                laser.SetTargetLayer(weak1LaserData.TargetLayer);
                yield return StartCoroutine(laser.FireLaser(startPosition, targetPosition));
            

            if (i < 6)
            {
                yield return new WaitForSeconds(weak1LaserData.LaserLockDuration);
            }
        }

        currentState = BossState.None;
        currentCoroutine = null;
    }
    #endregion

    #region 발악1 동시 실행을 위한 코루틴
    private IEnumerator WeakPattern1Wrapper(System.Action onComplete)
    {
        yield return StartCoroutine(WeakPattern1Des());
        onComplete?.Invoke();
    }
    private IEnumerator WeakPattern2Wrapper(System.Action onComplete)
    {
        isEnraged = true;
        int repeatCount = 0;
        while (repeatCount < 7)
        {
            yield return StartCoroutine(WeakPattern2());
            repeatCount++;
        }
        onComplete?.Invoke();
    }
    #endregion

    #region 발악패턴2
    public IEnumerator DesperatePattern2()
    {
        yield return new WaitForSeconds(0.5f);
        EndPattern = false;
        Debug.Log("발악패턴2");
        currentState = BossState.DesperatePattern2;

        #region 맵 데이터
        float bottomBound = mapWidthPositions[0].position.y;
        float topBound = mapWidthPositions[1].position.y;
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        #endregion

        // Initial position setup
        transform.position = new Vector2(leftBound + 10, bottomBound + 4);
        FacePlayer();

        bool pattern4Complete = false;
        bool pattern5Complete = false;

        #region 코루틴 동시 실행
        StartCoroutine(DesperatePattern2_Pattern4(() => {
            pattern4Complete = true;
        }));

        StartCoroutine(DesperatePattern2_Pattern5(() => {
            pattern5Complete = true;
        }));

        // 두 패턴이 끝날때까지 대기
        yield return new WaitUntil(() => pattern4Complete && pattern5Complete);
        #endregion


        StartCoroutine(FinishPattern());
    }
    #endregion

    #region 발악2의 약공4
    private IEnumerator DesperatePattern2_Pattern4(System.Action onComplete)
    {
        Debug.Log("발악패턴2 - 약공격4 부분");

        // 약공격 5를 병렬로 실행
        StartCoroutine(ContinuousPattern5());

        #region 맵데이터
        float bottomBound = mapWidthPositions[0].position.y;
        float topBound = mapWidthPositions[1].position.y;
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        #endregion

        List<ProjectileController> activeProjectileControllers = new List<ProjectileController>();

        try
        {
            // 3번의 패턴 실행
            for (int patternCount = 0; patternCount < 3; patternCount++)
            {
                Debug.Log($"약공격4 - {patternCount + 1}번째 실행");

                // 각 패턴당 6번의 폭발
                for (int attackCount = 0; attackCount < 6; attackCount++)
                {
                    Vector2 targetPosition = player.transform.position;

                    // 경고 표시 생성
                    GameObject warningObj = new GameObject($"Warning_Pattern4_{patternCount}_{attackCount}");
                    SpriteRenderer warningRenderer = warningObj.AddComponent<SpriteRenderer>();

                    #region 원형 스프라이트 생성
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

                    warningRenderer.sprite = circleSprite;
                    warningRenderer.color = new Color(1f, 0f, 0f, 0.7f);
                    warningRenderer.transform.position = targetPosition;
                    warningRenderer.transform.localScale = new Vector3(4f, 4f, 1f);
                    warningRenderer.sortingOrder = 10;

                    // 경고 표시 깜빡임
                    float warningDuration = 1f;
                    float currentTime = 0f;

                    while (currentTime < warningDuration)
                    {
                        float alpha = Mathf.PingPong(currentTime * 5f, 0.7f) + 0.3f;
                        warningRenderer.color = new Color(1f, 0f, 0f, alpha);
                        currentTime += Time.deltaTime;
                        yield return null;
                    }

                    Destroy(warningObj);
                    #endregion

                    #region 폭발 관련 코드
                    // 폭발 프로젝타일 생성
                    ProjectileController projectileController = ProjectileController.Create(
                        weak4ProjData,
                        transform,
                        player.transform,
                        MusicProjectile,
                        false
                    );
                    activeProjectileControllers.Add(projectileController);

                    GameObject projectile = Instantiate(MusicProjectile, targetPosition, Quaternion.identity);
                    ProjectileBehaviour behaviour = projectile.GetComponent<ProjectileBehaviour>();
                    if (behaviour == null)
                    {
                        behaviour = projectile.AddComponent<ProjectileBehaviour>();
                    }
                    behaviour.Initialize(weak4ProjData.Damage, null);

                    // 폭발 애니메이션
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
                    #endregion

                    if (attackCount < 5)
                    {
                        yield return new WaitForSeconds(1f);
                    }
                }

                if (patternCount < 2)
                {
                    yield return new WaitForSeconds(1.5f);
                }
            }
        }
        finally
        {
            // 모든 ProjectileController 정리
            foreach (var controller in activeProjectileControllers)
            {
                if (controller != null && controller.gameObject != null)
                {
                    Destroy(controller.gameObject);
                }
            }
            activeProjectileControllers.Clear();

            // Pattern5 정지 플래그 설정
            isPattern5Running = false;
        }

        onComplete?.Invoke();
    }
    #endregion

    private bool isPattern5Running = false;

    #region 발악2의 약공5 (발악2 약공4가 끝날때까지 반복)
    private IEnumerator ContinuousPattern5()
    {
        isPattern5Running = true;
        List<ProjectileController> activeProjectileControllers = new List<ProjectileController>();

        try
        {
            while (isPattern5Running)
            {
                // 10개의 랜덤 위치 선택
                List<Transform> selectedPositions = new List<Transform>();
                List<Transform> availablePositions = new List<Transform>(pattern51Positions);

                for (int i = 0; i < 10 && availablePositions.Count > 0; i++)
                {
                    int randomIndex = Random.Range(0, availablePositions.Count);
                    selectedPositions.Add(availablePositions[randomIndex]);
                    availablePositions.RemoveAt(randomIndex);
                }

                // 경고 표시 생성
                List<GameObject> warningObjects = new List<GameObject>();
                foreach (Transform position in selectedPositions)
                {
                    GameObject warningObj = new GameObject($"Warning_Pattern5_Continuous");
                    SpriteRenderer warningRenderer = warningObj.AddComponent<SpriteRenderer>();

                    // 원형 스프라이트 생성
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

                    warningRenderer.sprite = circleSprite;
                    warningRenderer.color = new Color(1f, 0f, 0f, 0.7f);
                    warningRenderer.transform.position = position.position;
                    warningRenderer.transform.localScale = new Vector3(4f, 4f, 1f);
                    warningRenderer.sortingOrder = 10;

                    warningObjects.Add(warningObj);
                }

                // 경고 표시 깜빡임
                float warningDuration = 1f;
                float currentTime = 0f;

                while (currentTime < warningDuration && isPattern5Running)
                {
                    float alpha = Mathf.PingPong(currentTime * 5f, 0.7f) + 0.3f;
                    foreach (GameObject warningObj in warningObjects)
                    {
                        if (warningObj != null)
                        {
                            SpriteRenderer warningRenderer = warningObj.GetComponent<SpriteRenderer>();
                            warningRenderer.color = new Color(1f, 0f, 0f, alpha);
                        }
                    }
                    currentTime += Time.deltaTime;
                    yield return null;
                }

                // 경고 표시 제거
                foreach (GameObject warningObj in warningObjects)
                {
                    if (warningObj != null)
                    {
                        Destroy(warningObj);
                    }
                }

                if (!isPattern5Running) break;

                // 폭발 생성
                List<GameObject> projectiles = new List<GameObject>();

                foreach (Transform position in selectedPositions)
                {
                    ProjectileController projectileController = ProjectileController.Create(
                        weak4ProjData,
                        transform,
                        player.transform,
                        MusicProjectile,
                        false
                    );
                    activeProjectileControllers.Add(projectileController);

                    GameObject projectile = Instantiate(MusicProjectile, position.position, Quaternion.identity);
                    ProjectileBehaviour behaviour = projectile.GetComponent<ProjectileBehaviour>();
                    if (behaviour == null)
                    {
                        behaviour = projectile.AddComponent<ProjectileBehaviour>();
                    }
                    behaviour.Initialize(weak4ProjData.Damage, null);
                    projectiles.Add(projectile);
                }

                // 폭발 애니메이션
                float explosionDuration = 0.5f;
                float startScale = 0.5f;
                float endScale = 2f;
                float elapsed = 0f;

                while (elapsed < explosionDuration && isPattern5Running)
                {
                    float scale = Mathf.Lerp(startScale, endScale, elapsed / explosionDuration);
                    float alpha = 1f - (elapsed / explosionDuration);

                    foreach (GameObject projectile in projectiles)
                    {
                        if (projectile != null)
                        {
                            projectile.transform.localScale = new Vector3(scale, scale, 1f);
                            SpriteRenderer projRenderer = projectile.GetComponent<SpriteRenderer>();
                            if (projRenderer != null)
                            {
                                projRenderer.color = new Color(1f, 1f, 1f, alpha);
                            }
                        }
                    }

                    elapsed += Time.deltaTime;
                    yield return null;
                }

                // 프로젝타일 제거
                foreach (GameObject projectile in projectiles)
                {
                    if (projectile != null)
                    {
                        Destroy(projectile);
                    }
                }

                // 다음 반복 전 대기
                yield return new WaitForSeconds(1.5f);
            }
        }
        finally
        {
            // 모든 ProjectileController 정리
            foreach (var controller in activeProjectileControllers)
            {
                if (controller != null && controller.gameObject != null)
                {
                    Destroy(controller.gameObject);
                }
            }
            activeProjectileControllers.Clear();
        }
    }
    #endregion

    #region 발악2의 약공5..? (발악 2 버그 확인되면 코드 고쳐봐야할거 같음)
    private IEnumerator DesperatePattern2_Pattern5(System.Action onComplete)
    {
        Debug.Log("발악패턴2 - 약공격5 부분");

        // 패턴 시작 시간 기록
        float patternStartTime = Time.time;
        float patternTotalDuration = 10.5f; // 3 * (1f + 0.5f + 1.5f) = 10.5f


        List<ProjectileController> activeProjectileControllers = new List<ProjectileController>();

        try
        {
            // 3번의 패턴 실행
            for (int patternCount = 0; patternCount < 3; patternCount++)
            {
                Debug.Log($"약공격5 - {patternCount + 1}번째 패턴");

                // 10개의 랜덤 위치 선택
                List<Transform> selectedPositions = new List<Transform>();
                List<Transform> availablePositions = new List<Transform>(pattern51Positions);

                for (int i = 0; i < 10 && availablePositions.Count > 0; i++)
                {
                    int randomIndex = Random.Range(0, availablePositions.Count);
                    selectedPositions.Add(availablePositions[randomIndex]);
                    availablePositions.RemoveAt(randomIndex);
                }

                // 모든 위치에 대한 경고 표시 동시 생성
                List<GameObject> warningObjects = new List<GameObject>();
                foreach (Transform position in selectedPositions)
                {
                    GameObject warningObj = new GameObject($"Warning_Pattern5_{patternCount}_{selectedPositions.IndexOf(position)}");
                    SpriteRenderer warningRenderer = warningObj.AddComponent<SpriteRenderer>();

                    // 원형 스프라이트 생성
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

                    warningRenderer.sprite = circleSprite;
                    warningRenderer.color = new Color(1f, 0f, 0f, 0.7f);
                    warningRenderer.transform.position = position.position;
                    warningRenderer.transform.localScale = new Vector3(4f, 4f, 1f);
                    warningRenderer.sortingOrder = 10;

                    warningObjects.Add(warningObj);
                }

                // 경고 표시 깜빡임
                float warningDuration = 1f;
                float currentTime = 0f;

                while (currentTime < warningDuration)
                {
                    float alpha = Mathf.PingPong(currentTime * 5f, 0.7f) + 0.3f;
                    foreach (GameObject warningObj in warningObjects)
                    {
                        SpriteRenderer warningRenderer = warningObj.GetComponent<SpriteRenderer>();
                        warningRenderer.color = new Color(1f, 0f, 0f, alpha);
                    }
                    currentTime += Time.deltaTime;
                    yield return null;
                }

                // 경고 표시 제거
                foreach (GameObject warningObj in warningObjects)
                {
                    Destroy(warningObj);
                }

                // 모든 위치에 동시에 폭발 생성
                List<GameObject> projectiles = new List<GameObject>();
                List<ProjectileController> projectileControllers = new List<ProjectileController>();

                foreach (Transform position in selectedPositions)
                {
                    ProjectileController projectileController = ProjectileController.Create(
                        weak4ProjData,
                        transform,
                        player.transform,
                        MusicProjectile,
                        false
                    );
                    activeProjectileControllers.Add(projectileController);

                    GameObject projectile = Instantiate(MusicProjectile, position.position, Quaternion.identity);
                    ProjectileBehaviour behaviour = projectile.GetComponent<ProjectileBehaviour>();
                    if (behaviour == null)
                    {
                        behaviour = projectile.AddComponent<ProjectileBehaviour>();
                    }
                    behaviour.Initialize(weak4ProjData.Damage, null);
                    projectiles.Add(projectile);
                }

                // 폭발 애니메이션
                float explosionDuration = 0.5f;
                float startScale = 0.5f;
                float endScale = 2f;
                float elapsed = 0f;

                while (elapsed < explosionDuration)
                {
                    float scale = Mathf.Lerp(startScale, endScale, elapsed / explosionDuration);
                    float alpha = 1f - (elapsed / explosionDuration);

                    foreach (GameObject projectile in projectiles)
                    {
                        projectile.transform.localScale = new Vector3(scale, scale, 1f);
                        SpriteRenderer projRenderer = projectile.GetComponent<SpriteRenderer>();
                        if (projRenderer != null)
                        {
                            projRenderer.color = new Color(1f, 1f, 1f, alpha);
                        }
                    }

                    elapsed += Time.deltaTime;
                    yield return null;
                }

                // 프로젝타일과 ProjectileController 정리
                foreach (GameObject projectile in projectiles)
                {
                    Destroy(projectile);
                }

                foreach (ProjectileController controller in projectileControllers)
                {
                    if (controller != null && controller.gameObject != null)
                    {
                        Destroy(controller.gameObject);
                    }
                }

                if (patternCount < 2)
                {
                    yield return new WaitForSeconds(1.5f);
                }
            }
        }
        finally
        {
            // 모든 ProjectileController 정리
            foreach (var controller in activeProjectileControllers)
            {
                if (controller != null && controller.gameObject != null)
                {
                    Destroy(controller.gameObject);
                }
            }
            activeProjectileControllers.Clear();
        }

        // 패턴의 정확한 지속 시간 보장
        float remainingTime = patternTotalDuration - (Time.time - patternStartTime);
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        onComplete?.Invoke();
    }
    #endregion

    #region 발악패턴3
    public IEnumerator DesperatePattern3()
    {
        yield return new WaitForSeconds(0.5f);
        EndPattern = false;
        Debug.Log("발악패턴3");
        currentState = BossState.DesperatePattern3;

        #region 맵 데이터
        float bottomBound = mapWidthPositions[0].position.y - 1;
        float topBound = mapWidthPositions[1].position.y + 1;
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        float centerBound = (leftBound + rightBound) / 2;
        float centerYBound = (topBound + bottomBound) / 2;
        #endregion

        #region 레이저 리스트
        List<List<(Vector2, Vector2)>> selectedSets = new List<List<(Vector2, Vector2)>>();

        // 계획서상 하단 이미지 레이저
        List<(Vector2, Vector2)> firstSet = new List<(Vector2, Vector2)>
    {
        (new Vector2(leftBound, topBound), new Vector2(rightBound, bottomBound)),
        (new Vector2(rightBound, topBound), new Vector2(leftBound, bottomBound)),
        (new Vector2(centerBound - 6, topBound), new Vector2(centerBound + 6, bottomBound)),
        (new Vector2(centerBound + 6, topBound), new Vector2(centerBound - 6, bottomBound))
    };

        // 계획서상 상단 이미지 레이저
        List<(Vector2, Vector2)> secondSet = new List<(Vector2, Vector2)>
    {
        (new Vector2(centerBound + 14, topBound), new Vector2(centerBound - 14, bottomBound)),
        (new Vector2(centerBound - 14, topBound), new Vector2(centerBound + 14, bottomBound)),
        (new Vector2(leftBound, centerYBound), new Vector2(rightBound, centerYBound)),
        (new Vector2(centerBound, topBound), new Vector2(centerBound, bottomBound))
    };
        #endregion

        #region 레이저 경고 및 발사 로직
        // 첫 번째와 두 번째 세트에서 랜덤으로 8개의 세트를 선택
        for (int i = 0; i < 8; i++)
        {
            if (Random.value < 0.5f)
            {
                selectedSets.Add(new List<(Vector2, Vector2)>(firstSet));
            }
            else
            {
                selectedSets.Add(new List<(Vector2, Vector2)>(secondSet));
            }
        }

        // 선택된 세트에 대해 경고선 표시
        foreach (var set in selectedSets)
        {
            List<LineRenderer> warningLines = new List<LineRenderer>();

            foreach (var (startPos, endPos) in set)
            {
                LineRenderer warningLine = CreateDangerZone(desperate3LaserData);
                warningLine.SetPosition(0, startPos);
                warningLine.SetPosition(1, endPos);
                StartCoroutine(BlinkDangerZone(warningLine));
                warningLines.Add(warningLine);
            }

            yield return new WaitForSeconds(0.5f); // 세트 전체가 표시된 후 잠시 대기

            // 경고선 제거
            foreach (var warningLine in warningLines)
            {
                Destroy(warningLine.gameObject);
            }
        }

        // 경고선 표시 후 2초 대기
        yield return new WaitForSeconds(2f);

        // 레이저 발사
        foreach (var set in selectedSets)
        {
            foreach (var (startPos, endPos) in set)
            {
                LaserController2 laser = LaserController2.Create(
                    desperate3LaserData,
                    startPos,
                    null
                );
                laser.SetTargetLayer(desperate3LaserData.TargetLayer);
                StartCoroutine(laser.FireLaser(startPos, endPos));
            }

            yield return new WaitForSeconds(0.5f); // 세트 간 딜레이
        }
        #endregion

        // 패턴 종료
        yield return new WaitForSeconds(desperatePattern3Data.AfterAttackDelay);


        StartCoroutine(FinishPattern());
    }
    #endregion

    #region 광폭화 연출
    private IEnumerator EnrageEffect(Vector2 bossPosition, Vector2 staticPlayerPosition)
    {
        Debug.Log("보스 광폭화 효과 시작!");

        // 보스 상태를 None으로 설정하여 다른 패턴이 시작되지 않도록 함
        currentState = BossState.None;

        // 광폭화 애니메이션 및 효과 실행
        //animator.SetTrigger("EnrageStart");
        isEnraged = true;
        //animator.SetBool("isEnraged", true);

        // 여기에 광폭화 효과 로직 추가
        // - 카메라 흔들림, 파티클, 사운드 등

        // 광폭화 연출 시간 동안 대기
        //yield return new WaitForSeconds(3.0f); // 필요에 따라 시간 조정

        Debug.Log("보스 광폭화 효과 완료!");

        // 광폭화 효과 후 Idle 상태로 전환
        currentState = BossState.Idle;
        currentCoroutine = null;
        yield return null;
    }
    #endregion

    #region 데스 연출
    private IEnumerator DeathEffect()
    {
        Debug.Log("보스 사망 효과 시작!");

        // 진행 중인 모든 코루틴 중지
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        // 모든 진행 중인 코루틴 중지 (패턴 포함)
        StopAllCoroutines();

        // 보스 상태 설정
        currentState = BossState.None;

        // 모든 투사체 찾아서 제거
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("BossProjectile");
        foreach (GameObject projectile in projectiles)
        {
            Destroy(projectile);
        }

        // 레이저 컨트롤러가 있다면 모든 레이저 비활성화
        DeactivateAllLasers();

        // 사망 애니메이션

        // 사망 사운드 

        // 카메라 효과

        // 사망 애니메이션/효과 지속 시간 동안 대기
        yield return new WaitForSeconds(3.0f);

        // 보스 오브젝트 비활성화 또는 제거 전 페이드 아웃 효과
        //SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        //if (renderer != null)
        //{
        //    float duration = 1.5f;
        //    float elapsed = 0;
        //    Color startColor = renderer.color;

        //    while (elapsed < duration)
        //    {
        //        elapsed += Time.deltaTime;
        //        float normalizedTime = elapsed / duration;
        //        renderer.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1, 0, normalizedTime));
        //        yield return null;
        //    }
        //}

        // 보스 오브젝트 제거
        //Destroy(gameObject);

    }

    public void DeactivateAllLasers()
    {
        LaserController[] laserControllers = FindObjectsOfType<LaserController>();
        foreach (LaserController laser in laserControllers)
        {
            laser.DeactivateLaser();
        }

        LaserController2[] laserControllers2 = FindObjectsOfType<LaserController2>();
        foreach (LaserController2 laser in laserControllers2)
        {
            laser.DeactivateLaser();
        }
    }
    #endregion

    #region 그로기
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
    #endregion

    #region 위험 구역 생성 및 깜빡임
    private LineRenderer CreateDangerZone(LaserScriptableObject laserData)
    {
        GameObject dangerZoneObj = new GameObject("DangerZone");
        LineRenderer lineRenderer = dangerZoneObj.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = laserData.LaserWidth*2;
        lineRenderer.endWidth = laserData.LaserWidth*2;

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
    #endregion

    #region 보스 시선 처리
    private void FacePlayer() 
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
    #endregion
}
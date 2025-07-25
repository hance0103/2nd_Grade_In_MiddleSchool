using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UIElements;

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
    [SerializeField]
    private BossState currentState;
    public GameObject player;
    private LaserController2 laserController;
    private ProjectileController projectileController;
    public bool EndPattern = false;

    private bool isDesperateEnd = false;
    private bool isDesperatePatternExecuted = false;

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
    [SerializeField] private Transform bossPatternPanel;

    [Header("약공격1 데이터")]
    [SerializeField] private BossScriptableObject weakPattern1Data;
    [SerializeField] private LaserScriptableObject weak1LaserData;
    [SerializeField] private Vector2[] weak1TeleportPosition;

    [Header("약공격2 데이터")]
    [SerializeField] private BossScriptableObject weakPattern2Data;
    [SerializeField] private LaserScriptableObject weak2LaserData;
    [SerializeField] private float _wp2LaserDelay;

    [Header("약공격3 데이터")]
    [SerializeField] private BossScriptableObject weakPattern3Data;
    [SerializeField] private LaserScriptableObject weak3LaserData;
    [SerializeField] private LaserScriptableObject weak3Laser_E_Data;

    [Header("약공격4 데이터")]
    [SerializeField] private BossScriptableObject weakPattern4Data;
    [SerializeField] private float _wp4NormalDelay;
    [SerializeField] private float _wp4EnranageDelay;
    [SerializeField] private List<Sprite> explosionSprites;

    [Header("약공격5 데이터")]
    [SerializeField] private BossScriptableObject weakPattern5Data;
    [SerializeField] private LaserScriptableObject weak5LaserData;
    [SerializeField] private GameObject weak5Object;
    [SerializeField] private GameObject warningPrefab;
    private List<GameObject> weak5ObjectList;
    private Dictionary<int, List<Vector3>> weak5PosDict;

    [SerializeField] private float weak5Delay;

    [Header("약공 4, 5 공용 데이터")]
    [SerializeField] private ProjectileScriptableObject weak4ProjData;
    [SerializeField] private GameObject MusicProjectile;
    [SerializeField] private float musicProjectileHitTime;
    [SerializeField] private float explosionDuration;

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
    [SerializeField] private Desperate2Position desperate2Position;
    [SerializeField] private float desperate2ExplosionObjectDelay;


    [Header("발악패턴3 데이터")]
    [SerializeField] private BossScriptableObject desperatePattern3Data;
    [SerializeField] private LaserScriptableObject desperate3LaserData;
    [SerializeField] private float desperate3Count;
    [SerializeField] private Desperate3Vec desperate3Vec1;
    [SerializeField] private Desperate3Vec desperate3Vec2;


    private BossHPManager bossHPManager; // BossHPManager ����
    private bool shouldTriggerEnrage = false;
    private bool isEnrageTriggered = false;
    private bool isDead = false;
    Animator animator;

    private BossState[] desperatePatternArray;
    #endregion

    #region 발악패턴2 관련
    [Serializable]
    public class Desperate2Position
    {
        public List<Desperate2PositionPair> positionList;

        public bool isInclude(int index)
        {
            foreach (Desperate2PositionPair pair in positionList)
            {
                if (pair.num == index && pair.isActive == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
    [Serializable]
    public class Desperate2PositionPair
    {
        public int num;
        public bool isActive;
    }
    #endregion
    #region 발악패턴3 관련

    [Serializable]
    public class Desperate3Vec
    {
        public List<VectorPair> vecList;
    }
    [Serializable]
    public class VectorPair
    {
        public Vector2 startVec;
        public Vector2 endVec;
    }

    #endregion
    private void Awake()
    {
        bossHPManager = GetComponent<BossHPManager>();


        weak5ObjectList = new();


        foreach (Transform child in weak5Object.transform)
        {
            weak5ObjectList.Add(child.gameObject);

        }

        weak5PosDict = new();
        int dictIndex = 1;
        foreach (GameObject weak5 in weak5ObjectList)
        {
            List<Vector3> vecList = new();

            foreach (Transform child in weak5.transform)
            {
                vecList.Add(child.position);
            }
            weak5PosDict.Add(dictIndex, vecList);
            dictIndex++;
        }

        foreach( var list in weak5PosDict)
        {
            Debug.Log($"{list.Key} : {list.Value.Count}");
        }
    }

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        if (isEnraged == true)
            animator.SetBool("isEnraged", true);

        desperatePatternArray = new BossState[] {
            BossState.DesperatePattern1,
            BossState.DesperatePattern2,
            BossState.DesperatePattern3
        };

        //patternDic.Add(0, new BossState[] {

        //    //BossState.WeakPattern4,
        //    //BossState.WeakPattern5,
        //    BossState.WeakPattern1,
        //    //BossState.WeakPattern2,
        //    //BossState.WeakPattern3,
        //    //BossState.WeakPattern4,
        //    //BossState.WeakPattern5,
        //    //BossState.EnragedPattern,
        //    //BossState.DesperatePattern1,
        //    //BossState.DesperatePattern2,
        //    //BossState.DesperatePattern3
        //});


        patternDic.Add(0, new BossState[] { BossState.WeakPattern1, BossState.WeakPattern3, BossState.WeakPattern2, BossState.WeakPattern1, BossState.WeakPattern5 });
        patternDic.Add(1, new BossState[] { BossState.WeakPattern1, BossState.WeakPattern3, BossState.WeakPattern1, BossState.WeakPattern4, BossState.WeakPattern5 });
        patternDic.Add(2, new BossState[] { BossState.WeakPattern1, BossState.WeakPattern4, BossState.WeakPattern3, BossState.WeakPattern4, BossState.WeakPattern2 });

        if (!isDesperate)
        {
            StartCoroutine(BeforeIdle());
        }
        
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
        if(ready4Desperate && EndPattern && !isDesperate)
        {
            Debug.Log("패턴 종료. 발악 시작");
            currentState = BossState.Idle;
            OnDesperate();
        }

        // 체력 0이 되어서 발악패턴 - isDesperate는 BossHpManager에서 True로 설정해줌
        if (isDesperate)
        {
            EndPattern = true;
            // 모든 발악패턴이 종료되어 보스가 완전히 죽음 - isDead는 발악패턴 코루틴이 모두 종료되면 true로 바꿈
            if (isDead)
            {

                bossHPManager.BossDie3Execute();
                StartCoroutine(DeathEffect());
                return;
            }
            if (!isDesperatePatternExecuted)
            {
                isDesperatePatternExecuted = true;
                
                StartCoroutine(DesperateIdle());
            }

            if (currentState == BossState.DesperatePattern1 && currentCoroutine == null)
            {
                currentCoroutine = StartCoroutine(DesperatePattern1());
            }
            else if (currentState == BossState.DesperatePattern2 && currentCoroutine == null)
            {
                currentCoroutine = StartCoroutine(DesperatePattern2());
            }
            else if (currentState == BossState.DesperatePattern3 && currentCoroutine == null)
            {
                currentCoroutine = StartCoroutine(DesperatePattern3());
            }


            return;
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
        int patternNum =  UnityEngine.Random.Range(0, patternDic.Count);
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

        int patternNum = UnityEngine.Random.Range(0, patternDic.Count);
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
    private IEnumerator DesperateIdle()
    {
        currentBossStateArray = desperatePatternArray;
        for (int i = 0; i < currentBossStateArray.Length; i++)
        {
            yield return new WaitForSeconds(PATTERN_GAP);
            currentState = currentBossStateArray[i];
            yield return new WaitUntil(() => currentState == BossState.None); // 패턴이 모두 실행되길 기다림

            currentState = BossState.Idle;
            currentCoroutine = null;
        }

        Debug.Log("모든 패턴 종료");
        isDead = true;
    }
    #region 약공격1
    public IEnumerator WeakPattern1()
    {
        yield return new WaitForSeconds(0.5f);
        EndPattern = false;
        Debug.Log(isEnraged ? "약공격1 - 광폭화" : "약공격1 - 기본");
        currentState = BossState.WeakPattern1;


        int randPoS = UnityEngine.Random.Range(0, weak1TeleportPosition.Length);

        transform.position = weak1TeleportPosition[randPoS];
        FacePlayer();

        #region 맵 데이터
        float bottomBound = mapWidthPositions[0].position.y;
        float topBound = mapWidthPositions[1].position.y;
        float mapHeight = topBound - bottomBound;
        float layerHeight = mapHeight / 3f;
        #endregion
        animator.SetTrigger("isNormal");
        float[] layerPositions = new float[3];
        for (int i = 0; i < 3; i++)
        {
            layerPositions[i] = bottomBound + (layerHeight * (i + 0.5f));
        }

        Vector2 leftPosition = new Vector2(mapWidthPositions[0].position.x - 15, 0);
        Vector2 rightPosition = new Vector2(mapWidthPositions[1].position.x + 15, 0);

        int attackCount = isEnraged ? 2 : 2;
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
                int firstLayer = availableLayers[UnityEngine.Random.Range(0, availableLayers.Count)];
                availableLayers.Remove(firstLayer);
                int secondLayer = availableLayers[UnityEngine.Random.Range(0, availableLayers.Count)];

                int[] selectedLayers = { firstLayer, secondLayer };
                List<LineRenderer> warningLines = new List<LineRenderer>();

                foreach (int layer in selectedLayers)
                {
                    float targetY = layerPositions[layer];
                    Vector2 startPosition = new Vector2(leftPosition.x, targetY);
                    Vector2 targetPosition = new Vector2(rightPosition.x, targetY);

                    LineRenderer warningLine = CreateDangerZone(weak1LaserData);
                    warningLine.transform.SetParent(bossPatternPanel.transform, false);
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
                    laser.transform.SetParent(bossPatternPanel.transform, false);
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
                int randomLayer = UnityEngine.Random.Range(0, 3);
                float targetY = layerPositions[randomLayer];

                Vector2 startPosition = new Vector2(leftPosition.x, targetY);
                Vector2 targetPosition = new Vector2(rightPosition.x, targetY);

                LineRenderer warningLine = CreateDangerZone(weak1LaserData);
                warningLine.transform.SetParent(bossPatternPanel.transform, false);
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
                laser.transform.SetParent(bossPatternPanel.transform, false);
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

        int randPoS = UnityEngine.Random.Range(0, weak1TeleportPosition.Length);

        transform.position = weak1TeleportPosition[randPoS];
        FacePlayer();
        animator.SetTrigger("isNormal");
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
                new int[] {3},
                new int[] {2, 4},
                new int[] {1, 5},
                new int[] {0, 6}
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
                    warning.transform.SetParent(bossPatternPanel.transform, false);
                    allWarnings.Add(warning);
                }
                yield return new WaitForSeconds(0.2f);
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
                    laser.transform.SetParent(bossPatternPanel.transform, false);
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
            bool startFrom = UnityEngine.Random.value > 0.5f;
            float startX = startFrom ? rightBound : leftBound;

            // 7개의 경고선 순차 생성
            List<LineRenderer> warningLines = new List<LineRenderer>();
            for (int i = 0; i < 7; i++)
            {
                LineRenderer warningLine = CreateDangerZone(weak2LaserData);
                StartCoroutine(BlinkDangerZone(warningLine));

                warningLine.SetPosition(0, new Vector2(sectionPositions[i], topBound));
                warningLine.SetPosition(1, new Vector2(sectionPositions[i], bottomBound));
                warningLine.transform.SetParent(bossPatternPanel.transform, false);
                warningLines.Add(warningLine);
                yield return new WaitForSeconds(0.2f);
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
                laser.transform.SetParent(bossPatternPanel.transform, false);
                StartCoroutine(laser.FireLaser(
                    new Vector2(sectionPositions[i], topBound),
                    new Vector2(sectionPositions[i], bottomBound)
                ));

                yield return new WaitForSeconds(_wp2LaserDelay);
            }
            yield return new WaitForSeconds(weak2LaserData.LaserLockDuration);
        }
        #endregion
        animator.SetTrigger("isStrong");
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
        float bottomBound = mapWidthPositions[0].position.y - 1;
        float topBound = mapWidthPositions[1].position.y + 1;
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        float centerBound = (leftBound + rightBound) / 2;
        #endregion

        int randPoS = UnityEngine.Random.Range(0, weak1TeleportPosition.Length);

        transform.position = weak1TeleportPosition[randPoS];
        FacePlayer();

        // 레이저 나오는 횟수
        int rand = UnityEngine.Random.Range(2, 4);
        Debug.Log(rand);


        for (int j = 0; j < rand; j++)
        {
            // 짝수번째 우물정자 레이저
            if (j % 2 == 1)
            {
                Debug.Log($"우물정자 레이저");

                // 우물 정자 레이저 생성 (세로 2개, 가로 2개)
                List<LineRenderer> warningLines = new List<LineRenderer>();
                float offset = 3f; // 오프셋 거리

                // 경고선 생성
                for (int i = 0; i < 4; i++)
                {
                    LineRenderer warningLine = CreateDangerZone(weak3LaserData);
                    warningLine.transform.SetParent(bossPatternPanel.transform, false);
                    StartCoroutine(BlinkDangerZone(warningLine));
                    warningLines.Add(warningLine);
                    
                }

                float trackingTime = 0f;

                while (trackingTime < selectedData.LaserFollowDuration)
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

                // 쏠 때까지의 딜레이
                yield return new WaitForSeconds(selectedData.LaserLockDuration);


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
                    laser.SetTargetLayer(selectedData.TargetLayer);
                    laser.transform.SetParent(bossPatternPanel.transform, false);
                    animator.SetTrigger("isNormal");
                    StartCoroutine(laser.FireLaser(
                        startPos,
                        endPos // 경고선의 끝 위치로 발사
                    ));
                }

                // 다음 공격 전 대기
                yield return new WaitForSeconds(selectedData.LaserLockDuration);
            }
            else    // 홀수번째 십자 레이저
            {
                Debug.Log($"십자 레이저");
                List<LineRenderer> warningLines = new List<LineRenderer>();
                for (int i = 0; i < 4; i++)
                {
                    LineRenderer warningLine = CreateDangerZone(weak3LaserData);
                    warningLine.transform.SetParent(bossPatternPanel.transform, false);
                    StartCoroutine(BlinkDangerZone(warningLine));
                    warningLines.Add(warningLine);
                }

                float trackingTime = 0f;
                float trackingDuration = selectedData.LaserFollowDuration;
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

                    trackingTime += Time.deltaTime;
                    yield return null;
                }

                fixedPosition = (Vector2)player.transform.position;
                isPositionFixed = true;
                yield return new WaitForSeconds(selectedData.LaserLockDuration);

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
                    laser.transform.SetParent(bossPatternPanel.transform, false);
                    laser.SetTargetLayer(selectedData.TargetLayer);
                    animator.SetTrigger("isNormal");
                    StartCoroutine(laser.FireLaser(
                        exactStartPos,
                        exactEndPos
                    ));
                }
                // 다음 공격 전 대기
                yield return new WaitForSeconds(selectedData.LaserLockDuration);
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

        int randPoS = UnityEngine.Random.Range(0, weak1TeleportPosition.Length);

        transform.position = weak1TeleportPosition[randPoS];
        FacePlayer();

        // 공격 반복
        int totalAttack = isEnraged ? 8 : 4;
        for (int attackCount = 0; attackCount < totalAttack; attackCount++)
        {
            
            StartCoroutine(WeakPattern4Execute(attackCount));

            float timeBetweenAttacks = isEnraged ? _wp4EnranageDelay : _wp4NormalDelay;
            yield return new WaitForSeconds(timeBetweenAttacks);

        }


        StartCoroutine(FinishPattern());
    }
    private IEnumerator WeakPattern4Execute(int attackCount)
    {            // 현재 플레이어 위치 저장
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
        string[] wp4Clips = { "3-4-1", "3-4-2", "3-4-3" };
        string randomClip = wp4Clips[UnityEngine.Random.Range(0, wp4Clips.Length)];
        SoundManager.Instance.EffectSoundOn(randomClip);
        GameObject projectile = Instantiate(MusicProjectile, targetPosition, Quaternion.identity);
        projectile.transform.SetParent(bossPatternPanel.transform, false);
        StartCoroutine(weak5HitRemove(projectile));
        ProjectileBehaviour behaviour = projectile.GetComponent<ProjectileBehaviour>();
        if (behaviour == null)
        {
            behaviour = projectile.AddComponent<ProjectileBehaviour>();
        }
        behaviour.Initialize(weak4ProjData.Damage, null);


        foreach (Sprite sprite in explosionSprites)
        {
            yield return new WaitForSeconds(explosionDuration/5);
            projectile.GetComponent<SpriteRenderer>().sprite = sprite;
        }
        Destroy(projectile);
        Destroy(projectileController.gameObject);
        #endregion

        
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

        // 보스 위치 조정
        transform.position = new Vector2(rightBound - 16, bottomBound + 4);
        FacePlayer();


        foreach (var list in weak5PosDict.Values)
        {
            foreach (var vec in list)
            {
                StartCoroutine(warningCoroutine(vec));

            }
            yield return new WaitForSeconds(weak5Delay);
        }

        
        StartCoroutine(FinishPattern());
    }
    private IEnumerator warningCoroutine(Vector3 vec)
    {
        // 경고 표시 생성
        GameObject warningObj = Instantiate(warningPrefab);
        warningObj.transform.SetParent(bossPatternPanel.transform, false);
        SpriteRenderer warningRenderer = warningObj.GetComponent<SpriteRenderer>();

        #region 경고 관련
        // 경고 표시 설정
        warningRenderer.color = new Color(1f, 0f, 0f, 0.7f); // 더 진한 빨간색
        warningRenderer.transform.position = vec;
        warningRenderer.sortingOrder = 10; // 레이어 순서를 높여서 확실히 보이게 함

        // 경고 표시 깜빡임
        float warningDuration = isEnraged ? 1.5f : 1.7f;
        float currentTime = 0f;

        while (currentTime < warningDuration)
        {
            float alpha = Mathf.PingPong(currentTime * 3f, 0.7f) + 0.3f; // 최소 알파값 증가
            warningRenderer.color = new Color(1f, 0f, 0f, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }

        Destroy(warningObj);
        #endregion

        #region 폭발 프로젝타일 생성
        ProjectileController projectileController = ProjectileController.Create(
            weak4ProjData,
            transform,
            player.transform,
            MusicProjectile,
            false
        );

        GameObject projectile = Instantiate(MusicProjectile, vec, Quaternion.identity);
        projectile.transform.SetParent(bossPatternPanel.transform, false);
        StartCoroutine(weak5HitRemove(projectile));
        PlayWeakPattern5SfxOnce();
        ProjectileBehaviour behaviour = projectile.GetComponent<ProjectileBehaviour>();
        if (behaviour == null)
        {
            behaviour = projectile.AddComponent<ProjectileBehaviour>();
        }
        behaviour.Initialize(weak4ProjData.Damage, null);

        foreach (Sprite sprite in explosionSprites)
        {
            yield return new WaitForSeconds(explosionDuration / 5);
            projectile.GetComponent<SpriteRenderer>().sprite = sprite;
        }

        ResetPatternFlags();
        Destroy(projectile);
        Destroy(projectileController.gameObject);
        #endregion

    }
    #region 효과음 랜덤 재생
    bool _wp5Played = false;
    public void PlayWeakPattern5SfxOnce()
    {
        if (_wp5Played) return;  // 이미 울렸으면 아무 것도 안 함

        string[] wp5Clips = { "3-5-1", "3-5-2", "3-5-3" };
        string randomClip = wp5Clips[UnityEngine.Random.Range(0, wp5Clips.Length)];
        SoundManager.Instance.EffectSoundOn(randomClip);

        _wp5Played = true;      // 플래그 ON
    }
    void ResetPatternFlags()
    {
        _wp5Played = false;
    }
    #endregion

    private IEnumerator weak5HitRemove(GameObject projectileObject)
    {
        
        yield return new WaitForSeconds(musicProjectileHitTime);
        projectileObject.GetComponent<CircleCollider2D>().enabled = false;
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

    public void OnDesperate()
    {
        isDesperate = true;

        currentState = BossState.None;
        // 현재 나와있는 패턴들 삭제해주기
        RemoveAllPattern();
    }
    #region 발악패턴1
    bool isDesperatePattern1End = false;

    public IEnumerator DesperatePattern1()
    {
        EndPattern = false;
        yield return new WaitForSeconds(0.5f);
        
        Debug.Log("발악패턴1");
        currentState = BossState.DesperatePattern1;

        StartCoroutine(DesperatePattern1_Sub());

        #region 맵 데이터
        float bottomBound = mapWidthPositions[0].position.y - 1;
        float topBound = mapWidthPositions[1].position.y + 1;
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        float centerBound = (leftBound + rightBound) / 2;
        #endregion

        transform.position = new Vector2(centerBound, bottomBound + 8.9f);
        animator.SetTrigger("isNormal");


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
                new int[] {3},
                new int[] {2, 4},
                new int[] {1, 5},
                new int[] {0, 6}
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
                warning.transform.SetParent(bossPatternPanel.transform, false);
                allWarnings.Add(warning);
            }
            yield return new WaitForSeconds(0.2f);
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
                laser.transform.SetParent(bossPatternPanel.transform, false);
                StartCoroutine(laser.FireLaser(
                    new Vector2(sectionPositions[pos], topBound),
                    new Vector2(sectionPositions[pos], bottomBound)
                ));
            }
            yield return new WaitForSeconds(_wp2LaserDelay);
        }

        StopCoroutine(DesperatePattern1_Sub());

        isDesperatePattern1End = true;
        StartCoroutine(FinishPattern());


    }
    public IEnumerator DesperatePattern1_Sub()
    {
        while (!isDesperatePattern1End)
        {


            int randPoS = UnityEngine.Random.Range(0, weak1TeleportPosition.Length);

            transform.position = weak1TeleportPosition[2];
            FacePlayer();

            #region 맵 데이터
            float bottomBound = mapWidthPositions[0].position.y;
            float topBound = mapWidthPositions[1].position.y;
            float mapHeight = topBound - bottomBound;
            float layerHeight = mapHeight / 3f;
            #endregion


            animator.SetTrigger("isNormal");
            float[] layerPositions = new float[3];
            for (int i = 0; i < 3; i++)
            {
                layerPositions[i] = bottomBound + (layerHeight * (i + 0.5f));
            }

            Vector2 leftPosition = new Vector2(mapWidthPositions[0].position.x - 15, 0);
            Vector2 rightPosition = new Vector2(mapWidthPositions[1].position.x + 15, 0);

            {
                // 기본 상태: 1개 레이어 공격 (기존 코드와 동일)
                int randomLayer = UnityEngine.Random.Range(0, 3);
                float targetY = layerPositions[randomLayer];

                Vector2 startPosition = new Vector2(leftPosition.x, targetY);
                Vector2 targetPosition = new Vector2(rightPosition.x, targetY);

                LineRenderer warningLine = CreateDangerZone(desperate1LaserData);
                warningLine.transform.SetParent(bossPatternPanel.transform, false);
                StartCoroutine(BlinkDangerZone(warningLine));
                warningLine.SetPosition(0, startPosition);
                warningLine.SetPosition(1, targetPosition);

                yield return new WaitForSeconds(desperate1LaserData.LaserLockDuration);
                Destroy(warningLine.gameObject);

                LaserController2 laser = LaserController2.Create(
                    desperate1LaserData,
                    startPosition,
                    null
                );
                laser.SetTargetLayer(desperate1LaserData.TargetLayer);
                laser.transform.SetParent(bossPatternPanel.transform, false);
                animator.SetTrigger("isNormal");
                yield return StartCoroutine(laser.FireLaser(startPosition, targetPosition));
            }

            yield return new WaitForSeconds(weak1LaserData.LaserFollowDuration);
            animator.SetTrigger("isStrong");


            yield return null;
        }
    }
    #endregion

    #region 발악패턴2
    bool isDesperatePattern2End = false;
    public IEnumerator DesperatePattern2()
    {
        EndPattern = false;
        yield return new WaitForSeconds(0.5f);


        Debug.Log("발악패턴2");
        StartCoroutine(DesperatePattern2_Sub());


        currentState = BossState.DesperatePattern2;


        // 발악패턴 패턴 리스트에 들어 있는 순서대로
        foreach(var pair in desperate2Position.positionList)
        {
            // 만약 해당하는 패턴이 액티브일때
            if (pair.isActive)
            {
                // 패턴 실행
                foreach(var vec in weak5PosDict[pair.num])
                {
                    StartCoroutine(warningCoroutine(vec));
                }

                yield return new WaitForSeconds(weak5Delay);
            }
        }
        StopCoroutine(DesperatePattern2_Sub());
        isDesperatePattern2End = true;

        StartCoroutine(FinishPattern());
    }
    public IEnumerator DesperatePattern2_Sub()
    {
        while (!isDesperatePattern2End)
        {

            Vector2 targetPosition = player.transform.position;

            // 경고 표시 생성
            GameObject warningObj = new GameObject($"Warning_Object");
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
            string[] wp4Clips = { "3-4-1", "3-4-2", "3-4-3" };
            string randomClip = wp4Clips[UnityEngine.Random.Range(0, wp4Clips.Length)];
            SoundManager.Instance.EffectSoundOn(randomClip);
            GameObject projectile = Instantiate(MusicProjectile, targetPosition, Quaternion.identity);
            projectile.transform.SetParent(bossPatternPanel.transform, false);
            StartCoroutine(weak5HitRemove(projectile));
            ProjectileBehaviour behaviour = projectile.GetComponent<ProjectileBehaviour>();
            if (behaviour == null)
            {
                behaviour = projectile.AddComponent<ProjectileBehaviour>();
            }
            behaviour.Initialize(weak4ProjData.Damage, null);


            foreach (Sprite sprite in explosionSprites)
            {
                yield return new WaitForSeconds(explosionDuration / 5);
                projectile.GetComponent<SpriteRenderer>().sprite = sprite;
            }
            Destroy(projectile);
            Destroy(projectileController.gameObject);
            #endregion



            yield return new WaitForSeconds(desperate2ExplosionObjectDelay);

        }
    }
    #endregion

    #region 발악패턴3
    public IEnumerator DesperatePattern3()
    {
        EndPattern = false;
        yield return new WaitForSeconds(0.5f);
        
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
    //    List<List<(Vector2, Vector2)>> selectedSets = new List<List<(Vector2, Vector2)>>();

    //    // 계획서상 하단 이미지 레이저
    //    List<(Vector2, Vector2)> firstSet = new List<(Vector2, Vector2)>
    //{
    //    (new Vector2(leftBound, topBound), new Vector2(rightBound, bottomBound)),
    //    (new Vector2(rightBound, topBound), new Vector2(leftBound, bottomBound)),
    //    (new Vector2(centerBound - 6, topBound), new Vector2(centerBound + 6, bottomBound)),
    //    (new Vector2(centerBound + 6, topBound), new Vector2(centerBound - 6, bottomBound))
    //};

    //    // 계획서상 상단 이미지 레이저
    //    List<(Vector2, Vector2)> secondSet = new List<(Vector2, Vector2)>
    //{
    //    (new Vector2(centerBound + 14, topBound), new Vector2(centerBound - 14, bottomBound)),
    //    (new Vector2(centerBound - 14, topBound), new Vector2(centerBound + 14, bottomBound)),
    //    (new Vector2(leftBound, centerYBound), new Vector2(rightBound, centerYBound)),
    //    (new Vector2(centerBound, topBound), new Vector2(centerBound, bottomBound))
    //};
        #endregion

        #region 레이저 경고 및 발사 로직
        // 첫 번째와 두 번째 세트에서 랜덤으로 8개의 세트를 선택
        //for (int i = 0; i < 8; i++)
        //{
        //    if (UnityEngine.Random.value < 0.5f)
        //    {
        //        selectedSets.Add(new List<(Vector2, Vector2)>(firstSet));
        //    }
        //    else
        //    {
        //        selectedSets.Add(new List<(Vector2, Vector2)>(secondSet));
        //    }
        //}

        List<int> patternTypeList = new();

        // 카운트 수 만큼
        for (int i = 0; i < desperate3Count; i++)
        {
            int rand = UnityEngine.Random.Range(1, 3);
            patternTypeList.Add(rand);
        }



        // 선택된 세트에 대해 경고선 표시
        foreach (var num in patternTypeList)
        {
            List<LineRenderer> warningLines = new List<LineRenderer>();


            if (num == 1)
            {
                foreach (var vec in desperate3Vec1.vecList)
                {
                    LineRenderer warningLine = CreateDangerZone(desperate3LaserData);
                    warningLine.SetPosition(0, vec.startVec);
                    warningLine.SetPosition(1, vec.endVec);
                    StartCoroutine(BlinkDangerZone(warningLine));
                    warningLines.Add(warningLine);
                }
            }
            else if (num == 2)
            {
                foreach (var vec in desperate3Vec2.vecList)
                {
                    LineRenderer warningLine = CreateDangerZone(desperate3LaserData);
                    warningLine.SetPosition(0, vec.startVec);
                    warningLine.SetPosition(1, vec.endVec);
                    StartCoroutine(BlinkDangerZone(warningLine));
                    warningLines.Add(warningLine);
                }
            }
            else
            {
                Debug.LogError("Error : 허용되지 않은 원소 할당");
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
        foreach (var num in patternTypeList)
        {
            if (num == 1)
            {
                foreach (var vec in desperate3Vec1.vecList)
                {
                    LaserController2 laser = LaserController2.Create(
                        desperate3LaserData,
                        vec.startVec,
                        null
                    );
                    laser.SetTargetLayer(desperate3LaserData.TargetLayer);
                    StartCoroutine(laser.FireLaser(vec.startVec, vec.endVec));
                }
            }
            else if (num == 2)
            {
                foreach (var vec in desperate3Vec2.vecList)
                {
                    LaserController2 laser = LaserController2.Create(
                        desperate3LaserData,
                        vec.startVec,
                        null
                    );
                    laser.SetTargetLayer(desperate3LaserData.TargetLayer);
                    StartCoroutine(laser.FireLaser(vec.startVec, vec.endVec));
                }
            }
            yield return new WaitForSeconds(1.2f); // 세트 간 딜레이
        }
        #endregion

        // 패턴 종료
        //yield return new WaitForSeconds(desperatePattern3Data.AfterAttackDelay);


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
    public void RemoveAllPattern()
    {
        StopAllCoroutines();
        foreach (Transform child in bossPatternPanel)
        {
            Destroy(child.gameObject);
        }
    }
    private bool ready4Desperate = false;
    public void DesperateReady()
    {
        ready4Desperate = true;
        Debug.Log("현재 진행중인 패턴이 끝나면 발악 시작");
    }
}
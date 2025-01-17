using System.Collections;
using System.Collections.Generic;
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
    [Tooltip("그로기 시간 설정")]
    [SerializeField] private float groggyTime = 5f;
    [Tooltip("맵 너비 계산")]
    [SerializeField] private Transform[] mapWidthPositions; // 맵 너비 계산

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

        // 패턴 구현

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern2()
    {
        Debug.Log("약공격2");
        currentState = BossState.WeakPattern2;

        // 패턴 구현

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern3()
    {
        Debug.Log("약공격3");
        currentState = BossState.WeakPattern3;

        // 패턴 구현

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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern2 : MonoBehaviour
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
    public Player player; // Player 타입의 변수를 선언해 참조 가져오기
    private LaserController laserController; // LaserController 참조
    private ProjectileController projectileController; // ProjectileController 참조

    [Header("광폭화 T/F")]
    [SerializeField] private bool isEnraged = false; // Inspector에서 설정 가능

    [Header("ScriptableObject 데이터")]
    [SerializeField] private BossScriptableObject weakPattern1Data;
    [SerializeField] private BossScriptableObject weakPattern2Data;
    [SerializeField] private BossScriptableObject weakPattern3Data;
    [SerializeField] private BossScriptableObject weakPattern4Data;
    [SerializeField] private BossScriptableObject weakPattern5Data;
    [SerializeField] private LaserScriptableObject weakLaserData;
    [SerializeField] private LaserScriptableObject strongLaserData;
    [SerializeField] private ProjectileScriptableObject projectileData;
    [SerializeField] private GameObject captureProjectile; // 속박 투사체 프리팹
    [SerializeField] private GameObject projectile; // 일반 투사체 프리팹
    [SerializeField] private GameObject rainProjectile; // 하늘에서 떨어지는 투사체 프리팹




    void Start()
    {
        //patternDic.Add(0, new BossState[] { BossState.WeakPattern1, BossState.WeakPattern2, BossState.WeakPattern3, BossState.StrongPattern1 });
        //patternDic.Add(1, new BossState[] { BossState.WeakPattern1, BossState.WeakPattern1, BossState.WeakPattern2, BossState.StrongPattern2 });

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

        if (currentState == BossState.Idle && currentCoroutine == null) // 패턴의 조합이 끝나면 다시 Idle()돌려서 패턴 실행하게 해주기
        {
            StartCoroutine(Idle());
        }
    }

    public IEnumerator Idle() // 패턴을 랜덤하게 선택해서 지정해주는 함수
    {
        yield return null;
    }

    public IEnumerator WeakPattern1()
    {
        Debug.Log("약공격1");
        currentState = BossState.WeakPattern1;
        //
        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern2() 
    {
        Debug.Log("약공격2");
        currentState = BossState.WeakPattern2;
        //
        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern3()
    {
        Debug.Log("약공격3");
        currentState = BossState.WeakPattern3;
        //
        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern4()
    {
        Debug.Log("약공격4");
        currentState = BossState.WeakPattern4;
        //
        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern5()
    {
        Debug.Log("약공격5");
        currentState = BossState.WeakPattern5;
        //
        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    private void FacePlayer() // 시선
    {
        if (player != null)
        {
            float direction = transform.position.x - player.transform.position.x;
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x) * (direction > 0 ? 1 : -1),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

}

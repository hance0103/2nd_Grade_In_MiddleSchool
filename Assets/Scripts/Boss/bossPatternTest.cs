using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossPatternTest : MonoBehaviour
{
    
    enum BossState
    {
        None,
        Idle,
        WeakPattern1,
        WeakPattern2,
        WeakPattern3,
        StrongPattern1,
        StrongPattern2,
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

    [SerializeField]
    private BossScriptableObject weakPattern1Data; // ScriptableObject 데이터
    [SerializeField]
    private BossScriptableObject weakPattern2Data; 
    [SerializeField]
    private BossScriptableObject weakPattern3Data; 
    [SerializeField]
    private BossScriptableObject strongPattern1Data; 
    [SerializeField]
    private BossScriptableObject strongPattern2Data; 


    void Start()
    {
        patternDic.Add(0, new BossState[] { BossState.WeakPattern1, BossState.WeakPattern2, BossState.WeakPattern3, BossState.StrongPattern1 });
        patternDic.Add(1, new BossState[] { BossState.WeakPattern1, BossState.WeakPattern1, BossState.WeakPattern2, BossState.StrongPattern2 });
        StartCoroutine(Idle());
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == BossState.WeakPattern1 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine (WeakPattern1Teleport());
        }

        if(currentState == BossState.WeakPattern2 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine (WeakPattern2());
        }

        if(currentState == BossState.WeakPattern3 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine (WeakPattern3());
        }

        if(currentState == BossState.StrongPattern1 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(StrongPattern1());
        }

        if( currentState == BossState.StrongPattern2 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(StrongPattern2());
        }

        if(currentState == BossState.Idle && currentCoroutine == null) // 패턴의 조합이 끝나면 다시 Idle()돌려서 패턴 실행하게 해주기
        {
            StartCoroutine(Idle());
        }
    }

    public IEnumerator Idle() // 패턴을 랜덤하게 선택해서 지정해주는 함수
    {
        int patternNum = Random.Range(0, patternDic.Count);
        BossState[] currentPattern = patternDic[patternNum];
        for(int i = 0; i < currentPattern.Length; i++)
        {
            currentState = currentPattern[i];
            yield return new WaitUntil(() => currentState == BossState.None); // currentState가 None이 되기 전까지 멈춤
            //currentCoroutine = null; // 이거 적절히 삽입해서 update문에서 제대로 동작하도록
        }
        yield return null;
    }

    public IEnumerator WeakPattern1Teleport()
    {
        //내용 기입
        StartCoroutine(WeakPattern1PreAttack()); // 다음 코루틴 실행
        yield return null;
    }

    public IEnumerator WeakPattern1PreAttack()
    {
        //내용 기입
        StartCoroutine(WeakPattern1Attacking()); // 다음 코루틴 실행
        yield return null;
    }

    public IEnumerator WeakPattern1Attacking() //플레이어 주변으로 텔레포트 후 근접공격
    {
        Debug.Log("패턴 이름 :: " + weakPattern1Data.PatternName);
        Debug.Log("공격력 :: " + weakPattern1Data.Damage);
        Debug.Log("텔레포트 오프셋 :: " + weakPattern1Data.TeleportOffset);
        Debug.Log("공격 딜레이 :: " + weakPattern1Data.AttackDelay);
        Debug.Log("이동속도 :: " + weakPattern1Data.MoveSpeed);

        // 패턴 로직 - 텔레포트와 공격 등
        //Vector3 targetPosition = player.position + weakPattern1Data.TeleportOffset; // 플레이어 주변으로 텔레포트
        //transform.position = targetPosition;
        yield return new WaitForSeconds(weakPattern1Data.AttackDelay);
        Debug.Log($"근접 공격 시작. 패턴: {weakPattern1Data.PatternName}, 공격력: {weakPattern1Data.Damage}");
        currentState = BossState.None;
        currentCoroutine = null;
        //내용 기입
        StartCoroutine(WeakPattern1PostAttack()); // 다음 코루틴 실행
        yield return null;
    }
    public IEnumerator WeakPattern1PostAttack()
    {
        //내용 기입
        currentState = BossState.None; // 패턴이 종료되었으니 currentState를 None으로
        yield return null;
    }

    public IEnumerator WeakPattern2() //플레이어와 이격된 부분으로 텔레포트 후 레이저 공격
    {
        Debug.Log("패턴 이름 :: " + weakPattern2Data.PatternName);
        Debug.Log("공격력 :: " + weakPattern2Data.Damage);
        Debug.Log("텔레포트 오프셋 :: " + weakPattern2Data.TeleportOffset);
        Debug.Log("공격 딜레이 :: " + weakPattern2Data.AttackDelay);
        Debug.Log("이동속도 :: " + weakPattern2Data.MoveSpeed);

        //Vector3 targetPosition = player.position + patternData.TeleportOffset; // 일정 거리에서 레이저 공격
        //transform.position = targetPosition;
        yield return new WaitForSeconds(weakPattern2Data.AttackDelay);
        FireLaser();
        Debug.Log($"레이저 공격 시작. 패턴: {weakPattern2Data.PatternName}, 공격력: {weakPattern2Data.Damage}");
        currentState = BossState.None;
        yield return null;
    }

    public IEnumerator WeakPattern3() //플레이어 위로 텔레포트 후 내려찍기
    {
        Debug.Log("패턴 이름 :: " + weakPattern3Data.PatternName);
        Debug.Log("공격력 :: " + weakPattern3Data.Damage);
        Debug.Log("텔레포트 오프셋 :: " + weakPattern3Data.TeleportOffset);
        Debug.Log("공격 딜레이 :: " + weakPattern3Data.AttackDelay);
        Debug.Log("이동속도 :: " + weakPattern3Data.MoveSpeed);

        //Vector3 targetPosition = player.position + Vector3.up * weakPattern3Data.TeleportOffset.y; // 플레이어 위에서 내려찍기
        //transform.position = targetPosition;
        yield return new WaitForSeconds(weakPattern3Data.AttackDelay);
        Debug.Log($"내려찍기 공격 시작. 패턴: {weakPattern3Data.PatternName} , 공격력:  {weakPattern3Data.Damage}");
        currentState = BossState.None;
        yield return null;
    }

    public IEnumerator StrongPattern1() //맵 사이드로 텔레포트 후 투사체 공격
    {
        yield return null;
    }

    public IEnumerator StrongPattern2() //카운트 다운이 끝나면 시간을 멈춘 후에 레이저 공격

    {
        Debug.Log("카운트다운 시작");

        // 카운트다운
        for (int i = 3; i > 0; i--)
        {
            Debug.Log("카운트다운: " + i);
            yield return new WaitForSeconds(1f); // 1초씩 카운트다운
        }

        // 시간 정지
        Debug.Log("시간 정지!");
        Time.timeScale = 0; // 시간을 일시적으로 멈춤
        yield return new WaitForSecondsRealtime(2f); // 실제 시간 기준 2초간 대기
        Time.timeScale = 1; // 시간 재개

        // 레이저 공격
        Debug.Log("레이저 공격 시작. 패턴: " + strongPattern2Data.PatternName + ", 공격력: " + strongPattern2Data.Damage);
        FireLaser(); // 레이저 공격 함수

        currentState = BossState.None; // 상태 초기화
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator Teleport()
    {
        yield return null;
    }

    private void FireLaser() //레이저 공격이 약2, 강2공격에 들어 있어서 함수로 구현하는게 맞을까요 아니면 코루틴이 맞나요..?
    {
        // 레이저 공격
        
    }

}

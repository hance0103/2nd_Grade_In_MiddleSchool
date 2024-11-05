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

    public IEnumerator WeakPattern1Attacking()
    {
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

    public IEnumerator WeakPattern2()
    {
        yield return null;
    }

    public IEnumerator WeakPattern3()
    {
        yield return null;
    }

    public IEnumerator StrongPattern1()
    {
        yield return null;
    }

    public IEnumerator StrongPattern2()
    {
        yield return null;
    }

    public IEnumerator Teleport()
    {
        yield return null;
    }
}

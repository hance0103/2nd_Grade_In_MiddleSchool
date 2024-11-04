using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossPatternTest : MonoBehaviour
{
    enum BossState
    {
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
    private Coroutine currentCoroutine; 
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
            currentCoroutine = StartCoroutine (WeakPattern1());
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
    }

    public IEnumerator Idle()
    {
        int patternNum = Random.Range(0, patternDic.Count);
        BossState[] currentPattern = patternDic[patternNum];
        for(int i = 0; i < currentPattern.Length; i++)
        {
            currentState = currentPattern[i];
        }
        yield return null;
    }

    public IEnumerator WeakPattern1()
    {
        StartCoroutine(Teleport());
        //yield return new WaitForSeconds();
        // 이 안에 들어가는 변수들이 많은텐데 어떻게 관리를 할것인가?
        // 스크립터블 오브젝트? 혹은 하드코딩? 혹은 다른 방법
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

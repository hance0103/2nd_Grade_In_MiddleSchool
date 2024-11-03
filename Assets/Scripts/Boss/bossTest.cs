using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossTest : MonoBehaviour
{
    private enum BossState //enum 열거형
    {
        Idle,
        weakPattern1,
        strongPattern2,
        groggy
    }

    private BossState currentState; 


    // Start is called before the first frame update
    void Start()
    {
        currentState = BossState.Idle; //현상태 = boss idle로 초기화
        StartCoroutine(StateCycle()); // 상태사이클 코루틴 시작
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == BossState.Idle)
        {
            Debug.Log("현재 상태: Idle");
        }
        else if (currentState == BossState.weakPattern1)
        {
            Debug.Log("현재 상태: weakPattern1");
        }
        else if (currentState == BossState.strongPattern2)
        {
            Debug.Log("현재 상태: strongPattern2");
        }
        else if (currentState == BossState.groggy)
        {
            Debug.Log("현재 상태: groggy");
        }
    }

    private IEnumerator StateCycle()
    {
        while(true)
        {yield return new WaitForSeconds(1f);
        Debug.Log("코루틴 실행");
            MoveToNextState();
        }
    }
    private void MoveToNextState()
    {
        // 상태를 다음 상태로 순환
        switch (currentState)
        {
            case BossState.Idle:
                currentState = BossState.weakPattern1;
                break;
            case BossState.weakPattern1:
                currentState = BossState.strongPattern2;
                break;
            case BossState.strongPattern2:
                currentState = BossState.groggy;
                break;
            case BossState.groggy:
                currentState = BossState.Idle;
                break;
        }
    }
}

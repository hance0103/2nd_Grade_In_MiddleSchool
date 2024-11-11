using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
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
    public Player player; // Player 타입의 변수를 선언해 참조 가져오기

    // ScriptableObject 데이터
    [SerializeField]
    private BossScriptableObject weakPattern1Data;
    [SerializeField]
    private BossScriptableObject weakPattern2Data; 
    [SerializeField]
    private BossScriptableObject weakPattern3Data; 
    [SerializeField]
    private BossScriptableObject strongPattern1Data; 
    [SerializeField]
    private BossScriptableObject strongPattern2Data;
    [SerializeField]
    private LaserScriptableObject weakLaserData;
    [SerializeField]
    private LaserScriptableObject strongLaserData;

    //[SerializeField]
    //private Animator animator; // 애니메이터 참조 추가

    //[SerializeField]
    //private float rotationSpeed = 5f; // 보스가 플레이어를 바라보는 회전 속도


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
        Debug.Log("약공격1 텔레포트");
        Debug.Log("텔레포트 오프셋 :: " + weakPattern1Data.TeleportOffset);
        Debug.Log("공격 딜레이 :: " + weakPattern1Data.AttackDelay);

        // 텔레포트 전 이펙트나 애니메이션 재생 가능
        //animator?.SetTrigger("StartTeleport");

        // 텔포할 위치 계산
        Vector3 teleportOffset = weakPattern1Data.TeleportOffset;

        // (Random.value는 0과 1 사이의 값을 반환하므로, 0.5f보다 큰 값이면 +x, 그렇지 않으면 -x으로 설정)
        teleportOffset.x = (Random.value > 0.5f) ? teleportOffset.x : -teleportOffset.x;

        // 플레이어 위치 + 텔포 오프셋
        Vector3 targetPosition = player.transform.position + teleportOffset;

        // 적을 텔포시킬 위치로 이동
        transform.position = targetPosition;

        yield return new WaitForSeconds(weakPattern1Data.AttackDelay);
        StartCoroutine(WeakPattern1PreAttack()); // 다음 코루틴 실행
        //yield return null;
    }

    public IEnumerator WeakPattern1PreAttack()
    {
        Debug.Log("약공격1 Pre");
        // 근접 공격을 위한 준비 단계 (필요한 애니메이션 또는 사운드 추가 가능)
        yield return new WaitForSeconds(0.5f); // 준비 시간
        StartCoroutine(WeakPattern1Attacking()); // 다음 코루틴 실행
        //yield return null;
    }

    public IEnumerator WeakPattern1Attacking() //플레이어 주변으로 텔레포트 후 근접공격
    {
        Debug.Log("약공격1 실행");
        Debug.Log("패턴 이름 :: " + weakPattern1Data.PatternName);
        Debug.Log("공격력 :: " + weakPattern1Data.Damage);
        Debug.Log("이동속도 :: " + weakPattern1Data.MoveSpeed);

        // 공격 애니메이션 시작
        //animator?.SetTrigger("Attack");
        //근접공격

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= weakPattern1Data.AttackRange)
        {
            // 공격 판정
            Debug.Log($"근접 공격 시작. 패턴: {weakPattern1Data.PatternName}, 공격력: {weakPattern1Data.Damage}");

            // 공격 이펙트 생성
            //SpawnAttackEffect();

            //player.TakeDamage(weakPattern1Data.Damage);

            // 근접 공격 애니메이션 또는 이펙트 추가 가능
        }
        else
        {
            Debug.Log("플레이어가 공격 범위 밖에 있습니다.");
        }

        currentState = BossState.None;
        currentCoroutine = null;
        //내용 기입
        StartCoroutine(WeakPattern1PostAttack()); // 다음 코루틴 실행
        yield return null;
    }
    public IEnumerator WeakPattern1PostAttack()
    {
        Debug.Log("약공격1 Post");

        // 공격 후 원상태로 돌아가는 애니메이션
        //animator?.SetTrigger("ReturnToIdle");

        // 공격 후 마무리 동작 (예: 후속 애니메이션 또는 이동)
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

        Vector3 targetPosition = player.transform.position + weakPattern2Data.TeleportOffset; // 일정 거리에서 레이저 공격
        transform.position = targetPosition;
        yield return new WaitForSeconds(weakPattern2Data.AttackDelay);
        FireLaser(weakLaserData);
        Debug.Log($"레이저 공격 시작. 패턴: {weakPattern2Data.PatternName}, 공격력: {weakPattern2Data.Damage}");
        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern3() //플레이어 위로 텔레포트 후 내려찍기
    {
        Debug.Log("패턴 이름 :: " + weakPattern3Data.PatternName);
        Debug.Log("공격력 :: " + weakPattern3Data.Damage);
        Debug.Log("텔레포트 오프셋 :: " + weakPattern3Data.TeleportOffset);
        Debug.Log("공격 딜레이 :: " + weakPattern3Data.AttackDelay);
        Debug.Log("이동속도 :: " + weakPattern3Data.MoveSpeed);

        Vector3 targetPosition = player.transform.position + Vector3.up * weakPattern3Data.TeleportOffset.y; // 플레이어 위에서 내려찍기
        transform.position = targetPosition;
        yield return new WaitForSeconds(weakPattern3Data.AttackDelay);
        Debug.Log($"내려찍기 공격 시작. 패턴: {weakPattern3Data.PatternName} , 공격력:  {weakPattern3Data.Damage}");
        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator StrongPattern1() //맵 사이드로 텔레포트 후 투사체 공격
    {
        Debug.Log("패턴 이름 :: " + strongPattern1Data.PatternName);
        Debug.Log("공격력 :: " + strongPattern1Data.Damage);
        Debug.Log("텔레포트 오프셋 :: " + strongPattern1Data.TeleportOffset);

        // 맵 사이드로 텔레포트
        Vector3 sidePosition = new Vector3(Random.Range(-1f, 1f) > 0 ? 10 : -10, transform.position.y, transform.position.z);
        transform.position = sidePosition;

        yield return new WaitForSeconds(strongPattern1Data.AttackDelay);

        // 투사체 발사 로직 추가
        FireProjectile();

        currentState = BossState.None;
        currentCoroutine = null;
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
        FireLaser(strongLaserData); // 레이저 공격 함수

        currentState = BossState.None; // 상태 초기화
        currentCoroutine = null;
        yield return null;
    }


    //레이저 공격 함수 (gpt활용이라 일단 주석처리)

    public void FireLaser(LaserScriptableObject laserData) //레이저 공격-> 약2, 강2공격 / 함수? 코루틴?
    {
    //    if (laserData == null)
    //    {
    //        Debug.LogError("레이저 데이터가 null입니다.");
    //        return;
    //    }

    //    // 레이저 관련 데이터 가져오기
    //    string laserType = laserData.LaserType;
    //    float damage = laserData.Damage;
    //    Vector3 offset = laserData.LaserOffset;
    //    float speed = laserData.LaserSpeed;
    //    float duration = laserData.LaserDuration;

    //    // 레이저의 발사 위치 계산
    //    Vector3 startPosition = transform.position + offset; // 레이저 시작 위치는 오프셋을 고려하여 계산

    //    // 레이저 생성 (프리팹을 사용하여 레이저 생성)
    //    GameObject laser = Instantiate(laserPrefab, startPosition, Quaternion.identity);
    //    laser.name = laserType;  // 레이저 객체에 이름 부여 (선택 사항)

    //    // 레이저 이동 코루틴 실행
    //    StartCoroutine(LaserMove(laser, damage, speed, duration));
    }

    //private IEnumerator LaserMove(GameObject laser, float damage, float speed, float duration)
    //{
    //    // 레이저 이동 시작
    //    float elapsedTime = 0f;
    //    Vector3 targetPosition = laser.transform.position + transform.forward * 10f; // 레이저 이동 목표

    //    while (elapsedTime < duration)
    //    {
    //        laser.transform.position = Vector3.MoveTowards(laser.transform.position, targetPosition, speed * Time.deltaTime);
    //        elapsedTime += Time.deltaTime;
    //        yield return null; // 매 프레임마다 이동 업데이트
    //    }

    //    // 지속 시간 후 레이저 객체를 삭제
    //    Destroy(laser);
    //}

    //강공격 1 투사체
    private void FireProjectile()
    {
        // 투사체 발사 로직 구현
        // ...
    }

    //private void SpawnAttackEffect()
    //{
    //    // 공격 이펙트 생성 로직
    //    // 예: GameObject effect = Instantiate(attackEffectPrefab, attackPoint.position, attackPoint.rotation);
    //}
}

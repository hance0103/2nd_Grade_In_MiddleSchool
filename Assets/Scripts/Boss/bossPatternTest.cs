using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.Pool;

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
    private LaserController laserController; // LaserController 참조
    private ProjectileController projectileController; // ProjectileController 참조

    // ScriptableObject 데이터
    [SerializeField] private BossScriptableObject weakPattern1Data;
    [SerializeField] private BossScriptableObject weakPattern2Data;
    [SerializeField] private BossScriptableObject weakPattern3Data;
    [SerializeField] private BossScriptableObject strongPattern1Data;
    [SerializeField] private BossScriptableObject strongPattern2Data;
    [SerializeField] private LaserScriptableObject weakLaserData;
    [SerializeField] private LaserScriptableObject strongLaserData;
    [SerializeField] private ProjectileScriptableObject projectileData;


    //[SerializeField]
    //private Animator animator; // 애니메이터 참조 추가
    //[SerializeField]
    //private float rotationSpeed = 5f; // 보스가 플레이어를 바라보는 회전 속도

    // 보스의 몸박 데미지 관련 설정
    //[SerializeField]
    //private float contactDamage = 10f;
    //[SerializeField]
    //private float damageRange = 1.5f;
    //[SerializeField]
    //private float damageCooldown = 1f;
    //private float lastDamageTime = 0f;



    void Start()
    {
        patternDic.Add(0, new BossState[] { BossState.WeakPattern1, BossState.WeakPattern2, BossState.WeakPattern3, BossState.StrongPattern1 });
        patternDic.Add(1, new BossState[] { BossState.WeakPattern1, BossState.WeakPattern1, BossState.WeakPattern2, BossState.StrongPattern2 });

        StartCoroutine(Idle());
    }

    // Update is called once per frame
    void Update()
    {
        //ApplyContactDamage(); // Update에서 몸박 데미지 처리

        if (currentState == BossState.WeakPattern1 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(WeakPattern1Teleport());
        }

        if (currentState == BossState.WeakPattern2 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(WeakPattern2());
        }

        if (currentState == BossState.WeakPattern3 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(WeakPattern3());
        }

        if (currentState == BossState.StrongPattern1 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(StrongPattern1());
        }

        if (currentState == BossState.StrongPattern2 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(StrongPattern2());
        }

        if (currentState == BossState.Idle && currentCoroutine == null) // 패턴의 조합이 끝나면 다시 Idle()돌려서 패턴 실행하게 해주기
        {
            StartCoroutine(Idle());
        }
    }

    public IEnumerator Idle() // 패턴을 랜덤하게 선택해서 지정해주는 함수
    {
        int patternNum = Random.Range(0, patternDic.Count);
        BossState[] currentPattern = patternDic[patternNum];
        for (int i = 0; i < currentPattern.Length; i++)
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
        currentState = BossState.WeakPattern1;

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

        yield return new WaitForSeconds(weakPattern1Data.BeforeAttackDelay);
        StartCoroutine(WeakPattern1PreAttack()); // 다음 코루틴 실행
        //yield return null;
    }

    public IEnumerator WeakPattern1PreAttack()
    {
        Debug.Log("약공격1 Pre");
        currentState = BossState.WeakPattern1;

        // 근접 공격을 위한 준비 단계 (필요한 애니메이션 또는 사운드 추가 가능)


        yield return new WaitForSeconds(0.5f); // 준비 시간
        StartCoroutine(WeakPattern1Attacking()); // 다음 코루틴 실행
        //yield return null;
    }

    public IEnumerator WeakPattern1Attacking() //플레이어 주변으로 텔레포트 후 근접공격
    {
        Debug.Log("약공격1 실행");
        currentState = BossState.WeakPattern1;

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
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern2() //플레이어와 이격된 부분으로 텔레포트 후 레이저 공격
    {
        Debug.Log("약공격2");
        currentState = BossState.WeakPattern2;

        //텔레포트
        Vector3 targetPosition = player.transform.position + weakPattern2Data.TeleportOffset; // 일정 거리에서 레이저 공격
        transform.position = targetPosition;
        yield return new WaitForSeconds(weakPattern2Data.BeforeAttackDelay);

        //공격의 방향은 플레이어를 바라보는 방향으로

        //레이저 공격 함수

        Debug.Log($"레이저 공격 시작. 패턴: {weakPattern2Data.PatternName}, 공격력: {weakPattern2Data.Damage}");

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern3() //플레이어 위로 텔레포트 후 내려찍기
    {
        Debug.Log("약공격3");
        currentState = BossState.WeakPattern3;

        // 플레이어 위로 텔레포트
        Vector3 teleportPosition = player.transform.position + Vector3.up * weakPattern3Data.TeleportOffset.y;
        transform.position = teleportPosition;

        // 카운트다운
        for (float i = weakPattern3Data.BeforeAttackDelay; i > 0; i--)
        {
            Debug.Log("카운트다운: " + i);
            yield return new WaitForSeconds(1f); // 1초씩 카운트다운
        }

        Debug.Log($"내려찍기 공격 시작. 패턴: {weakPattern3Data.PatternName} , 공격력:  {weakPattern3Data.Damage}");

        // 내려찍기 시작 위치와 목표 위치 설정
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = player.transform.position;

        ////////////////////////////////////////////////////////////////////////////////////(GPT)
        //Raycast로 지면 감지
        RaycastHit hit;
        float groundY = 0f;
        if (Physics.Raycast(targetPosition + Vector3.up * 10f, Vector3.down, out hit, 20f, LayerMask.GetMask("Ground")))
        {
            groundY = hit.point.y;
            Debug.Log($"지면 감지됨: {groundY}");
        }
        else
        {
            // Raycast가 실패할 경우 기본값 사용
            groundY = 0f; // 또는 원하는 기본 높이값
            Debug.Log("지면이 감지되지 않아 기본값 사용");
        }

        float elapsedTime = 0f;
        float attackDuration = 0.5f;
        float strikeHeight = groundY + 2f; // 지면으로부터 2유닛 위
        // 내려찍기 모션 실행
        while (elapsedTime < attackDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / attackDuration;
            float easedProgress = 1 - Mathf.Pow(1 - progress, 3);

            // 수정된 높이 계산
            float currentHeight = Mathf.Lerp(startPosition.y, groundY, easedProgress);
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            currentPosition.y = currentHeight;

            // 최소 높이 제한
            if (currentPosition.y < groundY)
            {
                currentPosition.y = groundY;
            }

            transform.position = currentPosition;
            yield return null;
        }

        
        // 최종 위치를 지면에 맞춤
        Vector3 finalPosition = transform.position;
        finalPosition.y = groundY;
        transform.position = finalPosition;
        // 착지 효과
        yield return StartCoroutine(CreateStrikeEffect());

        ///////////////////////////////////////////////////////////////////////////////////////


        // 공격 판정
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, weakPattern3Data.AttackRange);
        bool playerHit = false;

        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Player"))
            {
                Debug.Log("플레이어에게 내려찍기 공격 성공!");
                //player.TakeDamage(weakPattern3Data.Damage);
                playerHit = true;
                break;
            }
        }

        if (!playerHit)
        {
            Debug.Log("플레이어가 공격 범위 밖에 있습니다.");
        }

        // 패턴 종료 후 상태 및 코루틴 초기화
        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator StrongPattern1() //맵 사이드로 텔레포트 후 투사체 공격
    {
        Debug.Log("강공격1");
        currentState = BossState.StrongPattern1;

        // 맵 사이드로 텔레포트
        Debug.Log("강공격1 텔레포트");
        float teleportX = Random.value > 0.5f ? -strongPattern1Data.TeleportOffset.x : strongPattern1Data.TeleportOffset.x; //맵 크기 결정 후 s.obj에서 x값 조정 부탁드려요
        Vector3 targetPosition = new Vector3(teleportX, transform.position.y, transform.position.z);
        transform.position = targetPosition;

        yield return new WaitForSeconds(strongPattern1Data.BeforeAttackDelay);

        // 투사체 패턴 시작
        Debug.Log("투사체 패턴 시작");
        


        currentCoroutine = null;
        currentState = BossState.Idle;
        yield return null;
    }

    public IEnumerator StrongPattern2() //카운트 다운이 끝나면 시간을 멈춘 후에 레이저 공격

    {
        Debug.Log("강공격2");
        currentState = BossState.StrongPattern2;

        // 맵 사이드로 텔레포트
        Debug.Log("강공격2 텔레포트");
        float teleportX = Random.value > 0.5f ? -strongPattern2Data.TeleportOffset.x : strongPattern1Data.TeleportOffset.x; //맵 크기 결정 후 s.obj에서 x값 조정 부탁드려요
        Vector3 targetPosition = new Vector3(teleportX, transform.position.y, transform.position.z);
        transform.position = targetPosition;


        //위험지역 표시

        // 카운트다운
        Debug.Log("카운트다운 시작");
        for (float i = strongPattern2Data.BeforeAttackDelay; i > 0; i--)
        {
            Debug.Log("카운트다운: " + i);
            yield return new WaitForSeconds(1f); // 1초씩 카운트다운
        }

        //빨간불로

        // 시간 정지
        Debug.Log("시간 정지!");
        Time.timeScale = 0; // 시간을 일시적으로 멈춤
        yield return new WaitForSecondsRealtime(2f); // 실제 시간 기준 2초간 대기(조정)

        // 레이저 공격
        Debug.Log("레이저 공격 시작. 패턴: " + strongPattern2Data.PatternName + ", 공격력: " + strongPattern2Data.Damage);


        // 시간 재개
        Debug.Log("시간 재개");
        Time.timeScale = 1; 

       

        //레이저 공격 함수


        currentCoroutine = null;
        currentState = BossState.Idle;
        yield return null;
    }

    // 착지 효과 생성
    private IEnumerator CreateStrikeEffect()
    {
        // 카메라 흔들림 효과 (만약 구현되어 있다면)
        //CameraShake.Instance.ShakeCamera(0.5f, 0.5f);

        // 바닥 이펙트 생성 (파티클 시스템이 있다면)
        //if (strikeEffectPrefab != null)
        //{
        //    Instantiate(strikeEffectPrefab, transform.position, Quaternion.identity);
        //}

        // 잠깐의 경직
        yield return new WaitForSeconds(0.2f);
    }
}


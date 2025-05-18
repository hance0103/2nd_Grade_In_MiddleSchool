using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.Pool;
using static DG.Tweening.DOTweenModuleUtils;

public class bossPatternTest : MonoBehaviour
{
    #region enum 선언
    public enum BossState
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
    #endregion

    #region 변수 영역
    private Coroutine currentCoroutine = null;
    private Dictionary<int, BossState[]> patternDic = new();
    public BossState currentState;
    public GameObject player; // Player Ÿ���� ������ ������ ���� ��������
    private LaserController laserController; // LaserController ����
    private ProjectileController projectileController; // ProjectileController ����
    private BossHPManager bossHPManager; // BossHPManager ����
    private bool shouldTriggerEnrage = false;
    private bool isEnrageTriggered = false;
    private bool isDead = false;

    public bool EndPattern = false;

    [Header("광폭화 T/F")]
    [SerializeField] private bool isEnraged = false; // Inspector���� ���� ����

    [Header("시작 전 카운트다운")]
    [SerializeField] private float countDownBeforeStart = 5f;

    [Header("약공격3 공격 딜레이")]
    [SerializeField] private float weakPattern3AttackDuration = 0.5f; //�ð� ��� �ӵ��� �������� ª���� ������

    [Header("Strong Pattern Positions")]
    [SerializeField] private Transform[] strongPatternPositions; // �����ݿ� ��ġ��

    [Header("약공/광폭약공 패턴1")]
    [SerializeField] private BossScriptableObject weakPattern1Data;
    [SerializeField] private BossScriptableObject weakEnraged1Data;
   
    [Header("약공/광폭약공 패턴2")]
    [SerializeField] private BossScriptableObject weakPattern2Data;
    [SerializeField] private BossScriptableObject weakEnraged2Data;

    [Header("약공/광폭약공 패턴3")]
    [SerializeField] private BossScriptableObject weakPattern3Data;
    [SerializeField] private BossScriptableObject weakEnraged3Data;
    
    [Header("강공/광폭강공 패턴1")]
    [SerializeField] private BossScriptableObject strongPattern1Data;
    [SerializeField] private BossScriptableObject strongEnraged1Data;

    [Header("강공/광폭강공 패턴2")]
    [SerializeField] private BossScriptableObject strongEnraged2Data;
    [SerializeField] private BossScriptableObject strongPattern2Data;

    [Header("공격 스프라이트")]
    [SerializeField] private LaserScriptableObject weakLaserData;
    [SerializeField] private LaserScriptableObject EnragedWeakLaserData;
    [SerializeField] private LaserScriptableObject strongLaserData;
    [SerializeField] private LaserScriptableObject EnrangedLaserData;

    [Header("����ü ������")]
    [SerializeField] private ProjectileScriptableObject projectileData;
    [SerializeField] private ProjectileScriptableObject projectileEData;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject projectileEPrefab; 

    private Animator animator; // �ִϸ����� ���� �߰�
    //[SerializeField] private float rotationSpeed = 5f; // ������ �÷��̾ �ٶ󺸴� ȸ�� �ӵ�

    //// ������ ���� ������ ���� ����
    //[SerializeField] private float contactDamage = 10f;
    //[SerializeField] private float damageRange = 1.5f;
    //[SerializeField] private float damageCooldown = 1f;
    //[SerializeField] private float lastDamageTime = 0f;
    #endregion

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        if (isEnraged == true)
            animator.SetBool("isEnraged", true);
        
        // ���� �� ��ġ ����Ʈ���� �Ҵ�Ǿ����� Ȯ��
        if (strongPatternPositions == null || strongPatternPositions.Length == 0)
        {
            Debug.LogError("Strong pattern positions are not assigned!");
        }

        //patternDic.Add(0, new BossState[] {
        //    BossState.WeakPattern1,
        //    BossState.WeakPattern2,
        //    BossState.WeakPattern3,
        //    BossState.StrongPattern1,
        //    BossState.StrongPattern2
        //});
        patternDic.Add(0, new BossState[] { BossState.WeakPattern1, BossState.WeakPattern3, BossState.WeakPattern1 });
        patternDic.Add(1, new BossState[] { BossState.WeakPattern1, BossState.WeakPattern3, BossState.WeakPattern2 });
        StartCoroutine(BeforeIdle());
    }
    [Header("광폭화 팝업")]
    [SerializeField] private GameObject BossEnragePopup;
    [SerializeField] private BossEnragePopup BossEnragePopupScript;

    private bool Enrageactive = true;
    private void BossEnrage()
    {
        BossEnragePopup.SetActive(true);
        BossEnragePopupScript.OnEnrage();
    }
    // Update is called once per frame
    void Update()
    {
        //ApplyContactDamage(); // Update���� ���� ������ ó��
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
        #region 패턴 실행
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
        
        if (currentState == BossState.Idle && currentCoroutine == null) // ������ ������ ������ �ٽ� Idle()������ ���� �����ϰ� ���ֱ�
        {
            StartCoroutine(Idle());
        }
        #endregion

        #region 광폭화 체크
        if (!isEnraged && BossHPManager.Instance.GetCurrentHP() <= BossHPManager.Instance.GetMaxHP() * 0.5f)
        {
            isEnraged = true;
            animator.SetBool("isEnraged", true);
            // ����ȭ ȿ��
        }

        // 광폭화 조건 확인 - 체력이 50% 이하일 때
        if (!isEnraged && !isEnrageTriggered && BossHPManager.Instance.GetCurrentHP() <= BossHPManager.Instance.GetMaxHP() * 0.5f)
        {
            currentState = BossState.Idle;
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
    private const float PATTERN_GAP = 0.2f;
    private IEnumerator FinishPattern()
    {
        EndPattern = true;
        yield return new WaitForSeconds(PATTERN_GAP);

        currentState = BossState.None;
        currentCoroutine = null;
    }
    
    public IEnumerator Idle() // ������ �����ϰ� �����ؼ� �������ִ� �Լ�
    {
        int patternNum = Random.Range(0, patternDic.Count);
        BossState[] currentPattern = patternDic[patternNum];
        for (int i = 0; i < currentPattern.Length; i++)
        {
            yield return new WaitForSeconds(PATTERN_GAP);
            currentState = currentPattern[i];
            yield return new WaitUntil(() => currentState == BossState.None);
            currentState = BossState.Idle;
            currentCoroutine = null;
        }

        yield return null;
    }


    public IEnumerator BeforeIdle()
    {
        for (float i = countDownBeforeStart; i > 0; i--)
        {
            yield return new WaitForSeconds(1f);
        }
        int patternNum = Random.Range(0, patternDic.Count);
        BossState[] currentPattern = patternDic[patternNum];
        for (int i = 0; i < currentPattern.Length; i++)
        {
            currentState = currentPattern[i];
            yield return new WaitUntil(() => currentState == BossState.None);
            currentState = BossState.Idle;
            currentCoroutine = null;
        }

        yield return null;
    }

    #region 약패턴 1
    public IEnumerator WeakPattern1Teleport()
    {
        yield return new WaitForSeconds(0.5f);
        EndPattern = false;
        Debug.Log("약공격1 텔레포트");
        currentState = BossState.WeakPattern1;

        // ������ ��ġ ���
        Vector3 teleportOffset = weakPattern1Data.TeleportOffset;
        Vector3 playerPos = player.transform.position;

        // ������ �ڷ���Ʈ ��ġ ���
        Vector3 targetPosition = GetSafeTeleportPosition(playerPos, teleportOffset);

        //�ִϸ��̼� ����
        animator.SetTrigger("isWP1");
        animator.SetBool("isPre", true);

        // ���� ������ų ��ġ�� �̵�
        SoundManager.Instance.EffectSoundOn("18");
        transform.position = targetPosition;
        FacePlayer();

        float beforeDelay = isEnraged ? weakEnraged1Data.BeforeAttackDelay : weakPattern1Data.BeforeAttackDelay;
        yield return new WaitForSeconds(beforeDelay);

        StartCoroutine(WeakPattern1PreAttack(playerPos)); // ���� �÷��̾� ��ġ ����
        yield return null;
    }

    public IEnumerator WeakPattern1PreAttack(Vector3 targetPlayerPos)
    {
        Debug.Log("약공격1 Pre");
        currentState = BossState.WeakPattern1;

        // ���� �������θ� ����
        float directionX = (targetPlayerPos.x - transform.position.x);
        float dashDistance = isEnraged ? 7f : 6f; // ���� �Ÿ�

        // ���� ���� ���� (���� �Ǵ� ������)
        float horizontalDirection = Mathf.Sign(directionX);
        Vector3 dashEndPosition = transform.position + new Vector3(horizontalDirection * dashDistance, 0, 0);

        // ���� ���� ǥ�� (����׿�)
        Debug.DrawLine(transform.position, dashEndPosition, Color.red, 0.5f);

        // ���� �ܰ� ����
        StartCoroutine(WeakPattern1Attacking(dashEndPosition));
        yield return null;
    }

    public IEnumerator WeakPattern1Attacking(Vector3 targetPosition) //�÷��̾� �ֺ����� �ڷ���Ʈ �� ��������
    {
        Debug.Log("약공격1 실행");
        currentState = BossState.WeakPattern1;

        //�ִϸ��̼� ����
        animator.SetBool("isPre", false);
        SoundManager.Instance.EffectSoundOn("14");

        // ����ȭ ���¿� ���� ���� �ð� ����
        float dashDuration = isEnraged ? 0.15f : 0.2f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        // �� ���� �������� �־����� üũ�ϴ� ����
        bool hasDamaged = false;
        // ���� �̵�

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / dashDuration;

            // �ε巯�� Ease-in-out �̵�
            float smoothProgress = progress < 0.5f ?
                2f * progress * progress :
                1f - Mathf.Pow(-2f * progress + 2f, 2f) / 2f;

            float newX = Mathf.Lerp(startPosition.x, targetPosition.x, smoothProgress);
            transform.position = new Vector3(newX, startPosition.y, startPosition.z);

            // ���� �� �÷��̾� ���� �� ������(1���� ����)
            if (!hasDamaged)
            {
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
                    transform.position,
                    weakPattern1Data.AttackRange,
                    LayerMask.GetMask("Player")
                );

                foreach (Collider2D hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Player"))
                    {
                        // ����ȭ ���¿��� ������ ��� ����
                        float damage = isEnraged ? weakEnraged1Data.Damage : weakPattern1Data.Damage;
                        Debug.Log($"���� ���� ��Ʈ! ������: {damage}");
                        PlayerHPManager.Instance.TakeDamage(damage);

                        // �������� �� �� �� �ڿ��� �ߺ����� ���� �ʵ��� ó��
                        hasDamaged = true;
                        break;
                    }
                }
            }

            yield return null;
        }

        transform.position = new Vector3(targetPosition.x, startPosition.y, startPosition.z);
        StartCoroutine(WeakPattern1PostAttack());
        yield return null;
    }

    public IEnumerator WeakPattern1PostAttack()
    {
        Debug.Log("약공격1 Post");

        //�ִϸ��̼� ����
        animator.SetBool("isWP1", false);

        // ���� ��� ���� (�ִϸ��̼� ��ȯ ����)
        float afterDelay = isEnraged ? weakEnraged1Data.AfterAttackDelay : weakPattern1Data.AfterAttackDelay;
        yield return new WaitForSeconds(afterDelay);

        yield return StartCoroutine(FinishPattern());
    }
    #endregion

    #region 약패턴 2
    public IEnumerator WeakPattern2() //�÷��̾�� �̰ݵ� �κ����� �ڷ���Ʈ �� ������ ����
    {
        yield return new WaitForSeconds(0.5f);
        EndPattern = false;
        Debug.Log("약공격2");
        currentState = BossState.WeakPattern2;

        // �ڷ���Ʈ
        Vector3 playerPos = player.transform.position;
        Vector3 targetPosition = GetSafeTeleportPosition(playerPos, weakPattern2Data.TeleportOffset);
        SoundManager.Instance.EffectSoundOn("18");
        transform.position = targetPosition;
        FacePlayer();

        //�ִϸ��̼� ����
        animator.SetTrigger("isWP2");
        animator.SetBool("isPre", true);

        // �ڷ���Ʈ ���� ������ �÷��̾��� ��ġ�� ���� (��� �������� �� ��ġ�� ���)
        Vector2 bossPosition = transform.position - new Vector3(0, 0.5f,0);
        Vector2 savedPlayerPosition = new Vector2(
            player.transform.position.x,
            bossPosition.y  // ������ y���� ����Ͽ� ���� ����
        );
        Vector2 direction = (bossPosition - savedPlayerPosition).normalized;
        Vector3 rightPositionMover = new Vector3(1.9f, 0.5f, 0);
        Vector3 leftPositionMover = new Vector3(-1.9f, 0.5f, 0);
        GameObject laserStart;
        LaserController2 laser;

        if (!isEnraged)
        {
            //SoundManager.Instance.EffectSoundOn("16-1");
            if (direction == Vector2.right)
            {
                laserStart = Instantiate(Resources.Load<GameObject>("Prefabs/LaserStart"), transform.position - rightPositionMover, Quaternion.identity);
                Debug.Log(direction);
            }
            else
            {
                laserStart = Instantiate(Resources.Load<GameObject>("Prefabs/LaserStart"), transform.position - leftPositionMover, Quaternion.identity);
                Debug.Log(direction);
            }
            laserStart.GetComponent<SpriteRenderer>().sortingOrder = -1;
            //레이저 시작부분 크기 증가
            yield return StartCoroutine(ScaleUpSprite(laserStart.transform, new Vector3(1.7f, 1.7f, 1f), weakPattern2Data.BeforeAttackDelay));

            laser = LaserController2.Create(weakLaserData, bossPosition, player.transform);
            animator.SetBool("isPre", false);
            SoundManager.Instance.EffectSoundOn("16-2");

            yield return StartCoroutine(laser.FireLaser(bossPosition, savedPlayerPosition));
            
            Destroy(laserStart);
        }
        else 
        {
            //SoundManager.Instance.EffectSoundOn("16-1");
            if (direction == Vector2.right)
            {
                laserStart = Instantiate(Resources.Load<GameObject>("Prefabs/LaserStart_E"), transform.position - rightPositionMover, Quaternion.identity);
                Debug.Log(direction);
            }
            else
            {
                laserStart = Instantiate(Resources.Load<GameObject>("Prefabs/LaserStart_E"), transform.position - leftPositionMover, Quaternion.identity);
                Debug.Log(direction);
            }
            laserStart.GetComponent<SpriteRenderer>().sortingOrder = -1;
            //레이저 시작부분 크기 증가
            yield return StartCoroutine(ScaleUpSprite(laserStart.transform, new Vector3(1.7f, 1.7f, 1f), weakEnraged2Data.BeforeAttackDelay));

            laser = LaserController2.Create(EnragedWeakLaserData, bossPosition, player.transform);
            animator.SetBool("isPre", false);
            SoundManager.Instance.EffectSoundOn("16-2");
            yield return StartCoroutine(laser.FireLaser(bossPosition, savedPlayerPosition));
            
            animator.SetBool("isSecond", true);
            animator.SetBool("isPre", true);


            yield return new WaitForSeconds(0.5f);
            animator.SetBool("isPre", false);

            // ����� ������ ��ġ�� �� ��° ������ �߻�
            laser = LaserController2.Create(EnragedWeakLaserData, bossPosition, player.transform);
            SoundManager.Instance.EffectSoundOn("16-2");
            yield return StartCoroutine(laser.FireLaser(bossPosition, savedPlayerPosition));
            
            animator.SetBool("isSecond", false);
            Destroy(laserStart);

        }

        yield return new WaitForSeconds(weakPattern2Data.AfterAttackDelay);

        //�ִϸ��̼� ����
        Debug.Log("isWP2");
        animator.SetBool("isWP2", false);

        yield return StartCoroutine(FinishPattern());
    }

    private IEnumerator ScaleUpSprite(Transform target, Vector3 targetScale, float duration)
    {
        Vector3 startScale = target.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            target.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        // 최종 크기를 정확히 맞춤
        target.localScale = targetScale;
    }
    #endregion

    #region 약패턴 3
    public IEnumerator WeakPattern3()
    {
        yield return new WaitForSeconds(0.5f);
        EndPattern = false;
        Debug.Log("�����3");
        currentState = BossState.WeakPattern3;

        // 중력 설정 제거 및 속도 초기화
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;

        // 공격 횟수 설정 (광폭화 상태에 따라)
        int attackCount = isEnraged ? 3 : 1;

        for (int strike = 0; strike < attackCount; strike++)
        {
            // 플레이어 위치 텔포
            float targetX = player.transform.position.x;
            Vector3 teleportPosition = new Vector3(targetX,
                weakPattern3Data.TeleportOffset.y,
                transform.position.z);

            SoundManager.Instance.EffectSoundOn("15");
            transform.position = teleportPosition;
            FacePlayer();
            //애니메이션 재생
            animator.SetBool("isWP3", true);

            if (strike == 0)
            {
                // 카운트다운
                for (float i = weakPattern3Data.BeforeAttackDelay; i > 0; i--)
                {
                    Debug.Log("ī��Ʈ�ٿ�: " + i);
                    yield return new WaitForSeconds(0.5f);
                }
            }

            // 광폭화 상태일 때 대기시간 감소
            if (isEnraged)
            {
                yield return new WaitForSeconds(0.2f);
            }

            rb.gravityScale = 1f;
            bool hasDealtDamage = false;  // 한번만 데미지 적용하기 위한 플래그

            // 보스 높이 측정
            Vector3 startPosition = transform.position;
            float bossHeight = GetComponent<Collider2D>()?.bounds.size.y ?? 1f;

            RaycastHit2D groundHit = Physics2D.Raycast(
                 new Vector2(targetX, startPosition.y),
                 Vector2.down,
                 100f,
                 LayerMask.GetMask("Ground")
             );

            float groundY = groundHit.collider != null ?
                groundHit.point.y + (bossHeight / 2) : 0f;

            // 내려찍기
            float elapsedTime = 0f;
            SoundManager.Instance.EffectSoundOn("15-3");

            while (elapsedTime < weakPattern3AttackDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / weakPattern3AttackDuration;
                float easedProgress = 1 - Mathf.Pow(1 - progress, 3);  // ��¡ �������� �ڿ������� ���

                // 이징 함수로 자연스러운 하강 표현
                float currentY = Mathf.Lerp(startPosition.y, groundY, easedProgress);
                Vector3 newPosition = new Vector3(targetX,
                    Mathf.Max(currentY, groundY),
                    transform.position.z);

                // �÷��̾���� ���� �浹 üũ
                if (!hasDealtDamage)
                {
                    ContactFilter2D filter = new ContactFilter2D();
                    filter.SetLayerMask(LayerMask.GetMask("Player"));
                    Collider2D[] results = new Collider2D[1];

                    // ������ �ݶ��̴��� �÷��̾� �ݶ��̴��� ��ġ�� ������ ����
                    if (GetComponent<Collider2D>().OverlapCollider(filter, results) > 0)
                    {
                        if (results[0].CompareTag("Player"))
                        {
                            Debug.Log($"������� ���� ��Ʈ! ������: {weakPattern3Data.Damage}");
                            PlayerHPManager.Instance.TakeDamage(weakPattern3Data.Damage);
                            hasDealtDamage = true;
                        }
                    }
                }
                
                transform.position = newPosition;
                yield return null;
            }

            // 공격 타격 이펙트
            transform.position = new Vector3(targetX, groundY, transform.position.z);
            yield return StartCoroutine(CreateStrikeEffect());

            // 연속 공격을 위한 부양 단계 (마지막 공격이 아닐 경우)
            if (strike < attackCount - 1)
            {
                rb.gravityScale = 0f;
                rb.velocity = Vector2.zero;
                float risingDuration = 1.0f;
                elapsedTime = 0f;

                yield return new WaitForSeconds(0.1f);

                // 부드러운 상승 표현
                while (elapsedTime < risingDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float progress = elapsedTime / risingDuration;
                    float easedProgress = progress < 0.5f ?
                        2f * progress * progress :
                        1f - Mathf.Pow(-2f * progress + 2f, 2f) / 2f;

                    float currentY = Mathf.Lerp(transform.position.y, teleportPosition.y, easedProgress);
                    transform.position = new Vector3(targetX, currentY, transform.position.z);
                    yield return null;
                }

                transform.position = teleportPosition;
                yield return new WaitForSeconds(0.2f);
            }
        }

        // 종료
        yield return new WaitForSeconds(weakPattern3Data.AfterAttackDelay);
        //애니메이션 종료
        animator.SetBool("isWP3", false);
        yield return StartCoroutine(FinishPattern());

    }
    #endregion

    #region 강패턴 1
    public IEnumerator StrongPattern1() //�� ���̵�� �ڷ���Ʈ �� ����ü ����
    {
        yield return new WaitForSeconds(0.5f);
        EndPattern = false;
        Debug.Log("������1");
        currentState = BossState.StrongPattern1;

        // ������ ��ġ ����
        Transform selectedPosition = strongPatternPositions[Random.Range(0, strongPatternPositions.Length)];

        // ���õ� ��ġ�� �ڷ���Ʈ
        Debug.Log("������1 �ڷ���Ʈ");
        SoundManager.Instance.EffectSoundOn("18");
        transform.position = selectedPosition.position;
        FacePlayer();
        float beforeDelay = isEnraged ? strongEnraged1Data.BeforeAttackDelay : strongPattern1Data.BeforeAttackDelay;
        yield return new WaitForSeconds(beforeDelay);

        // ����ü ���� ����
        Debug.Log("����ü ���� ����");

        // ����ȭ ���¿� ���� �ٸ� �����Ϳ� ������ ���
        ProjectileScriptableObject currentProjectileData = isEnraged ? projectileEData : projectileData;
        GameObject currentPrefab = isEnraged ? projectileEPrefab : projectilePrefab;

        ProjectileController projectileController = ProjectileController.Create(currentProjectileData, transform, player.transform, currentPrefab, isEnraged);
        yield return StartCoroutine(projectileController.ExecutePattern(transform));
        Debug.Log("����ü ���� �Ϸ�");

        yield return new WaitForSeconds(strongPattern1Data.AfterAttackDelay);

        yield return StartCoroutine(FinishPattern());
    }
    #endregion

    #region 강패턴 2
    public IEnumerator StrongPattern2() //ī��Ʈ �ٿ��� ������ �ð��� ���� �Ŀ� ������ ����
    {
        yield return new WaitForSeconds(0.5f);
        EndPattern = false;
        Debug.Log("������2");
        currentState = BossState.StrongPattern2;

        // ������ ��ġ ����
        Transform selectedPosition = strongPatternPositions[Random.Range(0, strongPatternPositions.Length)];
        SoundManager.Instance.EffectSoundOn("18");
        transform.position = selectedPosition.position;
        FacePlayer();

        // 레이저 시작부분 조절
        Vector2 bossPosition = transform.position;
        Vector2 staticPlayerPosition = new Vector2(
            player.transform.position.x,
            bossPosition.y
        );
        Vector2 direction = (bossPosition - staticPlayerPosition).normalized;
        if(direction == Vector2.right)
            bossPosition = bossPosition - new Vector2(0.7f, 0);
        else
            bossPosition = bossPosition - new Vector2(-0.7f, 0);
        yield return StartCoroutine(NormalStrongPattern2(bossPosition, staticPlayerPosition));

        yield return StartCoroutine(FinishPattern());
    }

    private IEnumerator NormalStrongPattern2(Vector2 bossPosition, Vector2 staticPlayerPosition)
    {

        // �ӽ� ������ ��Ʈ�ѷ��� ����� ���� ���
        LaserController2 tempLaser = LaserController2.Create(strongLaserData, bossPosition, player.transform);
        Vector2 direction = tempLaser.GetHorizontalDirection(bossPosition, staticPlayerPosition);
        Vector2 endPosition = tempLaser.GetMapEndPoint(bossPosition, direction);
        Destroy(tempLaser.gameObject); // �ӽ� ��Ʈ�ѷ� ����

        // �������� ǥ��
        LineRenderer dangerZone = CreateDangerZone();
        dangerZone.SetPosition(0, bossPosition);
        dangerZone.SetPosition(1, endPosition);

        Coroutine blinkCoroutine = StartCoroutine(BlinkDangerZone(dangerZone));

        // ī��Ʈ�ٿ�
        Debug.Log("ī��Ʈ�ٿ� ����");
        animator.SetBool("isSP2", true);
        animator.SetBool("isPre", true);
        SoundManager.Instance.EffectSoundOn("16-1");

        Vector3 rightPositionMover = new Vector3(-3.5f, 0, 0);
        Vector3 leftPositionMover = new Vector3(3.5f, 0, 0);
        GameObject laserStart;
        

        // 레이저 시작 부분 애니메이션
        if (direction == Vector2.right)
        {
            laserStart = Instantiate(Resources.Load<GameObject>("Prefabs/LaserStart_E"), transform.position - rightPositionMover, Quaternion.identity);
            Debug.Log(direction);
        }
        else
        {
            laserStart = Instantiate(Resources.Load<GameObject>("Prefabs/LaserStart_E"), transform.position - leftPositionMover, Quaternion.identity);
            Debug.Log(direction);
        }
        laserStart.GetComponent<Transform>().localScale = new Vector3(2.6f, 2.6f, 1);
        SpriteRenderer laserStartSR = laserStart.GetComponent<SpriteRenderer>();
        yield return StartCoroutine(ScaleUpSprite(laserStart.transform, new Vector3(5.2f, 5.2f, 1f), weakPattern2Data.BeforeAttackDelay));
        laserStartSR.sortingOrder = -1;

        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        if (dangerZone != null) Destroy(dangerZone.gameObject);


        // 플레이어 멈추는 함수 넣는 위치
        // 여기

        animator.SetBool("isPre", false);
        yield return new WaitForSecondsRealtime(2f);

        LaserController2 laser = LaserController2.Create(strongLaserData, bossPosition, player.transform);

        Debug.Log($"약공격 2 실행: {weakPattern2Data.PatternName}, 약공격2데미지: {weakPattern2Data.Damage}");
        animator.SetBool("isPre", false);
        SoundManager.Instance.EffectSoundOn("16-2");

        yield return StartCoroutine(laser.FireStrongLaser(bossPosition, staticPlayerPosition));

        Destroy(laserStart);
        animator.SetBool("isSP2", false);
        player.GetComponent<Rigidbody2D>().isKinematic = false;
    }

    private IEnumerator EnragedStrongPattern2(Vector2 bossPosition)
    {
        int numberOfLasers = Random.Range(10,16);
        List<LaserController> activeLasers = new List<LaserController>();
        GameObject safeZone = null;

        RaycastHit2D groundHit = Physics2D.Raycast(
            new Vector2(0, Camera.main.orthographicSize),
            Vector2.down,
            Camera.main.orthographicSize * 2,
            LayerMask.GetMask("Ground")
        );

        if (!groundHit.collider)
        {
            Debug.LogError("Ground not found!");
            yield break;
        }

        float groundY = groundHit.point.y;
        float safeZoneX = Random.Range(-9f, 20f);
        float safeZoneWidth = 3f;
        float safeZoneHeight = 5f;
        float heightAboveGround = 1f;

        float safeZoneY = groundY + heightAboveGround + (safeZoneHeight / 2);

        safeZone = CreateSafeZoneVisual(new Vector2(safeZoneX, safeZoneY), safeZoneWidth, safeZoneHeight);

        if (safeZone != null)
        {
            SpriteRenderer safeZoneRenderer = safeZone.GetComponent<SpriteRenderer>();
            safeZoneRenderer.sortingOrder = 1;
            safeZoneRenderer.color = new Color(1, 1, 1, 0.5f);
        }

        // ī��Ʈ�ٿ�
        for (float i = strongEnraged2Data.BeforeAttackDelay; i > 0; i--)
        {
            Debug.Log("����ȭ ī��Ʈ�ٿ�: " + i);
            yield return new WaitForSeconds(1f);
        }

        // ������ ����
        for (int i = 0; i < numberOfLasers; i++)
        {
            float randomX = Random.Range(-9f, 20f);
            Vector2 startPos = new Vector2(randomX, Camera.main.orthographicSize * 2f);
            float widthModifier = Random.Range(-0.5f, 0.5f);
            bool isDiagonal = Random.value > 0.5f;
            Vector2 endPos;

            if (i == numberOfLasers - 1)
            {
                endPos = new Vector2(safeZoneX, groundY);
            }
            else if (isDiagonal)
            {
                // �밢�� ������ �پ��ϰ� ����
                float randomAngle = Random.Range(15f, 75f); // 15������ 75�� ������ ����
                bool isLeft = Random.value > 0.5f; // �¿� ���� ����

                // Y�� �̵���: ���������� groundY����
                float deltaY = startPos.y - groundY;

                // ������ �̿��� X�� �̵��� ���
                float deltaX = deltaY / Mathf.Tan(randomAngle * Mathf.Deg2Rad);
                if (isLeft) deltaX *= -1; // �¿� ���⿡ ���� ��ȣ ����

                // ���� ���
                endPos = new Vector2(startPos.x + deltaX, groundY-1f);
            }
            else
            {
                endPos = new Vector2(randomX, groundY);
            }



            LaserController laser = LaserController.Create(EnrangedLaserData, startPos, player.transform);
            activeLasers.Add(laser);
            StartCoroutine(laser.FireVerticalLaserWithoutFade(laser, startPos, endPos, EnrangedLaserData.LaserWidth + widthModifier));
            yield return new WaitForSeconds(0.2f);
        }

        // ��� �������� �߻�� �� ���� �ð���ŭ ���
        yield return new WaitForSeconds(strongLaserData.LaserDuration);

        // ��� ������ ���ÿ� ���̵�ƿ�
        float fadeOutDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);

            foreach (var laser in activeLasers)
            {
                if (laser != null && laser.gameObject != null)
                {
                    var lineRenderer = laser.GetComponent<LineRenderer>();
                    if (lineRenderer != null)
                    {
                        Color startColor = lineRenderer.startColor;
                        Color endColor = lineRenderer.endColor;
                        startColor.a = alpha;
                        endColor.a = alpha;
                        lineRenderer.startColor = startColor;
                        lineRenderer.endColor = endColor;
                    }
                }
            }
            yield return null;
        }

        // ��� ������ ����
        foreach (var laser in activeLasers)
        {
            if (laser != null && laser.gameObject != null)
            {
                Destroy(laser.gameObject);
            }
        }

        if (safeZone != null)
        {
            Destroy(safeZone);
        }
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

        yield return null;
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

    #region 약공 3 타격 효과
    private IEnumerator CreateStrikeEffect() // 타격 효과 생성
    {
        // 카메라 흔들림 효과 (직접 구현)
        StartCoroutine(ShakeMainCamera(0.2f, 0.2f));

        // 잠시 대기
        yield return new WaitForSeconds(0.2f);

        // 추가 약한 흔들림으로 여진 효과 생성
        StartCoroutine(ShakeMainCamera(0.1f, 0.1f));

        yield return new WaitForSeconds(0.1f);
    }

    // 간단한 카메라 흔들림 구현
    private IEnumerator ShakeMainCamera(float intensity, float duration)
    {
        Vector3 originalPos = Camera.main.transform.position;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * intensity;
            float y = UnityEngine.Random.Range(-1f, 1f) * intensity;

            Camera.main.transform.position = new Vector3(
                originalPos.x + x,
                originalPos.y + y,
                originalPos.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.position = originalPos;
    }
    #endregion

    private void FacePlayer() // �ü�
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

    private LineRenderer CreateDangerZone()
    {
        GameObject dangerZoneObj = new GameObject("DangerZone");
        LineRenderer lineRenderer = dangerZoneObj.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = strongLaserData.LaserWidth+3;  // LaserWidth ���
        lineRenderer.endWidth = strongLaserData.LaserWidth+3;    // LaserWidth ���

        // ������ ������ material ����
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(1f, 0f, 0f, 0.5f); // ������ ������
        lineRenderer.endColor = new Color(1f, 0f, 0f, 0.5f);

        return lineRenderer;
    }

    private GameObject CreateSafeZoneVisual(Vector2 position, float width, float height)
    {
        GameObject safeZone = new GameObject("SafeZone");
        SpriteRenderer spriteRenderer = safeZone.AddComponent<SpriteRenderer>();

        // 100x100 �ȼ� ũ���� �ؽ�ó ���� (Unity�� �⺻ Square�� ����)
        Texture2D texture = new Texture2D(100, 100);
        // �ؽ�ó�� ��� �ȼ��� ������� ����
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, Color.white);
            }
        }
        texture.Apply();

        // PPU�� 100���� �����Ͽ� Unity�� �⺻ Square�� ������ ũ��� ����
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f  // Pixels Per Unit = 100 (Unity �⺻��)
        );
        spriteRenderer.sprite = sprite;

        // ���� ������ ��ġ ����
        safeZone.transform.position = position;
        spriteRenderer.sprite = sprite;
        safeZone.transform.position = position;
        safeZone.transform.localScale = new Vector3(width, height, 1);
        spriteRenderer.color = new Color(1, 1, 1, 0.7f);

        return safeZone;
    }

    private IEnumerator BlinkDangerZone(LineRenderer dangerZone)
    {
        float blinkSpeed = 0.5f; // ������ �ӵ�

        while (dangerZone != null && dangerZone.gameObject != null) // null üũ �߰�
        {
            // ���İ� ������ ������ ȿ��
            if (dangerZone == null) yield break; // ���� ��ġ �߰�

            // Fade out
            for (float t = 0; t < blinkSpeed; t += Time.deltaTime)
            {
                if (dangerZone == null) yield break; // ���� ��ġ �߰�
                float alpha = Mathf.Lerp(0.5f, 0.1f, t / blinkSpeed);
                dangerZone.startColor = new Color(1f, 0f, 0f, alpha);
                dangerZone.endColor = new Color(1f, 0f, 0f, alpha);
                yield return null;
            }

            // Fade in
            for (float t = 0; t < blinkSpeed; t += Time.deltaTime)
            {
                if (dangerZone == null) yield break; // ���� ��ġ �߰�
                float alpha = Mathf.Lerp(0.1f, 0.5f, t / blinkSpeed);
                dangerZone.startColor = new Color(1f, 0f, 0f, alpha);
                dangerZone.endColor = new Color(1f, 0f, 0f, alpha);
                yield return null;
            }
        }
    }

    private Vector3 GetSafeTeleportPosition(Vector3 playerPos, Vector3 desiredOffset)
    {
        // 왼쪽과 오른쪽 위치 계산
        Vector3 leftPosition = new Vector3(
            playerPos.x - Mathf.Abs(desiredOffset.x),
            transform.position.y,
            playerPos.z + desiredOffset.z
        );

        Vector3 rightPosition = new Vector3(
            playerPos.x + Mathf.Abs(desiredOffset.x),
            transform.position.y,
            playerPos.z + desiredOffset.z
        );

        bool leftSafe = false;
        bool rightSafe = false;
        RaycastHit2D hit;
        float checkHeight = 10f;
        // 레이캐스트 경로 표시
        Debug.DrawRay(new Vector3(leftPosition.x, checkHeight, leftPosition.z), Vector2.down * checkHeight * 2, Color.red, 2f);
        Debug.DrawRay(new Vector3(rightPosition.x, checkHeight, rightPosition.z), Vector2.down * checkHeight * 2, Color.blue, 2f);
        // 레이어마스크 설정 준비
        int groundLayer = LayerMask.NameToLayer("Ground");
        int layerMask = 1 << groundLayer;
        // 왼쪽 위치 체크
        Vector2 leftRayStart = new Vector2(leftPosition.x, checkHeight);
        hit = Physics2D.Raycast(leftRayStart, Vector2.down, checkHeight * 2, layerMask);
        if (hit.collider != null)
        {
            leftSafe = true;
            Debug.Log($"왼쪽 레이캐스트 히트: {hit.collider.name}, 레이어: {hit.collider.gameObject.layer}");
        }
        else
        {
            Debug.Log("왼쪽 레이캐스트 미스");
        }
        // 오른쪽 위치 체크
        Vector2 rightRayStart = new Vector2(rightPosition.x, checkHeight);
        hit = Physics2D.Raycast(rightRayStart, Vector2.down, checkHeight * 2, layerMask);
        if (hit.collider != null)
        {
            rightSafe = true;
            Debug.Log($"오른쪽 레이캐스트 히트: {hit.collider.name}, 레이어: {hit.collider.gameObject.layer}");
        }
        else
        {
            Debug.Log("오른쪽 레이캐스트 미스");
        }
        // 결과 반환
        if (leftSafe && rightSafe)
        {
            Debug.Log("양쪽 모두 안전, 랜덤 선택");
            return Random.value > 0.5f ? rightPosition : leftPosition;
        }
        else if (leftSafe)
        {
            Debug.Log("왼쪽만 안전");
            return leftPosition;
        }
        else if (rightSafe)
        {
            Debug.Log("오른쪽만 안전");
            return rightPosition;
        }
        else
        {
            Debug.Log("안전한 위치 없음, 플레이어 주변으로 이동");
            float safeOffset = 2f;
            return new Vector3(
                playerPos.x + (Random.value > 0.5f ? safeOffset : -safeOffset),
                transform.position.y,
                playerPos.z
            );
        }
    }
}
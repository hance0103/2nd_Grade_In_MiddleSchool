using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.Pool;
using static DG.Tweening.DOTweenModuleUtils;

public class bossPatternTest : MonoBehaviour
{
    #region enum 선언
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
    #endregion

    #region 변수 영역
    private Coroutine currentCoroutine = null;
    private Dictionary<int, BossState[]> patternDic = new();
    private BossState currentState;
    public Player player; // Player Ÿ���� ������ ������ ���� ��������
    private LaserController laserController; // LaserController ����
    private ProjectileController projectileController; // ProjectileController ����

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

        patternDic.Add(0, new BossState[] { BossState.WeakPattern2, BossState.WeakPattern2, BossState.WeakPattern2, BossState.WeakPattern2, BossState.WeakPattern2 });
        //patternDic.Add(0, new BossState[] { BossState.StrongPattern2, BossState.StrongPattern2, BossState.StrongPattern2, BossState.StrongPattern2 });
        //patternDic.Add(0, new BossState[] { BossState.WeakPattern3 });

        StartCoroutine(BeforeIdle());
    }

    // Update is called once per frame
    void Update()
    {
        //ApplyContactDamage(); // Update���� ���� ������ ó��

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
        
        if (!isEnraged && BossHPManager.Instance.GetCurrentHP() <= BossHPManager.Instance.GetMaxHP() * 0.5f)
        {
            isEnraged = true;
            animator.SetBool("isEnraged", true);
            // ����ȭ ȿ��
        }
    }
    public IEnumerator Idle() // ������ �����ϰ� �����ؼ� �������ִ� �Լ�
    {
        yield return StartCoroutine(BeforeIdle());

        int patternNum = Random.Range(0, patternDic.Count);
        BossState[] currentPattern = patternDic[patternNum];
        for (int i = 0; i < currentPattern.Length; i++)
        {
            currentState = currentPattern[i];
            yield return new WaitUntil(() => currentState == BossState.None); // currentState�� None�� �Ǳ� ������ ����
            currentState = BossState.Idle;
            currentCoroutine = null; // �̰� ������ �����ؼ� update������ ����� �����ϵ���
        }

        yield return null;
    }


    public IEnumerator BeforeIdle()
    {
        // ī��Ʈ�ٿ�
        for (float i = countDownBeforeStart; i > 0; i--)
        {
            //Debug.Log("ī��Ʈ�ٿ�: " + i);
            yield return new WaitForSeconds(1f);
        }
        int patternNum = Random.Range(0, patternDic.Count);
        BossState[] currentPattern = patternDic[patternNum];
        for (int i = 0; i < currentPattern.Length; i++)
        {
            currentState = currentPattern[i];
            yield return new WaitUntil(() => currentState == BossState.None); // currentState�� None�� �Ǳ� ������ ����
            currentState = BossState.Idle;
            currentCoroutine = null; // �̰� ������ �����ؼ� update������ ����� �����ϵ���
        }

        yield return null;
    }

    #region 약패턴 1
    public IEnumerator WeakPattern1Teleport()
    {
        Debug.Log("약공격1 텔레포트");
        currentState = BossState.WeakPattern1;

        // ������ ��ġ ���
        Vector3 teleportOffset = weakPattern1Data.TeleportOffset;
        Vector3 playerPos = player.transform.position;

        // ������ �ڷ���Ʈ ��ġ ���
        Vector3 targetPosition = GetSafeTeleportPosition(playerPos, teleportOffset);

        //�ִϸ��̼� ����
        animator.SetBool("isWP1", true);
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

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }
    #endregion

    #region 약패턴 2
    public IEnumerator WeakPattern2() //�÷��̾�� �̰ݵ� �κ����� �ڷ���Ʈ �� ������ ����
    {
        Debug.Log("약공격2");
        currentState = BossState.WeakPattern2;

        // �ڷ���Ʈ
        Vector3 playerPos = player.transform.position;
        Vector3 targetPosition = GetSafeTeleportPosition(playerPos, weakPattern2Data.TeleportOffset);
        SoundManager.Instance.EffectSoundOn("18");
        transform.position = targetPosition;
        FacePlayer();

        //�ִϸ��̼� ����
        animator.SetBool("isWP2", true);
        animator.SetBool("isPre", true);

        // �ڷ���Ʈ ���� ������ �÷��̾��� ��ġ�� ���� (��� �������� �� ��ġ�� ���)
        Vector2 bossPosition = transform.position;
        Vector2 savedPlayerPosition = new Vector2(
            player.transform.position.x,
            bossPosition.y  // ������ y���� ����Ͽ� ���� ����
        );
        Vector2 direction = (bossPosition - savedPlayerPosition).normalized;
        Vector3 positionMover = new Vector3(1.5f, 0, 0);
        GameObject laserStart;
        LaserController2 laser;

        if (!isEnraged)
        {
            //SoundManager.Instance.EffectSoundOn("16-1");
            if (direction == Vector2.right)
            {
                laserStart = Instantiate(Resources.Load<GameObject>("Prefabs/LaserStart"), transform.position - positionMover, Quaternion.identity);
                Debug.Log(direction);
            }
            else
            {
                laserStart = Instantiate(Resources.Load<GameObject>("Prefabs/LaserStart"), transform.position + positionMover, Quaternion.identity);
                Debug.Log(direction);
            }
            for (int i = 0; i < 4; i++)
            {
                laserStart.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Sprites/Laser/LaserStart{i}");
                yield return new WaitForSeconds(weakPattern2Data.BeforeAttackDelay / 5f);
            }
            Debug.Log($"약공격 2 실행: {weakPattern2Data.PatternName}, 약공격2데미지: {weakPattern2Data.Damage}");
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
                laserStart = Instantiate(Resources.Load<GameObject>("Prefabs/LaserStart_E"), transform.position - positionMover, Quaternion.identity);
                Debug.Log(direction);
            }
            else
            {
                laserStart = Instantiate(Resources.Load<GameObject>("Prefabs/LaserStart_E"), transform.position + positionMover, Quaternion.identity);
                Debug.Log(direction);
            }
            for (int i = 0; i < 4; i++)
            {
                laserStart.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Sprites/Laser/LaserStart_E{i}");
                yield return new WaitForSeconds(weakPattern2Data.BeforeAttackDelay / 5f);
            }
            Debug.Log($"������ ���� ����. ����: {weakPattern2Data.PatternName}, ���ݷ�: {weakPattern2Data.Damage}");
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

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }
    #endregion

    #region 약패턴 3
    public IEnumerator WeakPattern3()
    {
        Debug.Log("�����3");
        currentState = BossState.WeakPattern3;

        // �߷� ���� ���� �� �ӵ� �ʱ�ȭ
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;

        // ���� Ƚ�� ���� (����ȭ ���¿� ����)
        int attackCount = isEnraged ? 3 : 1;

        for (int strike = 0; strike < attackCount; strike++)
        {
            // �÷��̾� ���� �ڷ���Ʈ
            float targetX = player.transform.position.x;
            Vector3 teleportPosition = new Vector3(targetX,
                player.transform.position.y + weakPattern3Data.TeleportOffset.y,
                transform.position.z);

            SoundManager.Instance.EffectSoundOn("15");
            transform.position = teleportPosition;
            FacePlayer();
            //�ִϸ��̼� ����
            animator.SetBool("isWP3", true);

            if (strike == 0)
            {
                // ī��Ʈ�ٿ�
                for (float i = weakPattern3Data.BeforeAttackDelay; i > 0; i--)
                {
                    Debug.Log("ī��Ʈ�ٿ�: " + i);
                    yield return new WaitForSeconds(1f);
                }
            }

            // ����ȭ ������ �� �����ð� ���
            if (isEnraged)
            {
                yield return new WaitForSeconds(0.2f);
            }

            rb.gravityScale = 1f;
            bool hasDealtDamage = false;  // �ѹ��� ������ �����ϱ� ���� �÷���

            // ���� ���� ���
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

            // ������� ��� ����
            float elapsedTime = 0f;
            SoundManager.Instance.EffectSoundOn("15-3");

            while (elapsedTime < weakPattern3AttackDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / weakPattern3AttackDuration;
                float easedProgress = 1 - Mathf.Pow(1 - progress, 3);  // ��¡ �������� �ڿ������� ���

                // ���ο� ��ġ ���
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

            // ���� Ÿ�� ����Ʈ
            transform.position = new Vector3(targetX, groundY, transform.position.z);
            yield return StartCoroutine(CreateStrikeEffect());

            // ���� ������ ���� ��� ��� (������ ������ �ƴ� ���)
            if (strike < attackCount - 1)
            {
                rb.gravityScale = 0f;
                rb.velocity = Vector2.zero;
                float risingDuration = 1.0f;
                elapsedTime = 0f;

                yield return new WaitForSeconds(0.1f);

                // �ε巯�� ��� ���
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

        // ���� ����
        yield return new WaitForSeconds(weakPattern3Data.AfterAttackDelay);
        //�ִϸ��̼� ����
        animator.SetBool("isWP3", false);
        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }
    #endregion

    #region 강패턴 1
    public IEnumerator StrongPattern1() //�� ���̵�� �ڷ���Ʈ �� ����ü ����
    {
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

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }
    #endregion

    #region 강패턴 2
    public IEnumerator StrongPattern2() //ī��Ʈ �ٿ��� ������ �ð��� ���� �Ŀ� ������ ����
    {
        Debug.Log("������2");
        currentState = BossState.StrongPattern2;

        // ������ ��ġ ����
        Transform selectedPosition = strongPatternPositions[Random.Range(0, strongPatternPositions.Length)];
        Debug.Log("������2 �ڷ���Ʈ");
        SoundManager.Instance.EffectSoundOn("18");
        transform.position = selectedPosition.position;
        FacePlayer();

        // ������ ���� ��ġ�� ��Ȯ�ϰ� ����
        Vector2 bossPosition = transform.position;
        Vector2 staticPlayerPosition = new Vector2(
            player.transform.position.x,
            bossPosition.y
        );

        yield return StartCoroutine(NormalStrongPattern2(bossPosition, staticPlayerPosition));

        //if (isEnraged)
        //{
        //    yield return StartCoroutine(EnragedStrongPattern2(bossPosition));
        //}
        //else
        //{
        //    yield return StartCoroutine(NormalStrongPattern2(bossPosition, staticPlayerPosition));
        //}

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
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

        Coroutine blinkCoroutine = StartCoroutine(BlinkDangerZone(dangerZone));     // �����̴� ȿ���� ����

        // ī��Ʈ�ٿ�
        Debug.Log("ī��Ʈ�ٿ� ����");
        animator.SetBool("isSP2", true);
        animator.SetBool("isPre", true);
        SoundManager.Instance.EffectSoundOn("16-1");
        for (int i = 1; i <= strongPattern2Data.BeforeAttackDelay; i++)
        {
            yield return new WaitForSeconds(1f); // 1�ʾ� ī��Ʈ�ٿ�
        }

        // ������ �ڷ�ƾ ���� �� �������� ǥ�� ����
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        if (dangerZone != null) Destroy(dangerZone.gameObject);

        //�����ҷ�

        // �ð� ����
        Debug.Log("�ð� ����!");
        Time.timeScale = 0;
        animator.SetBool("isPre", false);
        yield return new WaitForSecondsRealtime(2f);
        
        // ����� ��ġ�� ����Ͽ� ������ �߻�
        LaserController2 laser = LaserController2.Create(strongLaserData, bossPosition, player.transform);
        SoundManager.Instance.EffectSoundOn("16-2");
        yield return StartCoroutine(laser.FireStrongLaser(bossPosition, staticPlayerPosition));
        animator.SetBool("isSP2", false);
        Time.timeScale = 1;
    }

    private IEnumerator EnragedStrongPattern2(Vector2 bossPosition)
    {
        int numberOfLasers = Random.Range(10,16);
        List<LaserController> activeLasers = new List<LaserController>();
        GameObject safeZone = null;


        // ���� ��ġ ã��
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

        // ���� ���� ���� (���麸�� �ణ ���� ��ġ)
        float groundY = groundHit.point.y;
        float safeZoneX = Random.Range(-9f, 20f);
        float safeZoneWidth = 3f;
        float safeZoneHeight = 5f;
        float heightAboveGround = 1f; // �������κ����� �Ÿ�

        // ���� ������ �߽��� Y ��ġ�� �������κ��� ������ ���� + heightAboveGround��ŭ ���� ����
        float safeZoneY = groundY + heightAboveGround + (safeZoneHeight / 2);

        // ���� ���� ���� �� ���̸� ������ ��ġ ����
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

    private IEnumerator CreateStrikeEffect() // ���� ȿ�� ����
    {
        // ī�޶� ��鸲 ȿ�� (���� �����Ǿ� �ִٸ�)
        //CameraShake.Instance.ShakeCamera(0.5f, 0.5f);

        // �ٴ� ����Ʈ ���� (��ƼŬ �ý����� �ִٸ�)
        //if (strikeEffectPrefab != null)
        //{
        //    Instantiate(strikeEffectPrefab, transform.position, Quaternion.identity);
        //}

        // ����� ����
        yield return new WaitForSeconds(0.2f);
    }

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
        // ���ʰ� ������ ��ġ ���
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

        // ����� ���� ǥ��
        Debug.DrawRay(new Vector3(leftPosition.x, checkHeight, leftPosition.z), Vector2.down * checkHeight * 2, Color.red, 2f);
        Debug.DrawRay(new Vector3(rightPosition.x, checkHeight, rightPosition.z), Vector2.down * checkHeight * 2, Color.blue, 2f);

        // ���̾��ũ ���� ����
        int groundLayer = LayerMask.NameToLayer("Ground");
        int layerMask = 1 << groundLayer;

        // ���� ��ġ üũ
        Vector2 leftRayStart = new Vector2(leftPosition.x, checkHeight);
        hit = Physics2D.Raycast(leftRayStart, Vector2.down, checkHeight * 2, layerMask);
        if (hit.collider != null)
        {
            leftSafe = true;
            Debug.Log($"���� ����ĳ��Ʈ ��Ʈ: {hit.collider.name}, ���̾�: {hit.collider.gameObject.layer}");
        }
        else
        {
            Debug.Log("���� ����ĳ��Ʈ �̽�");
        }

        // ������ ��ġ üũ
        Vector2 rightRayStart = new Vector2(rightPosition.x, checkHeight);
        hit = Physics2D.Raycast(rightRayStart, Vector2.down, checkHeight * 2, layerMask);
        if (hit.collider != null)
        {
            rightSafe = true;
            Debug.Log($"������ ����ĳ��Ʈ ��Ʈ: {hit.collider.name}, ���̾�: {hit.collider.gameObject.layer}");
        }
        else
        {
            Debug.Log("������ ����ĳ��Ʈ �̽�");
        }

        // ��� ��ȯ
        if (leftSafe && rightSafe)
        {
            Debug.Log("���� ��� ����, ���� ����");
            return Random.value > 0.5f ? rightPosition : leftPosition;
        }
        else if (leftSafe)
        {
            Debug.Log("���ʸ� ����");
            return leftPosition;
        }
        else if (rightSafe)
        {
            Debug.Log("�����ʸ� ����");
            return rightPosition;
        }
        else
        {
            Debug.Log("������ ��ġ ����, �÷��̾� ��ó�� �̵�");
            float safeOffset = 2f;
            return new Vector3(
                playerPos.x + (Random.value > 0.5f ? safeOffset : -safeOffset),
                transform.position.y,
                playerPos.z
            );
        }
    }
}
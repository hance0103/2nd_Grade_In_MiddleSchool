using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.Pool;
using static DG.Tweening.DOTweenModuleUtils;

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

    [Header("광폭화 T/F")]
    [SerializeField] private bool isEnraged = false; // Inspector에서 설정 가능

    [Header("시작 대기 시간")]
    [SerializeField] private float countDownBeforeStart = 5f;

    [Header("약공3 내려찍는 시간")]
    [SerializeField] private float weakPattern3AttackDuration = 0.5f; //시간 길면 속도가 느려지고 짧으면 빨라짐

    [Header("Strong Pattern Positions")]
    [SerializeField] private Transform[] strongPatternPositions; // 강공격용 위치들

    [Header("약공격1 데이터")]
    [SerializeField] private BossScriptableObject weakPattern1Data;
    [SerializeField] private BossScriptableObject weakEnraged1Data;
   
    [Header("약공격2 데이터")]
    [SerializeField] private BossScriptableObject weakPattern2Data;
    [SerializeField] private BossScriptableObject weakEnraged2Data;

    [Header("약공격3 데이터")]
    [SerializeField] private BossScriptableObject weakPattern3Data;
    [SerializeField] private BossScriptableObject weakEnraged3Data;
    
    [Header("강공격1 데이터")]
    [SerializeField] private BossScriptableObject strongPattern1Data;
    [SerializeField] private BossScriptableObject strongEnraged1Data;

    [Header("강공격2 데이터")]
    [SerializeField] private BossScriptableObject strongEnraged2Data;
    [SerializeField] private BossScriptableObject strongPattern2Data;

    [Header("레이저 데이터")]
    [SerializeField] private LaserScriptableObject weakLaserData;
    [SerializeField] private LaserScriptableObject strongLaserData;
    [SerializeField] private LaserScriptableObject EnrangedLaserData;

    [Header("투사체 데이터")]
    [SerializeField] private ProjectileScriptableObject projectileData;
    [SerializeField] private ProjectileScriptableObject projectileEData;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject projectileEPrefab; 

    //[SerializeField] private Animator animator; // 애니메이터 참조 추가
    //[SerializeField] private float rotationSpeed = 5f; // 보스가 플레이어를 바라보는 회전 속도

    //// 보스의 몸박 데미지 관련 설정
    //[SerializeField] private float contactDamage = 10f;
    //[SerializeField] private float damageRange = 1.5f;
    //[SerializeField] private float damageCooldown = 1f;
    //[SerializeField] private float lastDamageTime = 0f;

    void Start()
    {
        // 시작 시 위치 포인트들이 할당되었는지 확인
        if (strongPatternPositions == null || strongPatternPositions.Length == 0)
        {
            Debug.LogError("Strong pattern positions are not assigned!");
        }

        patternDic.Add(0, new BossState[] { BossState.WeakPattern1, BossState.WeakPattern2, BossState.WeakPattern3, BossState.StrongPattern1 });
        patternDic.Add(1, new BossState[] { BossState.WeakPattern1, BossState.WeakPattern1, BossState.WeakPattern2, BossState.StrongPattern2 });
        //patternDic.Add(0, new BossState[] { BossState.WeakPattern3, BossState.WeakPattern3, BossState.WeakPattern3, BossState.WeakPattern3, BossState.WeakPattern3 });

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
        
        if (!isEnraged && BossHPManager.Instance.GetCurrentHP() <= BossHPManager.Instance.GetMaxHP() * 0.5f)
        {
            isEnraged = true;
            // 광폭화 효과
        }
    }
    public IEnumerator Idle() // 패턴을 랜덤하게 선택해서 지정해주는 함수
    {
        yield return StartCoroutine(BeforeIdle());

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


    public IEnumerator BeforeIdle()
    {
        // 카운트다운
        for (float i = countDownBeforeStart; i > 0; i--)
        {
            //Debug.Log("카운트다운: " + i);
            yield return new WaitForSeconds(1f);
        }

        yield return null;
    }

    public IEnumerator WeakPattern1Teleport()
    {
        Debug.Log("약공격1 텔레포트");
        currentState = BossState.WeakPattern1;

        // 텔포할 위치 계산
        Vector3 teleportOffset = weakPattern1Data.TeleportOffset;
        Vector3 playerPos = player.transform.position;

        // 안전한 텔레포트 위치 계산
        Vector3 targetPosition = GetSafeTeleportPosition(playerPos, teleportOffset);

        // 적을 텔포시킬 위치로 이동
        transform.position = targetPosition;
        FacePlayer();

        //animator?.SetTrigger("PreAttackStance"); // 공격 대기 모션 애니메이션

        float beforeDelay = isEnraged ? weakEnraged1Data.BeforeAttackDelay : weakPattern1Data.BeforeAttackDelay;
        yield return new WaitForSeconds(beforeDelay);

        StartCoroutine(WeakPattern1PreAttack(playerPos)); // 현재 플레이어 위치 전달
        yield return null;
    }

    public IEnumerator WeakPattern1PreAttack(Vector3 targetPlayerPos)
    {
        Debug.Log("약공격1 Pre");
        currentState = BossState.WeakPattern1;

        // 수평 방향으로만 돌진
        float directionX = (targetPlayerPos.x - transform.position.x);
        float dashDistance = isEnraged ? 7f : 6f; // 돌진 거리

        // 수평 방향 결정 (왼쪽 또는 오른쪽)
        float horizontalDirection = Mathf.Sign(directionX);
        Vector3 dashEndPosition = transform.position + new Vector3(horizontalDirection * dashDistance, 0, 0);

        // 돌진 방향 표시 (디버그용)
        Debug.DrawLine(transform.position, dashEndPosition, Color.red, 0.5f);

        // 다음 단계 실행
        StartCoroutine(WeakPattern1Attacking(dashEndPosition));
        yield return null;
    }

    public IEnumerator WeakPattern1Attacking(Vector3 targetPosition) //플레이어 주변으로 텔레포트 후 돌진공격
    {
        Debug.Log("약공격1 실행");
        currentState = BossState.WeakPattern1;

        //animator?.SetTrigger("DashAttack"); // 돌진 공격 애니메이션

        // 광폭화 상태에 따라 돌진 시간 조정
        float dashDuration = isEnraged ? 0.15f : 0.2f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        // 한 번만 데미지를 주었는지 체크하는 변수
        bool hasDamaged = false;
        // 돌진 이동

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / dashDuration;

            // 부드러운 Ease-in-out 이동
            float smoothProgress = progress < 0.5f ?
                2f * progress * progress :
                1f - Mathf.Pow(-2f * progress + 2f, 2f) / 2f;

            float newX = Mathf.Lerp(startPosition.x, targetPosition.x, smoothProgress);
            transform.position = new Vector3(newX, startPosition.y, startPosition.z);

            // 돌진 중 플레이어 감지 및 데미지(1번만 적용)
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
                        // 광폭화 상태에서 데미지 배수 적용
                        float damage = isEnraged ? weakEnraged1Data.Damage : weakPattern1Data.Damage;
                        Debug.Log($"돌진 공격 히트! 데미지: {damage}");
                        PlayerHPManager.Instance.TakeDamage(damage);

                        // 데미지를 한 번 준 뒤에는 중복으로 주지 않도록 처리
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

        // 공격 모션 유지 (애니메이션 전환 없음)
        float afterDelay = isEnraged ? weakEnraged1Data.AfterAttackDelay : weakPattern1Data.AfterAttackDelay;
        yield return new WaitForSeconds(afterDelay);

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern2() //플레이어와 이격된 부분으로 텔레포트 후 레이저 공격
    {
        Debug.Log("약공격2");
        currentState = BossState.WeakPattern2;

        // 텔레포트
        Vector3 playerPos = player.transform.position;
        Vector3 targetPosition = GetSafeTeleportPosition(playerPos, weakPattern2Data.TeleportOffset);
        transform.position = targetPosition;
        FacePlayer();

        // 텔레포트 직후 보스와 플레이어의 위치를 저장 (모든 레이저가 이 위치를 사용)
        Vector2 bossPosition = transform.position;
        Vector2 savedPlayerPosition = new Vector2(
            player.transform.position.x,
            bossPosition.y  // 보스의 y값을 사용하여 수평 유지
        );

        yield return new WaitForSeconds(weakPattern2Data.BeforeAttackDelay);

        // 첫 번째 레이저 발사
        Debug.Log($"레이저 공격 시작. 패턴: {weakPattern2Data.PatternName}, 공격력: {weakPattern2Data.Damage}");
        LaserController laser = LaserController.Create(weakLaserData, bossPosition, player.transform);
        yield return StartCoroutine(laser.FireLaser(bossPosition, savedPlayerPosition));

        // 광폭화 상태일 때 두 번째 레이저 공격
        if (isEnraged)
        {
            Debug.Log("광폭화 상태: 같은 위치로 두 번째 레이저 발사");
            yield return new WaitForSeconds(0.5f);

            // 저장된 동일한 위치로 두 번째 레이저 발사
            laser = LaserController.Create(weakLaserData, bossPosition, player.transform);
            yield return StartCoroutine(laser.FireLaser(bossPosition, savedPlayerPosition));
        }

        yield return new WaitForSeconds(weakPattern2Data.AfterAttackDelay);

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern3()
    {
        Debug.Log("약공격3");
        currentState = BossState.WeakPattern3;

        // 중력 영향 제거 및 속도 초기화
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;

        // 공격 횟수 결정 (광폭화 상태에 따라)
        int attackCount = isEnraged ? 3 : 1;

        for (int strike = 0; strike < attackCount; strike++)
        {
            // 플레이어 위로 텔레포트
            float targetX = player.transform.position.x;
            Vector3 teleportPosition = new Vector3(targetX,
                player.transform.position.y + weakPattern3Data.TeleportOffset.y,
                transform.position.z);

            transform.position = teleportPosition;
            FacePlayer();

            if (strike == 0)
            {
                // 카운트다운
                for (float i = weakPattern3Data.BeforeAttackDelay; i > 0; i--)
                {
                    Debug.Log("카운트다운: " + i);
                    yield return new WaitForSeconds(1f);
                }
            }

            // 광폭화 상태일 때 일정시간 대기
            if (isEnraged)
            {
                yield return new WaitForSeconds(0.2f);
            }

            rb.gravityScale = 1f;
            bool hasDealtDamage = false;  // 한번만 데미지 적용하기 위한 플래그

            // 지면 높이 계산
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

            // 내려찍기 모션 실행
            float elapsedTime = 0f;

            while (elapsedTime < weakPattern3AttackDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / weakPattern3AttackDuration;
                float easedProgress = 1 - Mathf.Pow(1 - progress, 3);  // 이징 적용으로 자연스러운 모션

                // 새로운 위치 계산
                float currentY = Mathf.Lerp(startPosition.y, groundY, easedProgress);
                Vector3 newPosition = new Vector3(targetX,
                    Mathf.Max(currentY, groundY),
                    transform.position.z);

                // 플레이어와의 직접 충돌 체크
                if (!hasDealtDamage)
                {
                    ContactFilter2D filter = new ContactFilter2D();
                    filter.SetLayerMask(LayerMask.GetMask("Player"));
                    Collider2D[] results = new Collider2D[1];

                    // 보스의 콜라이더와 플레이어 콜라이더가 겹치면 데미지 적용
                    if (GetComponent<Collider2D>().OverlapCollider(filter, results) > 0)
                    {
                        if (results[0].CompareTag("Player"))
                        {
                            Debug.Log($"내려찍기 공격 히트! 데미지: {weakPattern3Data.Damage}");
                            PlayerHPManager.Instance.TakeDamage(weakPattern3Data.Damage);
                            hasDealtDamage = true;
                        }
                    }
                }

                transform.position = newPosition;
                yield return null;
            }

            // 지면 타격 이펙트
            transform.position = new Vector3(targetX, groundY, transform.position.z);
            yield return StartCoroutine(CreateStrikeEffect());

            // 다음 공격을 위한 상승 모션 (마지막 공격이 아닐 경우)
            if (strike < attackCount - 1)
            {
                rb.gravityScale = 0f;
                rb.velocity = Vector2.zero;
                float risingDuration = 1.0f;
                elapsedTime = 0f;

                yield return new WaitForSeconds(0.1f);

                // 부드러운 상승 모션
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

        // 패턴 종료
        yield return new WaitForSeconds(weakPattern3Data.AfterAttackDelay);
        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }



    public IEnumerator StrongPattern1() //맵 사이드로 텔레포트 후 투사체 공격
    {
        Debug.Log("강공격1");
        currentState = BossState.StrongPattern1;

        // 랜덤한 위치 선택
        Transform selectedPosition = strongPatternPositions[Random.Range(0, strongPatternPositions.Length)];

        // 선택된 위치로 텔레포트
        Debug.Log("강공격1 텔레포트");
        transform.position = selectedPosition.position;
        FacePlayer();
        float beforeDelay = isEnraged ? strongEnraged1Data.BeforeAttackDelay : strongPattern1Data.BeforeAttackDelay;
        yield return new WaitForSeconds(beforeDelay);

        // 투사체 패턴 시작
        Debug.Log("투사체 패턴 시작");

        // 광폭화 상태에 따라 다른 데이터와 프리팹 사용
        ProjectileScriptableObject currentProjectileData = isEnraged ? projectileEData : projectileData;
        GameObject currentPrefab = isEnraged ? projectileEPrefab : projectilePrefab;

        ProjectileController projectileController = ProjectileController.Create(currentProjectileData, transform, player.transform, currentPrefab, isEnraged);
        yield return StartCoroutine(projectileController.ExecutePattern(transform));
        Debug.Log("투사체 패턴 완료");

        yield return new WaitForSeconds(strongPattern1Data.AfterAttackDelay);

        currentState = BossState.Idle;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator StrongPattern2() //카운트 다운이 끝나면 시간을 멈춘 후에 레이저 공격
    {
        Debug.Log("강공격2");
        currentState = BossState.StrongPattern2;

        // 랜덤한 위치 선택
        Transform selectedPosition = strongPatternPositions[Random.Range(0, strongPatternPositions.Length)];
        Debug.Log("강공격2 텔레포트");
        transform.position = selectedPosition.position;
        FacePlayer();

        // 보스의 시작 위치를 정확하게 저장
        Vector2 bossPosition = transform.position;
        Vector2 staticPlayerPosition = new Vector2(
            player.transform.position.x,
            bossPosition.y
        );

        if (isEnraged)
        {
            yield return StartCoroutine(EnragedStrongPattern2(bossPosition));
        }
        else
        {
            yield return StartCoroutine(NormalStrongPattern2(bossPosition, staticPlayerPosition));
        }

        currentState = BossState.Idle;
        currentCoroutine = null;
        yield return null;
    }

    private IEnumerator NormalStrongPattern2(Vector2 bossPosition, Vector2 staticPlayerPosition)
    {
        // 임시 레이저 컨트롤러로 방향과 끝점 계산
        LaserController tempLaser = LaserController.Create(strongLaserData, bossPosition, player.transform);
        Vector2 direction = tempLaser.GetHorizontalDirection(bossPosition, staticPlayerPosition);
        Vector2 endPosition = tempLaser.GetMapEndPoint(bossPosition, direction);
        Destroy(tempLaser.gameObject); // 임시 컨트롤러 제거

        // 위험지역 표시
        LineRenderer dangerZone = CreateDangerZone();
        dangerZone.SetPosition(0, bossPosition);
        dangerZone.SetPosition(1, endPosition);

        Coroutine blinkCoroutine = StartCoroutine(BlinkDangerZone(dangerZone));     // 깜빡이는 효과를 저장

        // 카운트다운
        Debug.Log("카운트다운 시작");
        for (float i = strongPattern2Data.BeforeAttackDelay; i > 0; i--)
        {
            Debug.Log("카운트다운: " + i);
            yield return new WaitForSeconds(1f); // 1초씩 카운트다운
        }

        // 깜빡임 코루틴 정지 후 위험지역 표시 제거
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        if (dangerZone != null) Destroy(dangerZone.gameObject);

        //빨간불로

        // 시간 정지
        Debug.Log("시간 정지!");
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(2f);

        // 저장된 위치를 사용하여 레이저 발사
        LaserController laser = LaserController.Create(strongLaserData, bossPosition, player.transform);
        yield return StartCoroutine(laser.FireStrongLaser(bossPosition, staticPlayerPosition));

        Time.timeScale = 1;
    }

    private IEnumerator EnragedStrongPattern2(Vector2 bossPosition)
    {
        int numberOfLasers = Random.Range(10,16);
        List<LaserController> activeLasers = new List<LaserController>();
        GameObject safeZone = null;


        // 지면 위치 찾기
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

        // 안전 구역 생성 (지면보다 약간 위에 위치)
        float groundY = groundHit.point.y;
        float safeZoneX = Random.Range(-9f, 20f);
        float safeZoneWidth = 3f;
        float safeZoneHeight = 5f;
        float heightAboveGround = 1f; // 지면으로부터의 거리

        // 안전 구역의 중심점 Y 위치를 지면으로부터 높이의 절반 + heightAboveGround만큼 위로 설정
        float safeZoneY = groundY + heightAboveGround + (safeZoneHeight / 2);

        // 안전 구역 생성 시 높이를 고려한 위치 전달
        safeZone = CreateSafeZoneVisual(new Vector2(safeZoneX, safeZoneY), safeZoneWidth, safeZoneHeight);

        if (safeZone != null)
        {
            SpriteRenderer safeZoneRenderer = safeZone.GetComponent<SpriteRenderer>();
            safeZoneRenderer.sortingOrder = 1;
            safeZoneRenderer.color = new Color(1, 1, 1, 0.5f);
        }

        // 카운트다운
        for (float i = strongEnraged2Data.BeforeAttackDelay; i > 0; i--)
        {
            Debug.Log("광폭화 카운트다운: " + i);
            yield return new WaitForSeconds(1f);
        }

        // 레이저 생성
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
                // 대각선 각도를 다양하게 설정
                float randomAngle = Random.Range(15f, 75f); // 15도에서 75도 사이의 각도
                bool isLeft = Random.value > 0.5f; // 좌우 방향 결정

                // Y축 이동량: 시작점에서 groundY까지
                float deltaY = startPos.y - groundY;

                // 각도를 이용한 X축 이동량 계산
                float deltaX = deltaY / Mathf.Tan(randomAngle * Mathf.Deg2Rad);
                if (isLeft) deltaX *= -1; // 좌우 방향에 따라 부호 반전

                // 끝점 계산
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

        // 모든 레이저가 발사된 후 지속 시간만큼 대기
        yield return new WaitForSeconds(strongLaserData.LaserDuration);

        // 모든 레이저 동시에 페이드아웃
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

        // 모든 레이저 제거
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

    private IEnumerator CreateStrikeEffect() // 착지 효과 생성
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

    private LineRenderer CreateDangerZone()
    {
        GameObject dangerZoneObj = new GameObject("DangerZone");
        LineRenderer lineRenderer = dangerZoneObj.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = strongLaserData.LaserWidth;  // LaserWidth 사용
        lineRenderer.endWidth = strongLaserData.LaserWidth;    // LaserWidth 사용

        // 빨간색 반투명 material 설정
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(1f, 0f, 0f, 0.5f); // 빨간색 반투명
        lineRenderer.endColor = new Color(1f, 0f, 0f, 0.5f);

        return lineRenderer;
    }

    private GameObject CreateSafeZoneVisual(Vector2 position, float width, float height)
    {
        GameObject safeZone = new GameObject("SafeZone");
        SpriteRenderer spriteRenderer = safeZone.AddComponent<SpriteRenderer>();

        // 100x100 픽셀 크기의 텍스처 생성 (Unity의 기본 Square와 동일)
        Texture2D texture = new Texture2D(100, 100);
        // 텍스처의 모든 픽셀을 흰색으로 설정
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, Color.white);
            }
        }
        texture.Apply();

        // PPU를 100으로 설정하여 Unity의 기본 Square와 동일한 크기로 만듦
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f  // Pixels Per Unit = 100 (Unity 기본값)
        );
        spriteRenderer.sprite = sprite;

        // 안전 구역의 위치 설정
        safeZone.transform.position = position;
        spriteRenderer.sprite = sprite;
        safeZone.transform.position = position;
        safeZone.transform.localScale = new Vector3(width, height, 1);
        spriteRenderer.color = new Color(1, 1, 1, 0.7f);

        return safeZone;
    }

    private IEnumerator BlinkDangerZone(LineRenderer dangerZone)
    {
        float blinkSpeed = 0.5f; // 깜빡임 속도

        while (dangerZone != null && dangerZone.gameObject != null) // null 체크 추가
        {
            // 알파값 조절로 깜빡임 효과
            if (dangerZone == null) yield break; // 안전 장치 추가

            // Fade out
            for (float t = 0; t < blinkSpeed; t += Time.deltaTime)
            {
                if (dangerZone == null) yield break; // 안전 장치 추가
                float alpha = Mathf.Lerp(0.5f, 0.1f, t / blinkSpeed);
                dangerZone.startColor = new Color(1f, 0f, 0f, alpha);
                dangerZone.endColor = new Color(1f, 0f, 0f, alpha);
                yield return null;
            }

            // Fade in
            for (float t = 0; t < blinkSpeed; t += Time.deltaTime)
            {
                if (dangerZone == null) yield break; // 안전 장치 추가
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

        // 디버그 레이 표시
        Debug.DrawRay(new Vector3(leftPosition.x, checkHeight, leftPosition.z), Vector2.down * checkHeight * 2, Color.red, 2f);
        Debug.DrawRay(new Vector3(rightPosition.x, checkHeight, rightPosition.z), Vector2.down * checkHeight * 2, Color.blue, 2f);

        // 레이어마스크 직접 지정
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
            Debug.Log("안전한 위치 없음, 플레이어 근처로 이동");
            float safeOffset = 2f;
            return new Vector3(
                playerPos.x + (Random.value > 0.5f ? safeOffset : -safeOffset),
                transform.position.y,
                playerPos.z
            );
        }
    }
}
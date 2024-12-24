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

    [Header("광폭화 T/F")]
    [SerializeField] private bool isEnraged = false; // Inspector에서 설정 가능

    [Header("Strong Pattern Positions")]
    [SerializeField] private Transform[] strongPatternPositions; // 강공격용 위치들

    [Header("ScriptableObject 데이터")]
    [SerializeField] private BossScriptableObject weakPattern1Data;
    [SerializeField] private BossScriptableObject weakPattern2Data;
    [SerializeField] private BossScriptableObject weakPattern3Data;
    [SerializeField] private BossScriptableObject strongPattern1Data;
    [SerializeField] private BossScriptableObject strongPattern2Data;
    [SerializeField] private LaserScriptableObject weakLaserData;
    [SerializeField] private LaserScriptableObject strongLaserData;
    [SerializeField] private ProjectileScriptableObject projectileData;
    [SerializeField] private GameObject projectilePrefab; // 투사체 프리팹

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
        Vector3 playerPos = player.transform.position;

        ////// (Random.value는 0과 1 사이의 값을 반환하므로, 0.5f보다 큰 값이면 +x, 그렇지 않으면 -x으로 설정)
        ////teleportOffset.x = (Random.value > 0.5f) ? teleportOffset.x : -teleportOffset.x;

        // 플레이어 위치 + 텔포 오프셋
        ////Vector3 playerPos = player.transform.position;
        ////Vector3 targetPosition = new Vector3(
        ////    playerPos.x + weakPattern1Data.TeleportOffset.x,
        ////    transform.position.y, // 현재 enemy의 y값 유지
        ////    playerPos.z + weakPattern1Data.TeleportOffset.z
        ////);
        // 안전한 텔레포트 위치 계산
        Vector3 targetPosition = GetSafeTeleportPosition(playerPos, teleportOffset);

        // 적을 텔포시킬 위치로 이동
        transform.position = targetPosition;
        FacePlayer();

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

        // 플레이어 위로 텔레포트
        Vector3 teleportPosition = player.transform.position + Vector3.up * weakPattern3Data.TeleportOffset.y;
        transform.position = teleportPosition;
        FacePlayer();

        // 플레이어의 X 위치 저장
        float targetX = player.transform.position.x;

        // 카운트다운
        for (float i = weakPattern3Data.BeforeAttackDelay; i > 0; i--)
        {
            transform.position = teleportPosition;
            Debug.Log("카운트다운: " + i);
            yield return new WaitForSeconds(1f);
        }

        // 공격 횟수 결정 (광폭화 상태에 따라)
        int attackCount = isEnraged ? 3 : 1;

        for (int strike = 0; strike < attackCount; strike++)
        {
            rb.gravityScale = 1f;

            Debug.Log($"내려찍기 공격 {strike + 1}/{attackCount} 시작. 패턴: {weakPattern3Data.PatternName}, 공격력: {weakPattern3Data.Damage}");

            // 시작 위치 저장
            Vector3 startPosition = transform.position;

            // 보스의 콜라이더 크기 가져오기
            float bossHeight = 1f;
            Collider2D bossCollider = GetComponent<Collider2D>();
            if (bossCollider != null)
            {
                bossHeight = bossCollider.bounds.size.y;
            }

            // 2D 레이캐스트로 지면 감지
            RaycastHit2D groundHit = Physics2D.Raycast(
                new Vector2(targetX, startPosition.y),
                Vector2.down,
                100f,
                LayerMask.GetMask("Ground")
            );

            float groundY = 0f;
            if (groundHit.collider != null)
            {
                groundY = groundHit.point.y + (bossHeight / 2);
                Debug.Log($"지면 감지됨: {groundY}, 충돌한 오브젝트: {groundHit.collider.name}");
            }
            else
            {
                Debug.LogWarning("지면이 감지되지 않아 기본값 사용");
            }

            float elapsedTime = 0f;
            float attackDuration = 0.5f;

            // 내려찍기 모션
            while (elapsedTime < attackDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / attackDuration;
                float easedProgress = 1 - Mathf.Pow(1 - progress, 3); // Ease-out cubic

                float currentY = Mathf.Lerp(startPosition.y, groundY, easedProgress);
                Vector3 currentPosition = new Vector3(targetX, currentY, transform.position.z);

                if (currentPosition.y < groundY)
                {
                    currentPosition.y = groundY;
                }

                transform.position = currentPosition;
                yield return null;
            }

            // 최종 위치 설정
            transform.position = new Vector3(targetX, groundY, transform.position.z);

            // 플레이어 감지를 위한 LayerMask
            int playerLayer = LayerMask.NameToLayer("Player");
            int playerMask = 1 << playerLayer;

            // 원형 범위 내 플레이어 감지
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
                transform.position,
                weakPattern3Data.AttackRange,
                playerMask
            );

            // 디버그용 원 그리기
            DrawDebugCircle(transform.position, weakPattern3Data.AttackRange, Color.red, 1f);

            bool playerHit = false;
            foreach (Collider2D hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
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

            // 착지 효과
            yield return StartCoroutine(CreateStrikeEffect());

            // 마지막 공격이 아닌 경우에만 위로 올라가는 모션 실행
            if (strike < attackCount - 1)
            {
                // 위로 올라가는 모션을 위한 설정
                rb.gravityScale = 0f;
                rb.velocity = Vector2.zero;

                float risingDuration = 1.0f; // 올라가는 시간
                elapsedTime = 0f;
                Vector3 groundPosition = transform.position;
                float maxHeight = teleportPosition.y;

                // 상승 시작 시 약간의 딜레이
                yield return new WaitForSeconds(0.1f);

                // 순수하게 수직으로만 올라가는 모션
                while (elapsedTime < risingDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float progress = elapsedTime / risingDuration;

                    // 부드러운 가속과 감속을 위한 이징
                    float easedProgress = progress < 0.5f ?
                        2f * progress * progress :
                        1f - Mathf.Pow(-2f * progress + 2f, 2f) / 2f;

                    // 수직 이동 (처음에는 빠르게, 나중에는 천천히)
                    float currentY = Mathf.Lerp(groundPosition.y, maxHeight, easedProgress);

                    // 현재 위치 업데이트 (X 위치는 고정)
                    transform.position = new Vector3(targetX, currentY, transform.position.z);

                    yield return null;
                }

                // 최종 위치 정확하게 설정
                transform.position = new Vector3(targetX, maxHeight, transform.position.z);

                // 다음 공격 전 준비 시간
                yield return new WaitForSeconds(0.2f);
            }
        }

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

        yield return new WaitForSeconds(strongPattern1Data.BeforeAttackDelay);

        // 투사체 패턴 시작
        Debug.Log("투사체 패턴 시작");
        ProjectileController projectileController = ProjectileController.Create(projectileData, transform, player.transform, projectilePrefab, isEnraged);
        yield return StartCoroutine(projectileController.ExecutePattern(transform));

        currentCoroutine = null;
        currentState = BossState.Idle;
        yield return null;
    }

    public IEnumerator StrongPattern2() //카운트 다운이 끝나면 시간을 멈춘 후에 레이저 공격
    {
        Debug.Log("강공격2");
        currentState = BossState.StrongPattern2;

        // 랜덤한 위치 선택 (StrongPattern1과 같은 위치 배열 사용)
        Transform selectedPosition = strongPatternPositions[Random.Range(0, strongPatternPositions.Length)];

        // 선택된 위치로 텔레포트
        Debug.Log("강공격2 텔레포트");
        transform.position = selectedPosition.position;
        FacePlayer();

        // 보스의 시작 위치를 정확하게 저장
        Vector2 bossPosition = transform.position;
        Vector2 staticPlayerPosition = new Vector2(
            player.transform.position.x,
            bossPosition.y
        );

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
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        if (dangerZone != null)
        {
            Destroy(dangerZone.gameObject);
        }

        //빨간불로

        // 시간 정지
        Debug.Log("시간 정지!");
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(2f);

        // 저장된 위치를 사용하여 레이저 발사
        LaserController laser = LaserController.Create(strongLaserData, bossPosition, player.transform);
        yield return StartCoroutine(laser.FireStrongLaser(bossPosition, staticPlayerPosition));

        // 시간 재개
        Debug.Log("시간 재개");
        Time.timeScale = 1;

        currentState = BossState.Idle;
        currentCoroutine = null;
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

    private Vector2 GetMapEndPoint(Vector2 startPos, Vector2 direction)
    {
        float vertExtent = Camera.main.orthographicSize;
        float horizExtent = vertExtent * Screen.width / Screen.height;

        Vector2 cameraPos = Camera.main.transform.position;
        Rect mapBounds = new Rect(
            cameraPos.x - horizExtent,
            cameraPos.y - vertExtent,
            horizExtent * 2,
            vertExtent * 2
        );

        float maxDistance = Mathf.Max(mapBounds.width, mapBounds.height) * 2;
        return startPos + (direction * maxDistance);
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

    // 디버그용 원 그리기 함수
    private void DrawDebugCircle(Vector3 center, float radius, Color color, float duration)
    {
        int segments = 36;
        float angle = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float currentAngle = angle * i * Mathf.Deg2Rad;
            float nextAngle = angle * (i + 1) * Mathf.Deg2Rad;

            Vector3 currentPoint = center + new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)) * radius;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle)) * radius;

            Debug.DrawLine(currentPoint, nextPoint, color, duration);
        }
    }
}


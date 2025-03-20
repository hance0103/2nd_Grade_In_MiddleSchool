using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossPattern2 : MonoBehaviour
{
    #region enum 선언
    enum BossState
    {
        None,
        Idle,
        WeakPattern1,
        WeakPattern2,
        WeakPattern3,
        WeakPattern4,
        WeakPattern5,
        WeakPattern6,
        Groggy,
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
    public GameObject player; // Player 타입의 변수를 선언해 참조 가져오기
    private LaserController2 laserController; // LaserController 참조
    private ProjectileController projectileController; // ProjectileController 참조
    private bool isPattern6Active = false;
    private Coroutine poisonRainCoroutine = null;

    [Header("보스 기본 설정")]
    [Tooltip("광폭화 설정")]
    [SerializeField] private bool isEnraged = false; // Inspector에서 설정 가능
    [Tooltip("그로기 시간 설정")]
    [SerializeField] private float groggyTime = 5f;
    [Tooltip("맵 너비 계산")]
    [SerializeField] private Transform[] mapWidthPositions; // 맵 너비 계산
    [Header("시작 전 카운트다운")]
    [SerializeField] private float countDownBeforeStart;


    [Header("약공격1 데이터")]
    [SerializeField] private BossScriptableObject weakPattern1Data;
    [SerializeField] private LaserScriptableObject weak1LaserData;

    [Header("약공격2 데이터")]
    [SerializeField] private BossScriptableObject weakPattern2Data;
    
    [Header("약공격3 데이터")]
    [SerializeField] private BossScriptableObject weakPattern3Data;
    [SerializeField] private LaserScriptableObject weak3LaserData;
    [SerializeField] private float weak3AttackCount = 3f;

    [Header("약공격4 데이터")]
    [SerializeField] private BossScriptableObject weakPattern4Data;
    
    [Header("약공격5 데이터")]
    [SerializeField] private BossScriptableObject weakPattern5Data;
    [Tooltip("독비 간격")]
    [SerializeField] private float rainSpaceWeak5 = 1.5f;
    
    [Header("약공격6 (체력패턴) 데이터")]
    [SerializeField] private BossScriptableObject weakPattern6Data;
    [SerializeField] private LaserScriptableObject weak6LaserData;
    [Tooltip("독비 지속 시간")]
    [SerializeField] private float poisonRainDuration = 15f;
    [Tooltip("독비 간격")]
    [SerializeField] private float poisonRainSpacing = 4f;

    [Header("투사체 데이터")]
    [Tooltip("약공1 투사체 데이터")]
    [SerializeField] private ProjectileScriptableObject projectileCapData;
    [SerializeField] private ProjectileScriptableObject projectileData;
    [SerializeField] private ProjectileScriptableObject projectileRainData;
    [SerializeField] private GameObject captureProjectile; // 속박 투사체 프리팹
    [SerializeField] private GameObject Projectile; // 일반 투사체 프리팹
    [SerializeField] private GameObject rainProjectile; // 하늘에서 떨어지는 투사체 프리팹

    private Animator animator;

    private BossHPManager bossHPManager; // BossHPManager ����
    private bool shouldTriggerEnrage = false;
    private bool isEnrageTriggered = false;
    private bool isDead = false;

    #endregion

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        if (isEnraged == true)
            animator.SetBool("isEnraged", true);

        patternDic.Add(0, new BossState[] { 
            BossState.WeakPattern1,
            //BossState.WeakPattern2,
            BossState.WeakPattern3,
            //BossState.WeakPattern4,
            //BossState.WeakPattern5,
            //BossState.WeakPattern6
        });

        if (isEnraged)
        {
            StartContinuousPoisonRain();
        }

        StartCoroutine(BeforeIdle());
    }

   
    void Update()
    {
        // 사망 조건 체크 - 최우선으로 처리
        if (BossHPManager.Instance.GetCurrentHP() <= 0 && !isDead)
        {
            isDead = true;
            StartCoroutine(DeathEffect());
            return; // 다른 업데이트 로직 실행 방지
        }

        #region 보스 상태 체크
        if (currentState == BossState.WeakPattern1 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(WeakPattern1());
        }

        if (currentState == BossState.WeakPattern2 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(WeakPattern2());
        }

        if (currentState == BossState.WeakPattern3 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(WeakPattern3());
        }

        if (currentState == BossState.WeakPattern4 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(WeakPattern4());
        }

        if (currentState == BossState.WeakPattern5 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(WeakPattern5());
        }

        if (currentState == BossState.WeakPattern6 && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(WeakPattern6());
        }

        if (currentState == BossState.Idle && currentCoroutine == null) // 패턴의 조합이 끝나면 다시 Idle()돌려서 패턴 실행하게 해주기
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
    public IEnumerator Idle() 
    {
        int patternNum = Random.Range(0, patternDic.Count);
        BossState[] currentPattern = patternDic[patternNum];
        for (int i = 0; i < currentPattern.Length; i++)
        {
            currentState = currentPattern[i];
            yield return new WaitUntil(() => currentState == BossState.None); // 패턴이 모두 실행되길 기다림
            currentState = BossState.Idle; // Idle에서 다시 새로운 패턴 받아오기
            currentCoroutine = null; // Idle 실행 조건
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
            yield return new WaitUntil(() => currentState == BossState.None); // 패턴이 모두 실행되길 기다림
            currentState = BossState.Idle; // Idle에서 다시 새로운 패턴 받아오기
            currentCoroutine = null; // Idle 실행 조건
        }

        yield return null;
    }
    #region 패턴 1
    public IEnumerator WeakPattern1()
    {
        Debug.Log("약공격1");
        currentState = BossState.WeakPattern1;

        // 보스가 플레이어를 바라보도록 설정
        //FacePlayer();

        // 카운트 다운
        for (float i = weakPattern1Data.BeforeAttackDelay; i > 0; i--)
        {
            Debug.Log("카운트다운: " + i);
            yield return new WaitForSeconds(1f);
        }

        // 1. 속박 탄환 방사형 발사
        Debug.Log("속박 탄환 방사형 발사");
        SoundManager.Instance.EffectSoundOn("23-1");
        ProjectileController projectileController = ProjectileController.Create(
            projectileCapData,
            transform,
            player.transform,
            captureProjectile,
            isEnraged
        );

        // 애니메이션 관련
        animator.SetTrigger("isSpike");
        yield return StartCoroutine(projectileController.ExecuteRadialPattern(transform));

        animator.SetBool("isPre", true);
        animator.SetBool("isLaser", true);

        // 2. 레이저 경고선 표시 및 플레이어 추적
        Debug.Log("추적 경고선");
        LineRenderer warningLine = CreateDangerZone(weak1LaserData);
        warningLine.GetComponent<LineRenderer>().sortingOrder = -1;
        StartCoroutine(BlinkDangerZone(warningLine)); // 깜빡임 효과 시작

        Vector2 fixedPlayerPos = Vector2.zero;
        float elapsed = 0f;

        // 보스의 위치 가져오기
        Vector2 bossStartPosition = transform.position - new Vector3(0, 0.8f, 0);

        // 플레이어 추적 단계
        while (elapsed < weak1LaserData.LaserFollowDuration)
        {
            Vector2 currentPlayerPos = player.transform.position;

            // 경고선 위치 업데이트 (보스에서 플레이어로)
            warningLine.SetPosition(0, bossStartPosition);
            warningLine.SetPosition(1, currentPlayerPos);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 위치 고정 및 발사 준비
        fixedPlayerPos = player.transform.position;

        // 경고선 최종 위치 고정
        warningLine.SetPosition(0, bossStartPosition);
        warningLine.SetPosition(1, fixedPlayerPos);

        yield return new WaitForSeconds(weak1LaserData.LaserLockDuration);

        Destroy(warningLine.gameObject);

        animator.SetBool("isPre", false);

        SoundManager.Instance.EffectSoundOn("24");

        LaserController2 laser = LaserController2.Create(
            weak1LaserData, 
            bossStartPosition, // 보스의 시작 위치
            player.transform
        );

        // 레이저가 타겟 레이어에 충돌하도록 설정
        laser.SetTargetLayer(weak1LaserData.TargetLayer);

        yield return StartCoroutine(laser.FireLaser(bossStartPosition, fixedPlayerPos));

        animator.SetBool("isLaser", false);
        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }
    #endregion 

    #region 패턴 2
    public IEnumerator WeakPattern2() 
    {
        Debug.Log("약공격2");
        currentState = BossState.WeakPattern2;

        // 슬로우 플랫폼? 

        // 카운트 다운
        for (float i = weakPattern2Data.BeforeAttackDelay; i > 0; i--)
        {
            Debug.Log("카운트다운: " + i);
            yield return new WaitForSeconds(1f);
        }

        // 탄환 방사형 발사
        Debug.Log("탄환 방사형 발사!");

        SoundManager.Instance.EffectSoundOn("23-2");

        ProjectileController projectileController = ProjectileController.Create(
            projectileData,
            transform,
            player.transform,
            Projectile,
            isEnraged
        );

        animator.SetTrigger("isSpike");

        yield return StartCoroutine(projectileController.ExecuteRadialPattern(transform));
        yield return new WaitForSeconds(weakPattern2Data.AfterAttackDelay);

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }
    #endregion

    #region 패턴 3
    public IEnumerator WeakPattern3()
    {
        Debug.Log("약공격3");
        currentState = BossState.WeakPattern3;
        Vector2 bossStartPosition = transform.position - new Vector3(0, 0.8f, 0);

        // 레이저 공격 반복 (추적 경고선 + 레이저 공격 Ver.)
        for (int attackCount = 0; attackCount < weak3AttackCount; attackCount++)
        {
            Debug.Log($"레이저 {attackCount + 1}회 공격 시작");

            // 1. 경고선 생성 및 플레이어 추적
            LineRenderer warningLine = CreateDangerZone(weak3LaserData);
            warningLine.GetComponent<LineRenderer>().sortingOrder = -1;
            StartCoroutine(BlinkDangerZone(warningLine));

            animator.SetBool("isPre", true);
            animator.SetBool("isLaser", true);

            // 플레이어 추적 단계
            float trackingTime = 0f;
            while (trackingTime < weak3LaserData.LaserFollowDuration)
            {
                Vector2 playerPosition = player.transform.position;
                warningLine.SetPosition(0, bossStartPosition);
                warningLine.SetPosition(1, playerPosition);
                trackingTime += Time.deltaTime;
                yield return null;
            }

            // 마지막 플레이어 위치 저장
            Vector2 targetPosition = player.transform.position;

            // 발사 전 잠깐의 대기 시간
            yield return new WaitForSeconds(weak3LaserData.LaserLockDuration);
            Destroy(warningLine.gameObject);

            // 2. 레이저 발사
            animator.SetBool("isPre", false);

            LaserController2 laser = LaserController2.Create(
                weak3LaserData,
                bossStartPosition,
                player.transform
            );
            laser.SetTargetLayer(weak3LaserData.TargetLayer);

            // 단일 레이저 발사
            SoundManager.Instance.EffectSoundOn("24");

            yield return StartCoroutine(laser.FireLaser(bossStartPosition, targetPosition));
            animator.SetBool("isLaser", false);
            // 다음 공격 전 대기
            if (attackCount < weak3AttackCount - 1) // 마지막 공격이 아닐 경우에만 대기
            {
                yield return new WaitForSeconds(weak3LaserData.LaserLockDuration);
            }
        }
        

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }
    #endregion

    #region 패턴 4
    public IEnumerator WeakPattern4()
    {
        Debug.Log("약공격4");
        currentState = BossState.WeakPattern4;

        // 카운트 다운
        for (float i = weakPattern4Data.BeforeAttackDelay; i > 0; i--)
        {
            Debug.Log("카운트다운: " + i);
            yield return new WaitForSeconds(1f);
        }

        ProjectileController Controller = ProjectileController.Create(
            projectileData,
            transform,
            player.transform,
            Projectile,
            isEnraged
        );

        //animator.SetTrigger("isSpike");

        yield return StartCoroutine(Controller.ExecuteParallelRadialPattern(transform));

        yield return new WaitForSeconds(weakPattern4Data.AfterAttackDelay);

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }
    #endregion

    #region 패턴 5
    public IEnumerator WeakPattern5()
    {
        Debug.Log("약공격5");
        currentState = BossState.WeakPattern5;

        // 맵 너비 계산
        float leftBound = mapWidthPositions[0].position.x;
        float rightBound = mapWidthPositions[1].position.x;
        float mapWidth = rightBound - leftBound;
        float mapCenter = (leftBound + rightBound) / 2f;
        
        // 카운트 다운
        for (float i = weakPattern5Data.BeforeAttackDelay; i > 0; i--)
        {
            Debug.Log("카운트다운: " + i);
            yield return new WaitForSeconds(1f);
        }

        // 비 패턴 실행을 위한 ProjectileController 생성
        ProjectileController rainController = ProjectileController.Create(
            projectileRainData,
            transform,
            player.transform,
            rainProjectile,
            isEnraged
        );

        #region  (안씀)Player의 너비를 컴포넌트에서 직접 가져오기
        /*float playerWidth = 1f;  // 기본값
        if (player.TryGetComponent<Collider2D>(out Collider2D collider))
        {
            playerWidth = collider.bounds.size.x;
        }
        else if (player.TryGetComponent<SpriteRenderer>(out SpriteRenderer renderer))
        {
            playerWidth = renderer.bounds.size.x;
        }
        float safeZoneWidth = playerWidth * 1.5f;*/
        #endregion

        float safeZoneWidth = rainSpaceWeak5;


        yield return StartCoroutine(rainController.ExecuteWeakPattern5Rain(
        transform,
            mapWidth,
            mapCenter,
            safeZoneWidth,
            leftBound,
            rightBound
        ));

        Debug.Log("약공격5 종료");
        yield return new WaitForSeconds(weakPattern5Data.AfterAttackDelay);

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }
    #endregion

    #region 패턴 6
    public IEnumerator WeakPattern6()
    {
        Debug.Log("약공격6");
        currentState = BossState.WeakPattern6;
        Vector2 bossStartPosition = transform.position - new Vector3(0, 1, 0);
        isPattern6Active = true;  // 패턴 시작

        // 독비 컨트롤러 생성
        ProjectileController poisonRainController = ProjectileController.Create(
            projectileRainData,
            transform,
            player.transform,
            rainProjectile,
            isEnraged
        );

        // 독비와 레이저 공격 동시 실행
        Coroutine poisonRainCoroutine = StartCoroutine(ExecutePoisonRain(poisonRainController));

        // 독비 패턴 시작
        Coroutine continuousRainCoroutine = StartCoroutine(poisonRainController.ExecuteContinuousRainPattern(
            transform,
            30f,  // mapWidth
            poisonRainSpacing,
            new System.Func<bool>(() => isPattern6Active)
        ));

        // 레이저 5회 공격
        LineRenderer warningLine = CreateDangerZone(weak3LaserData);
        StartCoroutine(BlinkDangerZone(warningLine));

        Debug.Log("플레이어 추적 시작");
        float trackingTime = 0f;

        while (trackingTime < weak3LaserData.LaserFollowDuration)
        {
            Vector2 playerPosition = player.transform.position;
            warningLine.SetPosition(0, bossStartPosition);
            warningLine.SetPosition(1, playerPosition);
            trackingTime += Time.deltaTime;
            yield return null;
        }
        Vector2 playerPos = player.transform.position;

        yield return new WaitForSeconds(weak3LaserData.LaserLockDuration);
        Destroy(warningLine.gameObject);

        // 5회 연속 발사
        for (int attack = 0; attack < 5; attack++)
        {
            Debug.Log($"레이저 {attack + 1}회 발사!");
            LaserController2 laser = LaserController2.Create(
                  weak6LaserData,
                  bossStartPosition,
                  player.transform
            );
            laser.SetTargetLayer(weak6LaserData.TargetLayer);

            if (attack == 0)
            {
                SoundManager.Instance.EffectSoundOn("24");
                yield return StartCoroutine(laser.FireLaser(bossStartPosition, playerPos));
            }
            else
            {
                Vector2 targetPos = player.transform.position;
                yield return new WaitForSeconds(0.3f);
                SoundManager.Instance.EffectSoundOn("24");
                yield return StartCoroutine(laser.FireLaser(bossStartPosition, targetPos));
            }

            if (attack < 4)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        // 레이저 패턴 종료 후 독비 중단
        isPattern6Active = false;  // 패턴 종료

        // 마지막 독비가 모두 떨어질 때까지 대기
        if (poisonRainController != null)
        {
            float remainingRainTime = projectileData.ProjectileSpeed > 0
                ? 10f / projectileData.ProjectileSpeed  // 화면 높이를 투사체 속도로 나눔
                : 2f;  // 기본 대기 시간

            yield return new WaitForSeconds(remainingRainTime);
            Destroy(poisonRainController.gameObject);
        }

        // 코루틴 정리
        if (poisonRainCoroutine != null)
            StopCoroutine(poisonRainCoroutine);
        if (continuousRainCoroutine != null)
            StopCoroutine(continuousRainCoroutine);

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
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

    private IEnumerator ExecutePoisonRain(ProjectileController poisonRainController)
    {
        float elapsedTime = 0f;

        while (elapsedTime < poisonRainDuration && isPattern6Active)
        {
            yield return new WaitForSeconds(poisonRainSpacing);
            elapsedTime += poisonRainSpacing;
        }
    }

    public IEnumerator GroggyState()
    {
        Debug.Log("그로기 상태");
        currentState = BossState.Groggy;

        for (float i = groggyTime; i > 0; i--)
        {
            Debug.Log("카운트다운: " + i);
            yield return new WaitForSeconds(1f);
        }

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
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

    private LineRenderer CreateDangerZone(LaserScriptableObject laserData)
    {
        GameObject dangerZoneObj = new GameObject("DangerZone");
        LineRenderer lineRenderer = dangerZoneObj.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = laserData.LaserWidth;
        lineRenderer.endWidth = laserData.LaserWidth;

        Color warningColor = new Color(1f, 0f, 0f, 0.5f);
        // 빨간색 반투명 material 설정
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = warningColor;
        lineRenderer.endColor = warningColor;

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
    private void StartContinuousPoisonRain()
    {
        // Create poison rain controller
        ProjectileController poisonRainController = ProjectileController.Create(
            projectileRainData,
            transform,
            player.transform,
            rainProjectile,
            isEnraged
        );

        // Start continuous rain
        poisonRainCoroutine = StartCoroutine(poisonRainController.ExecuteContinuousRainPattern(
            transform,
            30f,  // mapWidth
            poisonRainSpacing,
            () => true  // Always continue as long as enraged
        ));
    }
}

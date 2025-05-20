using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    [SerializeField]
    private BossState currentState;
    public GameObject player; // Player 타입의 변수를 선언해 참조 가져오기
    private LaserController2 laserController; // LaserController 참조
    private ProjectileController projectileController; // ProjectileController 참조
    private bool isPattern6Active = false;
    private Coroutine poisonRainCoroutine = null;
    public bool EndPattern = false;

    private BossState[] currentBossStateArray = null;

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
    [SerializeField] private float weak1StartAngle;
    [SerializeField] private float weak1EndAngle;
    [SerializeField] private ProjectileScriptableObject weak1projectileData;
    [SerializeField] private float weak1MiddleDelay;

    [Header("약공격2 데이터")]
    [SerializeField] private BossScriptableObject weakPattern2Data;
    [SerializeField] private float weak2StartAngle;
    [SerializeField] private float weak2EndAngle;
    [SerializeField] private ProjectileScriptableObject weak2projectileData;

    [Header("약공격3 데이터")]
    [SerializeField] private BossScriptableObject weakPattern3Data;
    [SerializeField] private LaserScriptableObject weak3LaserData;
    [SerializeField] private float weak3AttackCount = 3f;

    [Header("약공격4 데이터")]
    [SerializeField] private BossScriptableObject weakPattern4Data;
    [SerializeField] private float weak4FStartAngle;
    [SerializeField] private float weak4FEndAngle;
    [SerializeField] private float weak4SStartAngle;
    [SerializeField] private float weak4SEndAngle;
    [SerializeField] private ProjectileScriptableObject weak4projectileData;

    [Header("약공격5 데이터")]
    [SerializeField] private BossScriptableObject weakPattern5Data;

    [Tooltip("독비 간격")]
    [SerializeField] private float rainSpaceWeak5 = 1.5f;
    [SerializeField] private float poisonRainSpacing = 4f;

    [Header("투사체 데이터")]
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
            BossState.WeakPattern2,
            BossState.WeakPattern3,
            BossState.WeakPattern4,
            BossState.WeakPattern5,
            BossState.Groggy
        });

        if (isEnraged)
        {
            StartContinuousPoisonRain();
        }

        StartCoroutine(BeforeIdle());
    }

    [Header("광폭화 팝업")]
    [SerializeField] private GameObject BossEnragePopup;
    [SerializeField] private BossEnragePopup BossEnragePopupScript;

    private bool Enrageactive = true;
    private void BossEnrage()
    {
        player.GetComponent<PlayerController>().PlayerStop();
        BossEnragePopup.SetActive(true);
        BossEnragePopupScript.OnEnrage();
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
        if (Enrageactive && BossHPManager.Instance.GetCurrentHP() <= BossHPManager.Instance.GetMaxHP() * 0.5f && EndPattern)
        {
            Debug.Log(EndPattern);
            Enrageactive = false;
            BossEnrage();
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

        if (currentState == BossState.Groggy && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(GroggyState());
        }

        if (currentState == BossState.Idle && currentCoroutine == null) // 패턴의 조합이 끝나면 다시 Idle()돌려서 패턴 실행하게 해주기
        {
            if (currentBossStateArray == null)
            {
                //Debug.Log("새로운 패턴 리스트 배정");
                StartCoroutine(Idle());
            }
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
    private const float PATTERN_GAP = 0.001f;
    private IEnumerator FinishPattern()
    {
        EndPattern = true;
        yield return new WaitForSeconds(PATTERN_GAP);

        currentState = BossState.None;
        currentCoroutine = null;
    }
    public IEnumerator Idle() 
    {
        int patternNum = Random.Range(0, patternDic.Count);
        currentBossStateArray = patternDic[patternNum];
        for (int i = 0; i < currentBossStateArray.Length; i++)
        {
            currentState = currentBossStateArray[i];
            yield return new WaitUntil(() => currentState == BossState.None); // 패턴이 모두 실행되길 기다림
            currentState = BossState.Idle; // Idle에서 다시 새로운 패턴 받아오기
            currentCoroutine = null; // Idle 실행 조건
        }
        currentBossStateArray = null;
    }


    public IEnumerator BeforeIdle() 
    {
        yield return new WaitForSeconds(countDownBeforeStart);
        int patternNum = Random.Range(0, patternDic.Count);
        currentBossStateArray = patternDic[patternNum];
        for (int i = 0; i < currentBossStateArray.Length; i++)
        {
            currentState = currentBossStateArray[i];
            yield return new WaitUntil(() => currentState == BossState.None); // 패턴이 모두 실행되길 기다림
            currentState = BossState.Idle; // Idle에서 다시 새로운 패턴 받아오기
            currentCoroutine = null; // Idle 실행 조건
        }

        currentBossStateArray = null;
    }
    #region 패턴 1
    public IEnumerator WeakPattern1()
    {
        EndPattern = false;
        Debug.Log("약공격1");
        currentState = BossState.WeakPattern1;


        // 카운트 다운
        for (float i = weakPattern1Data.BeforeAttackDelay; i > 0; i--)
        {
            yield return new WaitForSeconds(1f);
        }

        // 1. 속박 탄환 방사형 발사
        SoundManager.Instance.EffectSoundOn("23-1");
        ProjectileController projectileController = ProjectileController.Create(
            weak1projectileData,
            transform,
            player.transform,
            captureProjectile,
            isEnraged
        );

        // 애니메이션 관련
        animator.SetTrigger("isSpike");

        StartCoroutine(projectileController.ExecuteRadialPattern(transform, weak1StartAngle, weak1EndAngle));

        yield return new WaitForSeconds(weak1MiddleDelay);

       

        animator.SetBool("isPre", true);
        animator.SetBool("isLaser", true);

        // 2. 레이저 경고선 표시 및 플레이어 추적
        //Debug.Log("추적 경고선");
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
            Vector2 dir = (currentPlayerPos - bossStartPosition).normalized;

            float extendLength = 10f;
            Vector2 extenedEndPos = currentPlayerPos + dir * extendLength;
            // 경고선 위치 업데이트 (보스에서 플레이어로)
            warningLine.SetPosition(0, bossStartPosition);
            warningLine.SetPosition(1, extenedEndPos);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 위치 고정 및 발사 준비
        fixedPlayerPos = player.transform.position;
        Vector2 fixedDir = (fixedPlayerPos - bossStartPosition).normalized;
        float fixedExtendLength = 10f;
        Vector2 fixedExtenedEndPos = fixedPlayerPos + fixedDir * fixedExtendLength;
        // 경고선 최종 위치 고정
        warningLine.SetPosition(0, bossStartPosition);


        warningLine.SetPosition(1, fixedExtenedEndPos);

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

        StartCoroutine(FinishPattern());
    }
    #endregion 

    #region 패턴 2
    public IEnumerator WeakPattern2() 
    {
        EndPattern = false;
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
            weak2projectileData,
            transform,
            player.transform,
            Projectile,
            isEnraged
        );

        animator.SetTrigger("isSpike");

        List<bool> coroutineFlagList = new();

        StartCoroutine(projectileController.ExecuteRadialPattern(transform, weak2StartAngle, weak2EndAngle, 0,coroutineFlagList));

        yield return new WaitForSeconds(weakPattern2Data.AfterAttackDelay);

        StartCoroutine(FinishPattern());

        while (!coroutineFlagList.Contains(true))
        {
            yield return null;
        }
        
        Destroy(projectileController.gameObject);

    }
    #endregion

    #region 패턴 3
    public IEnumerator WeakPattern3()
    {
        EndPattern = false;
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
                Vector2 dir = (playerPosition - bossStartPosition).normalized;

                float extendLength = 10f;
                Vector2 extendedEndPos = playerPosition + dir * extendLength;

                warningLine.SetPosition(0, bossStartPosition);
                warningLine.SetPosition(1, extendedEndPos);
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



        StartCoroutine(FinishPattern());
    }
    #endregion

    #region 패턴 4
    public IEnumerator WeakPattern4()
    {
        EndPattern = false;
        Debug.Log("약공격4");
        currentState = BossState.WeakPattern4;

        // 카운트 다운
        for (float i = weakPattern4Data.BeforeAttackDelay; i > 0; i--)
        {
            //Debug.Log("카운트다운: " + i);
            yield return new WaitForSeconds(1f);
        }

        ProjectileController Controller = ProjectileController.Create(
            weak4projectileData,
            transform,
            player.transform,
            Projectile,
            isEnraged
        );

        animator.SetTrigger("isSpike");
        SoundManager.Instance.EffectSoundOn("23-1");
        Coroutine firstLayer = StartCoroutine(Controller.ExecuteRadialPattern(transform, weak4FStartAngle, weak4FEndAngle)); // 첫 번째 층

        yield return new WaitForSeconds(1.3f);

        animator.SetTrigger("isSpike");
        SoundManager.Instance.EffectSoundOn("23-1");
        Coroutine secondLayer = StartCoroutine(Controller.ExecuteRadialPattern(transform, weak4SStartAngle, weak4SEndAngle,1)); // 두 번째 층

        yield return new WaitForSeconds(1.3f);

        animator.SetTrigger("isSpike");
        SoundManager.Instance.EffectSoundOn("23-1");
        Coroutine thirdLayer = StartCoroutine(Controller.ExecuteRadialPattern(transform, weak4FStartAngle, weak4FEndAngle)); // 첫 번째 층
        yield return new WaitForSeconds(1.3f);

        animator.SetTrigger("isSpike");
        SoundManager.Instance.EffectSoundOn("23-1");
        Coroutine fourthLayer = StartCoroutine(Controller.ExecuteRadialPattern(transform, weak4SStartAngle, weak4SEndAngle,1)); // 네 번째 층
        
        yield return new WaitForSeconds(weakPattern4Data.AfterAttackDelay);
        StartCoroutine(FinishPattern());

        // 두 코루틴이 모두 끝날 때까지 대기
        yield return firstLayer;
        yield return secondLayer;
        yield return thirdLayer;
        yield return fourthLayer;

        
    }
    #endregion

    #region 패턴 5
    public IEnumerator WeakPattern5()
    {
        EndPattern = false;
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
            //Debug.Log("카운트다운: " + i);
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

        List<bool> coroutineFlagList = new();

        yield return StartCoroutine(rainController.ExecuteWeakPattern5Rain(
        transform,
            mapWidth,
            mapCenter,
            safeZoneWidth,
            leftBound,
            rightBound
        ));

        

        yield return new WaitForSeconds(weakPattern5Data.AfterAttackDelay);
        Debug.Log("약공격5 종료");
        StartCoroutine(FinishPattern());

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

    #region 그로기 상태
    public IEnumerator GroggyState()
    {
        animator.SetTrigger("isGroggy");
        Debug.Log("그로기 상태");
        currentState = BossState.Groggy;

        for (float i = groggyTime - 1; i > 0; i--)
        {
            Debug.Log("카운트다운: " + i);
            yield return new WaitForSeconds(1f);
        }
        animator.SetTrigger("isRecovery");
        yield return new WaitForSeconds(1f);

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }
    #endregion

    #region 보스몹 시선 처리
    private void FacePlayer()
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
    #endregion

    #region 위험 구역 표시
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
    #endregion

    #region 위험 구역 깜빡임 효과
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
    #endregion
    private void StartContinuousPoisonRain()
    {
        float mapLeft = mapWidthPositions[0].position.x + 1f;
        float mapRight = mapWidthPositions[1].position.x + 1f;
        float mapWidth = mapRight - mapLeft;

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
            mapWidth,
            mapLeft,
            mapRight,
            poisonRainSpacing,
            () => true  // Always continue as long as enraged
        ));
    }
}

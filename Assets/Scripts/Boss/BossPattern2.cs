using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern2 : MonoBehaviour
{
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

    private Coroutine currentCoroutine = null;
    private Dictionary<int, BossState[]> patternDic = new();
    private BossState currentState;
    public Player player; // Player 타입의 변수를 선언해 참조 가져오기
    private LaserController laserController; // LaserController 참조
    private ProjectileController projectileController; // ProjectileController 참조

    [Header("광폭화 T/F")]
    [SerializeField] private bool isEnraged = false; // Inspector에서 설정 가능

    [Header("레이저 타이밍 설정")]
    [SerializeField] private float laserFollowDuration = 2f; // 레이저가 플레이어를 따라다니는 시간
    [SerializeField] private float laserLockDuration = 1f;   // 레이저가 고정되는 시간


    [Header("ScriptableObject 데이터")]
    [SerializeField] private BossScriptableObject weakPattern1Data;
    [SerializeField] private BossScriptableObject weakPattern2Data;
    [SerializeField] private BossScriptableObject weakPattern3Data;
    [SerializeField] private BossScriptableObject weakPattern4Data;
    [SerializeField] private BossScriptableObject weakPattern5Data;
    [SerializeField] private LaserScriptableObject weakLaserData;
    [SerializeField] private LaserScriptableObject strongLaserData;
    [SerializeField] private ProjectileScriptableObject projectileData;
    [SerializeField] private GameObject captureProjectile; // 속박 투사체 프리팹
    [SerializeField] private GameObject projectile; // 일반 투사체 프리팹
    [SerializeField] private GameObject rainProjectile; // 하늘에서 떨어지는 투사체 프리팹




    void Start()
    {
        patternDic.Add(0, new BossState[] { BossState.WeakPattern1, BossState.WeakPattern2, BossState.WeakPattern3, BossState.WeakPattern4, BossState.WeakPattern5 });
        //patternDic.Add(1, new BossState[] { BossState.WeakPattern1, BossState.WeakPattern1, BossState.WeakPattern2, BossState.StrongPattern2 });

        StartCoroutine(Idle());
    }

   
    void Update()
    {
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
        ProjectileController projectileController = ProjectileController.Create(
            projectileData,
            transform,
            player.transform,
            captureProjectile,
            isEnraged
        );

        yield return StartCoroutine(projectileController.ExecuteRadialPattern(transform));
        yield return new WaitForSeconds(1.5f);

        // 2. 레이저 경고선 표시 및 플레이어 추적
        Debug.Log("추적 경고선");
        LineRenderer warningLine = CreateDangerZone();
        StartCoroutine(BlinkDangerZone(warningLine)); // 깜빡임 효과 시작

        Vector2 fixedPlayerPos = Vector2.zero;
        float elapsed = 0f;

        // 보스의 위치 가져오기
        Vector2 bossStartPosition = transform.position;

        // 플레이어 추적 단계
        while (elapsed < laserFollowDuration)
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

        yield return new WaitForSeconds(laserLockDuration);

        Destroy(warningLine.gameObject);

        // 레이저 발사
        LaserController laser = LaserController.Create(
            weakLaserData, 
            bossStartPosition, // 보스의 시작 위치
            player.transform
        );

        // 레이저가 타겟 레이어에 충돌하도록 설정
        laser.SetTargetLayer(weakLaserData.TargetLayer);

        yield return StartCoroutine(laser.FireLaser(bossStartPosition, fixedPlayerPos));

        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern2() 
    {
        Debug.Log("약공격2");
        currentState = BossState.WeakPattern2;
        //
        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern3()
    {
        Debug.Log("약공격3");
        currentState = BossState.WeakPattern3;
        //
        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern4()
    {
        Debug.Log("약공격4");
        currentState = BossState.WeakPattern4;
        //
        currentState = BossState.None;
        currentCoroutine = null;
        yield return null;
    }

    public IEnumerator WeakPattern5()
    {
        Debug.Log("약공격5");
        currentState = BossState.WeakPattern5;
        //
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

    private LineRenderer CreateDangerZone()
    {
        GameObject dangerZoneObj = new GameObject("DangerZone");
        LineRenderer lineRenderer = dangerZoneObj.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = weakLaserData.LaserWidth;  // LaserWidth 사용
        lineRenderer.endWidth = weakLaserData.LaserWidth;    // LaserWidth 사용

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

}

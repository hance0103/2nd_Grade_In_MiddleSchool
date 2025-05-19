using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BossEnragePopup : MonoBehaviour
{
    [Header("스테이지")]
    [SerializeField] private int Stage;
    [Header("플레이어/보스 오브젝트")]
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Boss;
    private PlayerController _playerController;
    
    [Header("이동시킬 이미지 오브젝트")]
    [SerializeField] private GameObject TopBar;
    [SerializeField] private GameObject MiddleBar;
    [SerializeField] private GameObject BottomBar;
    [SerializeField] private Image TopText;
    [SerializeField] private Image BottomText;

    [Header("1차 이동 시간(초)")]
    [SerializeField] private float moveDuration1 = 0.5f;
    [Header("2차 이동 시간(초)")]
    [SerializeField] private float moveDuration2 = 0.5f;
    [Header("텍스트 이동 시간(초)")]
    [SerializeField] private float TextmoveDuration = 4f;

    [Header("1차 이동할 거리(양수면 오른쪽, 음수면 왼쪽)")]
    [SerializeField] private float moveFastRight1 = 1000f;  
    //[SerializeField] private float moveFastLeft1 = -1000f;  
    [SerializeField] private float moveSlowRight1 = 300f;   
    [SerializeField] private float moveSlowLeft1 = -300f;   

    [Header("2차 이동할 거리(양수면 오른쪽, 음수면 왼쪽)")]
    [SerializeField] private float moveFastRight2 = 2000f;  
    [SerializeField] private float moveFastLeft2 = -2000f;  
    

    [Header("텍스트 이동 거리(양수면 오른쪽, 음수면 왼쪽)")]
    [SerializeField] private float moveRight = 1000f;   
    [SerializeField] private float moveLeft = -1000f;   

    [Header("닫히기까지 기다릴 시간(초)")]
    [SerializeField] private float waitBeforeClose = 3f;

    [Header("글씨 깜빡이는 시간 및 속도")]
    [SerializeField] private float blinkDuration = 4f;
    [SerializeField] private float blinkSpeed = 4f;

    // 원본 위치 저장용 변수
    private Vector2 initTopBarPos;
    private Vector2 initMiddleBarPos;
    private Vector2 initBottomBarPos;
    private Vector2 initTopTextPos;
    private Vector2 initBottomTextPos;
    private void Awake()
    {
        _playerController = Player.GetComponent<PlayerController>();
    }
    private void Start()
    {
       
        // 시작 시, RectTransform들의 원본 위치를 기억
        initTopBarPos = TopBar.GetComponent<RectTransform>().anchoredPosition;
        initMiddleBarPos = MiddleBar.GetComponent<RectTransform>().anchoredPosition;
        initBottomBarPos = BottomBar.GetComponent<RectTransform>().anchoredPosition;
        initTopTextPos = TopText.GetComponent<RectTransform>().anchoredPosition;
        initBottomTextPos = BottomText.GetComponent<RectTransform>().anchoredPosition;
        OnEnrage();
    }
    public void OnEnrage() 
    {
        Player.transform.position = new Vector3(-4.0f, -2.946f, 0f);
        if (Stage == 1) 
        { 
            Boss.transform.position = new Vector3(4.6f, -1.0f, 0f);
            SoundManager.Instance.EffectSoundOn("stage1scream");
        }
        if (Stage == 2) 
        { 
            Boss.transform.position = new Vector3(6.0f, 1.7f, 0f);
            SoundManager.Instance.EffectSoundOn("stage2scream");
        }
        if (Stage == 3) 
        { 
            Boss.transform.position = new Vector3(4.6f, -1.0f, 0f);
            SoundManager.Instance.EffectSoundOn("stage3scream");
        }

        CameraMove.Instance.EnrageBoss();
        // 1) 보스 광폭화 패널(이 스크립트가 붙은 GameObject) 활성화
        gameObject.SetActive(true);
        // 트리거(또는 처음부터) 발동 시 코루틴 실행
        StartCoroutine(MoveFirst());
        StartCoroutine(MoveText());
    }

    private IEnumerator MoveText()
    {
        
        RectTransform topTextRect = TopText.GetComponent<RectTransform>();
        RectTransform bottomTextRect = BottomText.GetComponent<RectTransform>();
        Vector2 topTextStartPos = topTextRect.anchoredPosition;
        Vector2 bottomTextStartPos = bottomTextRect.anchoredPosition;

        StartCoroutine(BlinkText(TopText, blinkDuration, blinkSpeed));
        StartCoroutine(BlinkText(BottomText, blinkDuration, blinkSpeed));

        // 2) moveDuration 동안 옆으로 부드럽게 이동
        float elapsed = 0f;
        
        while (elapsed < TextmoveDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / TextmoveDuration;

            // TopText → SlowRight
            topTextRect.anchoredPosition = Vector2.Lerp(
                topTextStartPos,
                topTextStartPos + new Vector2(moveRight, 0f),
                t
            );

            // BottomText → SlowLeft
            bottomTextRect.anchoredPosition = Vector2.Lerp(
                bottomTextStartPos,
                bottomTextStartPos + new Vector2(moveLeft, 0f),
                t
            );

            yield return null; // 다음 프레임까지 대기
        }
        // 이동이 정확히 끝났는지 보정 (t=1 상태)
        
        topTextRect.anchoredPosition = topTextStartPos + new Vector2(moveSlowRight1, 0f);
        bottomTextRect.anchoredPosition = bottomTextStartPos + new Vector2(moveSlowLeft1, 0f);
        Debug.Log("광폭화 텍스트 코루틴 종료");
    }
    private IEnumerator BlinkText(Image targetImage, float blinkDuration, float blinkSpeed)
    {
        //글씨 깜빡이는 연출
        float elapsed = 0f;
        while (elapsed < blinkDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1.5f);

            Color c = targetImage.color;
            c.a = alpha;
            targetImage.color = c;

            yield return null;
        }

        // 끝나면 알파값 복원
        Color finalColor = targetImage.color;
        finalColor.a = 1f;
        targetImage.color = finalColor;
    }

    private IEnumerator MoveFirst()
    {
        // 1) 보스 광폭화 패널(이 스크립트가 붙은 GameObject) 활성화
        gameObject.SetActive(true);

        // 각각의 RectTransform을 구하고, 시작 위치를 기록
        RectTransform topBarRect = TopBar.GetComponent<RectTransform>();
        RectTransform middleBarRect = MiddleBar.GetComponent<RectTransform>();
        RectTransform bottomBarRect = BottomBar.GetComponent<RectTransform>();

        Vector2 topBarStartPos = topBarRect.anchoredPosition;
        Vector2 middleBarStartPos = middleBarRect.anchoredPosition;
        Vector2 bottomBarStartPos = bottomBarRect.anchoredPosition;
        
        Vector2 topBarMem = topBarRect.anchoredPosition;
        Vector2 middleBarMem = middleBarRect.anchoredPosition;
        Vector2 bottomBarMem = bottomBarRect.anchoredPosition;


        // 2) moveDuration 동안 옆으로 부드럽게 이동
        float elapsed = 0f;
        while (elapsed < moveDuration1)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / moveDuration1;

            // TopBar → FastRight
            topBarRect.anchoredPosition = Vector2.Lerp(
                topBarStartPos,
                topBarStartPos + new Vector2(moveFastRight1, 0f),
                t
            );

            // MiddleBar → SlowRight
            middleBarRect.anchoredPosition = Vector2.Lerp(
                middleBarStartPos,
                middleBarStartPos + new Vector2(moveSlowLeft1, 0f),
                t
            );

            // BottomBar → FastLeft
            bottomBarRect.anchoredPosition = Vector2.Lerp(
                bottomBarStartPos,
                bottomBarStartPos + new Vector2(moveFastRight1, 0f),
                t
            );

            yield return null; // 다음 프레임까지 대기
        }
        
        
        // 이동이 정확히 끝났는지 보정 (t=1 상태)
        topBarRect.anchoredPosition = topBarStartPos + new Vector2(moveFastRight1, 0f);
        middleBarRect.anchoredPosition = middleBarStartPos + new Vector2(moveSlowLeft1, 0f);
        bottomBarRect.anchoredPosition = bottomBarStartPos + new Vector2(moveFastRight1, 0f);
        

        // 3) 이동 후 2초 기다렸다가 닫는 연출
        yield return new WaitForSecondsRealtime(waitBeforeClose);
        StartCoroutine(MoveSecond());
    }
    private IEnumerator MoveSecond()
    {
        // 각각의 RectTransform을 구하고, 시작 위치를 기록
        RectTransform topBarRect = TopBar.GetComponent<RectTransform>();
        RectTransform middleBarRect = MiddleBar.GetComponent<RectTransform>();
        RectTransform bottomBarRect = BottomBar.GetComponent<RectTransform>();
        
        Vector2 topBarStartPos = topBarRect.anchoredPosition;
        Vector2 middleBarStartPos = middleBarRect.anchoredPosition;
        Vector2 bottomBarStartPos = bottomBarRect.anchoredPosition;
        
        // 2) moveDuration 동안 옆으로 부드럽게 이동
        float elapsed = 0f;
        while (elapsed < moveDuration2)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / moveDuration2;

            // TopBar → FastRight
            topBarRect.anchoredPosition = Vector2.Lerp(
                topBarStartPos,
                topBarStartPos + new Vector2(moveFastRight2, 0f),
                t
            );

            // MiddleBar → SlowRight
            middleBarRect.anchoredPosition = Vector2.Lerp(
                middleBarStartPos,
                middleBarStartPos + new Vector2(moveFastLeft2, 0f),
                t
            );

            // BottomBar → FastLeft
            bottomBarRect.anchoredPosition = Vector2.Lerp(
                bottomBarStartPos,
                bottomBarStartPos + new Vector2(moveFastRight2, 0f),
                t
            );

            yield return null; // 다음 프레임까지 대기
        }

        // 이동이 정확히 끝났는지 보정 (t=1 상태)
        topBarRect.anchoredPosition = topBarStartPos + new Vector2(moveFastRight2, 0f);
        middleBarRect.anchoredPosition = middleBarStartPos + new Vector2(moveFastLeft2, 0f);
        bottomBarRect.anchoredPosition = bottomBarStartPos + new Vector2(moveFastRight2, 0f);

        // 패널(보스 광폭화 팝업) 비활성화
        gameObject.SetActive(false);
        // 다시 켰을 때도 동일하게 연출되도록, 원본 위치로 복원
        TopBar.GetComponent<RectTransform>().anchoredPosition = initTopBarPos;
        MiddleBar.GetComponent<RectTransform>().anchoredPosition = initMiddleBarPos;
        BottomBar.GetComponent<RectTransform>().anchoredPosition = initBottomBarPos;
        TopText.GetComponent<RectTransform>().anchoredPosition = initTopTextPos;
        BottomText.GetComponent<RectTransform>().anchoredPosition = initBottomTextPos;
        // 코루틴 종료

        _playerController.PlayerResume();
        _playerController.DeactivateInvincible();
    }
}

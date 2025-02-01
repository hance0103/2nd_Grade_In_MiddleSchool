using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stage1OpeningTextPopup : MonoBehaviour
{
    [Header("참조 요소들")]
    public TMP_Text ChatText;      // 실제 채팅이 나오는 텍스트
    public TMP_Text CharacterName; // 캐릭터 이름이 나오는 텍스트
    public GameObject OpeningTextPanel;  // 오프닝 스크립트 패널
    public GameObject TempPenal;
    public Button NextButton;
    public Button SkipButton;
    [SerializeField] private GameObject Timer; // 타이머 활성화/비활성화 용도

    [Header("플레이어/보스 오브젝트")]
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject PlayerHP;
    [SerializeField] private GameObject Boss;
    [SerializeField] private GameObject BossHP;

    [Header("오프닝 스크립트 캐릭터/보스 스프라이트")]
    public GameObject CharacterPose1; // 손가락 포즈, 신난 표정
    public GameObject CharacterPose2; // 머리에 손 포즈, 눈 감고 미소
    public GameObject CharacterPose3; // 손가락 포즈, 화난 표정(이글이글)
    public GameObject CharacterPose4; // 머리에 손 포즈, 어둡고 째려보는 표정(분노를 억누르는 듯한)
    public GameObject CharacterPose5; // 젖히고 웃는 포즈
    public GameObject CharacterPose6; // 머리에 손 포즈, 진지한 표정
    public GameObject CharacterPose7; // 눈에 붉은기운이 돈다
    public GameObject BossPose1;

    private bool isFullTextDisplayed = false;
    private bool isNextButtonClicked = false;
    public string writerText = "";
    private static bool isFirstTime = true;
    void Start()
    {
        if (!isFirstTime)
        {
            OnSkipButtonClicked();
        }

        Open();
     }
   
    void Update()
    {
        
    }

    void Open()
    {
        Time.timeScale = 0f;
        var timer = FindObjectOfType<Timer>();
        timer.TimeActive = false;
        NextButton.onClick.AddListener(OnNextButtonClicked);
        if (isFirstTime) { StartCoroutine(OpeningTextStage1()); }
        Timer.SetActive(false);
    }
    void OnNextButtonClicked()
    {
        if (!isFullTextDisplayed)
        {
            isSkipping = true;
        }
        // 이미 대사가 다 나왔으면, 다음 대사로 넘어가는 신호
        else
        {
            isNextButtonClicked = true;
        }
    }

    public void OnSkipButtonClicked()
    {
        CloseOpeningText();
    }

    public float typingSpeed = 0.02f;
    public bool isSkipping = false;
    IEnumerator NormalChat(string narrator, string narration)
    {
        isFullTextDisplayed = false;
        isSkipping = false;

        int a = 0;
        CharacterName.text = narrator;
        ChatText.text = "";
        writerText = "";

        // 텍스트 타이핑 효과
        for (a = 0; a < narration.Length; a++)
        {
            if (isSkipping)
            {
                writerText = narration;
                ChatText.text = writerText;
                break;
            }
            writerText += narration[a];
            ChatText.text = writerText;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        isFullTextDisplayed = true;
        isSkipping = false;
        // 키를 다시 누를 때까지 무한정 대기
        isNextButtonClicked = false;
        yield return new WaitUntil(() => isNextButtonClicked);
    }

    IEnumerator OpeningTextStage1() //("등장인물", "대사")로 입력
    {
        yield return StartCoroutine(NormalChat("주인공", "오늘은 이 몸이 문화재에 공연을 하러 가는 초-스페셜한 날 !!"));
        yield return StartCoroutine(NormalChat("주인공", "모두 나에게 반해 친해지려 안달날 상황이 그려지는구나 크큭"));
        yield return StartCoroutine(NormalChat("주인공", "오늘만큼은 자비를 베풀어 인간놈들과 어울려 주겠다 !"));
        yield return StartCoroutine(NormalChat("주인공", "나타났구나."));
        yield return StartCoroutine(NormalChat("주인공", "엉겁의 세월 동안 [오 레 사 마]를 잡아두고"));
        yield return StartCoroutine(NormalChat("주인공", "지각이라는 치욕스러운 경험을 하게 했던..."));
        yield return StartCoroutine(NormalChat("주인공", "신호등 !!!"));
        yield return StartCoroutine(NormalChat("주인공", "오늘만큼은 네게 허비할 시간이 없다"));
        yield return StartCoroutine(NormalChat("주인공", "한 줌의 재가 되고 싶지 않다면..꺼져라."));
        yield return StartCoroutine(NormalChat("주인공", "내 안에 꿈틀거리는 [락의 영혼]이 [살의]를 내비치고 있다."));
        yield return StartCoroutine(NormalChat("신호등", "연약한 [소녀]여"));
        yield return StartCoroutine(NormalChat("신호등", "너 따위가 감히 나를 지나칠 수 있다 생각하느냐?"));
        yield return StartCoroutine(NormalChat("신호등", "안타깝지만 오늘도 [패배]를 안겨 주마."));
        yield return StartCoroutine(NormalChat("주인공", "아-? 하찮구나 고작 속세의 [미물] 따위가-"));
        yield return StartCoroutine(NormalChat("주인공", "감히 [마왕]에게 도전장을 던진다는 것이냐?"));
        yield return StartCoroutine(NormalChat("주인공", "뭐 그렇게 나온다면 할 수 없지.."));
        yield return StartCoroutine(NormalChat("주인공", "정의를 위해"));
        yield return StartCoroutine(NormalChat("주인공", "[처리한다.]"));
        yield return StartCoroutine(NormalChat("", "전투에 진입합니다"));
        CloseOpeningText();
    }

    void CloseOpeningText()
    { 
        OpeningTextPanel.SetActive(false); // 패널 비활성화
        isFirstTime = false;
        Time.timeScale = 1f;
        var timer = FindObjectOfType<Timer>();
        
        // 타이머의 TimeActive 켜고, 코루틴 수동 실행
        Timer.SetActive(true);
        timer.TimeActive = true;
        Player.SetActive(true);
        Boss.SetActive(true);
        PlayerHP.SetActive(true);
        BossHP.SetActive(true);
    }
}

using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stage3OpeningTextPopup : MonoBehaviour
{
    [Header("참조 요소들")]
    public TMP_Text ChatText;      // 실제 채팅이 나오는 텍스트
    public TMP_Text CharacterName; // 캐릭터 이름이 나오는 텍스트
    public GameObject OpeningTextPanel;  // 오프닝 스크립트 패널
    public GameObject TempPenal;
    public Button NextButton;
    public Button SkipButton;
    [SerializeField] private GameObject Timer; // 타이머 활성화/비활성화 용도

    [Header("오프닝 스크립트 캐릭터/보스 스프라이트")]
    public GameObject CharacterPose1; // 손가락 포즈, 신난 표정
    public GameObject CharacterPose2; // 머리에 손 포즈, 눈 감고 미소
    public GameObject CharacterPose3; // 손가락 포즈, 화난 표정(이글이글)
    public GameObject CharacterPose4; // 머리에 손 포즈, 어둡고 째려보는 표정(분노를 억누르는 듯한)
    public GameObject CharacterPose5; // 젖히고 웃는 포즈
    public GameObject CharacterPose6; // 머리에 손 포즈, 진지한 표정
    public GameObject CharacterPose7; // 눈에 붉은기운이 돈다
    public GameObject Boss;

    private bool isFullTextDisplayed = false;
    private bool isNextButtonClicked = false;
    public string writerText = "";
    void Start()
    {
        NextButton.onClick.AddListener(OnNextButtonClicked);
        StartCoroutine(OpeningTextStage3());
        Timer.SetActive(false);
    }

    void Update()
    {

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

    IEnumerator OpeningTextStage3() //("등장인물", "대사")로 입력
    {
        //변경 필요
        yield return StartCoroutine(NormalChat("주인공", "오늘만큼은 네게 허비할 시간이 없다"));
        yield return StartCoroutine(NormalChat("주인공", "한 줌의 재가 되고 싶지 않다면..꺼져라."));
        yield return StartCoroutine(NormalChat("주인공", "내 안에 꿈틀거리는 [락의 영혼]이 [살의]를 내비치고 있다."));
        yield return StartCoroutine(NormalChat("최종보스", "연약한 [소녀]여"));
        yield return StartCoroutine(NormalChat("최종보스", "너 따위가 감히 나를 지나칠 수 있다 생각하느냐?"));
        yield return StartCoroutine(NormalChat("최종보스", "안타깝지만 [패배]를 안겨 주마."));
        yield return StartCoroutine(NormalChat("주인공", "뭐 그렇게 나온다면 할 수 없지.."));
        yield return StartCoroutine(NormalChat("주인공", "정의를 위해"));
        yield return StartCoroutine(NormalChat("주인공", "[처리한다.]"));
        yield return StartCoroutine(NormalChat("", "전투에 진입합니다"));
        CloseOpeningText();
    }

    void CloseOpeningText()
    {
        OpeningTextPanel.SetActive(false); // 패널 비활성화
        TempPenal.SetActive(true);
        var timer = FindObjectOfType<Timer>();

        // 타이머의 TimeActive 켜고, 코루틴 수동 실행
        Timer.SetActive(true);
        


    }
}

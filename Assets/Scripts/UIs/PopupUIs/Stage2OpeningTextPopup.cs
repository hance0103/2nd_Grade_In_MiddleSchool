using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stage2OpeningTextPopup : MonoBehaviour
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
        StartCoroutine(OpeningTextStage1());
        Timer.SetActive(false);
        Time.timeScale = 0;
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

    IEnumerator OpeningTextStage1() //("등장인물", "대사")로 입력
    {
        yield return StartCoroutine(NormalChat("주인공", "쳇, 꽤나 시간을 허비했군.."));
        yield return StartCoroutine(NormalChat("주인공", "더 빠른 템포로 걸어갈 수밖에"));
        yield return StartCoroutine(NormalChat("주인공", "흐음 평화로워야 할 등굣길에 이 불길한 기운은 무엇이냐.."));
        yield return StartCoroutine(NormalChat("주인공", "자꾸만 걸음이 느려지고 손에 땀이 나는구나.."));
        yield return StartCoroutine(NormalChat("주인공", "손발이 어딘가에 속박되는 기분..."));
        yield return StartCoroutine(NormalChat("주인공", "설마 [긴장]이란 것을 하고 있나??"));
        yield return StartCoroutine(NormalChat("주인공", "이 몸이 긴장 따위 할 리 없다"));
        yield return StartCoroutine(NormalChat("주인공", "이런.."));
        yield return StartCoroutine(NormalChat("주인공", "주변에 누군가 성가신 놈이 있는 것이 틀림없군"));
        yield return StartCoroutine(NormalChat("주인공", "자, 어디냐 ! 숨어 있지 말고 나와라 !"));
        yield return StartCoroutine(NormalChat("주인공", "비겁한 자식.."));
        yield return StartCoroutine(NormalChat("거미", "앞뒤 없이 덤비는 건 한결같구나"));
        yield return StartCoroutine(NormalChat("거미", "나의 그물로 너의 몸 뿐만이 아니라 정신까지 [속박]시켜주마.."));
        yield return StartCoroutine(NormalChat("주인공", "벌레 주제에 쓸데없이 말이 길군.."));
        yield return StartCoroutine(NormalChat("주인공", "이 몸에게 걸맞는 [예의]를 갖출 수 있도록 무참히 교육시켜주마"));
        yield return StartCoroutine(NormalChat("주인공", "덤벼라 !!"));
        yield return StartCoroutine(NormalChat("", "전투에 진입합니다"));
        CloseOpeningText();
    }

    void CloseOpeningText()
    {
        OpeningTextPanel.SetActive(false); // 패널 비활성화
        TempPenal.SetActive(true);
        // 타이머의 TimeActive 켜고, 코루틴 수동 실행
        Timer.SetActive(true);
        Time.timeScale = 1;

    }
}

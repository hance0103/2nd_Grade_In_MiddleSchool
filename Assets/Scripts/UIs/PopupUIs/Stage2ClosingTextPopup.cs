using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stage2ClosingTextPopup : MonoBehaviour
{
    [Header("참조 요소들")]
    public TMP_Text ChatText;      // 실제 채팅이 나오는 텍스트
    public TMP_Text CharacterName; // 캐릭터 이름이 나오는 텍스트
    public GameObject ClosingTextPanel;  // 클로징 스크립트 패널
    public GameObject VictoryPanel;      // 승리 팝업 패널
    public Button NextButton;
    public Button SkipButton;
    public Button OpenClosingText; // 클로징 텍스트 패널 활성화 버튼
    [SerializeField] private GameObject Timer; // 타이머 활성화/비활성화 용도

    //변경 필요
    [Header("클로징 스크립트 캐릭터/보스 스프라이트")]
    public GameObject CharacterPose1; // 손가락 포즈, 비웃는 표정
    public GameObject CharacterPose2; // 머리에 손 포즈, 진지한 표정
    public GameObject CharacterPose3; // 자지러지게 웃는 표정
    public GameObject CharacterPose4; // 머리에 손 포즈, 신난 표정
    public GameObject Boss;

    private bool isFullTextDisplayed = false;
    private bool isNextButtonClicked = false;
    public string writerText = "";
    void Start()
    {

    }
    public void OnClosingTextButtonClicked() // 다음 스테이지 버튼을 눌렀을 때 실행되는 패널
    {
        Timer.SetActive(false);
        Time.timeScale = 1f;
        VictoryPanel.SetActive(false);
        ClosingTextPanel.SetActive(true);
        NextButton.onClick.AddListener(OnNextButtonClicked);
        StartCoroutine(ClosingTextStage2());
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
        CloseClosingText();
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
    
    IEnumerator ClosingTextStage2() //("등장인물", "대사")로 입력 
    {
        //변경 필요
        yield return StartCoroutine(NormalChat("주인공", "이 몸을 가뒀던 더러운 기운이 이제야 사라졌군.."));
        yield return StartCoroutine(NormalChat("주인공", "거미 주제에 날 성가시게 하다니.."));
        yield return StartCoroutine(NormalChat("주인공", "다시 마주치는 날엔 잘근잘근 밟아 주마"));
        yield return StartCoroutine(NormalChat("거미", "오만한 자여.."));
        yield return StartCoroutine(NormalChat("거미", "승리도 너의 착각이다.."));
        yield return StartCoroutine(NormalChat("거미", "내 저주는 널 따라다닐 것이다.."));
        yield return StartCoroutine(NormalChat("", "3스테이지에 진입합니다"));
        CloseClosingText();
    }

    void CloseClosingText()
    {
        ClosingTextPanel.SetActive(false);
        SceneManager.LoadScene("Stage3");

    }
}

using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stage1ClosingTextPopup : MonoBehaviour
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

    [Header("플레이어/보스 오브젝트")]
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Boss;

    [Header("클로징 스크립트 캐릭터/보스 스프라이트")]
    public GameObject CharacterPose1; // 손가락 포즈, 비웃는 표정
    public GameObject CharacterPose2; // 머리에 손 포즈, 진지한 표정
    public GameObject CharacterPose3; // 자지러지게 웃는 표정
    public GameObject CharacterPose4; // 머리에 손 포즈, 신난 표정
    public GameObject BossPose1;

    private bool isFullTextDisplayed = false;
    private bool isNextButtonClicked = false;
    public string writerText = "";
    void Start()
    {
        OnClosingText();
    }
    public void OnClosingText() // 실행되자마자 실행
    {
        Player.SetActive(false);
        Boss.SetActive(false);
        Timer.SetActive(false);
        
        NextButton.onClick.AddListener(OnNextButtonClicked);
        StartCoroutine(ClosingTextStage1());
    } 
    void Update()
    {
        
    }
    void OnNextButtonClicked()
    {
        SoundManager.Instance.EffectSoundOn("3");
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

    IEnumerator ClosingTextStage1() //("등장인물", "대사")로 입력
    {
        yield return StartCoroutine(NormalChat("주인공", "후훗, 네놈 따위가 [(별명)]에게 이길 수 있을 리가 없지"));
        yield return StartCoroutine(NormalChat("주인공", "당연하고 시시한 승리다."));
        yield return StartCoroutine(NormalChat("주인공", "어리석은 자여..다시는 이 몸을 방해할 생각하지 마라. 그땐 숨통을 끊어주마"));
        yield return StartCoroutine(NormalChat("신호등", "크윽..오늘은 여기까지만 하지.."));
        yield return StartCoroutine(NormalChat("신호등", "하지만 널 막을 자는 나뿐만이 아니다.."));
        yield return StartCoroutine(NormalChat("주인공", "크큭..그 꼴로 말은 잘 하는구나 !!"));
        yield return StartCoroutine(NormalChat("주인공", "나는 이만 가보겠다"));
        yield return StartCoroutine(NormalChat("주인공", "오늘은 특별한 날이거든☆"));
        yield return StartCoroutine(NormalChat("", "2스테이지에 진입합니다"));
        CloseClosingText();
    }

    void CloseClosingText()
    {
        gameObject.SetActive(false);
        VictoryPanel.SetActive(true);
        

    }
}

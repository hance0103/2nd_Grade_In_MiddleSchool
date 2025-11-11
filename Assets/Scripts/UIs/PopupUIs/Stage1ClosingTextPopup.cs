using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

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
    private GameObject Player;
    private GameObject Boss;

    [Header("클로징 스크립트 캐릭터/보스 스프라이트")]
    public Image character1; // 손가락 포즈, 비웃는 표정
    public Image character2; // 손가락 포즈, 진지한 표정
    public Image character3; // 머리에 손 포즈, 기본표정
    public Image character4; // 머리에 손 포즈, 놀란 표정
    public Image character5; // 머리에 손 포즈, 비웃는 표정
    public Image boss1;

    private bool isFullTextDisplayed = false;
    private bool isNextButtonClicked = false;
    public string writerText = "";
    void Start()
    {
        OnClosingText();
    }
    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
            Debug.LogError("Player 태그가 붙은 오브젝트를 찾을 수 없습니다!");
        
    }
    public void OnClosingText() // 실행되자마자 실행
    {
        Player.SetActive(false);
        Timer.SetActive(false);
        
        NextButton.onClick.AddListener(OnNextButtonClicked);
        StartCoroutine(ClosingTextStage1());
    }
    IEnumerator ClosingTextStage1() //("등장인물", "대사")로 입력
    {
        character1.gameObject.SetActive(true);
        yield return StartCoroutine(FadeInImageFromRight(character1, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "후훗, 네놈 따위가 이 몸에게 이길 수 있을 리가 없지"));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "시도는 칭찬해주마."));
        character1.gameObject.SetActive(false);

        character2.gameObject.SetActive(true);
        yield return StartCoroutine(FadeInImageFromRight(character2, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "하지만, 다시는 방해할 생각하지 마라. 어리석은 자여..."));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "다음은 없다."));
        character2.gameObject.SetActive(false);
        SetNarratorFontSize(50f);
        //yield return StartCoroutine(FadeInImageFromLeft(boss1, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("멈춤을 속삭이는 자", "크윽..오늘은 여기까지만 하지.."));
        yield return StartCoroutine(NormalChat("멈춤을 속삭이는 자", "하지만 널 막을 자는 나뿐만이 아니다.."));
        SetNarratorFontSize(60f);
        //boss1.gameObject.SetActive(false);
        yield return StartCoroutine(NormalChat("적혈의 서약자", "크큭..그 꼴로 말은 잘 하는구나 !!"));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "나는 이만 가보겠다"));

        character5.gameObject.SetActive(true);
        yield return StartCoroutine(FadeInImageFromRight(character5, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "오늘은 특별한 날이거든☆"));
        character5.gameObject.SetActive(false);
        CloseClosingText();
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

    
    IEnumerator FadeInImageFromRight(Image targetImage, float duration, float distance)
    {

        Color originalColor = targetImage.color;
        originalColor.a = 0f;
        targetImage.color = originalColor;

        RectTransform rt = targetImage.rectTransform;
        Vector2 finalPos = rt.anchoredPosition;
        Vector2 startPos = new Vector2(finalPos.x + distance, finalPos.y);

        rt.anchoredPosition = startPos;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            originalColor.a = t;
            targetImage.color = originalColor;

            rt.anchoredPosition = Vector2.Lerp(startPos, finalPos, t);

            yield return null;
        }

        // 보정 (알파값 / 위치)
        originalColor.a = 1f;
        targetImage.color = originalColor;
        rt.anchoredPosition = finalPos;
    }


    IEnumerator FadeInImageFromLeft(Image targetImage, float duration, float distance)
    {

        Color originalColor = targetImage.color;
        originalColor.a = 0f;
        targetImage.color = originalColor;

        RectTransform rt = targetImage.rectTransform;
        Vector2 finalPos = rt.anchoredPosition;
        Vector2 startPos = new Vector2(finalPos.x - distance, finalPos.y);

        rt.anchoredPosition = startPos;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            originalColor.a = t;
            targetImage.color = originalColor;

            rt.anchoredPosition = Vector2.Lerp(startPos, finalPos, t);

            yield return null;
        }

        // 보정 (알파값 / 위치)
        originalColor.a = 1f;
        targetImage.color = originalColor;
        rt.anchoredPosition = finalPos;
    }
    void CloseClosingText()
    {
        gameObject.SetActive(false);
        VictoryPanel.SetActive(true);
        

    }
    public void SetNarratorFontSize(float size)
    {
        if (CharacterName != null)
            CharacterName.fontSize = size;
    }
}

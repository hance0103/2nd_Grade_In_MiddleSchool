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
    [SerializeField] private GameObject Timer; // 타이머 활성화/비활성화 용도

    [Header("플레이어/보스 오브젝트")]
    private GameObject Player;
    private GameObject Boss;

    //변경 필요
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
    public void OnClosingText() // 다음 스테이지 버튼을 눌렀을 때 실행되는 패널
    {
        Player.SetActive(false);
        Timer.SetActive(false);
        NextButton.onClick.AddListener(OnNextButtonClicked);
        StartCoroutine(ClosingTextStage2());
    }
    IEnumerator ClosingTextStage2() //("등장인물", "대사")로 입력 
    {
        yield return StartCoroutine(FadeInImageFromRight(character2, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("주인공", "잘가라 [정념의 잔존물]이여...! 이걸로 끝이다!"));
        character2.gameObject.SetActive(false);
        yield return StartCoroutine(NormalChat("", "(소녀는 청심환을 삼켰다.)"));
        //yield return StartCoroutine(FadeInImageFromLeft(boss1, 0.5f, 100f));
        yield return StartCoroutine(NormalChat("거미", "큭... 승리도 너의 착각이다.."));
        yield return StartCoroutine(NormalChat("거미", "내 저주는 널 따라다닐 것이다.."));
        //boss1.gameObject.SetActive(false);
        yield return StartCoroutine(FadeInImageFromRight(character5, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("주인공", "흥, 어디 한 번 실컷 저주해 보거라 !"));
        character5.gameObject.SetActive(false);
        yield return StartCoroutine(FadeInImageFromRight(character2, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("주인공", "이 몸의 숙원을 막을 수 있다고 생각한 너의 그 [오만]..."));
        yield return StartCoroutine(NormalChat("주인공", "그것이 너의 패착이다.."));
        yield return StartCoroutine(NormalChat("주인공", "소음의 저편으로 사라져라."));
        character2.gameObject.SetActive(false);
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
}

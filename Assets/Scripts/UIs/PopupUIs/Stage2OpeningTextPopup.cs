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
    public GameObject ControllerPanel;
    public Button NextButton;
    public Button SkipButton;
    [SerializeField] private GameObject Timer; // 타이머 활성화/비활성화 용도
    public Image TimerBG;
    public GameObject controller;

    [Header("오프닝 스크립트 캐릭터/보스 스프라이트")]
    public Image character1; // 손가락 포즈, 비웃는 표정
    public Image character2; // 손가락 포즈, 진지한 표정
    public Image character3; // 머리에 손 포즈, 기본표정
    public Image character4; // 머리에 손 포즈, 놀란 표정
    public Image character5; // 머리에 손 포즈, 비웃는 표정
    public Image boss1;

    [Header("플레이어/보스 오브젝트")]
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject PlayerHP;
    [SerializeField] private GameObject Boss;
    [SerializeField] private GameObject BossHP;

    private bool isFullTextDisplayed = false;
    private bool isNextButtonClicked = false;
    public string writerText = "";
    public static bool isFirstTime2 = true;
    void Start()
    {
        //Debug.Log(isFirstTime2);
        if (!isFirstTime2)
        {
            OnSkipButtonClicked();
            Debug.Log("!isFristtime2");
        }
        else
        {
            Open();
            
        }
    }

    void Update()
    {

    }
    void Open()
    {
        NextButton.onClick.AddListener(OnNextButtonClicked);
        var timer = FindObjectOfType<Timer>();
        StartCoroutine(OpeningTextStage2());
        Timer.SetActive(false);
        timer.TimeActive = false;
    }
    IEnumerator OpeningTextStage2() //("등장인물", "대사")로 입력
    {
        character5.gameObject.SetActive(true);
        yield return StartCoroutine(FadeInImageFromRight(character5, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "무사히 도착했군-"));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "『약속된 종언』까지 얼마 남지 않았구나..."));
        character5.gameObject.SetActive(false);

        character3.gameObject.SetActive(true);
        yield return StartCoroutine(FadeInImageFromRight(character3, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "흐음 평화로워야 할 교실에 이 불길한 기운은 무엇이냐.."));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "자꾸만 걸음이 느려지고 손에 땀이 나는구나.."));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "손발이 어딘가에 속박되는 기분..."));
        character3.gameObject.SetActive(false);

        character4.gameObject.SetActive(true);
        yield return StartCoroutine(FadeInImageFromRight(character4, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "설마 [긴장]이란 것을 하고 있나??"));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "이 몸이 긴장 따위 할 리 없다"));
        character4.gameObject.SetActive(false);

        character3.gameObject.SetActive(true);
        yield return StartCoroutine(FadeInImageFromRight(character3, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "이런.."));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "분명 누군가 성가신 놈이 있는 것이 틀림없다..."));
        character3.gameObject.SetActive(false);

        character2.gameObject.SetActive(true);
        yield return StartCoroutine(FadeInImageFromRight(character2, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "자, 어디냐 ! 이 몸을 우롱하고도 무사하길 원하는 것은 아니겠지?"));
        character2.gameObject.SetActive(false);

        //yield return StartCoroutine(FadeInImageFromLeft(boss1, 0.5f, 100f));
        yield return StartCoroutine(NormalChat("???", "앞뒤 없이 덤비는 건 한결같구나"));
        yield return StartCoroutine(NormalChat("???", "나의 그물로 너의 몸 뿐만이 아니라 정신까지 [속박]시켜주마.."));
        //boss1.gameObject.SetActive(false);

        character2.gameObject.SetActive(true);
        yield return StartCoroutine(FadeInImageFromRight(character2, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "역시 네 녀석의 소행이었나...!"));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "[영혼의 직조자]...!"));
        character2.gameObject.SetActive(false);

        character5.gameObject.SetActive(true);
        yield return StartCoroutine(FadeInImageFromRight(character5, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "이 몸에게 걸맞는 [예의]를 갖출 수 있도록 무참히 교육시켜주마"));
        yield return StartCoroutine(NormalChat("적혈의 서약자", "덤벼라 !!"));
        character5.gameObject.SetActive(false);

        CloseOpeningText();
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

    void CloseOpeningText()
    {
        OpeningTextPanel.SetActive(false); // 패널 비활성화
        ControllerPanel.SetActive(true);
        isFirstTime2 = false;
        Time.timeScale = 1f;
        var timer = FindObjectOfType<Timer>();
        // 타이머의 TimeActive 켜고, 코루틴 수동 실행
        timer.TimeActive = true;
        Time.timeScale = 1;
        Timer.SetActive(true);
        TimerBG.gameObject.SetActive(true);
        timer.TimeActive = true;
        Player.SetActive(true);
        Boss.SetActive(true);
        PlayerHP.SetActive(true);
        BossHP.SetActive(true);
        controller.SetActive(true);
    }
}

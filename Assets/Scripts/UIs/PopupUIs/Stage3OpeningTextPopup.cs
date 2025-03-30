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
    public GameObject ControllerPanel;
    public Button NextButton;
    public Button SkipButton;
    public Transform targetObject;
    public Vector3 targetPosition;
    public float moveDuration = 2f;
    [SerializeField] private GameObject Timer; // 타이머 활성화/비활성화 용도
    public Image TimerBG;
    public GameObject controller;

    [Header("오프닝 스크립트 캐릭터/보스 스프라이트")]
    public GameObject CharacterPose1; // 손가락 포즈, 신난 표정
    public GameObject CharacterPose2; // 머리에 손 포즈, 눈 감고 미소
    public GameObject CharacterPose3; // 손가락 포즈, 화난 표정(이글이글)
    public GameObject CharacterPose4; // 머리에 손 포즈, 어둡고 째려보는 표정(분노를 억누르는 듯한)
    public GameObject CharacterPose5; // 젖히고 웃는 포즈
    public GameObject CharacterPose6; // 머리에 손 포즈, 진지한 표정
    public GameObject CharacterPose7; // 눈에 붉은기운이 돈다

    [Header("플레이어/보스 오브젝트")]
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject PlayerHP;
    [SerializeField] private GameObject Boss;
    [SerializeField] private GameObject BossHP;

    private bool isFullTextDisplayed = false;
    private bool isNextButtonClicked = false;
    public string writerText = "";
    public static bool isFirstTime3 = true;
    void Start()
    {
        if (!isFirstTime3)
        {
            OnSkipButtonClicked();
            Debug.Log("!isFristtime");
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
        StartCoroutine(OpeningTextStage3());
        Timer.SetActive(false);
        Time.timeScale = 0;
    }
    IEnumerator OpeningTextStage3() //("등장인물", "대사")로 입력
    {
        yield return StartCoroutine(NormalChat("", "(지루한 수업시간...)"));
        SoundManager.Instance.EffectSoundOn("28");
        yield return StartCoroutine(NormalChat("", "(딩동댕동~)"));
        yield return StartCoroutine(NormalChat("주인공", "드디어..!"));
        yield return StartCoroutine(NormalChat("", "(후다닥 뛰쳐 나간다)"));
        SoundManager.Instance.EffectSoundOn("running");
        StartCoroutine(MoveToPosition(targetObject, targetPosition, moveDuration));
        yield return StartCoroutine(NormalChat("주인공", "두근거린다.."));
        yield return StartCoroutine(NormalChat("주인공", "하루종일 기다렸던 무대..."));
        yield return StartCoroutine(NormalChat("주인공", "알 수 없는 이 떨림"));
        yield return StartCoroutine(NormalChat("주인공", "밴드의 선율이 온몸을 감싼다"));
        yield return StartCoroutine(NormalChat("주인공", "짜릿해 ! 즐거워 !"));
        yield return StartCoroutine(NormalChat("주인공", "이 순간이 영원했으면.."));
        yield return StartCoroutine(NormalChat("주인공", "알 수 없는 이 떨림"));

        yield return StartCoroutine(NormalChat("최종보스", "즐겁니?"));

        yield return StartCoroutine(NormalChat("주인공", "너..너는??"));
        yield return StartCoroutine(NormalChat("주인공", "내 숙원의 적이자 라이벌.."));
        yield return StartCoroutine(NormalChat("주인공", "하필 이런 곳에서 이 타이밍에.."));
        yield return StartCoroutine(NormalChat("주인공", "네놈은 항상 날 방해하는군.."));

        yield return StartCoroutine(NormalChat("최종보스", "후후.."));
        yield return StartCoroutine(NormalChat("최종보스", "무대를 망치러 왔다 !"));
        yield return StartCoroutine(NormalChat("최종보스", "함께 놀아 볼까?"));

        yield return StartCoroutine(NormalChat("주인공", "인정하기 싫지만 네놈은 나와 견줄 만한 [힘]을 갖고 있다"));
        yield return StartCoroutine(NormalChat("주인공", "승리를 장담할 수는 없지만.."));
        yield return StartCoroutine(NormalChat("주인공", "네놈이 원하는 대로 흘러가도록 둘 수 없다"));
        yield return StartCoroutine(NormalChat("주인공", "덤벼라"));
        yield return StartCoroutine(NormalChat("", "전투에 진입합니다"));
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
    private IEnumerator MoveToPosition(Transform objTransform, Vector3 endPos, float duration)
    {
        Vector3 startPos = objTransform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            objTransform.position = Vector3.Lerp(startPos, endPos, t);
            elapsedTime += Time.unscaledDeltaTime;

            yield return null;
        }
        objTransform.position = endPos;
    }
    void CloseOpeningText()
    {
        targetObject.position = targetPosition;
        OpeningTextPanel.SetActive(false); // 패널 비활성화
        ControllerPanel.SetActive(true);
        isFirstTime3 = false;
        Time.timeScale = 1f;
        var timer = FindObjectOfType<Timer>();
        // 타이머의 TimeActive 켜고, 코루틴 수동 실행
        Timer.SetActive(true);
        TimerBG.gameObject.SetActive(true);
        Time.timeScale = 1;
        timer.TimeActive = true;
        Player.SetActive(true);
        Boss.SetActive(true);
        PlayerHP.SetActive(true);
        BossHP.SetActive(true);
        controller.SetActive(true);
    }
}

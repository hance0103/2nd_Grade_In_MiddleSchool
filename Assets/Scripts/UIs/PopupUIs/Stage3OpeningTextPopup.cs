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
    }
    IEnumerator OpeningTextStage3() //("등장인물", "대사")로 입력
    {
        yield return StartCoroutine(FadeInImageFromRight(character5, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("주인공", "여기서 한발만 내딛으면... 분명 이 몸에 어울리는 동료를 많이..."));
        character5.gameObject.SetActive(false);

        yield return StartCoroutine(NormalChat("", "..."));
        yield return StartCoroutine(NormalChat("???", "( 고작 공연 따위를 잘한다고 동료가 생길까..?)"));

        yield return StartCoroutine(FadeInImageFromRight(character3, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("주인공", "... "));
        yield return StartCoroutine(NormalChat("주인공", "분명..."));
        character3.gameObject.SetActive(false);

        yield return StartCoroutine(NormalChat("???", "( 그럼 너의 그 1년 반은 뭐였지? )"));
        yield return StartCoroutine(NormalChat("", "..."));
        yield return StartCoroutine(NormalChat("???", "( 이 현실을 그냥 외면하고 있던 것 아닌가? )"));

        yield return StartCoroutine(FadeInImageFromRight(character3, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("주인공", "알아... "));
        yield return StartCoroutine(NormalChat("주인공", "안다고...! "));
        yield return StartCoroutine(NormalChat("주인공", "이 이상한 성격 때문인 거 잘 안다고..."));
        yield return StartCoroutine(NormalChat("주인공", "그렇지만 두렵다고..! "));
        yield return StartCoroutine(NormalChat("주인공", "사람들이랑 대화하는 것도..."));
        yield return StartCoroutine(NormalChat("주인공", "누군가와 관계를 맺는 것도..."));
        character3.gameObject.SetActive(false);

        yield return StartCoroutine(NormalChat("???", "( 분명 실수 하게 될 것이다. )"));

        yield return StartCoroutine(FadeInImageFromRight(character2, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("주인공", "...아니야!"));
        yield return StartCoroutine(NormalChat("주인공", "두 손을 벌벌 떨면서 공연 신청을 할 때도...!"));
        yield return StartCoroutine(NormalChat("주인공", "실수 하지 않으려고 수 백 수 천 번을 연습할 때도...! "));
        yield return StartCoroutine(NormalChat("주인공", "난 내 자신을 바꾸기 위해서 선택했고"));
        yield return StartCoroutine(NormalChat("주인공", "여기까지 온거라고...!."));
        character2.gameObject.SetActive(false);

        yield return StartCoroutine(NormalChat("???", "( 그렇다면 증명해라. )"));

        yield return StartCoroutine(FadeInImageFromRight(character3, 0.2f, 100f));
        yield return StartCoroutine(NormalChat("주인공", "..."));
        yield return StartCoroutine(NormalChat("주인공", "그래... "));
        yield return StartCoroutine(NormalChat("주인공", "들어줘... 나의 시작의 광시곡을..."));
        character3.gameObject.SetActive(false);

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
        //targetObject.position = targetPosition;
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

using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stage3ClosingTextPopup : MonoBehaviour
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
    public GameObject CharacterPose1; // 손가락 포즈, 비웃는 표정
    public GameObject CharacterPose2; // 머리에 손 포즈, 진지한 표정
    public GameObject CharacterPose3; // 자지러지게 웃는 표정
    public GameObject CharacterPose4; // 머리에 손 포즈, 신난 표정
    public GameObject BossPose;

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
        StartCoroutine(ClosingTextStage3());
    }
    IEnumerator ClosingTextStage3() //("등장인물", "대사")로 입력 
    {
        //변경 필요
        yield return StartCoroutine(NormalChat("주인공", "후훗, 네놈 따위가 [(별명)]에게 이길 수 있을 리가 없지"));
        yield return StartCoroutine(NormalChat("주인공", "당연하고 시시한 승리다."));
        yield return StartCoroutine(NormalChat("주인공", "드디어 끝인가"));
        yield return StartCoroutine(NormalChat("최종보스", "크윽..오늘은 여기까지만 하지.."));
        yield return StartCoroutine(NormalChat("최종보스", "하지만 너에게 도사리는 위험은 나뿐만이 아니다.."));
        yield return StartCoroutine(NormalChat("주인공", "크큭..그 꼴로 말은 잘 하는구나 !!"));
        yield return StartCoroutine(NormalChat("주인공", "이만 아디오스"));
        yield return StartCoroutine(NormalChat("", "모든 스테이지를 클리어하였습니다"));
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

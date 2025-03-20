using UnityEngine;
using UnityEngine.UI; // 일반 UI를 쓴다면
using TMPro;

public class VictoryTextInputPopup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField inputField; // TextMeshPro 버전
    [SerializeField] private Timer timer;
    [SerializeField] public GameObject Controller;
    public GameObject FinishPanel;
    private string savedData;

    [Header("Fade Image (화면 전체 덮는 Image)")]
    [SerializeField] private Image fadeImage; // 검은색 이미지

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 2f; // 페이드에 걸릴 시간

    [Header("Player & Boss")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject boss;
    /// <summary>
    /// 외부 버튼(메인 버튼)에서 이 함수를 연결하여 패널을 열도록 함
    /// </summary>
    public void Stage1OpenInputPanel()
    {
        StartCoroutine(DoFadeSequenceAndRespawn(-4.0f, -2.5f, 6.5f, -1.5f));
        Controller.SetActive(false);
        Timer timer = FindObjectOfType<Timer>();
        timer.TimeActive = false;
        // 패널이 열릴 때 입력란 초기화
        inputField.text = "";
    }
    public void Stage2OpenInputPanel()
    {
        StartCoroutine(DoFadeSequenceAndRespawn(-4.0f, -2.5f, 6.5f, -1.5f));
        Controller.SetActive(false);
        Timer timer = FindObjectOfType<Timer>();
        timer.TimeActive = false;
        // 패널이 열릴 때 입력란 초기화
        inputField.text = "";
    }

    public void Stage3OpenInputPanel()
    {
        StartCoroutine(DoFadeSequenceAndRespawn(-4.0f, -2.5f, 6.5f, -1.5f));
        Controller.SetActive(false);
        Timer timer = FindObjectOfType<Timer>();
        timer.TimeActive = false;
        // 패널이 열릴 때 입력란 초기화
        inputField.text = "";
    }
    /// <summary>
    /// 확인/저장 버튼 기능 , 스테이지 별로 피니쉬 대사를 따로 적용할 수 있도록 구분해주기
    /// </summary>
    public void Stage1SavePanel()
    {
        SoundManager.Instance.EffectSoundOn("3");
        savedData = inputField.text;
        PlayerPrefs.SetString("FinalText1", savedData);
        PlayerPrefs.Save();
        boss.SetActive(true);
        GameManager.isPlayerZoomOutAllowed = true;
        ClosePanel();
    }
    public void Stage2SavePanel()
    {
        SoundManager.Instance.EffectSoundOn("3");
        savedData = inputField.text;
        PlayerPrefs.SetString("FinalText2", savedData);
        PlayerPrefs.Save();
        boss.SetActive(true);
        GameManager.isPlayerZoomOutAllowed = true;
        StartCoroutine(DoFadeSequenceAndRespawn(-4.0f, -2.5f, 6.5f, -1.5f));
        ClosePanel();
    }
    public void Stage3SavePanel()
    {
        SoundManager.Instance.EffectSoundOn("3");
        savedData = inputField.text;
        PlayerPrefs.SetString("FinalText3", savedData);
        
        PlayerPrefs.Save();
        boss.SetActive(true);
        GameManager.isPlayerZoomOutAllowed = true;
        StartCoroutine(DoFadeSequenceAndRespawn(-4.0f, -2.5f, 6.5f, -1.5f));
        ClosePanel();
    }
    /// <summary>
    /// 페이드 아웃 → Player/Boss 재배치·활성화 → 페이드 인 → 게임 재시작
    /// </summary>
    private System.Collections.IEnumerator DoFadeSequenceAndRespawn(
        float playerX, float playerY,
        float bossX, float bossY)
    {
        //  텍스트 입력창 닫기
        inputField.gameObject.SetActive(false);

        //  페이드 아웃
        yield return StartCoroutine(FadeOutCoroutine());

        //  Player 재배치 및 활성화
        if (player != null)
        {
            player.transform.position = new Vector3(playerX, playerY, 0f);
            player.SetActive(true);
        }
        
        

        // 4) 페이드 인
        yield return StartCoroutine(FadeInCoroutine());

        // 5) 게임 재시작
        Time.timeScale = 1f;

        // 만약 ZoomOutAllowed 등의 게임 이벤트가 필요하다면
        GameManager.isPlayerZoomOutAllowed = true;
    }

    /// <summary>
    /// (코루틴) 화면을 점점 검게(Alpha 0→1) 만드는 페이드 아웃
    /// </summary>
    private System.Collections.IEnumerator FadeOutCoroutine()
    {
        float startAlpha = fadeImage.color.a; // 현재 알파 (보통 0으로 가정)
        float endAlpha = 1f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Time.timeScale=0이어도 페이드 동작하도록
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);

            Color c = fadeImage.color;
            c.a = newAlpha;
            fadeImage.color = c;

            yield return null;
        }

        // 최종 보정
        Color finalColor = fadeImage.color;
        finalColor.a = 1f;
        fadeImage.color = finalColor;
    }

    /// <summary>
    /// (코루틴) 화면을 점점 밝게(Alpha 1→0) 만드는 페이드 인
    /// </summary>
    private System.Collections.IEnumerator FadeInCoroutine()
    {
        float startAlpha = fadeImage.color.a; // 보통 1(완전 검정)
        float endAlpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);

            Color c = fadeImage.color;
            c.a = newAlpha;
            fadeImage.color = c;

            yield return null;
        }

        // 최종 보정
        Color finalColor = fadeImage.color;
        finalColor.a = 0f;
        fadeImage.color = finalColor;
        inputField.gameObject.SetActive(true);
    }

   
    void ClosePanel()
    {
        gameObject.SetActive(false);
        FinishPanel.SetActive(true);
    }
}

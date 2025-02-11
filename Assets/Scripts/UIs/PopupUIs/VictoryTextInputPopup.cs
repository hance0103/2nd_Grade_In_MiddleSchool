using UnityEngine;
using UnityEngine.UI; // 일반 UI를 쓴다면
using TMPro;

public class VictoryTextInputPopup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField inputField; // TextMeshPro 버전
    [SerializeField] private Timer timer;
    private string savedData;

    [Header("Fade Image (화면 전체 덮는 Image)")]
    [SerializeField] private Image fadeImage; // 검은색 이미지

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f; // 페이드에 걸릴 시간

    [Header("Player & Boss")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject boss;
    /// <summary>
    /// 외부 버튼(메인 버튼)에서 이 함수를 연결하여 패널을 열도록 함
    /// </summary>
    public void Stage1OpenInputPanel()
    {
        Time.timeScale = 0f; // 시간 정지
        
        // 패널이 열릴 때 입력란 초기화
        inputField.text = "";

        // 1. Timer 컴포넌트를 찾아서
        Timer timer = FindObjectOfType<Timer>();

        if (timer != null)
        {
            // 2. TimeActive를 false로 변경하여 타이머 정지
            timer.TimeActive = false;
            Debug.Log(timer.curTime);
            // 3. 측정된 시간( curTime or CurrentTime )을 PlayerPrefs로 저장
            PlayerPrefs.SetFloat("FinalTime1", timer.curTime);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Timer 스크립트를 찾을 수 없습니다!");
        }
    }
    public void Stage2OpenInputPanel()
    {
        Time.timeScale = 0f; // 시간 정지
        gameObject.SetActive(true);
        // 패널이 열릴 때 입력란 초기화
        inputField.text = "";

        // 1. Timer 컴포넌트를 찾아서
        Timer timer = FindObjectOfType<Timer>();

        if (timer != null)
        {
            // 2. TimeActive를 false로 변경하여 타이머 정지
            timer.TimeActive = false;
            Debug.Log(timer.curTime);
            // 3. 측정된 시간( curTime or CurrentTime )을 PlayerPrefs로 저장
            PlayerPrefs.SetFloat("FinalTime2", timer.curTime);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Timer 스크립트를 찾을 수 없습니다!");
        }
    }

    public void Stage3OpenInputPanel()
    {
        Debug.Log("Stage1OpenInputPanel() 호출됨");
        Time.timeScale = 0f; // 시간 정지
        gameObject.SetActive(true);
        // 패널이 열릴 때 입력란 초기화
        inputField.text = "";

        // 1. Timer 컴포넌트를 찾아서
        Timer timer = FindObjectOfType<Timer>();

        if (timer != null)
        {
            // 2. TimeActive를 false로 변경하여 타이머 정지
            timer.TimeActive = false;
            Debug.Log(timer.curTime);
            // 3. 측정된 시간( curTime or CurrentTime )을 PlayerPrefs로 저장
            PlayerPrefs.SetFloat("FinalTime3", timer.curTime);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Timer 스크립트를 찾을 수 없습니다!");
        }
    }
    /// <summary>
    /// 확인/저장 버튼 기능 , 스테이지 별로 피니쉬 대사를 따로 적용할 수 있도록 구분해주기
    /// </summary>
    public void Stage1SaveAndClosePanel()
    {
        savedData = inputField.text;
        PlayerPrefs.SetString("FinalText1", savedData);
        PlayerPrefs.Save();
        boss.SetActive(true);
        GameManager.isPlayerZoomOutAllowed = true;
        StartCoroutine(DoFadeSequenceAndRespawn(-4.0f, -2.5f, 6.5f, -1.5f));
    }
    public void Stage2SaveAndClosePanel()
    {
        savedData = inputField.text;
        PlayerPrefs.SetString("FinalText2", savedData);
        PlayerPrefs.Save();
        boss.SetActive(true);
        GameManager.isPlayerZoomOutAllowed = true;
        StartCoroutine(DoFadeSequenceAndRespawn(-4.0f, -2.5f, 6.5f, -1.5f));
    }
    public void Stage3SaveAndClosePanel()
    {
        savedData = inputField.text;
        PlayerPrefs.SetString("FinalText3", savedData);
        
        PlayerPrefs.Save();
        boss.SetActive(true);
        GameManager.isPlayerZoomOutAllowed = true;
        StartCoroutine(DoFadeSequenceAndRespawn(-4.0f, -2.5f, 6.5f, -1.5f));
    }
    /// <summary>
    /// 페이드 아웃 → Player/Boss 재배치·활성화 → 페이드 인 → 게임 재시작
    /// </summary>
    private System.Collections.IEnumerator DoFadeSequenceAndRespawn(
        float playerX, float playerY,
        float bossX, float bossY)
    {
        inputField.gameObject.SetActive(false);

        // 2) 페이드 아웃
        yield return StartCoroutine(FadeOutCoroutine());

        // 3) Player, Boss 재배치 및 활성화
        if (player != null)
        {
            player.transform.position = new Vector3(playerX, playerY, 0f);
            player.SetActive(true);
        }
        if (boss != null)
        {
            boss.transform.position = new Vector3(bossX, bossY, 0f);
            boss.SetActive(true);
        }
        // 여기서 HP 초기화, 애니 리셋, etc. 필요한 내용이 있으면 해도 됨

        // 4) 페이드 인
        yield return StartCoroutine(FadeInCoroutine());

        // 5) 게임 재시작
        Time.timeScale = 1f;

        // 만약 ZoomOutAllowed 등의 게임 이벤트가 필요하다면
        GameManager.isPlayerZoomOutAllowed = true;
    }

    // -----------------------------
    //    실제 페이드 코루틴들
    // -----------------------------

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
    }
}

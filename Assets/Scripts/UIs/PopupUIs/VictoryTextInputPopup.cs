using UnityEngine;
using UnityEngine.UI; // 일반 UI를 쓴다면
using TMPro;

public class VictoryTextInputPopup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField inputField; // TextMeshPro 버전
    [SerializeField] private Timer timer;
    private string savedData;

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
        gameObject.SetActive(false);
    }
    public void Stage2SaveAndClosePanel()
    {
        savedData = inputField.text;
        PlayerPrefs.SetString("FinalText2", savedData);
        PlayerPrefs.Save();
        gameObject.SetActive(false);
    }
    public void Stage3SaveAndClosePanel()
    {
        savedData = inputField.text;
        PlayerPrefs.SetString("FinalText3", savedData);
        PlayerPrefs.Save();
        gameObject.SetActive(false);
    }
}

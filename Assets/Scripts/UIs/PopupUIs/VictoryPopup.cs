using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupVictory : MonoBehaviour
{ 

    // 승리 팝업 열기 (게임 일시정지)
    public void OpenVictory1() //스테이지 분류로 시간 저장 영역 다르게
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
        //manager.Action(scanObject);

        // 1. Timer 컴포넌트를 찾아서
        Timer timer = FindObjectOfType<Timer>();
        if (timer != null)
        {
            // 2. TimeActive를 false로 변경하여 타이머 정지
            timer.TimeActive = false;

            // 3. 측정된 시간( curTime or CurrentTime )을 PlayerPrefs로 저장
            PlayerPrefs.SetFloat("FinalTime1", timer.curTime);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Timer 스크립트를 찾을 수 없습니다!");
        }
    }
    public void OpenVictory2() //스테이지 2
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
        //manager.Action(scanObject);

        // 1. Timer 컴포넌트를 찾아서
        Timer timer = FindObjectOfType<Timer>();
        if (timer != null)
        {
            // 2. TimeActive를 false로 변경하여 타이머 정지
            timer.TimeActive = false;

            // 3. 측정된 시간( curTime or CurrentTime )을 PlayerPrefs로 저장
            PlayerPrefs.SetFloat("FinalTime2", timer.CurrentTime);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Timer 스크립트를 찾을 수 없습니다!");
        }
    }
    public void OpenVictory3() //스테이지 3
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
        //manager.Action(scanObject);

        // 1. Timer 컴포넌트를 찾아서
        Timer timer = FindObjectOfType<Timer>();
        if (timer != null)
        {
            // 2. TimeActive를 false로 변경하여 타이머 정지
            timer.TimeActive = false;

            // 3. 측정된 시간( curTime or CurrentTime )을 PlayerPrefs로 저장
            PlayerPrefs.SetFloat("FinalTime3", timer.CurrentTime);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Timer 스크립트를 찾을 수 없습니다!");
        }
    }
    // 승리 팝업 닫기 (게임 재개)
    public void CloseVictory()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f; // 시간 재개
    }

    // 재시작 버튼 (게임 재시작)
    public void RestartGame()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f; // 시간 재개
    }
    
}
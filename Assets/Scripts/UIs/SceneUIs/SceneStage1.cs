using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStage1 : MonoBehaviour
{
    public void LoadMain()
    {
        Stage1OpeningTextPopup.isFirstTime1 = true;
        SceneManager.LoadScene("Main");
        SoundManager.Instance.MainBgmOn();
        Time.timeScale = 1.0f;
    }
    public void LoadStageSelect()
    {
        Stage1OpeningTextPopup.isFirstTime1 = true;
        SceneManager.LoadScene("StageSelect");
        SoundManager.Instance.MainBgmOn();
        Time.timeScale = 1.0f;
    }
    public void LoadStage2()
    {
        Stage1OpeningTextPopup.isFirstTime1 = true;
        SceneManager.LoadScene("Stage2");
        SoundManager.Instance.Stage2BgmOn();
        Time.timeScale = 1.0f;
    }
    public GameObject settingsPopup; // 설정 팝업
    public GameObject creditsPopup; // 크레딧 팝업
    public GameObject PausePopup; // 일시정지 팝업
    public GameObject TextPopup;
    // 설정 팝업 열기
    public void OpenSettings()
    {
        settingsPopup.SetActive(true);
    }

    // 설정 팝업 닫기
    public void CloseSettings()
    {
        settingsPopup.SetActive(false);
    }

    // 크레딧 팝업 열기
    public void OpenCredits()
    {
        creditsPopup.SetActive(true);
    }

    // 크레딧 팝업 닫기
    public void CloseCredits()
    {
        creditsPopup.SetActive(false);
    }
    public void OpenText()
    {
        TextPopup.SetActive(true);
    }
    public void CloseText()
    {
        TextPopup.SetActive(false);
    }

    // 일시정지 팝업 열기 (게임 일시정지)
    public void OpenPause()
    {
        PausePopup.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
    }

    // 일시정지 팝업 닫기 (게임 재개)
    public void ClosePause()
    {
        PausePopup.SetActive(false);
        Time.timeScale = 1f; // 시간 재개
    }

    // 재시작 버튼 (게임 재시작)
    public void RestartGame()
    {
        PausePopup.SetActive(false);
        Time.timeScale = 1f; // 시간 재개
        // 게임 재시작 로직 추가 (필요 시 장면 다시 로드 등)
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}

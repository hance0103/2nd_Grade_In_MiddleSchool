using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using TMPro.Examples;
public class StageSelect : MonoBehaviour
{
    public void LoadMainNoChange()
    {
        SceneManager.LoadScene("Main");
        Time.timeScale = 1.0f;
    }
    public void LoadStage1()
    {
        SceneManager.LoadScene("Stage1");
        SoundManager.Instance.Stage1BgmOn();
        Time.timeScale = 1.0f;
    }

    public void LoadStage2()
    {
        SceneManager.LoadScene("Stage2");
        SoundManager.Instance.Stage2BgmOn();
        Time.timeScale = 1.0f;
    }

    public void LoadStage3()
    {
        SceneManager.LoadScene("Stage3");
        SoundManager.Instance.Stage3BgmOn();
        Time.timeScale = 1.0f;
    }

    public GameObject settingsPopup; // 설정 팝업
   
    public GameObject creditsPopup; // 크레딧 팝업

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
    
}

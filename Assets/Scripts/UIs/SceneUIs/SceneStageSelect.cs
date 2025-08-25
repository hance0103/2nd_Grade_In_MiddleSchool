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
        SoundManager.Instance.EffectSoundOn("3");
        SceneManager.LoadScene("Main");
        Time.timeScale = 1.0f;
    }
    public void LoadStage1()
    {
        GameManager.Inst.SetNowStage(1);
        SoundManager.Instance.EffectSoundOn("3");
        SceneManager.LoadScene("Stage1");
        SoundManager.Instance.Stage1BgmOn();
        Time.timeScale = 1.0f;
    }

    public void LoadStage2()
    {
        GameManager.Inst.SetNowStage(2);
        SoundManager.Instance.EffectSoundOn("3");
        SceneManager.LoadScene("Stage2");
        SoundManager.Instance.Stage2BgmOn();
        Time.timeScale = 1.0f;
    }

    public void LoadStage3()
    {
        GameManager.Inst.SetNowStage(3);
        SoundManager.Instance.EffectSoundOn("3");
        SceneManager.LoadScene("Stage3");
        SoundManager.Instance.Stage3BgmOn();
        Time.timeScale = 1.0f;
    }

    public GameObject settingsPopup; // 설정 팝업
   
    public GameObject creditsPopup; // 크레딧 팝업

    // 설정 팝업 열기
    public void OpenSettings()
    {
        SoundManager.Instance.EffectSoundOn("3");
        settingsPopup.SetActive(true);
    }

    // 설정 팝업 닫기
    public void CloseSettings()
    {
        SoundManager.Instance.EffectSoundOn("3");
        settingsPopup.SetActive(false);
    }

    // 크레딧 팝업 열기
    public void OpenCredits()
    {
        SoundManager.Instance.EffectSoundOn("3");
        creditsPopup.SetActive(true);
    }

    // 크레딧 팝업 닫기
    public void CloseCredits()
    {
        SoundManager.Instance.EffectSoundOn("3");
        creditsPopup.SetActive(false);
    }
    
}

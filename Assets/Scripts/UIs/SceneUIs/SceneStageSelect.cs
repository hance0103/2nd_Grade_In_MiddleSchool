using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        SoundManager.Instance.StageBgmOn();
        Time.timeScale = 1.0f;
    }

    public void LoadStage2()
    {
        SceneManager.LoadScene("Stage2");
        SoundManager.Instance.StageBgmOn();
        Time.timeScale = 1.0f;
    }

    public void LoadStage3()
    {
        SceneManager.LoadScene("Stage3");
        SoundManager.Instance.StageBgmOn();
        Time.timeScale = 1.0f;
    }

    public GameObject settingsPopup; // ¼³Á¤ ÆË¾÷
    public GameObject DiaryPopup;  // ´ÙÀÌ¾î¸® ÆË¾÷
    public GameObject creditsPopup; // Å©·¹µ÷ ÆË¾÷

    // ¼³Á¤ ÆË¾÷ ¿­±â
    public void OpenSettings()
    {
        settingsPopup.SetActive(true);
    }

    // ¼³Á¤ ÆË¾÷ ´Ý±â
    public void CloseSettings()
    {
        settingsPopup.SetActive(false);
    }

    // Å©·¹µ÷ ÆË¾÷ ¿­±â
    public void OpenCredits()
    {
        creditsPopup.SetActive(true);
    }

    // Å©·¹µ÷ ÆË¾÷ ´Ý±â
    public void CloseCredits()
    {
        creditsPopup.SetActive(false);
    }

    // ´ÙÀÌ¾î¸® ÆË¾÷ ¿­±â
    public void OpenDiary()
    {
        DiaryPopup.SetActive(true);
    }

    // ´ÙÀÌ¾î¸® ÆË¾÷ ´Ý±â
    public void CloseDiary()
    {
        DiaryPopup.SetActive(false);
    }
}

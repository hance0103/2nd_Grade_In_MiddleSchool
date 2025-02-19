using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadMain()
    {
        SoundManager.Instance.EffectSoundOn("3");
        SceneManager.LoadScene("Main");
        SoundManager.Instance.MainBgmOn();
        Time.timeScale = 1.0f;
    }

    public void LoadMainNoChange()
    {
        SoundManager.Instance.EffectSoundOn("3");
        SceneManager.LoadScene("Main");
        Time.timeScale = 1.0f;
    }

    public void LoadStageSelectfromMain()
    {
        SoundManager.Instance.EffectSoundOn("3");
        SceneManager.LoadScene("StageSelect");
        Time.timeScale = 1.0f;
    }

    public void LoadStageSelect()
    {
        SoundManager.Instance.EffectSoundOn("3");
        SceneManager.LoadScene("StageSelect");
        SoundManager.Instance.MainBgmOn();
        Time.timeScale = 1.0f;
    }

    public void LoadStage1()
    {
        SoundManager.Instance.EffectSoundOn("3");
        SceneManager.LoadScene("Stage1");
        SoundManager.Instance.Stage1BgmOn();
        Time.timeScale = 1.0f;
    }

    public void LoadStage2()
    {
        SoundManager.Instance.EffectSoundOn("3");
        SceneManager.LoadScene("Stage2");
        SoundManager.Instance.Stage2BgmOn();
        Time.timeScale = 1.0f;
    }

    public void LoadStage3()
    {
        SoundManager.Instance.EffectSoundOn("3");
        SceneManager.LoadScene("Stage3");
        SoundManager.Instance.Stage3BgmOn();
        Time.timeScale = 1.0f;
    }
}
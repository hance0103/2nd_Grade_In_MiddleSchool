using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadMain()
    {
        SceneManager.LoadScene("Main");
        SoundManager.Instance.MainBgmOn();
    }

    public void LoadMainNoChange()
    {
        SceneManager.LoadScene("Main");
    }

    public void LoadStageSelectfromMain()
    {
        SceneManager.LoadScene("StageSelect");
    }

    public void LoadStageSelect()
    {
        SceneManager.LoadScene("StageSelect");
        SoundManager.Instance.MainBgmOn();
    }

    public void LoadStage1()
    { 
        SceneManager.LoadScene("Stage1");
        SoundManager.Instance.StageBgmOn();
    }

    public void LoadStage2()
    {
        SceneManager.LoadScene("Stage2");
        SoundManager.Instance.StageBgmOn();
    }

    public void LoadStage3()
    {
        SceneManager.LoadScene("Stage3");
        SoundManager.Instance.StageBgmOn();
    }

    
}
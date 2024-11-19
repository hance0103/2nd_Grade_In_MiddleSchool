using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public AudioClip mainBGM;
    public AudioClip stage1BGM;
    public AudioClip stage2BGM;
    public AudioClip stage3BGM;
    
    public void LoadMain()
    {
        DestroySoundManagerInstance(); // 기존 SoundManager 인스턴스를 제거
        SceneManager.LoadScene("Main");
        if (mainBGM != null)
        {
            SoundManager.Instance.ChangeBGM(mainBGM);
        }
    }

    public void LoadMainNoChange()
    {
        SceneManager.LoadScene("Main");
    }

    public void LoadStageSelectNoChange()
    {
        SceneManager.LoadScene("StageSelect");
    }

    public void LoadStageSelect()
    {
        DestroySoundManagerInstance(); // 기존 SoundManager 인스턴스를 제거
        SceneManager.LoadScene("StageSelect");
        if (mainBGM != null)
        {
            SoundManager.Instance.ChangeBGM(mainBGM);
        }
    }

    public void LoadStage1()
    {
        DestroySoundManagerInstance(); // 기존 SoundManager 인스턴스를 제거
        SceneManager.LoadScene("Stage1");
        if (stage1BGM != null)
        {
            SoundManager.Instance.ChangeBGM(stage1BGM);
        }
    }

    public void LoadStage2()
    {
        DestroySoundManagerInstance(); // 기존 SoundManager 인스턴스를 제거
        SceneManager.LoadScene("Stage2");
        if (stage2BGM != null)
        {
            SoundManager.Instance.ChangeBGM(stage2BGM);
        }
    }

    public void LoadStage3()
    {
        DestroySoundManagerInstance(); // 기존 SoundManager 인스턴스를 제거
        SceneManager.LoadScene("Stage3");
        if (stage3BGM != null)
        {
            SoundManager.Instance.ChangeBGM(stage3BGM);
        }
    }

    private void DestroySoundManagerInstance()
    {
        if (SoundManager.Instance != null)
        {
            Destroy(SoundManager.Instance.gameObject);
        }
    }
    
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    public AudioClip stage1BGM;
    public AudioClip stage2BGM;
    public AudioClip stage3BGM;
    public AudioClip VictoryBGM;

    public GameObject VictoryPopup; // 설정 팝업

    // 승리 팝업 열기 (게임 일시정지)
    public void OpenVictory()
    {
        VictoryPopup.SetActive(true);
        Time.timeScale = 0f; // 시간 정지

        
        
        
        SoundManager.Instance.ChangeBGM(VictoryBGM);
        
    }

    // 승리 팝업 닫기 (게임 재개)
    public void CloseVictory()
    {
        VictoryPopup.SetActive(false);
        Time.timeScale = 1f; // 시간 재개
    }

    // 재시작 버튼 (게임 재시작)
    public void RestartGame()
    {
        VictoryPopup.SetActive(false);
        Time.timeScale = 1f; // 시간 재개
        
        DestroySoundManagerInstance(); // 기존 SoundManager 인스턴스를 제거
        SceneManager.LoadScene("Stage1");
        if (stage1BGM != null)
        {
            SoundManager.Instance.ChangeBGM(stage1BGM);
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
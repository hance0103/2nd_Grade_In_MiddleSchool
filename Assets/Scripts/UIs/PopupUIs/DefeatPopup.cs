using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatManager : MonoBehaviour
{
    public AudioClip stage1BGM;
    public AudioClip stage2BGM;
    public AudioClip stage3BGM;
    public AudioClip DefeatBGM;

    public GameObject DefeatPopup; // 설정 팝업

    // 승리 팝업 열기 (게임 일시정지)
    public void OpenDefeat()
    {
        DefeatPopup.SetActive(true);
        Time.timeScale = 0f; // 시간 정지


        if (DefeatBGM != null)
        {
            SoundManager.Instance.ChangeBGM(DefeatBGM);
        }
    }

    // 승리 팝업 닫기 (게임 재개)
    public void CloseDefeat()
    {
        DefeatPopup.SetActive(false);
        Time.timeScale = 1f; // 시간 재개
    }

    // 재시작 버튼 (게임 재시작)
    public void RestartGame()
    {
        DefeatPopup.SetActive(false);
        Time.timeScale = 1f; // 시간 재개
        // 게임 재시작 로직 추가 (필요 시 장면 다시 로드 등)
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

using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupVictory : MonoBehaviour
{
    
    public GameObject VictoryPopup; // 승리 팝업

    // 승리 팝업 열기 (게임 일시정지)
    public void OpenVictory()
    {
        VictoryPopup.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
        
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
    }
    
}
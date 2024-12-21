using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatPopup : MonoBehaviour
{
    
    

    // 패배 팝업 열기 (게임 일시정지)
    public void OpenDefeat()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
    }

    // 패배 팝업 닫기 (게임 재개)
    public void CloseDefeat()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f; // 시간 재개
    }

    // 재시작 버튼 (게임 재시작)
    public void RestartGame()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f; // 시간 재개
        // 게임 재시작 로직 추가 (필요 시 장면 다시 로드 등)
        
    }
    
}

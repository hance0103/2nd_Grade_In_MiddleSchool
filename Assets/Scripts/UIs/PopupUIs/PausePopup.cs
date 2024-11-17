using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject PausePopup; // 일시정지 팝업

    void Start()
    {
        // 디폴트 값으로 팝업 비활성화
        PausePopup.SetActive(false);

        // 게임 시작 시 시간을 정상 속도로 설정
        Time.timeScale = 1f;
    }

    // 일시정지 팝업 열기 (게임 일시정지)
    public void OpenPause()
    {
        PausePopup.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
    }

    // 일시정지 팝업 닫기 (게임 재개)
    public void ClosePause()
    {
        PausePopup.SetActive(false);
        Time.timeScale = 1f; // 시간 재개
    }

    // 재시작 버튼 (게임 재시작)
    public void RestartGame()
    {
        PausePopup.SetActive(false);
        Time.timeScale = 1f; // 시간 재개
        // 게임 재시작 로직 추가 (필요 시 장면 다시 로드 등)
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
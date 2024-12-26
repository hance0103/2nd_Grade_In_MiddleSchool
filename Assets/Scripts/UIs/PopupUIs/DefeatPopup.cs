using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class DefeatPopup : MonoBehaviour
{
    [SerializeField]
    private string[] Defeattexts = {
        "네놈 때문에..오늘도 지각이구나..",
        "크윽...이 몸이 지다니..치욕스럽구나",
        "오늘의 패배는..죽을때까지 잊지 않겠다..",
        "믿을 수 없다..신호등 따위에 당하다니..",
        "방심했구나..다음 번엔 봐주지 않겠다 "
    };
    [SerializeField] private TMP_Text displayText;
    public void ShowRandomText()
    {
        // 배열 범위 내에서 무작위 인덱스 선택
        int randomIndex = Random.Range(0, Defeattexts.Length);

        // 선택된 텍스트를 UI에 표시
        if (displayText != null)
        {
            displayText.text = Defeattexts[randomIndex];
        }
    }

    // 패배 시간을 출력하기 위한 참조 변수들
    [SerializeField] private TMP_Text displayDefeatTime;
    [SerializeField] private float curTime;
    int minute;
    int second;
    // 패배 팝업 열기 (게임 일시정지)
    public void OpenDefeat()
    {
        ShowRandomText();
        gameObject.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
        Timer timer = FindObjectOfType<Timer>(); //패배한 시간을 저장하지 않으므로 참조만 하기
        curTime = timer.curTime;
        minute = (int)curTime / 60;
        second = (int)curTime % 60;
        displayDefeatTime.text = "분투한 시간 : " + minute.ToString("00") + ":" + second.ToString("00");
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    
}

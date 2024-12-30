using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class PopupVictory : MonoBehaviour
{
    
    
    private string[] Victorytexts1 = {
        "후훗, 시시하구나",
        "오늘도 이 몸의 승리군 !",
        "상대가 되지 않는구나",
        "이 몸은 무적이다 "
    };
    
    private string[] Victorytexts2 = {
        "역겨운 벌레 녀석",
        "승리다",
        "상대가 되지 않는군",
        "이 몸은 무적 "
    };
    
    private string[] Victorytexts3 = {
        "후훗, 시시하구나",
        "오늘도 이 몸의 승리군 !",
        "상대가 되지 않는구나",
        "이 몸은 무적이다 "
    };
    [SerializeField] private TMP_Text displayText;
    int Stage = 0;
    
    public void ShowClearTime()
    {
        Timer timer = FindObjectOfType<Timer>();
        displayClearTime.text = "클리어 타임 :" + timer.curTime;
    }

    //시간 출력을 위한 참조 변수들
    [SerializeField] private TMP_Text displayClearTime;
    int minute;
    int second;
    [SerializeField] private float time;
    [SerializeField] private float curTime;
    // 승리 팝업 열기 (게임 일시정지)
    public void OpenVictory1() //스테이지 분류로 시간 저장 영역 다르게
    {
        int randomIndex = Random.Range(0, Victorytexts1.Length); // 배열 범위 내에서 무작위 인덱스 선택
        displayText.text = Victorytexts1[randomIndex]; // 해당 무작위 인덱스 출력
        gameObject.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
        //manager.Action(scanObject);

        // PlayerPrefs의 시간 값 가져와서 클리어 타임 출력하기
        float savedTime = PlayerPrefs.GetFloat("FinalTime1", 0f);
        curTime = time;
        curTime = savedTime;
        Debug.Log(savedTime);
        minute = (int)curTime / 60;
        second = (int)curTime % 60;
        displayClearTime.text = "클리어 시간 : " + minute.ToString("00") + ":" + second.ToString("00");
    }
    public void OpenVictory2() //스테이지 2
    {
        int randomIndex = Random.Range(0, Victorytexts2.Length);
        displayText.text = Victorytexts2[randomIndex];
        gameObject.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
        //manager.Action(scanObject);

        // PlayerPrefs의 시간 값 가져와서 클리어 타임 출력하기
        float savedTime = PlayerPrefs.GetFloat("FinalTime2", 0f);
        curTime = time;
        curTime = savedTime;
        Debug.Log(savedTime);
        minute = (int)curTime / 60;
        second = (int)curTime % 60;
        displayClearTime.text = "클리어 시간 : " + minute.ToString("00") + ":" + second.ToString("00");
    }
    public void OpenVictory3() //스테이지 3
    {
        int randomIndex = Random.Range(0, Victorytexts3.Length);
        displayText.text = Victorytexts3[randomIndex];
        gameObject.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
        //manager.Action(scanObject);

        // PlayerPrefs의 시간 값 가져와서 클리어 타임 출력하기
        float savedTime = PlayerPrefs.GetFloat("FinalTime3", 0f);
        curTime = time;
        curTime = savedTime;
        Debug.Log(savedTime);
        minute = (int)curTime / 60;
        second = (int)curTime % 60;
        displayClearTime.text = "클리어 시간 : " + minute.ToString("00") + ":" + second.ToString("00");
    }
    // 승리 팝업 닫기 (게임 재개)
    public void CloseVictory()
    {
        gameObject.SetActive(false);
        
        Time.timeScale = 1f; // 시간 재개
    }

    // 재시작 버튼 (게임 재시작)
    public void RestartGame()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f; // 시간 재개
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
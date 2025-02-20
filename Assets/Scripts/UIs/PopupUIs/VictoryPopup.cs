using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class PopupVictory : MonoBehaviour
{
    
    
    private string[] Victorytexts1 = {
        "후훗, 시시하구나",
        "오늘도 이 몸의 승리군 !",
        "상대가 되지 않는구나",
        "잘가라 내가 없는 시대에 태어났을 뿐인 범부여",
        "이 몸이 하늘에 서겠다",
        "느리구나... 쓰러지는 것조차...",
        "꿇어라...이게 너와 나의 눈높이다",
        "나에게 등교는 살인이다",
        "시시해서 죽고싶어졌다"
    };
    
    private string[] Victorytexts2 = {
        "후훗, 시시하구나",
        "오늘도 이 몸의 승리군 !",
        "상대가 되지 않는구나",
        "잘가라 내가 없는 시대에 태어났을 뿐인 범부여",
        "이 몸이 하늘에 서겠다",
        "느리구나... 쓰러지는 것조차...",
        "꿇어라...이게 너와 나의 눈높이다",
        "나에게 등교는 살인이다",
        "시시해서 죽고싶어졌다"
    };
    
    private string[] Victorytexts3 = {
        "후훗, 시시하구나",
        "오늘도 이 몸의 승리군 !",
        "상대가 되지 않는구나",
        "잘가라 내가 없는 시대에 태어났을 뿐인 범부여",
        "이 몸이 하늘에 서겠다",
        "느리구나... 쓰러지는 것조차...",
        "꿇어라...이게 너와 나의 눈높이다",
        "나에게 등교는 살인이다",
        "시시해서 죽고싶어졌다"
    };
    
    
    public void ShowClearTime()
    {
        Timer timer = FindObjectOfType<Timer>();
        displayClearTime.text = "클리어 타임 :" + timer.curTime;
    }
    [Header("스테이지")]
    public int Stage;
    [Header("시간 출력을 위한 참조 요소들")]
    [SerializeField] private TMP_Text displayClearTime;
    [SerializeField] private TMP_Text displayText;
    [Header("엔딩 팝업")]
    [SerializeField] private GameObject EndingPopup;
    private int minute;
    private int second;
    private float time;
    private float curTime;

    void Start()
    {
        if (Stage == 1) { OpenVictory1(); }
        if (Stage == 2) { OpenVictory2(); }
        if (Stage == 3) { OpenVictory3(); }
    }
    // 승리 팝업 열기 (게임 일시정지)
    public void OpenVictory1() //스테이지 분류로 시간 저장 영역 다르게
    {
        GameManager.isFinishBossZoominAllowed = false;
        SoundManager.Instance.winBgmOn();
        int randomIndex = Random.Range(0, Victorytexts1.Length); // 배열 범위 내에서 무작위 인덱스 선택
        displayText.text = Victorytexts1[randomIndex]; // 해당 무작위 인덱스 출력
        gameObject.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
        //manager.Action(scanObject);

        // PlayerPrefs의 시간 값 가져와서 클리어 타임 출력하기

        float savedTime1 = PlayerPrefs.GetFloat("FinalTime1", 0f);
        curTime = savedTime1;
        Debug.Log(savedTime1);
        minute = (int)curTime / 60;
        second = (int)curTime % 60;
        displayClearTime.text = "클리어 시간 : " + minute.ToString("00") + ":" + second.ToString("00");
    }
    public void OpenVictory2() //스테이지 2
    {
        GameManager.isFinishBossZoominAllowed = false;
        SoundManager.Instance.winBgmOn();
        int randomIndex = Random.Range(0, Victorytexts2.Length);
        displayText.text = Victorytexts2[randomIndex];
        gameObject.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
        //manager.Action(scanObject);

        // PlayerPrefs의 시간 값 가져와서 클리어 타임 출력하기
        float savedTime2 = PlayerPrefs.GetFloat("FinalTime2", 0f);
        curTime = time;
        curTime = savedTime2;
        Debug.Log(savedTime2);
        minute = (int)curTime / 60;
        second = (int)curTime % 60;
        displayClearTime.text = "클리어 시간 : " + minute.ToString("00") + ":" + second.ToString("00");
    }
    public void OpenVictory3() //스테이지 3
    {
        GameManager.isFinishBossZoominAllowed = false;
        SoundManager.Instance.winBgmOn();
        int randomIndex = Random.Range(0, Victorytexts3.Length);
        displayText.text = Victorytexts3[randomIndex];
        gameObject.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
        //manager.Action(scanObject);

        // PlayerPrefs의 시간 값 가져와서 클리어 타임 출력하기
        float savedTime3 = PlayerPrefs.GetFloat("FinalTime3", 0f);
        curTime = time;
        curTime = savedTime3;
        Debug.Log(savedTime3);
        minute = (int)curTime / 60;
        second = (int)curTime % 60;
        displayClearTime.text = "클리어 시간 : " + minute.ToString("00") + ":" + second.ToString("00");
    }
    // 승리 팝업 닫기 (게임 재개)
    public void CloseVictory()
    {
        
        Time.timeScale = 1f; // 시간 재개
    }
    public void Ending()
    {
        EndingPopup.SetActive(true);
    }
    // 재시작 버튼 (게임 재시작)
    public void RestartGame()
    {
        if (Stage == 1) { SoundManager.Instance.Stage1BgmOn(); }
        if (Stage == 2) { SoundManager.Instance.Stage2BgmOn(); }
        if (Stage == 3) { SoundManager.Instance.Stage3BgmOn(); }

        gameObject.SetActive(false);
        Time.timeScale = 1f; // 시간 재개
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        
    }
}
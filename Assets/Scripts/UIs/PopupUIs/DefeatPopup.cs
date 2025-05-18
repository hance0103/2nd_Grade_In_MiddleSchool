using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class DefeatPopup : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log("dd");
    }
    [SerializeField]
    private string[] Defeattexts1 = {
        "네놈 때문에..오늘도 지각이구나..",
        "크윽...이 몸이 지다니..치욕스럽구나",
        "오늘의 패배는..죽을때까지 잊지 않겠다..",
        "믿을 수 없다..신호등 따위에 당하다니..",
        "방심했구나..다음 번엔 봐주지 않겠다 "
    };
    [SerializeField]
    private string[] Defeattexts2 = {
        "네놈 때문에...",
        "크윽...치욕스럽구나",
        "오늘의 패배는..잊지 않겠다..",
        "믿을 수 없다..당하다니..",
        "방심했구나.."
    };
    [SerializeField]
    private string[] Defeattexts3 = {
        "네놈 때문에..오늘도 지각이구나..",
        "크윽...이 몸이 지다니..치욕스럽구나",
        "오늘의 패배는..죽을때까지 잊지 않겠다..",
        "믿을 수 없다..신호등 따위에 당하다니..",
        "방심했구나..다음 번엔 봐주지 않겠다 "
    };
    [SerializeField] private TMP_Text displayText;
    [Header("스테이지")]
    [SerializeField] private int Stage;
    public void ShowRandomText1()
    {
        // 배열 범위 내에서 무작위 인덱스 선택
        int randomIndex = Random.Range(0, Defeattexts1.Length);

        // 선택된 텍스트를 UI에 표시
        if (displayText != null)
        {
            displayText.text = Defeattexts1[randomIndex];
        }
    }
    public void ShowRandomText2()
    {
        // 배열 범위 내에서 무작위 인덱스 선택
        int randomIndex = Random.Range(0, Defeattexts2.Length);

        // 선택된 텍스트를 UI에 표시
        if (displayText != null)
        {
            displayText.text = Defeattexts2[randomIndex];
        }
    }
    public void ShowRandomText3()
    {
        // 배열 범위 내에서 무작위 인덱스 선택
        int randomIndex = Random.Range(0, Defeattexts3.Length);

        // 선택된 텍스트를 UI에 표시
        if (displayText != null)
        {
            displayText.text = Defeattexts3[randomIndex];
        }
    }
    // 패배 시간을 출력하기 위한 텍스트 오브젝트들
    [SerializeField] private TMP_Text displayDefeatTime;
    [SerializeField] private float curTime;
    int minute;
    int second;
    // 패배 팝업 열기 (게임 일시정지)
    public void OpenDefeat1()
    {
        SoundManager.Instance.loseBgmOn();
        ShowRandomText1();
        gameObject.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
        Timer timer = FindObjectOfType<Timer>(); //패배한 시간을 저장하지 않으므로 참조만 하기
        curTime = timer.curTime;
        minute = (int)curTime / 60;
        second = (int)curTime % 60;
        displayDefeatTime.text = "분투한 시간 : " + minute.ToString("00") + ":" + second.ToString("00");
    }
    public void OpenDefeat2()
    {
        SoundManager.Instance.loseBgmOn();
        ShowRandomText2();
        gameObject.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
        Timer timer = FindObjectOfType<Timer>(); //패배한 시간을 저장하지 않으므로 참조만 하기
        curTime = timer.curTime;
        minute = (int)curTime / 60;
        second = (int)curTime % 60;
        displayDefeatTime.text = "분투한 시간 : " + minute.ToString("00") + ":" + second.ToString("00");
    }
    public void OpenDefeat3()
    {
        SoundManager.Instance.loseBgmOn();
        ShowRandomText3();
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
        SoundManager.Instance.EffectSoundOn("3");
        gameObject.SetActive(false);
        Time.timeScale = 1f; // 시간 재개
    }

    // 재시작 버튼 (게임 재시작)
    public void RestartGame()
    {
        if (Stage == 1)
        {
            SoundManager.Instance.Stage1BgmOn();
        }
        if (Stage == 2)
        {
            SoundManager.Instance.Stage2BgmOn();
        }
        if (Stage == 3)
        {
            SoundManager.Instance.Stage3BgmOn();
        }
        SoundManager.Instance.EffectSoundOn("3");
        gameObject.SetActive(false);
        Time.timeScale = 1f; // 시간 재개
        // 게임 재시작 로직 추가 (필요 시 장면 다시 로드 등)
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    
}

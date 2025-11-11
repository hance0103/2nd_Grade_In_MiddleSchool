using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
public class DefeatPopup : MonoBehaviour
{
    [SerializeField]
    private string[] Defeattexts1 = {
        "네놈 때문에..오늘도 지각이구나...",
        "으으... 오늘은 집에 갈래...",
        "크윽... 이런 추태를 보이다니...",
        "튜... 튜닝이 덜 된 것 뿐이야!",
        "방심했구나..다음 번엔 봐주지 않겠다 "
    };
    [SerializeField]
    private string[] Defeattexts2 = {
        "으으... 오늘은 집에 갈래...",
        "크윽... 이런 추태를 보이다니...",
        "튜... 튜닝이 덜 된 것 뿐이야!",
        "방심했구나..다음 번엔 봐주지 않겠다"
    };
    [SerializeField]
    private string[] Defeattexts3 = {
        "으으... 오늘은 집에 갈래...",
        "크윽... 이런 추태를 보이다니...",
        "튜... 튜닝이 덜 된 것 뿐이야!",
        "방심했구나..다음 번엔 봐주지 않겠다"
    };
    public TMP_Text ChatText;      // 패배 대사
    [Header("스테이지")]
    [SerializeField] private int Stage;

    //컨트롤러 끄기
    private GameObject Controller;
    private PlayerController pc;
    // 패배 시간을 출력하기 위한 텍스트 오브젝트들
    [SerializeField] private TMP_Text displayDefeatTime;
    [SerializeField] private float curTime;
    int minute;
    int second;
    int hour;
    // 패배 팝업 열기 (게임 일시정지)
    private void Awake()
    {
        if (pc == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player) pc = player.GetComponent<PlayerController>();
        }
        //Controller = GameObject.FindGameObjectWithTag("Controller");
        //if (Controller == null)
        //Debug.LogError("Controller 태그가 붙은 오브젝트를 찾을 수 없습니다!");
    }
    public void OpenDefeat1()
    {
        pc.ChangeState(new IdleState(pc));
        SoundManager.Instance.StopLoopEffect();
        //Controller.SetActive(false);
        ScreenGrayscale.SetGrayscale(false, 0.1f);
        SoundManager.Instance.loseBgmOn();
        int randomIndex = Random.Range(0, Defeattexts1.Length);
        StartCoroutine(NormalChat(Defeattexts1[randomIndex]));
        gameObject.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
        Timer timer = FindObjectOfType<Timer>(); //패배한 시간을 저장하지 않으므로 참조만 하기
        curTime = timer.curTime;
        minute = (int)curTime / 60;
        second = (int)curTime % 60;
        hour = (int)curTime / 3600;
        displayDefeatTime.text = hour.ToString("00") + " : " + minute.ToString("00") + " : " + second.ToString("00");
    }
    public void OpenDefeat2()
    {
        pc.ChangeState(new IdleState(pc));
        SoundManager.Instance.StopLoopEffect();
        //Controller.SetActive(false);
        ScreenGrayscale.SetGrayscale(false, 0.1f);
        SoundManager.Instance.loseBgmOn();
        int randomIndex = Random.Range(0, Defeattexts2.Length);
        StartCoroutine(NormalChat(Defeattexts2[randomIndex]));
        gameObject.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
        Timer timer = FindObjectOfType<Timer>(); //패배한 시간을 저장하지 않으므로 참조만 하기
        curTime = timer.curTime;
        minute = (int)curTime / 60;
        second = (int)curTime % 60;
        hour = (int)curTime / 3600;
        displayDefeatTime.text = hour.ToString("00") + " : " + minute.ToString("00") + " : " + second.ToString("00");
    }
    public void OpenDefeat3()
    {
        pc.ChangeState(new IdleState(pc));
        SoundManager.Instance.StopLoopEffect();
        ScreenGrayscale.SetGrayscale(false, 0.1f);
        SoundManager.Instance.loseBgmOn();
        int randomIndex = Random.Range(0, Defeattexts3.Length);
        StartCoroutine(NormalChat(Defeattexts3[randomIndex]));
        gameObject.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
        Timer timer = FindObjectOfType<Timer>(); //패배한 시간을 저장하지 않으므로 참조만 하기
        curTime = timer.curTime;
        minute = (int)curTime / 60;
        second = (int)curTime % 60;
        hour = (int)curTime / 3600;
        displayDefeatTime.text = hour.ToString("00") + " : " + minute.ToString("00") + " : " + second.ToString("00");
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
            SoundManager.Instance.StopLoopEffect();
            SoundManager.Instance.Stage1BgmOn();
        }
        if (Stage == 2)
        {
            SoundManager.Instance.StopLoopEffect();
            SoundManager.Instance.Stage2BgmOn();
        }
        if (Stage == 3)
        {
            SoundManager.Instance.StopLoopEffect();
            SoundManager.Instance.Stage3BgmOn();
        }
        SoundManager.Instance.EffectSoundOn("3");
        gameObject.SetActive(false);
        Time.timeScale = 1f; // 시간 재개
        // 게임 재시작 로직 추가 (필요 시 장면 다시 로드 등)
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    public float typingSpeed = 0.1f;
    public bool isSkipping = false;
    private bool isFullTextDisplayed = false;
    public string writerText = "";
    IEnumerator NormalChat(string narration)
    {
        isFullTextDisplayed = false;

        int a = 0;
        ChatText.text = "";
        writerText = "";

        // 텍스트 타이핑 효과
        for (a = 0; a < narration.Length; a++)
        {
            if (isSkipping)
            {
                writerText = narration;
                ChatText.text = writerText;
                break;
            }
            writerText += narration[a];
            ChatText.text = writerText;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }
}

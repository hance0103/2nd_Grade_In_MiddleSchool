using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class FinishingPopup : MonoBehaviour
{
    [Header("참조 요소들")]
    public TMP_Text ChatText;         // 저장된 채팅이 나오는 텍스트
    public GameObject TextBox;
    public GameObject ClosingTextPanel;
    public GameObject Timer;

    [Header("플레이어/보스 오브젝트")]
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Boss;

    [Header("기존 보스 오브젝트")]
    [SerializeField] private GameObject oldBoss; // 기존 보스 지우기용

    [Header("캐릭터 스프라이트")]
    public GameObject CharacterPose;

    
    [Header("폭발 오브젝트들 (스프라이트 애니메이션 포함)")]
    public GameObject explosionObject1;
    public GameObject explosionObject2;
    public GameObject explosionObject3;
    public GameObject BossPose1;

    [Header("텍스트 연출")]
    public Vector3 startPosition = new Vector3(-4.0f, -2.5f, 0f);  // 출발 지점
    public Vector3 endPosition = new Vector3(6.5f, -1.5f, 0f);   // 도착 지점
    public GameObject letterPrefab;       // 텍스트 생성 인스턴스 프리팹
    public Canvas UI;                     // 텍스트 띄울 캔버스

    [Header("피니시 연출 시간")]
    public float FinishTime = 10f;    // 전체 연출 시간(예상 10초)

    [Header("스테이지")]
    public int Stage;

    private Animator Playeranimator;
    private Animator Bossanimator;

    void Start()
    {
        Open();
    }

    void Update()
    {

    }

    void Open()
    {
        // 대화창 활성화
        ChatText.gameObject.SetActive(true);
        Playeranimator = Player.GetComponent<Animator>();
        Bossanimator = Boss.GetComponent<Animator>();
        // 스테이지별 대사 불러오기(PlayerPrefs에서)
        string finalChat = "";
        if (Stage == 1)
        {
            finalChat = PlayerPrefs.GetString("FinalText1", "죽어라!");
        }
        else if (Stage == 2)
        {
            finalChat = PlayerPrefs.GetString("FinalText2", "죽어라!");
        }
        else if (Stage == 3)
        {
            finalChat = PlayerPrefs.GetString("FinalText3", "죽어라!");
        }

        // 대사를 차례로 출력하는 코루틴 시작
        StartCoroutine(OpenText(finalChat));
    }

    
    private float totalTypingDuration = 5f; // 대사가 완전히 출력되는 데 걸리는 시간 (5초)
    IEnumerator OpenText(string narration)
    {
        Timer.SetActive(false);
        oldBoss.SetActive(false);
        Boss.SetActive(true);
        if (Boss != null)
        {
            Boss.transform.position = new Vector3(6.5f, -1.5f, 0f);
            Boss.SetActive(true);
        }
        // (필요하다면) 대사가 시작될 때 사운드 이펙트
        // SoundManager.Instance.EffectSoundOn("18"); // 예시

        // 대사를 3초에 걸쳐 천천히 타이핑
        //yield return StartCoroutine(TypingText(narration, totalTypingDuration));
        //yield return StartCoroutine(SendFlyingText(narration, totalTypingDuration));

        // 대화창 비활성화
        ChatText.gameObject.SetActive(false);
        TextBox.gameObject.SetActive(false);
        CharacterPose.gameObject.SetActive(false);
       
        bool isAtk = true;
        Playeranimator.SetBool("IsNormalAttack", isAtk);
        //StartCoroutine(ExplosionRoutine());
        StartCoroutine(SendFlyingText(narration, totalTypingDuration));
        yield return new WaitForSeconds(5.2f);
        isAtk = false;
        Playeranimator.SetBool("IsNormalAttack", isAtk);


        // 특수 이펙트(이펙트 오브젝트 활성화, 사운드 재생 등)
        // Effect.SetActive(true);
        // SoundManager.Instance.EffectSoundOn(""); // 마무리타 때리는 느낌의 사운드 등

        // 보스 쓰러지는 애니메이터 추가 예정
        // 예: bossAnimator.SetTrigger("Death");
        GameManager.isFinishBossZoominAllowed = true;
        yield return new WaitForSeconds(FinishTime - 6f);


        // 마지막으로 연출이 끝났을 때 화면 전환 또는 오브젝트 비활성화
        CloseFinishing();
    }
    IEnumerator SendFlyingText(string narration, float duration)
    {
        StartCoroutine(ExplosionRoutine());
        // 글자 총 길이에 따라 각 글자 생성 간격
        float timePerChar = duration / narration.Length;

        for (int i = 0; i < narration.Length; i++)
        {
            // 글자를 하나 생성해서 날리는 코루틴
            StartCoroutine(SpawnAndMoveLetter(narration[i].ToString()));

            // 다음 글자까지 대기 (타이핑 효과)
            yield return new WaitForSeconds(timePerChar);
        }
    }
    IEnumerator SpawnAndMoveLetter(string letter)
    {
        // 글자 프리팹 생성 (플레이어 위치에서)
        GameObject letterObj = Instantiate(letterPrefab, UI.transform);
        letterObj.transform.SetParent(UI.transform, false);
        RectTransform letterRect = letterObj.GetComponent<RectTransform>();
        Vector3 startScreenPos = Camera.main.WorldToScreenPoint(startPosition);
        Vector3 endScreenPos = Camera.main.WorldToScreenPoint(endPosition);

        // 초기 위치 지정
        letterRect.anchoredPosition = startScreenPos;

        // 텍스트 설정
        TMP_Text letterTMP = letterObj.GetComponentInChildren<TMP_Text>();
        if (letterTMP != null)
        {
            letterTMP.text = letter;
        }

        float moveTime = 0.8f;
        float elapsed = 0f;
        while (elapsed < moveTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveTime;
            // 시작~끝 스크린좌표를 Lerp, 그리고 RectTransform의 anchoredPosition에 대입
            Vector3 currentPos = Vector3.Lerp(startScreenPos, endScreenPos, t);
            letterRect.anchoredPosition = currentPos;
            yield return null;
        }
        letterObj.SetActive(false);
    }

    IEnumerator ExplosionRoutine()
    {
        for (int i = 0; i < 7; i++)
        {
            // 폭발 오브젝트들을 각각 랜덤 위치에 배치
            explosionObject1.transform.position = GetRandomPosition();
            explosionObject2.transform.position = GetRandomPosition();
            explosionObject3.transform.position = GetRandomPosition();

            // 활성화(애니메이션 시작)
            StartCoroutine(Explosion1());
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(Explosion2());
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(Explosion3());
            yield return new WaitForSeconds(0.2f);
        }
    }
    IEnumerator Explosion1()
    {
        explosionObject1.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        explosionObject1.SetActive(false);
    }
    IEnumerator Explosion2()
    {
        explosionObject2.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        explosionObject2.SetActive(false);
    }
    IEnumerator Explosion3()
    {
        explosionObject3.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        explosionObject3.SetActive(false);
    }

    private Vector3 GetRandomPosition()
    {
        float randX = Random.Range(1300f, 1100f);
        float randY = Random.Range(300f, 100f);
        return new Vector3(randX, randY, 0f);
    }
    IEnumerator TypingText(string narration, float duration)
    {
        ChatText.text = "";
        float timePerChar = duration / narration.Length;
        string writerText = "";

        for (int i = 0; i < narration.Length; i++)
        {
            writerText += narration[i];
            ChatText.text = writerText;
            yield return new WaitForSeconds(timePerChar);
        }
    }

    void CloseFinishing()
    {
        // 패널 비활성화
        gameObject.SetActive(false);
        ClosingTextPanel.SetActive(true);

        // 플레이어, 보스 오브젝트 비활성화
        Player.SetActive(false);
        Boss.SetActive(false);
    }
}

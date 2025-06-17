using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FinishingPopup : MonoBehaviour
{
    [Header("참조 요소들")]
    public TMP_Text ChatText;         // 저장된 채팅이 나오는 텍스트
    public GameObject TextBox;
    public GameObject ClosingTextPanel;
    public GameObject Timer;

    [Header("플레이어/보스 오브젝트")]
    private GameObject player;
    [SerializeField] private GameObject Boss;

    [Header("기존 보스 오브젝트")]
    [SerializeField] private GameObject oldBoss; // 기존 보스 지우기용

    [Header("글자 대기 범위(말풍선 주변)")]
    private float waitRangeX = 500f;   // 가로 ±500 px
    private float waitRangeY = 200f;   // 세로 ±200 px
    public float waitHoldTime = 0.15f; // 대기 시간(원하면 0으로)

    // 모든 글자를 모아 두었다가 한꺼번에 날리기 위해
    private readonly List<RectTransform> readyRects = new();

    [Header("폭발 오브젝트들 (스프라이트 애니메이션 포함)")]
    public GameObject explosionObject1;
    public GameObject explosionObject2;
    public GameObject explosionObject3;
    public GameObject BossPose1;

    [Header("텍스트 연출")]
    public Vector3 startPosition = new Vector3(-4.0f, -2.5f, 0f);  // 출발 지점
    public Vector3 endPosition = new Vector3(6.5f, -2.5f, 0f);   // 도착 지점
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

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            Debug.LogError("Player 태그가 붙은 오브젝트를 찾을 수 없습니다!");

    }
    void Open()
    {
        // 대화창 활성화
        ChatText.gameObject.SetActive(true);
        Playeranimator = player.GetComponent<Animator>();
        Bossanimator = Boss.GetComponent<Animator>();
        
        // 스테이지별 대사 불러오기(PlayerPrefs에서)
        string finalChat = "";
        if (Stage == 1)
        {
            finalChat = PlayerPrefs.GetString("FinalText1", "죽어라!");
            Bossanimator.SetBool("isEnraged", true);
        }
        else if (Stage == 2)
        {
            finalChat = PlayerPrefs.GetString("FinalText2", "죽어라!");
            Bossanimator.SetBool("isEnraged", true);
        }
        else if (Stage == 3)
        {
            finalChat = PlayerPrefs.GetString("FinalText3", "죽어라!");
        }

        // 대사를 차례로 출력하는 코루틴 시작
        StartCoroutine(OpenText(finalChat));
    }
    
    private float totalTypingDuration = 0.5f; // 대사가 완전히 출력되는 데 걸리는 시간 (5초)
    IEnumerator OpenText(string narration)
    {
        Timer.SetActive(false);
        oldBoss.SetActive(false);
        Boss.SetActive(true);
        if (Boss != null)
        {
            if (Stage == 2)
            {
                Boss.transform.position = new Vector3(6.5f, 1.67f, 0f);
            }
            else
            {
                Boss.transform.position = new Vector3(6.5f, -2.7f, 0f);
            }
            Boss.SetActive(true);
        }
        // (필요하다면) 대사가 시작될 때 사운드 이펙트
        // SoundManager.Instance.EffectSoundOn("18"); // 예시

        // 대사를 3초에 걸쳐 천천히 타이핑;


        TextBox.SetActive(true);          // 말풍선 박스
        ChatText.gameObject.SetActive(true);

        /* -- 원하는 ‘빠르기’로 조절 --
           (글자 수 × 0.03초 ≈ 30 ms/글자, 최소 0.5 초 보장)                  */
        float typingDuration = Mathf.Max(0.5f, narration.Length * 0.1f);
        yield return StartCoroutine(TypingText(narration, typingDuration));
        // 잠깐 여운
        yield return new WaitForSeconds(1f);

       
        Playeranimator.Play("NormalAttack");
        //StartCoroutine(ExplosionRoutine());
        StartCoroutine(SendFlyingText(narration, totalTypingDuration));
        yield return new WaitForSeconds(4.5f);
        Playeranimator.Play("Idle");


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
    private int lettersWaiting;
    IEnumerator SendFlyingText(string narration, float totalTypingDuration)
    {
        float dt = totalTypingDuration / narration.Length;
        ChatText.text = narration;
        lettersWaiting = narration.Length;

        foreach (char c in narration)
        {
            StartCoroutine(SpawnAndWaitLetter(c.ToString()));
            if (ChatText.text.Length > 0)
                ChatText.text = ChatText.text.Substring(1);
            yield return new WaitForSeconds(dt);
        }

        // 모든 글자가 대기 위치에 도착할 때까지 대기
        yield return new WaitUntil(() => lettersWaiting == 0);

        // 대기 풍선 숨김
        ChatText.gameObject.SetActive(false);
        TextBox.SetActive(false);

        // 집단 돌진 & 폭발
        yield return StartCoroutine(LaunchLettersToBoss());
    }
    private float launchInterval = 0.1f;
    IEnumerator LaunchLettersToBoss()
    {
        StartCoroutine(ExplosionRoutine());      // 폭발 이펙트 병행 실행

        Vector2 bossPos = Camera.main.WorldToScreenPoint(endPosition);
        float moveTime = 0.25f;                // 한 글자 이동 시간

        foreach (RectTransform rect in readyRects)
        {
            // 글자 하나 이동 시작
            StartCoroutine(MoveUI(rect, rect.anchoredPosition, bossPos, moveTime));

            // 다음 글자까지 대기 → 0.1초 간격 “다다다닥”
            yield return new WaitForSeconds(launchInterval);
        }

        // 마지막 글자가 도착할 때까지 안전 대기
        yield return new WaitForSeconds(moveTime);

        // 정리
        foreach (RectTransform rect in readyRects)
            rect.gameObject.SetActive(false);
        readyRects.Clear();
    }
    IEnumerator SpawnAndWaitLetter(string letter)
    {
        GameObject obj = Instantiate(letterPrefab, UI.transform, false);
        RectTransform rect = obj.GetComponent<RectTransform>();

        Vector2 startPos = Camera.main.WorldToScreenPoint(startPosition);
        Vector2 endPosBubble = TextBox.GetComponent<RectTransform>().anchoredPosition +
                               new Vector2(Random.Range(-waitRangeX, waitRangeX),
                                           Random.Range(-waitRangeY, waitRangeY));

        rect.anchoredPosition = startPos;

        TMP_Text tmp = obj.GetComponentInChildren<TMP_Text>();
        if (tmp) tmp.text = letter;

        // 시작 → 랜덤 대기
        yield return MoveUI(rect, startPos, endPosBubble, 0.15f);
        yield return new WaitForSeconds(waitHoldTime);

        // 대기 완료 등록
        readyRects.Add(rect);
        lettersWaiting--;
    }
 

    // 공통 보간 함수
    IEnumerator MoveUI(RectTransform rect, Vector2 from, Vector2 to, float time)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / time;
            rect.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }
    }

    IEnumerator ExplosionRoutine()
    {
        for (int i = 0; i < 5; i++)
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
        float randX = Random.Range(2000f, 1800f);
        float randY = Random.Range(400f, 200f);
        if (Stage == 2)
        {
            randY = Random.Range(800f, 600f);
        }
        
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
        player.SetActive(false);
        Boss.SetActive(false);
    }
}

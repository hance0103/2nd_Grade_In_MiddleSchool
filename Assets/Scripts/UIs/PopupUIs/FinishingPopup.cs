using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinishingPopup : MonoBehaviour
{
    [Header("참조 요소들")]
    public TMP_Text ChatText;         // 저장된 채팅이 나오는 텍스트
    public GameObject TextBox;
    public GameObject ClosingTextPanel;

    [Header("플레이어/보스 오브젝트")]
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Boss;

    [Header("캐릭터 스프라이트")]
    public GameObject CharacterPose;

    
    [Header("폭발 오브젝트들 (스프라이트 애니메이션 포함)")]
    public GameObject explosionObject1;
    public GameObject explosionObject2;
    public GameObject explosionObject3;
    public GameObject BossPose1;

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

    
    private float totalTypingDuration = 3f; // 대사가 완전히 출력되는 데 걸리는 시간 (3초)
    IEnumerator OpenText(string narration)
    {
        Boss.SetActive(true);
        if (Boss != null)
        {
            Boss.transform.position = new Vector3(6.5f, -1.5f, 0f);
            Boss.SetActive(true);
        }
        // (필요하다면) 대사가 시작될 때 사운드 이펙트
        // SoundManager.Instance.EffectSoundOn("18"); // 예시

        // 대사를 3초에 걸쳐 천천히 타이핑
        yield return StartCoroutine(TypingText(narration, totalTypingDuration));
        // 대화창 비활성화
        ChatText.gameObject.SetActive(false);
        TextBox.gameObject.SetActive(false);
        CharacterPose.gameObject.SetActive(false);
        // 대사 출력이 모두 끝나고 약간의 텀(연출용)
        StartCoroutine(ExplosionRoutine());
        yield return new WaitForSeconds(3.6f);
        // 여기서 Player나 Boss의 애니메이터를 건드려서 마무리 연출 시작
        // 예: playerAnimator.SetTrigger("normalAttack");


        // 특수 이펙트(이펙트 오브젝트 활성화, 사운드 재생 등)
        // Effect.SetActive(true);
        // SoundManager.Instance.EffectSoundOn("19"); // 마무리타 때리는 느낌의 사운드 등

        // 보스 스프라이트에게 카메라 줌 인 연출
        // 보스 쓰러지는 애니메이터 추가 예정
        // 예: bossAnimator.SetTrigger("Death");
        GameManager.isFinishBossZoominAllowed = true;
        yield return new WaitForSeconds(FinishTime - 6f);
        // (위에서 대사 3초 + 대사 끝 후 3초 + 카메라 보스에게 줌 인 연출 = 총합 10초)

        // 마지막으로 연출이 끝났을 때 화면 전환 또는 오브젝트 비활성화
        CloseFinishing();
    }

    
    IEnumerator ExplosionRoutine()
    {
        for (int i = 0; i < 6; i++)
        {
            // 1) 폭발 오브젝트들을 각각 랜덤 위치에 배치
            explosionObject1.transform.position = GetRandomPosition();
            explosionObject2.transform.position = GetRandomPosition();
            explosionObject3.transform.position = GetRandomPosition();
            
            // 2) 활성화(애니메이션 시작)
            explosionObject1.SetActive(true);
            explosionObject2.SetActive(true);
            explosionObject3.SetActive(true);

            // 폭발 애니메이션이 재생되는 동안 기다림
            // 예시: intervalTime 만큼
            yield return new WaitForSeconds(0.6f);

            // 3) 폭발 오브젝트 비활성화
            explosionObject1.SetActive(false);
            explosionObject2.SetActive(false);
            explosionObject3.SetActive(false);

            // 만약 한 번 터지고 사라진 뒤 잠시 텀을 두고 싶다면
            // 추가로 시간 지연을 둘 수도 있음
            // yield return new WaitForSeconds(0.2f); // 예시
        }
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

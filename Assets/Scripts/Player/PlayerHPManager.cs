using System.Collections;
using UnityEngine;


public class PlayerHPManager : MonoBehaviour
{
    public static PlayerHPManager Instance { get; private set; }
    [SerializeField]
    private GameObject playerController;
    [Header("�÷��̾� HP ����")]
    [SerializeField] private float maxHP = 100f;
    [Header("�÷��̾� ������Ʈ")]
    [SerializeField] private GameObject playerObject;
    private PlayerController player;
    [Header("�й� �˾�")]
    [SerializeField] private GameObject DefeatPopup;
    [SerializeField] private DefeatPopup DefeatPopupScript;
    [Header("��������")]
    [SerializeField] private int Stage;
    private float currentHP;
    [SerializeField]
    public float _InvincibleTime = 1f;

    private Coroutine _blinkCoroutine;
    private bool _gameOver = false;
    public float GetttingCurrentHP() => currentHP;
    public float GettingMaxHP() => maxHP;

    private bool isBinded = false;
    private bool duringBindContactDamaged = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        currentHP = maxHP;
    }
    private void Update()
    {

    }
    void Start()
    {
        playerObject = this.gameObject;
        player = GetComponent<PlayerController>();
    }

    public void RestartHP()
    {
        currentHP = maxHP;
    }
    private IEnumerator InvincibleCounter()
    {
        float counter = 0;
        
        while (counter < _InvincibleTime)
        {

            counter += Time.deltaTime;
            yield return null;
        }
        //Debug.Log("무적 끝");
        //StopCoroutine(_blinkCoroutine);

        player.DeactivateInvincible();
    }


    public void TakeDamage(float damage, PlayerDamagedType type = PlayerDamagedType.NormalDamage)
    {
        if (player.IsInvincible() || _gameOver || duringBindContactDamaged)
        {
            return;
        }
        // 몸박뎀을 받았는데
        if (type == PlayerDamagedType.ContactDamage)
        {
            // 스테이지1 바인드 걸려 있는 상태라면
            if (isBinded)
            {
                // 바인드동안 몸박뎀 받았다고 표시
                duringBindContactDamaged = true;
            }
        }
        currentHP -= damage;
        Debug.Log($"플레이어 {damage}데미지 히트. 남은 HP: {currentHP}");
        player.ActivateInvincible();
        StartCoroutine(InvincibleCounter());

        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }

        _blinkCoroutine = StartCoroutine(player.InvincibleBlink());
        player.CamShake(player.playerHitShakeMagnitude, player.playerHitShakeDuration);


        if (currentHP <= 0 )
        {
            _gameOver = true;
            currentHP = 0;
            Debug.Log("플레이어 사망");
            DefeatPopup.SetActive(true);
            player.PlayerDefeat();
            player.ActivateInvincible();
            playerController.SetActive(false);
            switch (Stage)
            {
                case 1:
                    DefeatPopupScript.OpenDefeat1();
                    break;
                case 2:
                    DefeatPopupScript.OpenDefeat2();
                    break;
                case 3:
                    DefeatPopupScript.OpenDefeat3();
                    break;
                default:
                    Debug.LogError("할당되지 않은 스테이지입니다.");
                    break;
            }
            Time.timeScale = 0f;
        }
    }
    public float GetCurrentHP()
    {
        return currentHP;
    }

    public float GetMaxHP()
    {
        return maxHP;
    }
    public void ResetBindContactDamageFlag()
    {
        isBinded = false;
        duringBindContactDamaged = false;
    }
    public void Stage1_SP2_Bind()
    {
        isBinded = true;
    }
}

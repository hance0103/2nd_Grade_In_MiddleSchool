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
    public float GetttingCurrentHP() => currentHP;
    public float GettingMaxHP() => maxHP;
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
    void Start()
    {
        playerObject = this.gameObject;
        player = GetComponent<PlayerController>();
    }

    public void RestartHP()
    {
        currentHP = maxHP;
    }


    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"{damage}데미지 히트. 남은 HP: {currentHP}");
        if (currentHP <= 0 )
        {
            currentHP = 0;
            Debug.Log("플레이어 사망");
            DefeatPopup.SetActive(true);
            player.PlayerDefeat();
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
                    UnityEngine.Debug.LogError("할당되지 않은 스테이지입니다.");
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
    

}

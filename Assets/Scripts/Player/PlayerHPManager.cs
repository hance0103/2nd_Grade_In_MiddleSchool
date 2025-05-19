using System.Collections;
using UnityEngine;
using UnityEngine.U2D;


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

    private bool _gameOver = false;
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
        player.DeactivateInvincible();
    }


    public void TakeDamage(float damage)
    {
        if (player.IsInvincible() || _gameOver)
        {
            return;
        }
        currentHP -= damage;
        Debug.Log($"플레이어 {damage}데미지 히트. 남은 HP: {currentHP}");
        player.ActivateInvincible();
        StartCoroutine(InvincibleCounter());
        StartCoroutine(player.InvincibleBlink());
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

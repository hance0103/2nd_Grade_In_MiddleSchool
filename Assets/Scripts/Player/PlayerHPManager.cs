using System;
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
    [Header("스테이지")]
    [SerializeField] public int Stage;
    private float currentHP;
    [SerializeField]
    public float _InvincibleTime = 1f;
    [Header("컬러 카메라")]
    [SerializeField] private Camera Colorcamera;
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

        //StopCoroutine(_blinkCoroutine);
        //Debug.Log("무적 끝");
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

        if (currentHP <= 0)
        {
            if (Stage == 1) { Colorcamera.enabled = false; }
            _gameOver = true;
            currentHP = 0;
            StartCoroutine(PlayerDying());
            Debug.Log($"[DeathCheck] Stage = {Stage} (직전 값)");
            player.PlayerDefeat();
            player.ActivateInvincible();
            playerController.SetActive(false);
            Time.timeScale = 0f;
            return;
        }

        player.ActivateInvincible();
        StartCoroutine(InvincibleCounter());

        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }

        _blinkCoroutine = StartCoroutine(player.InvincibleBlink());
        player.CamShake(player.playerHitShakeMagnitude, player.playerHitShakeDuration);


       
    }
    private IEnumerator PlayerDying()
    {
        CameraMove.Instance.PlayerDie();
        yield return new WaitForSecondsRealtime(2f);
        DefeatPopup.SetActive(true);
        switch (Stage)
        {
            case 1:
                DefeatPopupScript.OpenDefeat1();
                Debug.Log("플레이어 사망1");
                break;
            case 2:
                DefeatPopupScript.OpenDefeat2();
                Debug.Log("플레이어 사망2");
                break;
            case 3:
                DefeatPopupScript.OpenDefeat3();
                Debug.Log("플레이어 사망3");
                break;
            default:
                Debug.LogError("할당되지 않은 스테이지입니다.");
                break;
        }
        player.ResumeAfterDefeat();
        yield return null;
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

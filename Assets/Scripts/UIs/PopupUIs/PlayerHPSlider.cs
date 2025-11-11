using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHPSlider : MonoBehaviour
{
    [Header("플레이어 HP 슬라이더")]
    [SerializeField] private TMP_Text HPText;
    [SerializeField] private Image hpBarImage;

    private GameObject Player;
    private Material _instancedMat;

    [Range(0f, 1f)]
    public float fillAmount = 1f;

    private void Awake()
    {
        Player = GameObject.Find("Player");

        // 원본 머티리얼을 복제해서 개별 인스턴스화
        if (hpBarImage != null && hpBarImage.material != null)
        {
            _instancedMat = Instantiate(hpBarImage.material);
            hpBarImage.material = _instancedMat;
        }
    }

    private void Update()
    {
        if (Player == null)
        {
            Player = GameObject.Find("Player");
            if (Player == null) return;
        }

        if (PlayerHPManager.Instance == null) return;

        float currentHP = PlayerHPManager.Instance.GetCurrentHP();
        float maxHP = PlayerHPManager.Instance.GetMaxHP();
        if (maxHP <= 0f) return;

        float hpRatio = currentHP / maxHP;

        SetFillAmount(hpRatio);
        HPText.text = $"{(int)currentHP}/{(int)maxHP}";
    }

    public void SetFillAmount(float targetPercent)
    {
        if (_instancedMat == null) return;

        float clamped = Mathf.Clamp01(targetPercent);
        fillAmount = clamped;

        // 쉐이더의 _Fill 값에 비율 전달
        _instancedMat.SetFloat("_Fill", clamped);
    }
}

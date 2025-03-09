using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHPSlider : MonoBehaviour
{
    [Header("플레이어 HP 슬라이더")]
    [SerializeField] private Slider PlayerHpSlider;
    [SerializeField] private TMP_Text HPText;
    GameObject Player;
    private void Start()
    {
        // 슬라이더의 MinValue, MaxValue를 0~1로 맞추는 경우
        PlayerHpSlider.minValue = 0f;
        PlayerHpSlider.maxValue = 1f;

        // 시작 시 풀피 상태
        PlayerHpSlider.value = 1f;
    }
    private void Awake()
    {
        Player = GameObject.Find("Player");
    }
    private void Update()
    {
        if (Player == null)
        {
            Player = GameObject.Find("Player");
            // 찾지 못했다면 더 이상 진행할 수 없으니 return
            if (Player == null) return;
        }
        // BossHPManager 싱글톤에서 HP 정보 가져오기
        float currentHP = PlayerHPManager.Instance.GetCurrentHP();
        float maxHP = PlayerHPManager.Instance.GetMaxHP();

        // 현재 HP 비율을 0~1로 환산
        float hpRatio = currentHP / maxHP;

        // 슬라이더의 값 갱신
        PlayerHpSlider.value = hpRatio;
        HPText.text = $"{(int)maxHP}/{(int)currentHP}";
    }
}

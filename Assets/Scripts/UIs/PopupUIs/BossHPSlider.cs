using UnityEngine;
using UnityEngine.UI;

public class BossHPSlider : MonoBehaviour
{
    [Header("보스 HP 슬라이더")]
    [SerializeField] private Slider bossSlider;

    [SerializeField] GameObject Boss;
    private void Start()
    {
        // 슬라이더의 MinValue, MaxValue를 0~1로 맞추는 경우
        bossSlider.minValue = 0f;
        bossSlider.maxValue = 1f;

        // 시작 시 풀피 상태
        bossSlider.value = 1f;
    }

    private void Update()
    {
        // BossHPManager 싱글톤에서 HP 정보 가져오기
        float currentHP = BossHPManager.Instance.GetCurrentHP();
        float maxHP = BossHPManager.Instance.GetMaxHP();

        // 현재 HP 비율을 0~1로 환산
        float hpRatio = currentHP / maxHP;

        // 슬라이더의 값 갱신
        bossSlider.value = hpRatio;
    }
}
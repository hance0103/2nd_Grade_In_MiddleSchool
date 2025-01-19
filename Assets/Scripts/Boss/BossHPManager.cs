using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHPManager : MonoBehaviour
{
    //보스 hp를 참조하는 곳이 많으므로 접근하기 쉽게 싱글톤으로 구현, 씬 이동시 초기화
    public static BossHPManager Instance { get; private set; }

    [Header("보스 HP 설정")]
    [SerializeField] private float maxHP = 100f;
    [Header("승리 텍스트 입력 팝업")]
    [SerializeField] private GameObject VictoryInputPopup;
    [SerializeField] private VictoryTextInputPopup victoryTextPopupScript;
    [Header("광폭화 팝업")]
    [SerializeField] private GameObject BossEnragePopup;
    [SerializeField] private BossEnragePopup BossEnragePopupScript;
    private float currentHP;
    public float GetttingCurrentHP() => currentHP;
    public float GettingMaxHP() => maxHP;
    private void Awake()
    {
        // 싱글톤 기본 구현
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 씬 전환 시 파괴되도록 설정 
        // 시작 시 HP 초기화
        currentHP = maxHP;
    }

    
    private bool Enrageactive = true;
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"보스가 {damage} 데미지를 받음. 남은 HP: {currentHP}");

        if (Enrageactive && currentHP <= maxHP * 0.5f)
        {
            Enrageactive = false;
            BossEnrage();
        }
        if (currentHP <= 0)
        {
            currentHP = 0;
            BossDie();
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
    private void BossEnrage()
    {
        
        BossEnragePopup.SetActive(true);
        BossEnragePopupScript.OnEnrage();
    }
    private void BossDie()
    {
        Debug.Log("보스가 사망했습니다.");
        VictoryInputPopup.SetActive(true);
        victoryTextPopupScript.Stage1OpenInputPanel();
        Time.timeScale = 0f;
        // 보스 사망 처리 로직 (애니메이션, 드롭 아이템 등)
        // 예) 게임 오브젝트 비활성화, 패턴 루틴 종료 등
    }
}
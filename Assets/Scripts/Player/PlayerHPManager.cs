using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHPManager : MonoBehaviour
{
    //플레이어 hp를 싱글톤으로 구현, 씬 이동시 초기화
    public static PlayerHPManager Instance { get; private set; }

    [Header("플레이어 HP 설정")]
    [SerializeField] private float maxHP = 100f;
    [Header("플레이어 오브젝트")]
    [SerializeField] private GameObject Player;
    [Header("패배 팝업")]
    [SerializeField] private GameObject DefeatPopup;
    [SerializeField] private DefeatPopup DefeatPopupScript;
    [Header("스테이지")]
    [SerializeField] private int Stage;
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

    public void RestartHP()
    {
        currentHP = maxHP;
    }


    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"플레이어가 {damage} 데미지를 받음. 남은 HP: {currentHP}");

        
        if (currentHP <= 0 && Stage == 1)
        {
            currentHP = 0;
            PlayerDie1();
        }
        if (currentHP <= 0 && Stage == 2)
        {
            currentHP = 0;
            PlayerDie2();
        }
        if (currentHP <= 0 && Stage == 3)
        {
            currentHP = 0;
            PlayerDie3();
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
    
    private void PlayerDie1()
    {
        Debug.Log("플레이어가 사망했습니다.");
        DefeatPopup.SetActive(true);
        Player.SetActive(false);
        DefeatPopupScript.OpenDefeat1();
        Time.timeScale = 0f;
        // 보스 사망 처리 로직 (애니메이션, 드롭 아이템 등)
        // 예) 게임 오브젝트 비활성화, 패턴 루틴 종료 등
    }
    private void PlayerDie2()
    {
        Debug.Log("플레이어가 사망했습니다.");
        DefeatPopup.SetActive(true);
        Player.SetActive(false);
        DefeatPopupScript.OpenDefeat2();
        Time.timeScale = 0f;
        // 보스 사망 처리 로직 (애니메이션, 드롭 아이템 등)
        // 예) 게임 오브젝트 비활성화, 패턴 루틴 종료 등
    }
    private void PlayerDie3()
    {
        Debug.Log("플레이어가 사망했습니다.");
        DefeatPopup.SetActive(true);
        Player.SetActive(false);
        DefeatPopupScript.OpenDefeat3();
        Time.timeScale = 0f;
        // 보스 사망 처리 로직 (애니메이션, 드롭 아이템 등)
        // 예) 게임 오브젝트 비활성화, 패턴 루틴 종료 등
    }
}

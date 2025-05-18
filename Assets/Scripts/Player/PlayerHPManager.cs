using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHPManager : MonoBehaviour
{
    //�÷��̾� hp�� �̱������� ����, �� �̵��� �ʱ�ȭ
    public static PlayerHPManager Instance { get; private set; }

    [Header("�÷��̾� HP ����")]
    [SerializeField] private float maxHP = 100f;
    [Header("�÷��̾� ������Ʈ")]
    [SerializeField] private GameObject Player;
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
        // �̱��� �⺻ ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // �� ��ȯ �� �ı��ǵ��� ���� 
        // ���� �� HP �ʱ�ȭ
        currentHP = maxHP;
    }
    void Start()
    {
        Player = this.gameObject;
    }

    public void RestartHP()
    {
        currentHP = maxHP;
    }

    private void Update()
    {
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

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"�÷��̾ {damage} �������� ����. ���� HP: {currentHP}");
        
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
        DefeatPopup.SetActive(true);
        Player.SetActive(false);
        DefeatPopupScript.OpenDefeat1();
        Time.timeScale = 0f;
    }
    private void PlayerDie2()
    {
        Debug.Log("�÷��̾ ����߽��ϴ�.");
        DefeatPopup.SetActive(true);
        Player.SetActive(false);
        DefeatPopupScript.OpenDefeat2();
        Time.timeScale = 0f;
        // ���� ��� ó�� ���� (�ִϸ��̼�, ��� ������ ��)
        // ��) ���� ������Ʈ ��Ȱ��ȭ, ���� ��ƾ ���� ��
    }
    private void PlayerDie3()
    {
        Debug.Log("�÷��̾ ����߽��ϴ�.");
        DefeatPopup.SetActive(true);
        Player.SetActive(false);
        DefeatPopupScript.OpenDefeat3();
        Time.timeScale = 0f;
        // ���� ��� ó�� ���� (�ִϸ��̼�, ��� ������ ��)
        // ��) ���� ������Ʈ ��Ȱ��ȭ, ���� ��ƾ ���� ��
    }
}

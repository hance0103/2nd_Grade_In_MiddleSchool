using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.U2D;

public class BossHPManager : MonoBehaviour
{
    //보스 hp를 참조하는 곳이 많으므로 접근하기 쉽게 싱글톤으로 구현, 씬 이동시 초기화
    public static BossHPManager Instance { get; private set; }

    [Header("보스 HP 설정")]
    [SerializeField] public float maxHP = 100f;
    [Header("보스 현재 HP")]
    public float currentHP;
    [Header("승리 텍스트 입력 팝업")]
    [SerializeField] private GameObject VictoryInputPopup;
    [SerializeField] private VictoryTextInputPopup victoryTextPopupScript;
    [Header("광폭화 팝업")]
    [SerializeField] private GameObject BossEnragePopup;
    [SerializeField] private BossEnragePopup BossEnragePopupScript;
    [Header("보스 오브젝트")]
    [SerializeField] private GameObject Boss;
    [Header("스테이지")]
    [SerializeField] private int Stage;
    [Header("타이머")]
    [SerializeField] private GameObject Timer;
    [Header("보스 피격시")]
    [SerializeField] private float _blinkInterval = 0.15f;
    [SerializeField] private int _blinkCount = 1;

    private SpriteRenderer _sprite;
    
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

        _sprite = GetComponent<SpriteRenderer>();
    }

    public void RestartHP()
    {
        currentHP = maxHP;
        Enrageactive = true;
    }

    private bool Enrageactive = true;
    public void TakeDamage(float damage)
    {
        // 보스 피격시 스프라이트 깜박임
        StartCoroutine(BossHitBlink());
        currentHP -= damage;
        //Debug.Log($"보스 {damage} 데미지. 남은 HP: {currentHP}");
        if (currentHP <= 0&& Stage ==1)
        {
            currentHP = 0;
            BossDie1();
        }
        if (currentHP <= 0 && Stage == 2)
        {
            currentHP = 0;
            BossDie2();
        }
        if (currentHP <= 0 && Stage == 3)
        {
            currentHP = 0;
            BossDie3();
        }
    }
    private IEnumerator BossHitBlink()
    {
        for (int i = 0; i < _blinkCount; i++)
        {
            _sprite.color = new Color(70 / 255f, 70 / 255f, 70 / 255f);
            yield return new WaitForSeconds(_blinkInterval);
            _sprite.color = Color.white;
            yield return new WaitForSeconds(_blinkInterval);
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
    private void BossDie1()
    {
        // 1. Timer 컴포넌트를 찾아서
        Timer timer = FindObjectOfType<Timer>();

        if (timer != null)
        {
            // 2. TimeActive를 false로 변경하여 타이머 정지
            timer.TimeActive = false;
            Debug.Log(timer.curTime);
            // 3. 측정된 시간( curTime or CurrentTime )을 PlayerPrefs로 저장
            PlayerPrefs.SetFloat("FinalTime1", timer.curTime);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Timer 스크립트를 찾을 수 없습니다!");
        }
        Debug.Log("스테이지 1 보스가 사망했습니다.");
        Boss.SetActive(false);
        VictoryInputPopup.SetActive(true);
        victoryTextPopupScript.Stage1OpenInputPanel();
        
        // 보스 사망 처리 로직 (애니메이션, 드롭 아이템 등)
        // 예) 게임 오브젝트 비활성화, 패턴 루틴 종료 등
    }
    private void BossDie2()
    {
        // 1. Timer 컴포넌트를 찾아서
        Timer timer = FindObjectOfType<Timer>();

        if (timer != null)
        {
            // 2. TimeActive를 false로 변경하여 타이머 정지
            timer.TimeActive = false;
            Debug.Log(timer.curTime);
            // 3. 측정된 시간( curTime or CurrentTime )을 PlayerPrefs로 저장
            PlayerPrefs.SetFloat("FinalTime2", timer.curTime);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Timer 스크립트를 찾을 수 없습니다!");
        }
        Debug.Log("스테이지 2 보스가 사망했습니다.");
        Boss.SetActive(false);
        VictoryInputPopup.SetActive(true);
        victoryTextPopupScript.Stage2OpenInputPanel();
        
        // 보스 사망 처리 로직 (애니메이션, 드롭 아이템 등)
        // 예) 게임 오브젝트 비활성화, 패턴 루틴 종료 등
    }
    private void BossDie3()
    {
        // 1. Timer 컴포넌트를 찾아서
        Timer timer = FindObjectOfType<Timer>();

        if (timer != null)
        {
            // 2. TimeActive를 false로 변경하여 타이머 정지
            timer.TimeActive = false;
            Debug.Log(timer.curTime);
            // 3. 측정된 시간( curTime or CurrentTime )을 PlayerPrefs로 저장
            PlayerPrefs.SetFloat("FinalTime3", timer.curTime);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Timer 스크립트를 찾을 수 없습니다!");
        }
        Debug.Log("스테이지 3 보스가 사망했습니다.");
        Boss.SetActive(false);
        VictoryInputPopup.SetActive(true);
        victoryTextPopupScript.Stage3OpenInputPanel();
        
        // 보스 사망 처리 로직 (애니메이션, 드롭 아이템 등)
        // 예) 게임 오브젝트 비활성화, 패턴 루틴 종료 등
    }
}
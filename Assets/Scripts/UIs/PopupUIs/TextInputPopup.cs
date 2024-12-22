using UnityEngine;
using UnityEngine.UI; // 일반 UI를 쓴다면
using TMPro;

public class TextInputPopup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject inputPanel;   // 입력 패널(Panel) GameObject
    [SerializeField] private TMP_InputField inputField; // TextMeshPro 버전

    private string savedData;

    /// <summary>
    /// 외부 버튼(메인 버튼)에서 이 함수를 연결하여 패널을 열도록 함
    /// </summary>
    public void Stage1OpenInputPanel()
    {
        Time.timeScale = 0f; // 시간 정지
        inputPanel.SetActive(true);
        
        
        // 패널이 열릴 때 입력란 초기화
        inputField.text = "";
    }

    /// <summary>
    /// 확인/저장 버튼 기능 , 스테이지 별로 피니쉬 대사를 따로 적용할 수 있도록 구분해주기
    /// </summary>
    public void Stage1SaveAndClosePanel()
    {
        Time.timeScale = 1f; // 시간 재개
        savedData = inputField.text;
        PlayerPrefs.SetString("FinalText1", savedData);
        PlayerPrefs.Save();
        inputPanel.SetActive(false);
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI; // 일반 UI를 쓴다면
// using TMPro;       // TextMeshPro를 쓴다면

public class InputPanelManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inputPanel; // 입력 패널(Panel) GameObject
    public InputField inputField; // 일반 InputField
    // public TMP_InputField tmpInputField; // TextMeshPro 버전을 쓰고 싶다면

    private string savedData;

    
    /// <summary>
    /// 외부 버튼(메인 버튼)에서 이 함수를 연결하여 패널을 열도록 함
    /// </summary>
    public void OpenInputPanel()
    {
        inputPanel.SetActive(true);
        // 패널이 열릴 때 입력란 초기화
        inputField.text = "";
    }

    /// <summary>
    /// 확인/저장 버튼 기능
    /// </summary>
    public void SaveAndClosePanel()
    {
        // 입력된 텍스트 저장
        savedData = inputField.text;

        
        PlayerPrefs.SetString("UserInput", savedData);
        PlayerPrefs.Save(); // PlayerPrefs

        // 패널 닫기
        inputPanel.SetActive(false);
    }
}

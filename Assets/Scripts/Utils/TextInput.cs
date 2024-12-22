using UnityEngine;
using TMPro;

public class TMPInputFieldExample : MonoBehaviour
{
    public TMP_InputField myTMPInputField;

    private void Start()
    {
        // 값이 변경될 때마다 호출될 함수를 등록
        myTMPInputField.onValueChanged.AddListener(OnValueChanged);

        // 엔터키(Submit) 입력 시 호출될 함수를 등록
        myTMPInputField.onEndEdit.AddListener(OnEndEdit);
    }

    private void OnValueChanged(string value)
    {
        Debug.Log("TMP 현재 입력중: " + value);
    }

    private void OnEndEdit(string value)
    {
        Debug.Log("TMP 최종 입력: " + value);
    }
}

using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPopup; // 설정 팝업


    void Start()
    {
        // 디폴트 값으로 팝업 비활성화
        //settingsPopup.SetActive(false);

    }

    // 설정 팝업 열기
    public void OpenSettings()
    {
        settingsPopup.SetActive(true);
    }

    // 설정 팝업 닫기
    public void CloseSettings()
    {
        settingsPopup.SetActive(false);
    }

}

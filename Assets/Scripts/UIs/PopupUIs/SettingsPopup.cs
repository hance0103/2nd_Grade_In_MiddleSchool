using UnityEngine;

public class PopupSettings : MonoBehaviour
{
    public GameObject settingsPopup; // ¼³Á¤ ÆË¾÷

    // ¼³Á¤ ÆË¾÷ ¿­±â
    public void OpenSettings()
    {
        settingsPopup.SetActive(true);
    }

    // ¼³Á¤ ÆË¾÷ ´Ý±â
    public void CloseSettings()
    {
        settingsPopup.SetActive(false);
    }

}

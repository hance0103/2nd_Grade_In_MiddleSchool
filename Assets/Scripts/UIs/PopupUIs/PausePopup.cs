using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject PausePopup;  // Å©·¹µ÷ ÆË¾÷

    void Start()
    {
        // µðÆúÆ® °ªÀ¸·Î ÆË¾÷ ºñÈ°¼ºÈ­

        PausePopup.SetActive(false);
    }




    // Å©·¹µ÷ ÆË¾÷ ¿­±â
    public void OpenPause()
    {
        PausePopup.SetActive(true);
    }

    // Å©·¹µ÷ ÆË¾÷ ´Ý±â
    public void ClosePause()
    {
        PausePopup.SetActive(false);
    }
}

using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    public GameObject creditsPopup;  // Å©·¹µ÷ ÆË¾÷

    void Start()
    {
        // µðÆúÆ® °ªÀ¸·Î ÆË¾÷ ºñÈ°¼ºÈ­

        //creditsPopup.SetActive(false);
    }


    

    // Å©·¹µ÷ ÆË¾÷ ¿­±â
    public void OpenCredits()
    {
        creditsPopup.SetActive(true);
    }

    // Å©·¹µ÷ ÆË¾÷ ´Ý±â
    public void CloseCredits()
    {
        creditsPopup.SetActive(false);
    }
}

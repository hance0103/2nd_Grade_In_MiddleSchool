using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    public GameObject creditsPopup; // ¼³Á¤ ÆË¾÷

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

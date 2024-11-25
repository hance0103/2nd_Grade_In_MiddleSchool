using UnityEngine;

public class PopupDiary : MonoBehaviour
{
    public GameObject DiaryPopup;  // 다이어리 팝업

    // 다이어리 팝업 열기
    public void OpenDiary()
    {
        DiaryPopup.SetActive(true);
    }

    // 다이어리 팝업 닫기
    public void CloseDiary()
    {
        DiaryPopup.SetActive(false);
    }
}
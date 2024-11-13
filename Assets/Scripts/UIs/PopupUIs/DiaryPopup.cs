using UnityEngine;

public class DiaryManager : MonoBehaviour
{
    public GameObject DiaryPopup;  // 다이어리 팝업

    void Start()
    {
        // 디폴트 값으로 팝업 비활성화

        DiaryPopup.SetActive(false);
    }




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
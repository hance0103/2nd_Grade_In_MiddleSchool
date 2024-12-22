using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro.Examples;

public class DiaryPopup : MonoBehaviour
{

    [SerializeField] private TMP_Text Stage1ClearTime;
    [SerializeField] private TMP_Text Stage1ClearText;
    [SerializeField] private TMP_Text Stage2ClearTime;
    [SerializeField] private TMP_Text Stage2ClearText;
    [SerializeField] private TMP_Text Stage3ClearTime;
    [SerializeField] private TMP_Text Stage3ClearText;
    [SerializeField] private float time;
    [SerializeField] private float curTime;

    int minute;
    int second;
    // 다이어리 팝업 열기
    public void OpenDiaryPage1()
    {
        gameObject.SetActive(true);
        float savedTime = PlayerPrefs.GetFloat("FinalTime1", 0f);
        curTime = time;
        curTime = savedTime;
        Debug.Log(savedTime);
        minute = (int)curTime / 60;
        second = (int)curTime % 60;
        Stage1ClearTime.text = "클리어 시간 : " + minute.ToString("00") + ":" + second.ToString("00");
        
        string loadedData = PlayerPrefs.GetString("FinalText1", "기본값");
        Debug.Log("불러온 데이터: " + loadedData);
        Stage1ClearText.text = "피니쉬 대사: " + loadedData;

    }
    
    public void OpenDiaryPage2()
    {
        
        gameObject.SetActive(true);
        /*
        float savedTime = PlayerPrefs.GetFloat("FinalTime2", 0f);
        curTime = time;
        curTime = savedTime;
        Debug.Log(savedTime);
        minute = (int)curTime / 60;
        second = (int)curTime % 60;

        Stage2ClearTime.text = "클리어 시간 : " + minute.ToString("00") + ":" + second.ToString("00");

        

        float savedTime = PlayerPrefs.GetFloat("FinalTime3", 0f);
        curTime = time;
        curTime = savedTime;
        Debug.Log(savedTime);
        minute = (int)curTime / 60;
        second = (int)curTime % 60;

        Stage3ClearTime.text = "클리어 시간 : " + minute.ToString("00") + ":" + second.ToString("00");
        */
    }


    // 다이어리 팝업 닫기
    public void CloseDiaryPage()
    {
        gameObject.SetActive(false);
    }
}

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
        SoundManager.Instance.EffectSoundOn("1");
        gameObject.SetActive(true);
        float savedTime1 = PlayerPrefs.GetFloat("FinalTime1", 0f);
        curTime = time;
        curTime = savedTime1;
        Debug.Log(savedTime1);
        minute = (int)curTime / 60;
        second = (int)curTime % 60;
        Stage1ClearTime.text = "클리어 시간 : " + minute.ToString("00") + ":" + second.ToString("00");
        
        string SavedText1 = PlayerPrefs.GetString("FinalText1", "아직 도륙내지 않았다!");
        Debug.Log("불러온 데이터: " + SavedText1);
        Stage1ClearText.text = "피니쉬 대사: " + SavedText1;

    }
    
    public void OpenDiaryPage2()
    {
        SoundManager.Instance.EffectSoundOn("1");
        gameObject.SetActive(true);
        
        float savedTime2 = PlayerPrefs.GetFloat("FinalTime2", 0f);
        curTime = time;
        curTime = savedTime2;
        Debug.Log(savedTime2);
        minute = (int)curTime / 60;
        second = (int)curTime % 60;
        Stage2ClearTime.text = "클리어 시간 : " + minute.ToString("00") + ":" + second.ToString("00");
        string SavedText2 = PlayerPrefs.GetString("FinalText2","아직 도륙내지 않았다!");
        Debug.Log("불러온 데이터: " + SavedText2);
        Stage2ClearText.text = "피니쉬 대사: " + SavedText2;


        float savedTime3 = PlayerPrefs.GetFloat("FinalTime3", 0f);
        curTime = time;
        curTime = savedTime3;
        Debug.Log(savedTime3);
        minute = (int)curTime / 60;
        second = (int)curTime % 60;
        Stage3ClearTime.text = "클리어 시간 : " + minute.ToString("00") + ":" + second.ToString("00");
        string SavedText3 = PlayerPrefs.GetString("FinalText3", "아직 도륙내지 않았다!");
        Debug.Log("불러온 데이터: " + SavedText3);
        Stage3ClearText.text = "피니쉬 대사: " + SavedText3;

    }


    // 다이어리 팝업 닫기
    public void CloseDiaryPage()
    {
        SoundManager.Instance.EffectSoundOn("3");
        gameObject.SetActive(false);
    }
}

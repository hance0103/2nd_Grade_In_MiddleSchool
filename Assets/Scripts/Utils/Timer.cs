using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    [SerializeField] private float time;
    [SerializeField] public float curTime;

    int minute;
    int second;
    public bool TimeActive = true;
    private void Awake()
    {
        time = 0;
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        curTime = time;
        while (TimeActive)
        {
            curTime += Time.deltaTime;
            
            minute = (int)curTime / 60;
            second = (int)curTime % 60;
            text.text = minute.ToString("00") + ":" + second.ToString("00");
            
            yield return null;

            if (!TimeActive)
            {
                Debug.Log("시간 종료");
                Debug.Log(curTime);
                curTime = 0;
                yield break;
                
            }
        }
        
    }
    public float CurrentTime
    {
        get { return curTime; }
    }
}
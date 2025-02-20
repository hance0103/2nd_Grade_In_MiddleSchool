using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class SceneMain : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private float soundEffect_delayTime;

    private bool isDraggingAllowed = false; // 드래그 가능 여부 플래그

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 시 현재 선택된 UI 요소가 DraggableButton을 포함하고 있는지 확인 후 맞다면 실행

        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("DraggableButton"))
        {
            isDraggingAllowed = true;
            Debug.Log("드래그 시작");
        }
        else
        {
            isDraggingAllowed = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDraggingAllowed)
        {
            
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDraggingAllowed)
        {
            Debug.Log("드래그 종료");
            StartCoroutine(LoadStageSelect());
        }
    }

    private IEnumerator LoadStageSelect()
    {
        SoundManager.Instance.EffectSoundOn("2"); // 효과음 재생

        yield return new WaitForSeconds(soundEffect_delayTime);

        SceneManager.LoadScene("StageSelect");
    }

    public GameObject settingsPopup; // 설정 팝업
    public GameObject creditsPopup; // 크레딧 팝업

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

    // 크레딧 팝업 열기
    public void OpenCredits()
    {
        creditsPopup.SetActive(true);
    }

    // 크레딧 팝업 닫기
    public void CloseCredits()
    {
        creditsPopup.SetActive(false);
    }
}
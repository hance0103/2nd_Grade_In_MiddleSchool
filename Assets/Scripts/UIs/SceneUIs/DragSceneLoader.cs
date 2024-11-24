using TMPro.Examples;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class DragAndExecute : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private float delayBeforeSceneLoad = 1.6f; // 씬 전환 전 대기 시간
    private Vector3 initialPosition; // 드래그 시작 전 위치 저장

    private void Start()
    {
        // 오브젝트의 초기 위치 저장
        initialPosition = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 시작");
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 중: 오브젝트를 따라 움직임
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 종료");

        // 드래그가 끝나면 특정 기능 실행
        StartCoroutine(ExecuteFunction());

        // 오브젝트를 원래 위치로 되돌림
        transform.position = initialPosition;
    }

    private IEnumerator ExecuteFunction()
    {
        PlayEffectSound();
        Debug.Log(delayBeforeSceneLoad);
        yield return new WaitForSeconds(delayBeforeSceneLoad);

        Debug.Log("실행 여부판단");
        LoadStageSelect();
    }

    private void PlayEffectSound()
    {
        // 효과음 로드
        SoundManager.Instance.EffectSoundOn("Guitarplug");
        Debug.Log("효과음 재생");
       
    }

    private void LoadStageSelect()
    {
        Debug.Log("LoadScene");
        SceneManager.LoadScene("StageSelect");
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class DragAndExecute : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private float soundEffect_delayTime;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 시작");
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 종료");

        StartCoroutine(LoadStageSelect());
    }

    private IEnumerator LoadStageSelect()
    {
        SoundManager.Instance.EffectSoundOn("Guitarplug"); // 효과음 재생

        yield return new WaitForSeconds(soundEffect_delayTime);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("StageSelect");
        while (!asyncLoad.isDone)
        {
            yield return null; // 씬 로드가 완료될 때까지 대기
        }
    }
}

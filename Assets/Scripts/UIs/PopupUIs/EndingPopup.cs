using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndingPopup : MonoBehaviour
{
    [Header("Image References")]
    public Image background1;
    public Image frame1;
    public Image background2;
    public Image frame2;
    public Image background3;
    public Image frame3;

    [Header("Timings")]
    // 페이드 인에 걸리는 시간(초)
    public float fadeDuration = 1f;
    // 각 페이드 인이 완료된 후 대기 시간(초)
    public float waitDuration = 4f;

    private void OnEnable()
    {
        SoundManager.Instance.EndingBgmOn();
        Time.timeScale = 1f;
        // 패널이 활성화될 때 연출 코루틴을 시작
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        // 1) background1 서서히 등장
        yield return StartCoroutine(FadeInImage(background1, fadeDuration));
        yield return new WaitForSeconds(fadeDuration);

        // 2) frame1 서서히 등장
        yield return StartCoroutine(FadeInImage(frame1, fadeDuration));
        yield return new WaitForSeconds(waitDuration);

        // 3) background2 서서히 등장
        yield return StartCoroutine(FadeInImage(background2, fadeDuration));
        yield return new WaitForSeconds(fadeDuration);

        // 4) frame2 서서히 등장
        yield return StartCoroutine(FadeInImage(frame2, fadeDuration));
        yield return new WaitForSeconds(waitDuration);

        // 5) background3 서서히 등장
        yield return StartCoroutine(FadeInImage(background3, fadeDuration));
        yield return new WaitForSeconds(fadeDuration);

        // 6) frame3 서서히 등장
        yield return StartCoroutine(FadeInImage(frame3, fadeDuration));
        // 모든 연출 종료
        yield return new WaitForSeconds(10f);
        End();
    }

    private IEnumerator FadeInImage(Image targetImage, float duration)
    {
        targetImage.gameObject.SetActive(true);

        // 색상 알파를 0으로 설정해 완전히 투명하게 만든 뒤 점차 1로 올린다
        Color originalColor = targetImage.color;
        originalColor.a = 0f;
        targetImage.color = originalColor;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            originalColor.a = alpha;
            targetImage.color = originalColor;
            yield return null;
        }

        // 마지막에 확실히 1로 고정
        originalColor.a = 1f;
        targetImage.color = originalColor;
    }

    void End()
    {
        SoundManager.Instance.MainBgmOn();
        SceneManager.LoadScene("Main");
    }
}
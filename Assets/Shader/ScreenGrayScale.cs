using System.Collections;
using UnityEngine;

/// <summary>
/// 싱글톤 흑백 효과 관리자.
/// ScreenGrayscale.SetGrayscale(true/false, fadeTime);
/// ScreenGrayscale.Flash(duration);
/// </summary>
[ExecuteInEditMode, DisallowMultipleComponent]
public class ScreenGrayscale : MonoBehaviour
{
    public static ScreenGrayscale Instance { get; private set; }

    [Tooltip("GrayScale 셰이더(Material) 지정이 없으면 코드에서 자동 생성")]
    [SerializeField] Material grayscaleMat;

    [Range(0f, 1f)] public float currentWeight;   // Inspector 확인용
    [SerializeField] float fadeSpeed = 3f;        // 1초 동안 3만큼 이동(기본)

    bool targetOn;

    #region ───── 초기화 ─────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;


        if (grayscaleMat == null)
        {
            Shader s = Shader.Find("Hidden/GrayScale");
            grayscaleMat = new Material(s);
        }
    }
    #endregion

    #region ───── 퍼블릭 API ─────
    /// <summary>흑백 ON/OFF.  fadeTime=0 이면 즉시 전환</summary>
    public static void SetGrayscale(bool on, float fadeTime = 0.3f)
    {
        if (Instance == null) return;
        Instance.targetOn = on;
        Instance.fadeSpeed = (fadeTime <= 0f) ? 999f : 1f / fadeTime;
    }

    /// <summary>duration 초 만큼 흑백으로 유지 후 자동 복귀</summary>
    public static void Flash(float duration, float fadeIn = 0.15f, float fadeOut = 0.4f)
    {
        if (Instance == null) return;
        Instance.StartCoroutine(Instance.CoFlash(duration, fadeIn, fadeOut));
    }
    #endregion

    #region ───── 구현부 ─────
    void Update()
    {
        float target = targetOn ? 1f : 0f;
        if (!Mathf.Approximately(currentWeight, target))
        {
            currentWeight = Mathf.MoveTowards(currentWeight, target, fadeSpeed * Time.unscaledDeltaTime);
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (currentWeight <= 0f)
        {
            Graphics.Blit(src, dst);           // 효과 꺼짐
        }
        else
        {
            grayscaleMat.SetFloat("_Weight", currentWeight);
            Graphics.Blit(src, dst, grayscaleMat);
        }
    }

    IEnumerator CoFlash(float holdTime, float fadeIn, float fadeOut)
    {
        SetGrayscale(true, fadeIn);
        yield return new WaitForSecondsRealtime(fadeIn + holdTime);
        SetGrayscale(false, fadeOut);
    }
    #endregion
}
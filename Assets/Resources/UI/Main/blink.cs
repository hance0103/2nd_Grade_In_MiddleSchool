using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Image(들)의 알파를 0 ↔ maxAlpha 사이로 깜빡이게 함.
/// 오브젝트에 붙이기만 하면 동작. 필요시 자식까지 포함 가능.
/// </summary>
[DisallowMultipleComponent]
public class UIImageAlphaFlicker : MonoBehaviour
{
    [Header("Flicker 설정")]
    [Range(0f, 1f)] public float maxAlpha = 0.5f;   // 0 ~ 0.5 사이로 깜빡임
    [Min(0f)] public float speed = 6f;              // 깜빡임 속도(높을수록 빠름)
    public bool includeChildren = true;             // 자식까지 적용
    public bool useUnscaledTime = true;              // Time.timeScale 영향 안 받기

    [Header("시작/종료 동작")]
    public bool playOnEnable = true;                 // 활성화 시 자동 시작
    public bool setAlphaZeroOnDisable = true;        // 비활성화/중지 시 알파 0으로 정리

    // 내부
    readonly List<Image> _images = new();
    bool _playing;

    void Awake()
    {
        CacheImages();
    }

    void OnEnable()
    {
        if (playOnEnable) StartFlicker();
    }

    void OnDisable()
    {
        if (setAlphaZeroOnDisable) SetAlpha(0f);
        _playing = false;
    }

    void Update()
    {
        if (!_playing) return;

        float t = (useUnscaledTime ? Time.unscaledTime : Time.time) * speed;
        // 0 ↔ maxAlpha 사이를 PingPong
        float a = Mathf.PingPong(t, maxAlpha); // 0 ~ maxAlpha

        SetAlpha(a);
    }

    /// <summary> 깜빡임 시작 </summary>
    public void StartFlicker()
    {
        if (_images.Count == 0) CacheImages();
        _playing = true;
    }

    /// <summary> 깜빡임 정지 (옵션: 알파 0으로) </summary>
    public void StopFlicker(bool forceAlphaZero = true)
    {
        _playing = false;
        if (forceAlphaZero) SetAlpha(0f);
    }

    /// <summary> 대상 Image들 캐싱 </summary>
    void CacheImages()
    {
        _images.Clear();
        if (includeChildren)
            _images.AddRange(GetComponentsInChildren<Image>(true));
        else
        {
            var img = GetComponent<Image>();
            if (img != null) _images.Add(img);
        }
    }

    /// <summary> RGB는 유지하고 알파만 설정 </summary>
    void SetAlpha(float a)
    {
        for (int i = 0; i < _images.Count; i++)
        {
            var img = _images[i];
            if (!img) continue;

            var c = img.color;
            c.a = a;
            img.color = c;
        }
    }
}
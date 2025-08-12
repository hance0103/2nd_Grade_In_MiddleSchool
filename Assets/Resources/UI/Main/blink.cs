using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SpriteRenderer(들)의 알파를 0 ↔ maxAlpha 사이로 깜빡이게 함.
/// 오브젝트에 붙이기만 하면 동작. 필요시 자식까지 포함 가능.
/// </summary>
[DisallowMultipleComponent]
public class SpriteAlphaFlicker : MonoBehaviour
{
    [Header("Flicker 설정")]
    [Range(0f, 1f)] public float maxAlpha = 0.5f;   // 0 ~ 0.5 사이로 깜빡임
    [Min(0f)] public float speed = 6f;              // 깜빡임 속도(높을수록 빠름)
    public bool includeChildren = true;             // 자식까지 적용
    public bool useUnscaledTime = true;             // Time.timeScale 영향 안 받기

    [Header("시작/종료 동작")]
    public bool playOnEnable = true;                // 활성화 시 자동 시작
    public bool setAlphaZeroOnDisable = true;       // 비활성화/중지 시 알파 0으로 정리

    // 내부
    readonly List<SpriteRenderer> _renderers = new();
    bool _playing;

    void Awake()
    {
        CacheRenderers();
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
        if (_renderers.Count == 0) CacheRenderers();
        _playing = true;
    }

    /// <summary> 깜빡임 정지 (옵션: 알파 0으로) </summary>
    public void StopFlicker(bool forceAlphaZero = true)
    {
        _playing = false;
        if (forceAlphaZero) SetAlpha(0f);
    }

    /// <summary> 대상 SpriteRenderer들 캐싱 </summary>
    void CacheRenderers()
    {
        _renderers.Clear();
        if (includeChildren)
            _renderers.AddRange(GetComponentsInChildren<SpriteRenderer>(true));
        else
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null) _renderers.Add(sr);
        }
    }

    /// <summary> RGB는 유지하고 알파만 설정 </summary>
    void SetAlpha(float a)
    {
        for (int i = 0; i < _renderers.Count; i++)
        {
            var sr = _renderers[i];
            if (!sr) continue;

            var c = sr.color;
            c.a = a;
            sr.color = c;
        }
    }
}
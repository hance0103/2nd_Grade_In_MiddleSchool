using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    [SerializeField] private AudioSource bgmAudioSource; // 인스펙터에서 BGM 오디오 소스 설정
    [SerializeField] private AudioSource sfxAudioSource; // 인스펙터에서 SFX 오디오 소스 설정

    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundManager>();
                if (_instance == null)
                {
                    _instance = new GameObject("SoundManager").AddComponent<SoundManager>();
                }
            }
            return _instance;
        }
    }

    

    public AudioMixer audioMixer;
    public float currentBGMVolume { get; set; }
    public float currentEffectVolume { get; set; }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        currentBGMVolume = 1;
        currentEffectVolume = 1;

        audioMixer = Resources.Load<AudioMixer>("NewMixer");

        // AudioSources가 인스펙터에서 할당되었는지 확인
        if (bgmAudioSource == null || sfxAudioSource == null)
        {
            Debug.LogError("AudioSource가 인스펙터에 할당되지 않았습니다.");
            return;
        }
        
        bgmAudioSource.loop = true;
        bgmAudioSource.volume = currentBGMVolume;
    }

    public void Play(AudioClip audioClip, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null) return;

        if (type == Sound.Bgm)
        {
            if (bgmAudioSource.isPlaying)
                bgmAudioSource.Stop();
            bgmAudioSource.pitch = pitch;
            bgmAudioSource.clip = audioClip;
            bgmAudioSource.Play();
        }
        else
        {
            sfxAudioSource.pitch = pitch;
            sfxAudioSource.PlayOneShot(audioClip);
        }
    }

    public bool IsBGMPlaying()
    {
        return bgmAudioSource != null && bgmAudioSource.isPlaying;
    }

    // BGM 오디오 소스를 외부에서 접근할 수 있는 프로퍼티
    public AudioSource BgmAudioSource => bgmAudioSource;
}

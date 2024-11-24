/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static SoundManager Instance { get; private set; }

    AudioSource[] _audioSources = new AudioSource[(int)Sound.MaxCount];
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    public AudioMixer audioMixer;
    public float currentBGMVolume { get; set; }
    public float currentEffectVolume { get; set; }

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
            Init(); // 초기화
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 새 오브젝트를 삭제
        }
    }

    public void Init()
    {
        currentBGMVolume = 1;
        currentEffectVolume = 1;

        // AudioMixer 로드
        audioMixer = Resources.Load<AudioMixer>("NewMixer");
        if (audioMixer == null)
        {
            Debug.LogError("AudioMixer를 찾을 수 없습니다. 'NewMixer'가 Resources 폴더에 있는지 확인하세요.");
            return;
        }

        // AudioMixerGroup 명확히 가져오기
        AudioMixerGroup sfxGroup = null;
        AudioMixerGroup bgmGroup = null;

        foreach (var group in audioMixer.FindMatchingGroups("Master"))
        {
            if (group.name.Equals("SFX", System.StringComparison.OrdinalIgnoreCase))
            {
                sfxGroup = group;
            }
            else if (group.name.Equals("BGM", System.StringComparison.OrdinalIgnoreCase))
            {
                bgmGroup = group;
            }
        }

        if (sfxGroup == null)
        {
            Debug.LogError("SFX AudioMixerGroup을 찾을 수 없습니다. 'SFX' 그룹 이름을 확인하세요.");
        }

        if (bgmGroup == null)
        {
            Debug.LogError("BGM AudioMixerGroup을 찾을 수 없습니다. 'BGM' 그룹 이름을 확인하세요.");
        }

        // @Sound 오브젝트 설정
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            root.transform.parent = this.transform;
            DontDestroyOnLoad(root);
        }

        // AudioSource 배열 초기화
        if (_audioSources == null || _audioSources.Length != (int)Sound.MaxCount)
        {
            _audioSources = new AudioSource[(int)Sound.MaxCount];
        }

        // AudioSource 생성 및 설정
        string[] soundNames = System.Enum.GetNames(typeof(Sound));
        for (int i = 0; i < (int)Sound.MaxCount; i++)
        {
            if (_audioSources[i] == null)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;

                // AudioMixerGroup 연결
                if (i == (int)Sound.Effect)
                {
                    _audioSources[i].outputAudioMixerGroup = sfxGroup;
                    if (sfxGroup != null)
                    {
                        Debug.Log($"SFX AudioSource가 SFX AudioMixerGroup에 연결되었습니다.");
                    }
                }
                else if (i == (int)Sound.Bgm)
                {
                    _audioSources[i].outputAudioMixerGroup = bgmGroup;
                    if (bgmGroup != null)
                    {
                        Debug.Log($"BGM AudioSource가 BGM AudioMixerGroup에 연결되었습니다.");
                    }
                }
            }
        }

        // BGM AudioSource 루프 설정
        if (_audioSources[(int)Sound.Bgm] != null)
        {
            _audioSources[(int)Sound.Bgm].loop = true;
        }
        else
        {
            Debug.LogError("BGM AudioSource가 초기화되지 않았습니다.");
        }
    }


    public void SetBGMVolume(float volume)
    {
        float linearVolume = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat("BGMVolume", linearVolume);
    }
    public void SetEffectVolume(float volume)
    {
        float linearVolume = Mathf.Log10(volume) * 20; // 볼륨을 dB로 변환
        audioMixer.SetFloat("EffectVolume", linearVolume);
    }

    public float GetEffectVolume()
    {
        if (audioMixer.GetFloat("EffectVolume", out float volume))
        {
            return Mathf.Pow(10, volume / 20); // dB를 일반 볼륨 값으로 변환
        }
        return 1.0f; // 기본값
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    public void Play(AudioClip audioClip, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
        {
            Debug.LogError("재생할 AudioClip이 null입니다.");
            return;
        }

        AudioSource audioSource = _audioSources[(int)type];
        Debug.Log(audioSource.name);
        if (audioSource == null)
        {
            Debug.LogError($"{type} AudioSource가 초기화되지 않았습니다.");
            return;
        }

        // MixerGroup 연결 상태 확인
        if (audioSource.outputAudioMixerGroup != null)
        {
            Debug.Log($"{type} AudioSource가 {audioSource.outputAudioMixerGroup.name} 그룹에 연결되었습니다.");
        }
        else
        {
            Debug.LogError($"{type} AudioSource가 AudioMixerGroup에 연결되지 않았습니다.");
        }

        audioSource.pitch = pitch;

        if (type == Sound.Bgm)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
            audioSource.clip = audioClip;
            audioSource.Play();
            Debug.Log($"BGM 재생 시작: {audioClip.name}");
        }
        else
        {
            audioSource.PlayOneShot(audioClip);
            Debug.Log($"효과음 재생 시작: {audioClip.name}");
        }
    }

    public void Play(string path, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

    public void ChangeBGM(AudioClip newBGM)
    {
        if (newBGM == null) return;

        AudioSource bgmSource = _audioSources[(int)Sound.Bgm];
        if (bgmSource == null)
        {
            Debug.LogError("BGM AudioSource가 초기화되지 않았습니다.");
            return;
        }

        // 이미 재생 중인 배경음악이라면 중복 재생 방지
        if (bgmSource.clip == newBGM && bgmSource.isPlaying)
        {
            return; // 같은 음악이 이미 재생 중이면 종료
        }

        bgmSource.Stop(); // 기존 배경음악 멈춤
        bgmSource.clip = newBGM; // 새로운 배경음악 설정
        bgmSource.Play(); // 새로운 배경음악 재생
    }

    public AudioClip GetOrAddAudioClip(string path, Sound type = Sound.Effect)
    {
        if (!path.Contains("Sounds/"))
            path = $"Sounds/{path}";
        AudioClip audioClip = null;

        if (type == Sound.Bgm)
        {
            audioClip = GameManager.Resource.Load<AudioClip>(path);
        }
        else
        {
            if (!_audioClips.TryGetValue(path, out audioClip))
            {
                audioClip = GameManager.Resource.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.Log($"AudioClip Missing {path}");

        return audioClip;
    }

    public bool isBGMPlaying()
    {
        return _audioSources[(int)Sound.Bgm].isPlaying;
    }
}
*/
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

        audioMixer = Resources.Load<AudioMixer>("NewMixer");
        if (audioMixer == null)
        {
            Debug.LogError("AudioMixer를 찾을 수 없습니다. 'NewMixer'가 Resources 폴더에 있는지 확인하세요.");
            return;
        }

        AudioMixerGroup[] audioMixerGroups = audioMixer.FindMatchingGroups("Master");
        if (audioMixerGroups.Length == 0)
        {
            Debug.LogError("AudioMixerGroup을 찾을 수 없습니다.");
            return;
        }

        // @Sound 오브젝트 설정
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            root.transform.parent = this.transform;
            DontDestroyOnLoad(root);
        }

        // 배열 크기 확인 및 AudioSource 초기화
        if (_audioSources == null || _audioSources.Length != (int)Sound.MaxCount)
        {
            _audioSources = new AudioSource[(int)Sound.MaxCount];
        }

        // AudioSource 생성 및 설정
        string[] soundNames = System.Enum.GetNames(typeof(Sound));
        for (int i = 0; i < (int)Sound.MaxCount; i++) // MaxCount까지 반복
        {
            if (_audioSources[i] == null)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;

                if (i < audioMixerGroups.Length)
                {
                    _audioSources[i].outputAudioMixerGroup = audioMixerGroups[i];
                }
                else
                {
                    Debug.LogWarning($"AudioMixerGroup 할당 실패: {soundNames[i]}에 적절한 그룹이 없습니다.");
                }
            }
        }

        // BGM AudioSource 설정 확인
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
            return;
        }
        if (type == Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Sound.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();
            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
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

    AudioClip GetOrAddAudioClip(string path, Sound type = Sound.Effect)
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

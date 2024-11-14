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
    public void SetBGMVolume(float volume)
    {
        if (volume <= 0.0001f) // 슬라이더 값이 0에 가까우면 아주 작은 값으로 설정
        {
            audioMixer.SetFloat("BGMVolume", -80f); // 최소 볼륨 (일반적으로 -80dB은 무음으로 간주)
        }
        else
        {
            audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20); // 로그 변환으로 볼륨 조정
        }
    }
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
        // 나중에 세이브데이터에서 받아오기
        currentBGMVolume = 1;
        currentEffectVolume = 1;

        // AudioMixer 로드
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
            root.transform.parent = this.transform; // SoundManager의 자식으로 설정
            DontDestroyOnLoad(root);

            // AudioSource 생성 및 설정
            string[] soundNames = System.Enum.GetNames(typeof(Sound));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;

                // AudioSource에 AudioMixerGroup 할당
                if (i + 1 < audioMixerGroups.Length)
                {
                    _audioSources[i].outputAudioMixerGroup = audioMixerGroups[i + 1];
                }
                else
                {
                    Debug.LogWarning($"AudioMixerGroup 할당 실패: {soundNames[i]}에 적절한 그룹이 없습니다.");
                }
            }

            // 배경음악 AudioSource 설정
            _audioSources[(int)Sound.Bgm].loop = true;
        }
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

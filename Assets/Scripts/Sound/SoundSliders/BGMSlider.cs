using UnityEngine;
using UnityEngine.UI;

public class BackgroundAndEffectMusicSlider : MonoBehaviour
{
    public Slider bgmSlider;           // 배경음 슬라이더
    private AudioSource bgmSource;     // 배경음악 Audio Source

    void Start()
    {
        // `BackgroundMusicDontDestroy` 오브젝트의 AudioSource를 가져오기
        BackgroundMusicDontDestroy musicManager = FindObjectOfType<BackgroundMusicDontDestroy>();
        if (musicManager != null)
        {
            bgmSource = musicManager.GetComponent<AudioSource>();
        }

        // AudioSource가 유효한지 확인
        if (bgmSource == null)
        {
            Debug.LogError("AudioSource를 찾을 수 없습니다. 오브젝트가 삭제되었는지 확인하세요.");
            return;
        }

        // 슬라이더 값 설정 및 이벤트 등록

        float savedVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        bgmSlider.value = savedVolume;
        bgmSource.volume = savedVolume;

        // 슬라이더 값이 변경될 때 이벤트 등록
        bgmSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.onValueChanged.AddListener(ChangeBGMVolume);
    }

    // 배경음 볼륨 조절
    public void ChangeBGMVolume(float volume)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = volume;
            PlayerPrefs.SetFloat("BGMVolume", volume); // 슬라이더 값을 저장
            PlayerPrefs.Save(); // PlayerPrefs에 변경 사항 저장
        }
        else
        {
            Debug.LogWarning("AudioSource가 null입니다. 볼륨 조정을 할 수 없습니다.");
        }
    }
}
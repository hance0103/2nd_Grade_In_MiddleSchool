using UnityEngine;
using UnityEngine.UI;

public class BackgroundAndEffectMusicSlider : MonoBehaviour
{
    public Slider bgmSlider; // 배경음 슬라이더

    void Start()
    {
        // SoundManager 싱글톤을 통해 AudioSource 가져오기
        AudioSource bgmSource = SoundManager.Instance.BgmAudioSource;

        // AudioSource가 유효한지 확인
        if (bgmSource == null)
        {
            Debug.LogError("AudioSource를 찾을 수 없습니다. SoundManager를 확인하세요.");
            return;
        }

        // 슬라이더 값 설정 및 이벤트 등록
        float savedVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        bgmSlider.value = savedVolume;
        bgmSource.volume = savedVolume;

        // 슬라이더 값이 변경될 때 이벤트 등록
        bgmSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.onValueChanged.AddListener(volume => ChangeBGMVolume(bgmSource, volume));
    }

    // 배경음 볼륨 조절
    void ChangeBGMVolume(AudioSource bgmSource, float volume)
    {
        bgmSource.volume = volume;
        PlayerPrefs.SetFloat("BGMVolume", volume); // 슬라이더 값을 저장
        PlayerPrefs.Save(); // PlayerPrefs에 변경 사항 저장
    }
}
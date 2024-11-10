using UnityEngine;
using UnityEngine.UI;

public class BackgroundAndEffectMusicSlider : MonoBehaviour
{
    public Slider bgmSlider;           // 배경음 슬라이더
    public AudioSource bgmSource;      // 배경음악 Audio Source
    /*
    public Slider sfxSlider;           // 효과음 슬라이더
    public AudioSource sfxSource;      // 효과음 Audio Source
    */
    void Start()
    {
        // 배경음 슬라이더 값 설정 및 이벤트 등록
        float savedVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        bgmSlider.value = savedVolume;
        bgmSource.volume = savedVolume;

        // 슬라이더 값이 변경될 때 이벤트 등록
        bgmSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.onValueChanged.AddListener(ChangeBGMVolume);

        /* 효과음 슬라이더 값 설정 및 이벤트 등록
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        sfxSlider.value = savedSFXVolume;
        sfxSource.volume = savedSFXVolume;
        sfxSlider.onValueChanged.AddListener(ChangeSFXVolume);
        */
    }

    // 배경음 볼륨 조절
    public void ChangeBGMVolume(float volume)
    {
        bgmSource.volume = volume;
        PlayerPrefs.SetFloat("BGMVolume", volume);  // 슬라이더 값을 저장
        PlayerPrefs.Save();  // PlayerPrefs에 변경 사항 저장
    }

    /* 효과음 볼륨 조절
    public void ChangeSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
    */
}
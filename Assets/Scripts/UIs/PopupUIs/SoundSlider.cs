using UnityEngine;
using UnityEngine.UI;

public class BackgroundAndEffectMusicSlider : MonoBehaviour
{
    public Slider bgmSlider;           // 배경음 슬라이더
    public AudioSource bgmSource;      // 배경음악 Audio Source
    public Slider sfxSlider;           // 효과음 슬라이더
    public AudioSource sfxSource;      // 효과음 Audio Source

    void Start()
    {
        // 배경음 슬라이더 값 설정 및 이벤트 등록
        bgmSlider.value = bgmSource.volume;
        bgmSlider.onValueChanged.AddListener(ChangeBGMVolume);

        // 효과음 슬라이더 값 설정 및 이벤트 등록
        sfxSlider.value = sfxSource.volume;
        sfxSlider.onValueChanged.AddListener(ChangeSFXVolume);
    }

    // 배경음 볼륨 조절
    public void ChangeBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    // 효과음 볼륨 조절
    public void ChangeSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
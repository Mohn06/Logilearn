using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public enum VolumeType { Music, SFX }
    public VolumeType volumeType;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        // Load saved volume
        float savedValue = PlayerPrefs.GetFloat(volumeType + "Volume", 1f);
        slider.value = savedValue;

        // Apply it immediately
        OnSliderChanged(savedValue);

        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        if (volumeType == VolumeType.Music)
            AudioManager.Instance.SetMusicVolume(value);
        else
            AudioManager.Instance.SetSFXVolume(value);
    }
}

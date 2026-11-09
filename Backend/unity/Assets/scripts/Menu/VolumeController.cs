using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        SetVolume(volumeSlider.value);
    }

    public void SetVolume(float value)
    {
        float volume = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;

        audioMixer.SetFloat("MasterVolume", volume);

        PlayerPrefs.SetFloat("Volume", value);
        PlayerPrefs.Save();
    }
}
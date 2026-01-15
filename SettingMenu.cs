using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;

    void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioManager.Instance.GetVolume();
            volumeSlider.onValueChanged.AddListener(UpdateVolume);
        }
    }

    void UpdateVolume(float value)
    {
        AudioManager.Instance.SetVolume(value);
    }
}

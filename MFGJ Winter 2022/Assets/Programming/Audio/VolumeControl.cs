using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] Slider[] volumeSlider;
    //[SerializeField] Toggle[] muteToggle;

    AudioMixerManager audioMixerManager;

    void Start()
    {
        audioMixerManager = AudioMixerManager.Instance;

        //applySliderChange = applyToggleChange = false;
        for (int i = 0; i < volumeSlider.Length; i++)
        {
            audioMixerManager.SetVolume(i, volumeSlider[i].value = audioMixerManager.LoadVolume(i));
            //muteToggle[i].isOn = volumeSlider[i].value == volumeSlider[i].minValue ? false : true;
        }
        //applySliderChange = applyToggleChange = true;
    }

    bool applySliderChange = true;
    public void OnVolumeSliderChange(int index)
    {
        if (!applySliderChange)
            return;

        float volumeValue = volumeSlider[index].value;
        audioMixerManager.SetVolume(index, volumeValue);
        audioMixerManager.SaveVolume(index, volumeValue);

        /*applyToggleChange = false;
        if (volumeSlider[index].value == volumeSlider[index].minValue)
            muteToggle[index].isOn = false;
        else if (!muteToggle[index].isOn)
            muteToggle[index].isOn = true;
        applyToggleChange = true;*/
    }

    /*bool applyToggleChange;
    public void OnMuteToggleChange(int index)
    {
        if (!applyToggleChange)
            return;

        float volumeValue = muteToggle[index].isOn ? volumeSlider[index].maxValue
                                                   : volumeSlider[index].minValue;
        audioMixerManager.SetVolume(index, volumeValue);

        applySliderChange = false;
        volumeSlider[index].value = volumeValue;
        audioMixerManager.SaveVolume(index, volumeSlider[index].value);
        applySliderChange = true;
    }*/

    public void LoadDefault()
    {
        applySliderChange = false;
        for (int i = 0; i < volumeSlider.Length; i++)
        {
            audioMixerManager.SetVolume(i, volumeSlider[i].value = audioMixerManager.LoadDefaultVolume(i));
            audioMixerManager.SaveVolume(i, volumeSlider[i].value);
        }
        applySliderChange = true;

        /*applyToggleChange = false;
        for (int i = 0; i < muteToggle.Length; i++)
            muteToggle[i].isOn = true;
        applyToggleChange = true;*/
    }
}
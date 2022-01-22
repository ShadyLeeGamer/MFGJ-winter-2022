using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] string[] volumeParameter;
    [SerializeField, Range(-80, 0)] float[] defaultVolume;

    public static AudioMixerManager Instance { get; set; }

    void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    void Start()
    {
        RefreshVolumes();
    }

    public void RefreshVolumes()
    {
        for (int i = 0; i < volumeParameter.Length; i++)
            SetVolume(i, LoadVolume(i));
    }

    public void SetVolume(int index, float volume)
    {
        mixer.SetFloat(volumeParameter[index], volume);
    }

    public float LoadVolume(int index)
    {
        return PlayerPrefs.GetFloat(volumeParameter[index], defaultVolume[index]);
    }

    public void SaveVolume(int index, float value)
    {
        PlayerPrefs.SetFloat(volumeParameter[index], value);
    }

    public float LoadDefaultVolume(int index)
    {
        return defaultVolume[index];
    }
}
using UnityEngine;
using System.Collections.Generic;

public class AudioStation : MonoBehaviour
{
    [SerializeField] AudioPlayer audioPlayerPrefab;
    //[SerializeField] AudioPlayer CloseListennerAudioPlayerPrefab;
    public static AudioStation Instance { get; private set; }

    [HideInInspector] public List<AudioPlayer> audioPlayers = new List<AudioPlayer>();
    AudioPlayer currentMusicPlayer;

    //ObjectPooler objectPooler;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        //objectPooler = ObjectPooler.Instance;
    }

    public void StartNewRandomSFXPlayer(AudioClip[] clips, Vector3 pos = default, Transform parent = null,
                                        float pitchMin = 1, float pitchMax = 1, bool is2D = false)
    {
        //AudioPlayer newAudioPlayer = objectPooler.GetAudioPlayer(pos, Quaternion.identity, default);
        AudioPlayer sfxAudioPlayer = Instantiate(audioPlayerPrefab, pos, Quaternion.identity);
        sfxAudioPlayer.transform.SetParent(parent ? parent : transform);
        audioPlayers.Add(sfxAudioPlayer);
        sfxAudioPlayer.SetupSFX(clips, pitchMin, pitchMax, is2D);
        sfxAudioPlayer.Play();
    }

    public void StartNewSFXPlayer(AudioClip clip, Vector3 pos = default, Transform parent = null,
                                  float pitchMin = 1, float pitchMax = 1, bool is2D = false)
    {
        //AudioPlayer newAudioPlayer = objectPooler.GetAudioPlayer(pos, Quaternion.identity, default);
        if (parent)
            pos = parent.position;
        AudioPlayer sfxAudioPlayer = Instantiate(audioPlayerPrefab, pos, Quaternion.identity);
        sfxAudioPlayer.transform.SetParent(parent ? parent : transform);
        audioPlayers.Add(sfxAudioPlayer);
        sfxAudioPlayer.SetupSFX(clip, pitchMin, pitchMax, is2D);
        sfxAudioPlayer.Play();
    }

    public void StartNewMusicPlayer(AudioClip clip, bool loop)
    {
        if (currentMusicPlayer != null)
            if (clip == currentMusicPlayer.AudioSource.clip)
                return;
            else
            {
                audioPlayers.Remove(currentMusicPlayer);
                Destroy(currentMusicPlayer.gameObject);
            }

        currentMusicPlayer = Instantiate(audioPlayerPrefab, transform.position, Quaternion.identity);
        currentMusicPlayer.transform.SetParent(transform);
        audioPlayers.Add(currentMusicPlayer);
        currentMusicPlayer.SetupMusic(clip, loop);
        currentMusicPlayer.Play();
    }

    public void ClearSFXPlayers()
    {
        for (int i = 0; i < audioPlayers.Count; i++)
            if (audioPlayers[i] != currentMusicPlayer)
            {
                Destroy(audioPlayers[i]);
                audioPlayers.Remove(audioPlayers[i]);
            }
    }

    public void SetAllPlayerPause(bool isPaused)
    {
        for (int i = 0; i < audioPlayers.Count; i++)
            if (isPaused)
                audioPlayers[i].AudioSource.Pause();
            else
                audioPlayers[i].AudioSource.UnPause();
    }
}
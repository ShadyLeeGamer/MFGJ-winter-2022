using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour//, IPooledObject
{
    public AudioSource AudioSource { get; private set; }
    [SerializeField] AudioMixerGroup musicGroup, SFXGroup;

    AudioStation audioStation;
    //ObjectPooler objectPooler;

    bool started;

    void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        audioStation = AudioStation.Instance;
        //objectPooler = ObjectPooler.Instance;
    }

   // public void Initialise (ObjectData audioPlayerData) { }

    public void SetupSFX(AudioClip[] clips, float audioPitchMin, float audioPitchMax, bool is2D)
    {
        AudioClip randomClip = clips[Random.Range(0, clips.Length)];
        SetupSFX(randomClip, audioPitchMin, audioPitchMax, is2D);
    }

    public void SetupSFX(AudioClip clip, float audioPitchMin, float audioPitchMax, bool is2D)
    {
        AudioSource.clip = clip;
        AudioSource.outputAudioMixerGroup = SFXGroup;
        AudioSource.pitch = Random.Range(audioPitchMin, audioPitchMax);
        AudioSource.spatialBlend = is2D ? 0 : 1;
        Play();
    }

    public void SetupMusic(AudioClip clip, bool loop)
    {
        AudioSource.clip = clip;
        AudioSource.outputAudioMixerGroup = musicGroup;
        AudioSource.loop = loop;
        AudioSource.pitch = 1;
        AudioSource.reverbZoneMix = 0; // NO REVERB ZONE EFFECT TO MUSIC
        AudioSource.spatialBlend = 0;
        Play();
    }

    public void Play()
    {
        name = AudioSource.clip.name;
        AudioSource.Play();
        started = true;
    }

    void Update()
    {
        if (started)
            if (!AudioSource.loop)
                StartCoroutine(RecycleAfterAudioEnd());
    }

    IEnumerator RecycleAfterAudioEnd()
    {
        yield return new WaitForSeconds(AudioSource.clip.length + .5f);
        audioStation.audioPlayers.Remove(this);
        //objectPooler.RecycleAudioPlayer(this);
    }
}
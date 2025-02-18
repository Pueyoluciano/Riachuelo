using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public SoundList name;
        public AudioClip clip;
        public bool loop;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [Header("Audio Sources")]
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioSource clipsAudioSource;


    public List<Sound> sounds = new List<Sound>();
    private Dictionary<SoundList, Sound> soundDict;
    private AudioSource audioSource;

    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        audioSource = gameObject.AddComponent<AudioSource>();
        soundDict = new Dictionary<SoundList, Sound>();

        foreach (var sound in sounds)
        {
            soundDict[sound.name] = sound;
        }
    }

    public void PlaySound(SoundList soundName)
    {
        if (soundDict.TryGetValue(soundName, out Sound sound))
        {
            if (sound.loop)
            {
                musicAudioSource.loop = sound.loop;
                musicAudioSource.clip = sound.clip;
                musicAudioSource.volume = sound.volume;
                musicAudioSource.Play();
            }
            else
            {
                audioSource.PlayOneShot(sound.clip, sound.volume);
            }
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Sonido '{soundName}' no encontrado.");
        }
    }
}

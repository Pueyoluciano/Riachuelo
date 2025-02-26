using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioSource clipsAudioSource;

    [Header("Audio Database")]
    public Audio audios;

    public void PlaySound(AudioData audioData)
    {
        if (audioData.loop)
        {
            musicAudioSource.loop = audioData.loop;
            musicAudioSource.clip = audioData.clip;
            musicAudioSource.volume = audioData.volume;
            musicAudioSource.Play();
        }
        else
        {
            clipsAudioSource.PlayOneShot(audioData.clip, audioData.volume);
        }
    }
}

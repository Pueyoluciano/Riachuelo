using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioDatabase", menuName = "ScriptableObjects/Audio/AudioDatabase", order = 1)]
public class Audio : ScriptableObject
{
    [Header("Music")]
    public AudioData LocationMusic;

    [Header("Clip")]
    [Header("Take Picture")]
    public AudioData Shutter;

    [Header("Confirm Picture")]
    public AudioData SavePicture;
    public AudioData DiscardPicture;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioData", menuName = "ScriptableObjects/Audio/AudioData", order = 2)]
public class AudioData : ScriptableObject
{
    public AudioClip clip; 
    public bool loop;
    [Range(0f, 1f)] public float volume = 1f;
}

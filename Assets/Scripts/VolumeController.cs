using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [SerializeField] Image volumeImage;

    [Header("Sprites")]
    [SerializeField] Sprite volumeOnSprite;
    [SerializeField] Sprite volumeOffSprite;

    bool isVolumeOn;

    public bool IsVolumeOn { get => isVolumeOn;}

    public static Action<bool> VolumeToggled;

    private void Awake()
    {
        isVolumeOn = true;
        volumeImage.sprite = volumeOnSprite;
    }

    public void ToggleVolume()
    {
        isVolumeOn = !isVolumeOn;
        volumeImage.sprite = isVolumeOn? volumeOnSprite: volumeOffSprite;
        VolumeToggled?.Invoke(isVolumeOn);
    }
}

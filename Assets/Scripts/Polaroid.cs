using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Polaroid : MonoBehaviour
{
    [Header("Picture")]
    [SerializeField] RawImage pictureFrame;

    [Header("Texts")]
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI subtitle;

    public RawImage PictureFrame { get => pictureFrame; set => pictureFrame = value; }
    public TextMeshProUGUI Title { get => title; set => title = value; }
    public TextMeshProUGUI Subtitle { get => subtitle; set => subtitle = value; }
}

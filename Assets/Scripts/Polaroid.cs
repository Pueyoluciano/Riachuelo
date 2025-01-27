using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Polaroid : MonoBehaviour
{
    [SerializeField] Camera polaroidCamera;
    // [SerializeField] RawImage image;
    [SerializeField] RectTransform whiteFrameRectTransform;
    [SerializeField] CanvasScaler canvas;

    [SerializeField] Image pictureFrame;
    [SerializeField] RectTransform oldTVMaskRectTransform;

    [Header("polaroid Info")]
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI subtitle;

    public Camera PolaroidCamera { get => polaroidCamera;}
    // public RawImage Image { get => image; set => image = value; }
    public Vector2 Size { get => canvas.referenceResolution; }
    public string Title { get => title.text; set => title.text = value; }
    public string Subtitle { get => subtitle.text; set => subtitle.text = value; }
    public RectTransform WhiteFrameRectTransform { get => whiteFrameRectTransform; }
    public Image PictureFrame { get => pictureFrame; }
    public RectTransform OldTVMaskRectTransform { get => oldTVMaskRectTransform; }
}

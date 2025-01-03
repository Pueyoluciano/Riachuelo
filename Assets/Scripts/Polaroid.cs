using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Polaroid : MonoBehaviour
{
    [SerializeField] Camera polaroidCamera;
    [SerializeField] RawImage image;
    [SerializeField] CanvasScaler canvas;

    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI subtitle;

    public Camera PolaroidCamera { get => polaroidCamera;}
    public RawImage Image { get => image; set => image = value; }
    public Vector2 Size { get => canvas.referenceResolution; }
    public string Title { get => title.text; set => title.text = value; }
    public string Subtitle { get => subtitle.text; set => subtitle.text = value; }
}

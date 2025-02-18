using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PolaroidController : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField] Camera polaroidCamera;
    [SerializeField] CanvasScaler canvas;

    [Header("Polaroid")]
    [SerializeField] Polaroid polaroid;

    public Camera PolaroidCamera { get => polaroidCamera;}
    public Vector2 Size { get => canvas.referenceResolution; }
    public string Title { get => polaroid.Title.text; set => polaroid.Title.text = value; }
    public string Subtitle { get => polaroid.Subtitle.text; set => polaroid.Subtitle.text = value; }
    public RawImage PictureFrame { get => polaroid.PictureFrame; }
}

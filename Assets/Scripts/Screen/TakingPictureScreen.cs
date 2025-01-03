using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TakingPictureScreen : UIScreen
{
    [SerializeField] Image pictureFrame;
    [SerializeField] RectTransform canvasRectTransform;
    [SerializeField] int frameScale;
    [SerializeField] float movementSpeed;

    [Header("Screenshot")]
    [SerializeField] Camera screenshotCamera;
    [SerializeField] GameObject CameraDisplayOverlay;

    [Header("Polaroid")]
    [SerializeField] Polaroid polaroidCanvas;

    string folderPath = "Screenshots";
    public override void Init()
    {
        pictureFrame.rectTransform.sizeDelta = new Vector3(canvasRectTransform.rect.width * frameScale, 1, 0);
        polaroidCanvas.Image.color = Color.white;
    }

    public override void Enable()
    {
        base.Enable();
        pictureFrame.sprite = GameManager.Instance.PerspectiveScreen.GetCurrentPerspective;
    }

    public override void GetInput()
    {
        LookAround();

        if (Input.GetKeyDown(KeyCode.P))
        {
            TakePolaroidScreenshot();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            NextScreen = Screens.Perspective;
        }
    }

    private void LookAround()
    {
        Vector2 displacement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * movementSpeed * Time.deltaTime;
        Vector2 newPosition = pictureFrame.rectTransform.anchoredPosition - displacement;

        Vector2 canvasSize = canvasRectTransform.sizeDelta;
        Vector2 imageSize = pictureFrame.rectTransform.sizeDelta;

        imageSize.x *= pictureFrame.rectTransform.localScale.x;
        imageSize.y *= pictureFrame.rectTransform.localScale.y;


        float minX = -(imageSize.x - canvasSize.x);
        float maxX = 0;
        float minY = -(imageSize.y - canvasSize.y);
        float maxY = 0;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        pictureFrame.rectTransform.anchoredPosition = newPosition;
    }

    private void TakePolaroidScreenshot()
    {
        CameraDisplayOverlay.SetActive(false);
        
        Texture2D miniature = TakeScreenshot(screenshotCamera, (int)canvasRectTransform.rect.height, (int)canvasRectTransform.rect.height);

        CameraDisplayOverlay.SetActive(true);

        polaroidCanvas.Image.texture = miniature;

        polaroidCanvas.Title = "Fotito";
        polaroidCanvas.Subtitle = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        TakeScreenshot(polaroidCanvas.PolaroidCamera, (int)polaroidCanvas.Size.x, (int)polaroidCanvas.Size.y, true);
    }

    private Texture2D TakeScreenshot(Camera camera, int width, int height, bool saveToDisk=false)
    {
        if(saveToDisk && !Directory.Exists(folderPath))
             Directory.CreateDirectory(folderPath);

        RenderTexture rt = new RenderTexture(width, height, 24);
        camera.targetTexture = rt;
        camera.Render();

        RenderTexture.active = rt;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        if (saveToDisk)
        {
            byte[] bytes = screenshot.EncodeToPNG();
            string screenshotName = $"{Directory.GetCurrentDirectory()}/{folderPath}/Screenshot_ {DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.png";
            File.WriteAllBytes(screenshotName, bytes);

            Debug.Log($"Screenshot guardado en: {screenshotName}");
        }

        camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        return screenshot;

        /*
        // Generar el nombre del archivo
        string screenshotName = $"{System.IO.Directory.GetCurrentDirectory()}/{folderPath}/Screenshot_ {DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.png";
        Debug.Log($"Screenshot guardado en: {screenshotName}");

        // Capturar el screenshot
        ScreenCapture.CaptureScreenshot(screenshotName, 2);

        */
    }
}

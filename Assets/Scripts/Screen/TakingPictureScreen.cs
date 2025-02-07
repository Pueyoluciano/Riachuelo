using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TakingPictureScreen : UIScreen
{
    [SerializeField] Image pictureFrame;
    [SerializeField] Image mask;
    [SerializeField] GameObject cameraDisplayOverlay;

    [Header("Looking Around")]
    [SerializeField] float movementSpeed;
    
    [Header("Zoom")]
    [SerializeField] RectTransform center;
    [SerializeField] float frameScale;
    [SerializeField] float minFrameScale;
    [SerializeField] float maxFrameScale;
    [SerializeField] float zoomSpeed;

    [Header("Polaroid")]
    [SerializeField] Polaroid polaroidCanvas;

    readonly string folderPath = "Screenshots";

    public override void Init()
    {
        UpdateFrameScale();
        polaroidCanvas.PictureFrame.color = Color.white;        
    }

    public override void OnEnter()
    {
        base.OnEnter();
        pictureFrame.sprite = GameManager.Instance.PerspectiveScreen.GetCurrentLocation.GetPerspective();

        // reset zoom and position
        frameScale = minFrameScale;
        Vector2 actualSize = pictureFrame.rectTransform.sizeDelta * pictureFrame.rectTransform.localScale;
        Vector2 centerActualSize = center.sizeDelta * center.localScale;
        pictureFrame.rectTransform.anchoredPosition = (-actualSize + centerActualSize) / 2;
    }

    public override void GetInput()
    {
        Zoom();
        LookAround();

        if (Input.GetKeyDown(KeyCode.P))
        {
            TakePolaroidScreenshot();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            NextScreen = Screens.Perspective;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            NextScreen = Screens.Messages;
        }
    }

    private void UpdateFrameScale()
    {
        pictureFrame.transform.localScale = Vector3.one * frameScale;
    }

    private void Zoom()
    {
        bool zoomIn = Input.GetKey(KeyCode.Z);
        bool zoomOut = Input.GetKey(KeyCode.X);

        if (!zoomIn && !zoomOut)
            return;

        float deltaZoom = 0;

        if (zoomIn)
        {
            deltaZoom = zoomSpeed;
        }

        if (zoomOut)
        {
            deltaZoom = -zoomSpeed;
        }

        frameScale = Mathf.Clamp(frameScale + deltaZoom * Time.deltaTime, minFrameScale, maxFrameScale);

        /*https://discussions.unity.com/t/scale-around-point-similar-to-rotate-around/531171/8 */

        // diff from object pivot to desired pivot/origin
        Vector3 pivotDelta = pictureFrame.transform.localPosition - new Vector3(center.pivot.x, center.pivot.y, 0);
        
        Vector3 scaleFactor = new Vector3(
            frameScale / pictureFrame.transform.localScale.x,
            frameScale / pictureFrame.transform.localScale.y,
            frameScale / pictureFrame.transform.localScale.z);
        pivotDelta.Scale(scaleFactor);
        
        pictureFrame.transform.localPosition = new Vector3(center.pivot.x, center.pivot.y, 0) + pivotDelta;

        UpdateFrameScale();
    }
    private void LookAround()
    {
        Vector2 displacement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * movementSpeed * Time.deltaTime;
        Vector2 newPosition = pictureFrame.rectTransform.anchoredPosition - displacement;

        Vector2 imageSize = pictureFrame.rectTransform.sizeDelta;
        Vector2 maskSize = mask.rectTransform.rect.size;

        imageSize.x *= pictureFrame.rectTransform.localScale.x;
        imageSize.y *= pictureFrame.rectTransform.localScale.y;


        Vector2 min = -(imageSize - maskSize);
        Vector2 max = new (0, 0);

        newPosition.x = Mathf.Clamp(newPosition.x, min.x, max.x);
        newPosition.y = Mathf.Clamp(newPosition.y, min.y, max.y);

        pictureFrame.rectTransform.anchoredPosition = newPosition;
    }

    private void TakePolaroidScreenshot()
    {
        polaroidCanvas.Title = GameManager.Instance.PerspectiveScreen.GetCurrentLocation.LocationName;
        polaroidCanvas.Subtitle = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        cameraDisplayOverlay.SetActive(false);
        Texture2D miniature = TakeScreenshot(Camera.main, (int)mask.rectTransform.rect.width, (int)mask.rectTransform.rect.height, 2, true, false);
        cameraDisplayOverlay.SetActive(true);

        polaroidCanvas.PictureFrame.texture = miniature;
        TakeScreenshot(polaroidCanvas.PolaroidCamera, (int)polaroidCanvas.Size.x, (int)polaroidCanvas.Size.y,2 ,false, true);
    }
    private Texture2D TakeScreenshot(Camera camera, int width, int height, int scale = 2, bool crop=true, bool saveToDisk=false)
    {
        if (saveToDisk && !Directory.Exists(folderPath))
             Directory.CreateDirectory(folderPath);

        // Tomo un screenshot con las dimensiones del canvas
        RenderTexture rt = new RenderTexture(width * scale, height * scale, 24);
        camera.targetTexture = rt;
        camera.Render();

        RenderTexture.active = rt;
        Texture2D screenshot = new Texture2D(width * scale, height * scale, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, width * scale, height * scale), 0, 0);
        screenshot.Apply();

        if (crop)
        {
            // Hago un crop del screenshot original para dejar solo el rectangulo cuadrado correspondiente al modo foto.
            // Hago un Hardcodeo nefasto para calcular los valores de crop.
            // TODO: Dios tenga en la gloria a la persona que resuelva como calcular este crop de forma correcta.

            var newPixels = screenshot.GetPixels(22 * scale, 160 * scale, 592 * scale, 592 * scale);
            var croppedScreenshot = new Texture2D(592 * scale, 592 * scale);

            croppedScreenshot.SetPixels(newPixels);
            croppedScreenshot.Apply();

            screenshot = croppedScreenshot;
        }
        
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
    }
}

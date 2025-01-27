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
    [SerializeField] int frameScale;
    [SerializeField] float movementSpeed;

    [Header("Polaroid")]
    [SerializeField] Polaroid polaroidCanvas;

    readonly string folderPath = "Screenshots";
    public override void Init()
    {
        pictureFrame.rectTransform.sizeDelta = new Vector3(1, mask.rectTransform.rect.height * frameScale, 0);
        polaroidCanvas.PictureFrame.rectTransform.sizeDelta = new Vector3(1, polaroidCanvas.OldTVMaskRectTransform.rect.width * frameScale, 0);
        polaroidCanvas.PictureFrame.color = Color.white;
    }

    public override void Enable()
    {
        base.Enable();
        pictureFrame.sprite = GameManager.Instance.PerspectiveScreen.GetCurrentLocation.GetPerspective();
        polaroidCanvas.PictureFrame.sprite = GameManager.Instance.PerspectiveScreen.GetCurrentLocation.GetPerspective();
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
    private Vector2 GetRelativePosition(Vector2 position, RectTransform container)
    {
        Vector2 containerSize = container.rect.size;
        return new Vector2(position.x / containerSize.x, position.y / containerSize.y);
    }

    private Vector2 GetAbsolutePosition(Vector2 relativePosition, RectTransform container, RectTransform image)
    {
        Vector2 containerSize = container.rect.size;

        float absoluteX = relativePosition.x * containerSize.x;
        float absoluteY = relativePosition.y * containerSize.y;

        return new Vector2(absoluteX, absoluteY);
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

        Vector2 relativePosition = GetRelativePosition(newPosition, mask.rectTransform);
        Vector2 polaroidNewPosition = GetAbsolutePosition(relativePosition, polaroidCanvas.OldTVMaskRectTransform, polaroidCanvas.PictureFrame.rectTransform);
        polaroidCanvas.PictureFrame.rectTransform.anchoredPosition = polaroidNewPosition;

    }

    private void TakePolaroidScreenshot()
    {
        polaroidCanvas.Title = GameManager.Instance.PerspectiveScreen.GetCurrentLocation.LocationName;
        polaroidCanvas.Subtitle = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        TakeScreenshot(polaroidCanvas.PolaroidCamera, (int)polaroidCanvas.Size.x, (int)polaroidCanvas.Size.y, true);
    }

    private Texture2D TakeScreenshot(Camera camera, int width, int height, bool saveToDisk=false)
    {
        if(saveToDisk && !Directory.Exists(folderPath))
             Directory.CreateDirectory(folderPath);

        RenderTexture rt = new RenderTexture(width * 2, height * 2, 24);
        camera.targetTexture = rt;
        camera.Render();

        RenderTexture.active = rt;
        Texture2D screenshot = new Texture2D(width * 2, height * 2, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, width * 2, height * 2), 0, 0);
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
    }
}

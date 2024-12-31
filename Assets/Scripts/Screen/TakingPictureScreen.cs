using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakingPictureScreen : UIScreen
{
    [SerializeField] Image pictureFrame;
    [SerializeField] RectTransform canvasRectTransform;
    [SerializeField] int frameScale;
    [SerializeField] float movementSpeed;

    string folderPath = "Screenshots";
    private int screenshotCount;

    public override void Init()
    {
        screenshotCount = 0;
        pictureFrame.rectTransform.sizeDelta = new Vector3(canvasRectTransform.rect.width * frameScale, 1, 0);
    }

    public override void GetInput()
    {
        LookAround();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            TakeScreenshot();
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

    private void TakeScreenshot()
    {
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }

        // Generar el nombre del archivo
        string screenshotName = $"{folderPath}/Screenshot_{screenshotCount}.png";
        Debug.Log($"Screenshot guardado en: {screenshotName}");

        // Capturar el screenshot
        ScreenCapture.CaptureScreenshot(screenshotName);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Perspective")]
    [SerializeField] LocationData locationData;
    [SerializeField] Image perspective;

    [Header("Taking Picture")]
    [SerializeField] Image pictureFrame;
    [SerializeField] RectTransform canvasRectTransform;
    [SerializeField] int frameScale;
    [SerializeField] float movementSpeed;

    [Header("Fade In/Out")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeInDuration;
    [SerializeField] float fadeOutDuration;

    int currentPerspective;
    bool isFading;

    string folderPath = "Screenshots";
    private int screenshotCount;

    CameraStates cameraStates;

    private void Start()
    {
        currentPerspective = 0;
        canvasGroup.alpha = 1f;
        isFading = false;
        
        cameraStates = CameraStates.TakingPicture;

        screenshotCount = 0;
        pictureFrame.rectTransform.sizeDelta = new Vector3(canvasRectTransform.rect.width * frameScale, 1, 0);
        UpdatePerspective();
    }

    private void Update()
    {
        switch(cameraStates)
        {
            case CameraStates.Perspective:
                if (!isFading && Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    PreviousPerspective();
                }

                if (!isFading && Input.GetKeyDown(KeyCode.RightArrow))
                {
                    NextPerspective();
                }
                break;

            case CameraStates.TakingPicture:
                LookAround();

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    TakeScreenshot();
                }

                break;
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


        float minX = - (imageSize.x - canvasSize.x);
        float maxX = 0;
        float minY = - (imageSize.y - canvasSize.y);
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

    private void PreviousPerspective()
    {
        currentPerspective = (currentPerspective - 1 + locationData.perspectivesList.Count) % locationData.perspectivesList.Count;
        StartCoroutine(FadeSequence());
    }

    private void NextPerspective()
    {
        currentPerspective = (currentPerspective + 1) % locationData.perspectivesList.Count;
        StartCoroutine(FadeSequence());
    }

    private void UpdatePerspective()
    {
        perspective.sprite = locationData.perspectivesList[currentPerspective];
    }

    public void FadeIn()
    {
        StartCoroutine(Fade(0f, 1f, fadeInDuration));
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(1f, 0f, fadeOutDuration));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }

    private IEnumerator FadeSequence()
    {
        isFading = true;
        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));

        UpdatePerspective();

        yield return StartCoroutine(Fade(0f, 1f, fadeOutDuration));

        isFading = false;
    }
}

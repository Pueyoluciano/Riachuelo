using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TakingPictureScreen : UIScreen
{
    [SerializeField] Image pictureFrame;
    [SerializeField] Image mask;
    [SerializeField] GameObject cameraDisplayOverlay;

    [Header("Looking Around")]
    [SerializeField] float movementSpeed;

    [Header("Ponder")]
    [SerializeField] RectTransform PonderingPoint;
    [SerializeField] float ponderingCooldown;
    [SerializeField] RectTransform mainCanvas;

    [Header("Wobble")]
    public float minWobbleAmount = 0.1f;
    public float maxWobbleAmount = 0.3f;
    public float wobbleSpeed = 1f;

    [Header("Zoom")]
    [SerializeField] TextMeshProUGUI zoomText;
    [SerializeField] RectTransform center;
    [SerializeField] float frameScale;
    [SerializeField] float minFrameScale;
    [SerializeField] float maxFrameScale;
    [SerializeField] float zoomSpeed;

    [Header("Shutter Effect")]
    [SerializeField] Image shutterPanel;

    [Header("Picture Validation")]
    [SerializeField] RectTransform limitArea;
    [SerializeField] RectTransform minArea;
    [SerializeField] RectTransform maxArea;

    readonly string folderPath = "Screenshots";

    private PolaroidController polaroidController;
    private ConversationManager conversationManager;

    // The copied temporal Points of interest.
    // This is populated when entering to this screen and
    // wiped out when leaving.
    private List<PointOfInterest> currentPointsOfInterest;

    private float lastSuccessfullPonder;

    // Picture
    private Texture2D lastTakenScreenshot;
    private ConversationData lastTakenScreenshotData;
    bool isLastTakenScreenshotValid;

    public override bool IsOverlay => false;

    public ConversationData LastTakenScreenshotData { get => lastTakenScreenshotData; }
    public bool IsLastTakenScreenshotValid { get => isLastTakenScreenshotValid; }

    public override void Init()
    {
        UpdateFrameScale();
        polaroidController = GameManager.Instance.PolaroidController;
        polaroidController.PictureFrame.color = Color.white;
        currentPointsOfInterest = new List<PointOfInterest>();
        lastSuccessfullPonder = -1;

        conversationManager = GameManager.Instance.ConversationManger;

        // Only show inspectables in editor Mode

        limitArea.gameObject.GetComponent<Image>().enabled = GameManager.Instance.visualDebugEnabled;
        minArea.gameObject.GetComponent<Image>().enabled = GameManager.Instance.visualDebugEnabled;
        maxArea.gameObject.GetComponent<Image>().enabled = GameManager.Instance.visualDebugEnabled;
    }

    public override void OnEnter(bool resetState)
    {
        base.OnEnter(resetState);
        pictureFrame.sprite = GameManager.Instance.PerspectiveScreen.GetCurrentLocation.GetPerspective();

        CopyPointsOfInterestToFrame();

        if (resetState)
        {
            // reset zoom and position
            frameScale = minFrameScale;
            UpdateFrameScale();
            Vector2 actualSize = pictureFrame.rectTransform.sizeDelta * pictureFrame.rectTransform.localScale;
            Vector2 centerActualSize = center.sizeDelta * center.localScale;
            pictureFrame.rectTransform.anchoredPosition = (-actualSize + centerActualSize) / 2;
        }
    }

    public override void OnExit(bool isNextScreenOverlay)
    {
        base.OnExit(isNextScreenOverlay);

        DeletePointsOfInterestFromFrame();
    }

    public override void GetInput()
    {
        Zoom();
        LookAround();

        if (Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Instance.ActionsController.TakePicture.Use();
            TakePolaroidScreenshot();
            NextScreen = Screens.ConfirmPicture;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            NextScreen = Screens.Logbook;
            GameManager.Instance.ActionsController.Logbook.Use();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            NextScreen = Screens.Gallery;
            GameManager.Instance.ActionsController.Gallery.Use();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            NextScreen = Screens.Perspective;
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            Ponder();
        }
    }
    private void Ponder()
    {
        if (GameManager.Instance.MessagesController.StartedConversation || 
            lastSuccessfullPonder != -1 && !Utilities.CheckCooldown(ponderingCooldown, lastSuccessfullPonder))
            return;

        foreach (var pointOfInterest in currentPointsOfInterest)
        {
            if(IsOverlapping(pointOfInterest.GetComponent<RectTransform>(), PonderingPoint))
            {
                UsePonderAction(pointOfInterest.PonderData);
                return;
            }
        }

        UsePonderAction(conversationManager.conversation.Nothing_Interesting);
    }

    private bool IsOverlapping(RectTransform rectA, RectTransform rectB)
    {        
        Rect rect1 = GetCanvasRect(rectA);
        Rect rect2 = GetCanvasRect(rectB);
        return rect1.Overlaps(rect2);
    }

    private bool IsContained(RectTransform containerRectTransform, RectTransform contentRectTransform)
    {
        Rect container = GetCanvasRect(containerRectTransform);
        Rect content = GetCanvasRect(contentRectTransform);

        return container.Contains(new Vector2(content.xMin, content.yMin)) &&
        container.Contains(new Vector2(content.xMax, content.yMin)) &&
        container.Contains(new Vector2(content.xMin, content.yMax)) &&
        container.Contains(new Vector2(content.xMax, content.yMax));
    }
    
    private Rect GetCanvasRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners); // Obtiene las esquinas en coordenadas mundiales

        Camera cam = Camera.main; // Cámara usada por el Canvas
        Vector2 min = RectTransformUtility.WorldToScreenPoint(cam, corners[0]); // Esquina inferior izquierda
        Vector2 max = RectTransformUtility.WorldToScreenPoint(cam, corners[2]); // Esquina superior derecha

        // Crear el rect en coordenadas de pantalla
        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);   
    }

    private void UsePonderAction(ConversationData conversationData)
    {
        MessagesController.OnNewConversation?.Invoke(conversationData);
        GameManager.Instance.ActionsController.Ponder.Use();
        GameManager.Instance.ActionsController.Ponder.StartCooldown(ponderingCooldown);
        lastSuccessfullPonder = Time.time;
    }

    private void UpdateFrameScale()
    {
        pictureFrame.transform.localScale = Vector3.one * frameScale;
    }

    private void Zoom()
    {
        bool zoomIn = Input.GetKey(KeyCode.Z);
        bool zoomOut = Input.GetKey(KeyCode.X);

        zoomText.text = (frameScale / 3).ToString("#.0") + "x";

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
    private Vector2 Wobble(Vector2 originalPosition)
    {
        float wobbleAmount = Mathf.PingPong(Time.time * minWobbleAmount, maxWobbleAmount);

        wobbleAmount *= (frameScale / 3); // Tengo en cuenta el zoom

        float wobbleX = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
        float wobbleY = Mathf.Cos(Time.time * (wobbleSpeed * 1.2f)) * wobbleAmount; // Frecuencia distinta para variar

        return originalPosition += new Vector2(wobbleX, wobbleY);
    }
    private void LookAround()
    {
        Vector2 displacement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * movementSpeed * Time.deltaTime;
        Vector2 newPosition = pictureFrame.rectTransform.anchoredPosition - displacement;

        Vector2 imageSize = pictureFrame.rectTransform.sizeDelta;
        Vector2 maskSize = mask.rectTransform.rect.size;

        imageSize.x *= pictureFrame.rectTransform.localScale.x;
        imageSize.y *= pictureFrame.rectTransform.localScale.y;

        newPosition = Wobble(newPosition);

        Vector2 min = -(imageSize - maskSize);
        Vector2 max = new (0, 0);

        newPosition.x = Mathf.Clamp(newPosition.x, min.x, max.x);
        newPosition.y = Mathf.Clamp(newPosition.y, min.y, max.y);

        pictureFrame.rectTransform.anchoredPosition = newPosition;
    }

    private void ValidatePicture()
    {
        int count = 0;
        foreach (var pointOfInterest in currentPointsOfInterest)
        {
            
            // TODO: Esta logica hay que revisarla mas adelante.
            if(count > 1) // Too many pois.
            {
                lastTakenScreenshotData = conversationManager.conversation.POI_Too_Many;
                isLastTakenScreenshotValid = false;
                return;
            }

            RectTransform POIRectTransform = pointOfInterest.GetComponent<RectTransform>();
            if (IsOverlapping(POIRectTransform, limitArea))
            {
                count++;

                if (!IsContained(limitArea, POIRectTransform)) // POI not inside the accepted limit.
                {
                    lastTakenScreenshotData = conversationManager.conversation.POI_Out_Of_Bounds;
                    isLastTakenScreenshotValid = false;
                    return;
                }

                Vector3 minAreaOriginalPosition = minArea.position;
                minArea.position = POIRectTransform.position;

                bool isPOITooSmall = IsContained(minArea, POIRectTransform);

                minArea.position = minAreaOriginalPosition;

                if (isPOITooSmall) // POI is smaller than the minimum 
                {
                    lastTakenScreenshotData = conversationManager.conversation.POI_Too_Small;
                    isLastTakenScreenshotValid = false;
                    return;
                }

                Vector3 maxAreaOriginalPosition = maxArea.position;
                maxArea.position = POIRectTransform.position;

                bool isPOITooBig = !IsContained(maxArea, POIRectTransform);

                maxArea.position = maxAreaOriginalPosition;

                if (isPOITooBig) // POI is bigger than the maximum 
                {
                    lastTakenScreenshotData = conversationManager.conversation.POI_Too_Big;
                    isLastTakenScreenshotValid = false;
                    return;
                }


                // Valid Picture.
                lastTakenScreenshotData = pointOfInterest.PictureData;
                isLastTakenScreenshotValid = true;
                return;
            }
        }

        lastTakenScreenshotData = conversationManager.conversation.Nothing_Interesting;
        isLastTakenScreenshotValid = false;
    }

    private void TakePolaroidScreenshot()
    {
        polaroidController.Title = GameManager.Instance.PerspectiveScreen.GetCurrentLocation.LocationName;
        polaroidController.Subtitle = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        cameraDisplayOverlay.SetActive(false);
        zoomText.gameObject.SetActive(false);

        Texture2D miniature = TakeScreenshot(Camera.main, (int)mask.rectTransform.rect.width, (int)mask.rectTransform.rect.height, 2, true);
        
        cameraDisplayOverlay.SetActive(true);
        zoomText.gameObject.SetActive(true);
        
        polaroidController.PictureFrame.texture = miniature;
        lastTakenScreenshot = TakeScreenshot(polaroidController.PolaroidCamera, (int)polaroidController.Size.x, (int)polaroidController.Size.y,2 ,false);

        StartCoroutine(Utilities.ShutterEffect(shutterPanel, 0.5f, 1, 0));

        ValidatePicture();

        AudioManager.Instance.PlaySound(SoundList.Shutter);
    }
    private Texture2D TakeScreenshot(Camera camera, int width, int height, int scale = 2, bool crop=true)
    {
        if (!Directory.Exists(folderPath))
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

        camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        return screenshot;
    }

    public void SaveLastTakenPicture()
    {
        byte[] bytes = lastTakenScreenshot.EncodeToPNG();
        string screenshotName = $"{Directory.GetCurrentDirectory()}/{folderPath}/Screenshot_ {DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.png";
        File.WriteAllBytes(screenshotName, bytes);

        Debug.Log($"Screenshot guardado en: {screenshotName}");
    }

    public void CopyPointsOfInterestToFrame()
    {
        Location currentLocation = GameManager.Instance.PerspectiveScreen.GetCurrentLocation;
        PointOfInterest[] pointOfInterestList = currentLocation.PointOfInterestList;

        foreach (var pointOfInterest in pointOfInterestList)
        {
            PointOfInterest newPointOfInterest = Instantiate(pointOfInterest);

            currentPointsOfInterest.Add(newPointOfInterest);

            newPointOfInterest.transform.SetParent(pictureFrame.transform, false);

            RectTransform currentLocationRectTransform = currentLocation.GetComponent<RectTransform>();
            RectTransform poiRectTransform = pointOfInterest.GetComponent<RectTransform>();
            RectTransform newPoiRectTransform = newPointOfInterest.GetComponent<RectTransform>();
            
            // Obtener el factor de escala entre los contenedores
            Vector2 scaleFactor = new Vector2(
                pictureFrame.rectTransform.rect.width / currentLocationRectTransform.rect.width,
                pictureFrame.rectTransform.rect.height / currentLocationRectTransform.rect.height
            );

            // Ajustar la posición relativa
            Vector2 relativePos = poiRectTransform.anchoredPosition;
            newPoiRectTransform.anchoredPosition = new Vector2(relativePos.x * scaleFactor.x, relativePos.y * scaleFactor.y);

            // Ajustar la escala relativa
            newPoiRectTransform.localScale = new Vector3(
                newPoiRectTransform.localScale.x * scaleFactor.x,
                newPoiRectTransform.localScale.y * scaleFactor.y,
                1f
            );
        }
    }

    private void DeletePointsOfInterestFromFrame()
    {       
        for ( int i = 0; i < currentPointsOfInterest.Count; i++)
        {
            Destroy(currentPointsOfInterest[i].gameObject);
        }

        currentPointsOfInterest.Clear();
    }
}

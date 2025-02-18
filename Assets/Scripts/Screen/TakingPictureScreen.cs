using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Metadata;

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

    readonly string folderPath = "Screenshots";

    private PolaroidController polaroidController;

    // The copied temporal inspectables.
    // This is populated when entering to this screen and
    // wiped out when leaving.
    private List<Inspectable> currentInspectables;

    private float lastSuccessfullPonder;

    private Texture2D lastTakenScreenshot;

    public override bool IsOverlay => false;
    public override void Init()
    {
        UpdateFrameScale();
        polaroidController = GameManager.Instance.PolaroidController;
        polaroidController.PictureFrame.color = Color.white;
        currentInspectables = new List<Inspectable>();
        lastSuccessfullPonder = -1;
    }

    public override void OnEnter(bool resetState)
    {
        base.OnEnter(resetState);
        pictureFrame.sprite = GameManager.Instance.PerspectiveScreen.GetCurrentLocation.GetPerspective();

        CopyInspectablesToFrame();

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

        DeleteInspectablesFromFrame();
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

        foreach (var inspectable in currentInspectables)
        {
            if(IsOverlapping(inspectable.GetComponent<RectTransform>(), PonderingPoint))
            {
                UsePonderAction(inspectable.ConversationTrigger.conversation);
                break;
            }
        }

        UsePonderAction(Conversations.Nothing_Interesting);
    }

    private bool IsOverlapping(RectTransform rectA, RectTransform rectB)
    {        
        Rect rect1 = GetCanvasRect(rectA);
        Rect rect2 = GetCanvasRect(rectB);
        return rect1.Overlaps(rect2);
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

    private void UsePonderAction(Conversations conversation)
    {
        MessagesController.OnNewConversation?.Invoke(conversation);
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

    public void CopyInspectablesToFrame()
    {
        Location currentLocation = GameManager.Instance.PerspectiveScreen.GetCurrentLocation;
        Inspectable[] inspectables = currentLocation.Inspectables;

        foreach (var inspectable in inspectables)
        {
            Inspectable newInspectable = Instantiate(inspectable);

            currentInspectables.Add(newInspectable);

            newInspectable.transform.SetParent(pictureFrame.transform, false);

            RectTransform currentLocationRectTransform = currentLocation.GetComponent<RectTransform>();
            RectTransform inspectableRectTransform = inspectable.GetComponent<RectTransform>();
            RectTransform newInspectableRectTransform = newInspectable.GetComponent<RectTransform>();
            
            // Obtener el factor de escala entre los contenedores
            Vector2 scaleFactor = new Vector2(
                pictureFrame.rectTransform.rect.width / currentLocationRectTransform.rect.width,
                pictureFrame.rectTransform.rect.height / currentLocationRectTransform.rect.height
            );

            // Ajustar la posición relativa
            Vector2 relativePos = inspectableRectTransform.anchoredPosition;
            newInspectableRectTransform.anchoredPosition = new Vector2(relativePos.x * scaleFactor.x, relativePos.y * scaleFactor.y);

            // Ajustar la escala relativa
            newInspectableRectTransform.localScale = new Vector3(
                newInspectableRectTransform.localScale.x * scaleFactor.x,
                newInspectableRectTransform.localScale.y * scaleFactor.y,
                1f
            );
        }
    }

    private void DeleteInspectablesFromFrame()
    {       
        for ( int i = 0; i < currentInspectables.Count; i++)
        {
            Destroy(currentInspectables[i]);
        }

        currentInspectables.Clear();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PerspectiveScreen : UIScreen
{
    [Header("Location")]
    [SerializeField] Location initialLocation;
    [SerializeField] TextMeshProUGUI locationNameText;
    [SerializeField] DirectionsHUD directionsHUD;

    [Header("Minimap")]
    [SerializeField] MiniMap miniMap;

    [Header("Fade In/Out")]
    [SerializeField] float fadeInDuration;
    [SerializeField] float fadeOutDuration;

    public Location GetCurrentLocation {  get => currentLocation;  }

    public override bool IsOverlay => false;

    CanvasGroup canvasGroup;
    bool isFading;

    Location currentLocation;

    readonly KeyCode[] directionKeys = { KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow };
    public override void Init()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        currentLocation = initialLocation;
        canvasGroup.alpha = 1f;
        isFading = false;
        UpdateCurrentLocation(currentLocation);
    }
    public override void GetInput()
    {
        if (isFading)
            return;

        foreach (KeyCode key in directionKeys)
        {
            if (Input.GetKeyDown(key))
            {
                GoToLocation(key);
                break;
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            NextScreen = Screens.TakingPicture;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            NextScreen = Screens.Messages;
        }
    }

    private void GoToLocation(KeyCode keyCode)
    {
        Location newLocation = currentLocation.GetLocation(keyCode);
        if (newLocation == null)
            return;

        StartCoroutine(FadeSequence(newLocation));
    }

    private void UpdateCurrentLocation(Location newLocation)
    {
        if(currentLocation) currentLocation.gameObject.SetActive(false);
        currentLocation = newLocation;
        currentLocation.gameObject.SetActive(true);
        locationNameText.text = newLocation.LocationName;
        directionsHUD.SetDirections(newLocation.GetAvailableDirections());
        miniMap.UpdateCells();
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
    private IEnumerator FadeSequence(Location newLocation)
    {
        isFading = true;
        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));

        UpdateCurrentLocation(newLocation);

        yield return StartCoroutine(Fade(0f, 1f, fadeOutDuration));

        isFading = false;
    }
}

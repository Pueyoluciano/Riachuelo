using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Image Perspective;

    [Header("Perspective")]
    [SerializeField] LocationData locationData;

    [Header("Fade In/Out")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeInDuration;
    [SerializeField] float fadeOutDuration;

    int currentPerspective;
    bool isFading;

    private void Start()
    {
        currentPerspective = 0;
        canvasGroup.alpha = 1f;
        isFading = false;
        UpdatePerspective();
    }

    private void Update()
    {
        if (!isFading && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousPerspective();
        }

        if (!isFading && Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextPerspective();
        }
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
        Perspective.sprite = locationData.perspectivesList[currentPerspective];
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

        canvasGroup.alpha = endAlpha; // Asegurarse de que llega al valor final
    }

    private IEnumerator FadeSequence()
    {
        isFading = true;
        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));

        UpdatePerspective();

        //yield return new WaitForSeconds(delayBetweenFades);
        yield return StartCoroutine(Fade(0f, 1f, fadeOutDuration));

        isFading = false;
    }
}

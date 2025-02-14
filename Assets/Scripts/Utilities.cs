using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Utilities
{
    public static IEnumerator ShutterEffect(Image image, float duration, float initialAlpha, float endAlpha)
    {
        float elapsed = 0f;

        image.color = new(image.color.r, image.color.g, image.color.b, initialAlpha);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            image.color = new(image.color.r, image.color.g, image.color.b, Mathf.Lerp(initialAlpha, endAlpha, elapsed / duration));
            yield return null;
        }

        image.color = new(image.color.r, image.color.g, image.color.b, endAlpha);
    }
}

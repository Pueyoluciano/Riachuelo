using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionItem : MonoBehaviour
{
    [SerializeField] Image cooldownMask;
    [SerializeField] Image loadImage;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        cooldownMask.gameObject.SetActive(false);
        loadImage.sprite = image.sprite;
    }

    public void Use()
    {
        StartCoroutine(Utilities.ShutterEffect(image, 0.5f, 0.2f, 1f));
    }

    public void StartCooldown(float time)
    {
        StartCoroutine(Cooldown(time));
    }

    private IEnumerator Cooldown(float time)
    {
        cooldownMask.gameObject.SetActive(true);

        var initialySize = cooldownMask.rectTransform.sizeDelta.y;

        float elapsed = 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float ySize = Mathf.Lerp(initialySize, 0, elapsed / time);
            cooldownMask.rectTransform.sizeDelta = new Vector2(cooldownMask.rectTransform.sizeDelta.x, ySize);
            yield return null;
        }

        cooldownMask.rectTransform.sizeDelta = new Vector2(cooldownMask.rectTransform.sizeDelta.x, initialySize);

        cooldownMask.gameObject.SetActive(false);
    }
}

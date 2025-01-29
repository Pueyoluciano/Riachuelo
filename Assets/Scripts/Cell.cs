using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    private Image image;
    [SerializeField] public MiniMap.CellType cellType;

    private MiniMap miniMapReference;

    public enum Rotation { Zero = 0, Ninenty = 90, OneEighty = 180, ThoHandredSeventy = 270 }

    [SerializeField] Rotation rotation;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetSpriteForType(Sprite newSprite)
    {
        if (image == null)
            return;

        image.sprite = newSprite;

        if (newSprite == null)
            image.color = new Color(0, 0, 0, 0);
        else
            image.color = Color.white;
    }

    private void UpdateCell()
    {
        if (miniMapReference == null)
            miniMapReference = GetComponentInParent<MiniMap>();

        if(image == null)
            image = GetComponent<Image>();

        SetSpriteForType(miniMapReference.GetSpriteForType(cellType));
        transform.rotation = Quaternion.Euler(0, 0, (int)rotation);
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Evitar ejecutar en el prefab
        if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
            return;

        UpdateCell();
    }
#endif
}

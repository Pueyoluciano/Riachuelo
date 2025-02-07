using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField] public MiniMap.CellType cellType;
    [SerializeField] public Location location;
    [SerializeField] public Rotation rotation;

    private Image image;
    private MiniMap miniMapReference;
    bool isVisited;

    public enum Rotation { Zero = 0, Ninenty = 90, OneEighty = 180, ThoHandredSeventy = 270 }
    public bool IsLocation 
    { 
        get =>
            cellType == MiniMap.CellType.Location_Active ||
            cellType == MiniMap.CellType.Location_Not_Visited ||
            cellType == MiniMap.CellType.Location_Visited;
    }
    public bool IsVisited { get => isVisited; }

    private void Awake()
    {
        image = GetComponent<Image>();
        isVisited = false;
    }

    private void SetSpriteForType(Sprite newSprite)
    {
        if (image == null)
            return;

        image.sprite = newSprite;

        if (newSprite == null)
            image.color = new Color(0, 0, 0, 0);
        else
            image.color = Color.white;
    }

    public void UpdateCell()
    {
        if (miniMapReference == null)
            miniMapReference = GetComponentInParent<MiniMap>();

        if(image == null)
            image = GetComponent<Image>();

        SetSpriteForType(miniMapReference.GetSpriteForType(cellType));
        transform.rotation = Quaternion.Euler(0, 0, (int)rotation);
    }

    public void SetCell(MiniMap.CellType type, Rotation rotation = Rotation.Zero)
    {
        cellType = type;
        image.sprite = miniMapReference.GetSpriteForType(cellType);
        this.rotation = rotation;

        isVisited = isVisited || type == MiniMap.CellType.Location_Active;
        UpdateCell();
    }
}

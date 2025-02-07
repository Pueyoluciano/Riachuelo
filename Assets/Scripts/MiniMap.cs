using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [Serializable]
    public struct SpriteMapping
    {
        public CellType type;
        public Sprite sprite;

        public SpriteMapping(CellType type, Sprite sprite)
        {
            this.type = type;
            this.sprite = sprite;
        }
    }
    public enum CellType
    {
        Empty,
        Line_Straight,
        Line_Square,
        Line_T,
        Line_Cross,
        Location_Not_Visited,
        Location_Visited,
        Location_Active
    }

    [SerializeField] public List<SpriteMapping> spriteMappings;

    public static MiniMap Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void UpdateCells()
    {
        foreach (var cell in GetComponentsInChildren<Cell>())
        {
            if (cell.IsLocation)
            {
                if (cell.location == null)
                {
                    throw new Exception("La celda de tipo Location no puede tener su valor location en null");
                }
                if (cell.location == GameManager.Instance.PerspectiveScreen.GetCurrentLocation)
                {
                    cell.SetCell(CellType.Location_Active);
                }
                else if(cell.IsVisited)
                {
                    cell.SetCell(CellType.Location_Visited);
                } else
                {
                    cell.SetCell(CellType.Location_Not_Visited);
                }
            }
        }
    }

    public Sprite GetSpriteForType(CellType type)
    {
        foreach (var mapping in spriteMappings)
        {
            if (mapping.type == type)
            {
                return mapping.sprite;
            }
        }
        return null;
    }

    public void OnValidate()
    {
        var CellChildren = GetComponentsInChildren<Cell>();
        foreach (var child in CellChildren)
        {
            // child.SetSpriteForType(GetSpriteForType(child.cellType));
            child.UpdateCell();
        }
    }
}

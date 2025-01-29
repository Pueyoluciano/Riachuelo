using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    /*public enum prueba
    {
        Empty,
        StraightLine,
        SquareLine,
        Location_Unavailable,
        Location_Available,
        Location_Active
    }

    [SerializeField] public prueba asd;
    */
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
        Location_Unavailable,
        Location_Available,
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
            child.SetSpriteForType(GetSpriteForType(child.cellType));
        }
    }

    /*
    // Campos condicionales
    [SerializeField] public int valorA; // Solo para TipoA
    [SerializeField] public float valorB; // Solo para TipoB
    [SerializeField] public string textoC; // Solo para TipoC
    */
}

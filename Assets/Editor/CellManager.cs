using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Cell;

[CustomEditor(typeof(Cell))]
public class CellManager : Editor
{
    public override void OnInspectorGUI()
    {
        Cell manager = (Cell)target;

        // Mandatory Fields
        manager.cellType = (MiniMap.CellType)EditorGUILayout.EnumPopup("Tipo", manager.cellType);
        manager.rotation = (Rotation)EditorGUILayout.EnumPopup("Rotacion", manager.rotation);

        // Conditional fields
        if (manager.IsLocation)
        
            manager.location = (Location)EditorGUILayout.ObjectField("Location", manager.location, typeof(Location), true);

        if (GUI.changed)
        {
            // Evitar ejecutar en el prefab
            if (!PrefabUtility.IsPartOfPrefabAsset(this))
                manager.UpdateCell();

            EditorUtility.SetDirty(manager);
        }
    }
}

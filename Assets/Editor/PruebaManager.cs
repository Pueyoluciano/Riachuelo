using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


[CustomEditor(typeof(MiniMap))]
public class PruebaManager : Editor
{
    /*public override void OnInspectorGUI()
    {
        // Obtén la referencia al objeto objetivo
        MiniMap manager = (MiniMap)target;

        // Dibuja el campo del Enum
        manager.asd = (MiniMap.prueba)EditorGUILayout.EnumPopup("Tipo", manager.asd);

        // Muestra los campos condicionalmente
        switch (manager.asd)
        {
            case MiniMap.prueba.Empty:
                manager.valorA = EditorGUILayout.IntField("Valor A", manager.valorA);
                break;
            case MiniMap.prueba.StraightLine:
                manager.valorB = EditorGUILayout.FloatField("Valor B", manager.valorB);
                break;
            case MiniMap.prueba.SquareLine:
                manager.textoC = EditorGUILayout.TextField("Texto C", manager.textoC);
                break;
        }

        // Guarda los cambios realizados
        if (GUI.changed)
        {
            EditorUtility.SetDirty(manager);
        }
    }*/
}
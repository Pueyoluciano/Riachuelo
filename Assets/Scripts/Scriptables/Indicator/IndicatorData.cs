using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewIndicatorData", menuName = "ScriptableObjects/Indicator/IndicatorData", order = 1)]
public class IndicatorData : ScriptableObject
{
    public float rotationSpeed;
    public Vector3 localScale;
    public Color color;
    public Sprite sprite;

    public void Copy(IndicatorData other)
    {
        rotationSpeed = other.rotationSpeed;
        localScale = other.localScale;
        color = other.color;
        sprite = other.sprite;
    }
}

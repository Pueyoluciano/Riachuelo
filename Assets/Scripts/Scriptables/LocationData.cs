using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLocationData", menuName = "ScriptableObjects/LocationData", order = 1)]
public class LocationData : ScriptableObject
{
    public List<Sprite> perspectivesList;
}
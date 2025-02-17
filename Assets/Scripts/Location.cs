using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Location : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] string locationName;

    [Header("Next Locations")]
    [SerializeField] Location frontLocation;
    [SerializeField] Location previousLocation;
    [SerializeField] Location leftLocation;
    [SerializeField] Location rightLocation;

    Image perspective;
    Dictionary<KeyCode, Location> locationByKeyCode;

    private Inspectable[] inspectables;
    public string LocationName { get => locationName; }
    public Inspectable[] Inspectables { get => inspectables; }

    private void Awake()
    {
        perspective = GetComponent<Image>();
        locationByKeyCode = new()
        {
            { KeyCode.UpArrow, frontLocation},
            { KeyCode.DownArrow, previousLocation},
            { KeyCode.LeftArrow, leftLocation},
            { KeyCode.RightArrow, rightLocation}
        };

        inspectables = gameObject.GetComponentsInChildren<Inspectable>();
    }

    public Location GetLocation(KeyCode keyCode)
    {
        return locationByKeyCode[keyCode];
    }

    public Sprite GetPerspective()
    {
        return perspective.sprite;
    }

    public (bool up, bool down, bool left, bool right) GetAvailableDirections()
    {
        return 
        (
            locationByKeyCode[KeyCode.UpArrow] != null, 
            locationByKeyCode[KeyCode.DownArrow] != null,
            locationByKeyCode[KeyCode.LeftArrow] != null,
            locationByKeyCode[KeyCode.RightArrow] != null
        );
    }
}

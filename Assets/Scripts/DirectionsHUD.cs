using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectionsHUD : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] Sprite on;
    [SerializeField] Sprite off;

    [Header("Directions")]
    [SerializeField] Image up;
    [SerializeField] Image down;
    [SerializeField] Image left;
    [SerializeField] Image right;

    public void SetDirections((bool up, bool down, bool left, bool right) availableDirections)
    {
       this.up.sprite = availableDirections.up ? on : off;
       this.down.sprite = availableDirections.down ? on : off;
       this.left.sprite = availableDirections.left ? on : off;
       this.right.sprite = availableDirections.right ? on : off;
    }
}

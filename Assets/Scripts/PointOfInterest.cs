using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointOfInterest : MonoBehaviour
{
    [Header("Ponder")]
    [SerializeField] ConversationData ponderData;

    [Header("Picture")]
    [SerializeField] ConversationData pictureData;
    public ConversationData PonderData { get => ponderData; }
    public ConversationData PictureData { get => pictureData; }

    private void Start()
    {
        // Only show inspectables in editor Mode
        gameObject.GetComponent<Image>().enabled = GameManager.Instance.visualDebugEnabled;
    }
}

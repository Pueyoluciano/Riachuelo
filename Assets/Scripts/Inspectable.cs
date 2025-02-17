using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inspectable : MonoBehaviour
{
    private ConversationTrigger conversationTrigger;

    public ConversationTrigger ConversationTrigger { get => conversationTrigger;}

    private void Awake()
    {
        // Only show inspectables in editor Mode
        gameObject.GetComponent<Image>().enabled = false;
        conversationTrigger = GetComponent<ConversationTrigger>();
    }
}

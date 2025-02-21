using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MiniMap;

[CreateAssetMenu(fileName = "NewConversationData", menuName = "ScriptableObjects/ConversationData", order = 2)]
public class ConversationData : ScriptableObject
{
    [Serializable]
    public struct Message
    {
        public string name;
        public Color nameColor;

        [TextArea(3, 10)]
        public string text;

        public Message(string name, Color nameColor, string text)
        {
            this.name = name;
            this.nameColor = nameColor;
            this.text = text;
        }
    }
    public List<Message> messagesList;
}
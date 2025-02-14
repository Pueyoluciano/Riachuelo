using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ConversationManager : MonoBehaviour
{
    [Serializable]
    public struct Chat
    {
        public Conversations conversationID;
        public ConversationData conversationData;

        public Chat(Conversations conversationID, ConversationData conversationData)
        {
            this.conversationID = conversationID;
            this.conversationData = conversationData;
        }
    }

    [SerializeField] List<Chat> conversationList;

    Dictionary<Conversations, ConversationData> conversationDict;

    public Dictionary<Conversations, ConversationData> ConversationDict { get => conversationDict; }

    public ConversationData GetConversationData(Conversations conversationID)
    {
        return conversationDict[conversationID]; 
    }

    private Chat GetOrCreateChat(List<Chat> list, Conversations conversationID)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].conversationID == conversationID && list[i].conversationData != null)
                return list[i];
        }

        return new Chat(conversationID, null);
    }

    public void OnValidate()
    {
        List<Chat> newConversationList = new();
        Dictionary<Conversations, ConversationData> newConversationDict = new();

        foreach (Conversations conversationID in Enum.GetValues(typeof(Conversations)))
        {
            var chat = GetOrCreateChat(conversationList, conversationID);
            newConversationList.Add(chat);
            newConversationDict.Add(chat.conversationID, chat.conversationData);
        }

        conversationList = newConversationList;
        conversationDict = newConversationDict;
    }
}

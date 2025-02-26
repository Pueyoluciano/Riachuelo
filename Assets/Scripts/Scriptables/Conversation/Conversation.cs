using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewConversationDatabase", menuName = "ScriptableObjects/Conversation/ConversationDatabase", order = 1)]
public class Conversation : ScriptableObject
{
    [Header("Picture")]
    public ConversationData Nothing_Interesting;
    public ConversationData Bad_Picture;
    public ConversationData Repited_Picture;

    [Header("Point Of Interest")]
    public ConversationData POI_Too_Many;
    public ConversationData POI_Out_Of_Bounds;
    public ConversationData POI_Too_Small;
    public ConversationData POI_Too_Big;
}

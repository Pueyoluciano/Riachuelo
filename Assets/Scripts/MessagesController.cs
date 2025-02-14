using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MessagesController : MonoBehaviour
{
    [Header("Fields")]
    [SerializeField] TextMeshProUGUI message;

    [Header("Type Writer")]
    [SerializeField] float typingSpeed;
    [SerializeField, Tooltip("Waiting time after pressing an enter to accept another one.")] float waitingTime;

    [Header("Message Example")]
    [SerializeField] ConversationData exampleConversationData;

    private bool isTyping;
    private bool finishPhrase;
    private float finishPhraseElapsedTime;
    private float blinkingElapsedTime;
    private float blinkingTime = 0.5f;

    private bool blinking;
    private string blinkingText = " _";

    private bool startedConversation;
    private bool finishedMessage;
    public bool IsTyping { get => isTyping; }

    private bool CanPressEnterAgain { get => Time.time - finishPhraseElapsedTime >= waitingTime; }
    private bool ShoudUpdateBlink { get => Time.time - blinkingElapsedTime >= blinkingTime; }
    public bool StartedConversation { get => startedConversation; }

    public static Action<Conversations> OnNewConversation;

    private void Awake()
    {

        OnNewConversation += OnNewConversationHandler;
    }

    private void OnDestroy()
    {
        OnNewConversation -= OnNewConversationHandler;
    }
    private void Start()
    {
        ResetConversation();
    }

    private void Update()
    {
        GetInput();
        UpdateBlinking();
    }

    private void ResetConversation()
    {
        CleanViewport();
        isTyping = false;
        finishPhrase = false;
        blinking = true;
        startedConversation = false;
        finishedMessage = false;
    }

    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) && CanPressEnterAgain)
        {
            if (isTyping)
            {
                finishPhrase = true;
                finishPhraseElapsedTime = Time.time;
            }
            else
            {
                isTyping = true;
            }
        }
    }

    private void CleanViewport()
    {
        message.text = "";
    }
    private void UpdateBlinking()
    {
        if (startedConversation && finishedMessage && !isTyping && ShoudUpdateBlink)
        {
            if (blinking)
                message.text += blinkingText;
            else
                message.text = message.text.Substring(0, message.text.Length - blinkingText.Length);

            blinking = !blinking;
            blinkingElapsedTime = Time.time;
        }
    }

    private void OnNewConversationHandler(Conversations conversationID)
    {
        if (startedConversation)
            return;

        GameManager.Instance.ActionsController.Ponder.Use();
        GameManager.Instance.ActionsController.Ponder.StartCooldown(15f); 

        ResetConversation();
        StartCoroutine(TypeText(GameManager.Instance.ConversationManger.GetConversationData(conversationID)));
    }

    private string coloredString(Color color, string name)
    {
        return $"<color=#{color.ToHexString()}>{name}</color>";
    }
    private IEnumerator TypeText(ConversationData conversation)
    {
        startedConversation = true;

        foreach (var convMessage in conversation.messagesList)
        {
            message.text = coloredString(convMessage.nameColor, convMessage.name) + ": ";
            string fullText = "";

            fullText += "\"";
            fullText += convMessage.text;
            fullText += "\"";

            isTyping = true;
            finishedMessage = false;

            foreach (char letter in fullText)
            {
                if (finishPhrase)
                {
                    finishPhrase = false;
                    message.text = coloredString(convMessage.nameColor, convMessage.name) + ": " + fullText;
                    break;
                }
                message.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            // Cuando termino un mensaje actualizo el tiempo de espera.
            // para que tenga que esperar unos momentos antes de pasar al siguiente texto.
            finishPhraseElapsedTime = Time.time;

            isTyping = false;
            blinking = true;
            finishedMessage = true;

            while (!isTyping)
                yield return null;
        }

        startedConversation = false;
        CleanViewport();
    }
}

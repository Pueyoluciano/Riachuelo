using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessagesController : MonoBehaviour
{
    [Header("Fields")]
    [SerializeField] TextMeshProUGUI title;
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

    private bool finishedConversation;
    private bool finishedMessage;
    public bool IsTyping { get => isTyping; }

    private bool CanPressEnterAgain { get => Time.time - finishPhraseElapsedTime >= waitingTime; }
    private bool ShoudUpdateBlink { get => Time.time - blinkingElapsedTime >= blinkingTime; }
    public bool FinishedConversation { get => finishedConversation; }

    public static Action OnNewConversation;

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
        finishedConversation = false;
        finishedMessage = false;
    }

    private void GetInput()
    {
        // TODO: codigo temporal para probar los mensajes
        if (Input.GetKeyDown(KeyCode.M))
        {
            OnNewConversation?.Invoke();
        }

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
        title.text = "";
        message.text = "";
    }
    private void UpdateBlinking()
    {
        if (!finishedConversation && finishedMessage && !isTyping && ShoudUpdateBlink)
        {
            if (blinking)
                message.text += blinkingText;
            else
                message.text = message.text.Substring(0, message.text.Length - blinkingText.Length);

            blinking = !blinking;
            blinkingElapsedTime = Time.time;
        }
    }

    private void OnNewConversationHandler()
    {
        ResetConversation();
        StartCoroutine(TypeText(exampleConversationData));
    }
    private IEnumerator TypeText(ConversationData conversation)
    {
        foreach (var convMessage in conversation.messagesList)
        {
            title.text = convMessage.name;
            title.color = convMessage.nameColor;
            message.text = "";
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
                    message.text = fullText;
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

        finishedConversation = true;
        CleanViewport();
    }
}

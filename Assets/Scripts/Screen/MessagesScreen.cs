using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessagesScreen : UIScreen
{
    [Header("Fields")]
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI message;

    [Header("Type Writer")]
    [SerializeField] float typingSpeed;
    [SerializeField, Tooltip("Waiting time after pressing an enter to accept another one.")] float waitingTime;

    [Header("Message Example")]
    [SerializeField] ConversationData exampleConversationData;

    public static Action ShowConversation;

    private bool isTyping;
    private bool finishPhrase;
    private float finishPhraseElapsedTime;
    private float blinkingElapsedTime;
    private float blinkingTime = 0.5f;

    private bool blinking;
    private string blinkingText = " _";

    private bool finishedConversation;
    public bool IsTyping { get => isTyping; }

    private bool CanPressEnterAgain { get => Time.time - finishPhraseElapsedTime >= waitingTime; }
    private bool ShoudUpdateBlink { get => Time.time - blinkingElapsedTime >= blinkingTime; }
    protected override void Awake()
    {
        base.Awake();

        ShowConversation += ShowConversationHandler;
    }

    private void OnDestroy()
    {
        ShowConversation -= ShowConversationHandler;
    }

    public override void Init()
    {

    }

    public override void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) && CanPressEnterAgain)
        {
            if (isTyping)
            {
                finishPhrase = true;
                finishPhraseElapsedTime = Time.time;
            } else
            {
                isTyping = true;
            }
        }

        if (finishedConversation)
            // TODO: Agregar un checkeo adicional para ocultar o no la previous Screen.
            NextScreen = Screens.PreviousScreen;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        title.text = "";
        message.text = "";
        isTyping = false;
        finishPhrase = false;
        blinking = true;
        finishedConversation = false;

        StartCoroutine(TypeText(exampleConversationData));
    }
    private void ShowConversationHandler()
    {

    }

    public override Screens Execute()
    {
        if (!isTyping)
            UpdateBlinking();

        return base.Execute();
    }

    private void UpdateBlinking()
    {
        if (ShoudUpdateBlink)
        {

            if (blinking)
                message.text += blinkingText;
            else
                message.text = message.text.Substring(0, message.text.Length - blinkingText.Length);
            
            blinking = !blinking;
            blinkingElapsedTime = Time.time;
        }
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

            while(!isTyping)
                yield return null;
        }

        finishedConversation = true;
    }
}

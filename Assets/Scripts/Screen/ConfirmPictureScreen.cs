using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmPictureScreen : UIScreen
{
    [Header("Polariod")]
    [SerializeField] Polaroid polaroid;

    public override bool IsOverlay => true;

    private AudioManager audioManager;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SavePicture();
            NextScreen = Screens.PreviousScreen;
        }

        if (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown(KeyCode.Backspace))
        {
            DiscardPicture();
            NextScreen = Screens.PreviousScreen;
        }
    }

    public override void Init()
    {
        audioManager = GameManager.Instance.AudioManager;
    }

    public override void OnEnter(bool resetState)
    {
        base.OnEnter(resetState);

        polaroid.PictureFrame.texture = GameManager.Instance.PolaroidController.PictureFrame.texture;
        polaroid.Title.text = GameManager.Instance.PolaroidController.Title;
        polaroid.Subtitle.text = GameManager.Instance.PolaroidController.Subtitle;

        MessagesController.OnNewConversation?.Invoke(GameManager.Instance.TakingPictureScreen.LastTakenScreenshotData);
    }

    private void SavePicture() 
    {
        GameManager.Instance.TakingPictureScreen.SaveLastTakenPicture();
        audioManager.PlaySound(audioManager.audios.SavePicture);
    }

    private void DiscardPicture()
    {
        audioManager.PlaySound(audioManager.audios.DiscardPicture);
    }
}

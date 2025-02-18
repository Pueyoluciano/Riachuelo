using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmPictureScreen : UIScreen
{
    [Header("Polariod")]
    [SerializeField] Polaroid polaroid;

    public override bool IsOverlay => true;

    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
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
        
    }

    public override void OnEnter(bool resetState)
    {
        base.OnEnter(resetState);

        polaroid.PictureFrame.texture = GameManager.Instance.PolaroidController.PictureFrame.texture;
        polaroid.Title.text = GameManager.Instance.PolaroidController.Title;
        polaroid.Subtitle.text = GameManager.Instance.PolaroidController.Subtitle;

    }

    private void SavePicture() 
    {
        GameManager.Instance.TakingPictureScreen.SaveLastTakenPicture();
        AudioManager.Instance.PlaySound(SoundList.SavePicture);
    }

    private void DiscardPicture()
    {
        AudioManager.Instance.PlaySound(SoundList.DiscardPicture);
    }
}

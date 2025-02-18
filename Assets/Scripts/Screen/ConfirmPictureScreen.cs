using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmPictureScreen : UIScreen
{
    [Header("Polariod")]
    [SerializeField] Polaroid polaroid;
    public override bool IsOverlay => true;

    public override void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
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
}

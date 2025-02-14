using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogbookScreen : UIScreen
{
    public override bool IsOverlay => false;

    public override void GetInput()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            NextScreen = Screens.PreviousScreen;
        }
    }

    public override void Init()
    {

    }
}

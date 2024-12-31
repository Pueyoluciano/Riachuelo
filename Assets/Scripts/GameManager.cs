using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Screens")]
    [SerializeField] UIScreen perspectiveScreen;
    [SerializeField] UIScreen takingPictureScreen;

    UIScreen currentScreen;

    Dictionary<Screens, UIScreen> UIScreens;

    private void Start()
    {
        UIScreens = new()
        {
            { Screens.Perspective, perspectiveScreen },
            { Screens.TakingPicture, takingPictureScreen }
        };

        SetActiveScreen(Screens.Perspective);
    }

    private void Update()
    {
        currentScreen.Execute();
        /*switch(currentScreen)
        {
            case Screens.Perspective:
                
                break;

            case Screens.TakingPicture:

                break;
        }*/
    }

    private void SetActiveScreen(Screens nextScreen)
    {
        if(currentScreen) currentScreen.Disable();

        currentScreen = UIScreens[nextScreen];
        currentScreen.Init(); // TODO: Revisar cuando hay que llamar al init realmente
        currentScreen.Enable();
    }    
}

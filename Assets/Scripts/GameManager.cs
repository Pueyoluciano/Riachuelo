using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Pause")]
    [SerializeField] GameObject pausePanel;

    [Header("UI Screens")]
    [SerializeField] PerspectiveScreen perspectiveScreen;
    [SerializeField] TakingPictureScreen takingPictureScreen;
    [SerializeField] MessagesScreen messagesScreen;

    Screens currentScreen;
    Screens previousScreen;
    Dictionary<Screens, UIScreen> UIScreens;

    public bool IsPaused { get => isPaused; }
    bool isPaused = false;
    public static GameManager Instance { get; private set; }
    public PerspectiveScreen PerspectiveScreen { get => perspectiveScreen; }
    public TakingPictureScreen TakingPictureScreen { get => takingPictureScreen; }
    public MessagesScreen MessagesScreen { get => messagesScreen; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }
    private void Start()
    {
        isPaused = false;

        UIScreens = new()
        {
            { Screens.Perspective, perspectiveScreen },
            { Screens.TakingPicture, takingPictureScreen },
            { Screens.Messages, messagesScreen }
        };

        SetActiveScreen(Screens.Perspective);

        // TODO: corregir esto. Deberia guardar la ultima pantalla que visite.
        previousScreen = Screens.Perspective;

        foreach (var UIScreen in UIScreens.Values)
        {
            UIScreen.Init();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                UnPause();
            }
            else
            {
                Pause();
            }
        }

        if (isPaused)
            return;

        Screens nextScreen = UIScreens[currentScreen].Execute();
        if(nextScreen != Screens.NoScreen && nextScreen != currentScreen)
        {
            if(nextScreen == Screens.PreviousScreen)
                SetActiveScreen(previousScreen);
            else
                SetActiveScreen(nextScreen);
        }
    }

    private void SetActiveScreen(Screens nextScreen)
    {
        if(currentScreen!= Screens.NoScreen) UIScreens[currentScreen].OnExit();

        currentScreen = nextScreen;
        UIScreens[currentScreen].OnEnter();
    }

    public void Pause()
    {
        if (pausePanel) pausePanel.SetActive(true);
        PausedState();
    }

    public void UnPause()
    {
        if (pausePanel) pausePanel.SetActive(false);
        UnPausedState();
    }
    private void PausedState()
    {
        Time.timeScale = 0;
        isPaused = true;
    }

    private void UnPausedState()
    {
        Time.timeScale = 1;
        isPaused = false;
    }
}

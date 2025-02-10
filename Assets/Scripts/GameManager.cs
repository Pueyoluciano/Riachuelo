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

    [Header("Messages Controller")]
    [SerializeField] MessagesController messagesController;

    Screens currentScreen;
    Screens previousScreen;
    Dictionary<Screens, UIScreen> UIScreens;

    public bool IsPaused { get => isPaused; }
    bool isPaused = false;
    public static GameManager Instance { get; private set; }
    public PerspectiveScreen PerspectiveScreen { get => perspectiveScreen; }
    public TakingPictureScreen TakingPictureScreen { get => takingPictureScreen; }
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
            { Screens.TakingPicture, takingPictureScreen }
        };

        SetActiveScreen(Screens.Perspective);

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
        
        SetActiveScreen(nextScreen);        
    }

    private void SetActiveScreen(Screens nextScreen)
    {
        if (nextScreen == Screens.NoScreen || nextScreen == currentScreen)
            return;

        bool shouldResetState = true;
        if (nextScreen == Screens.PreviousScreen)
        {
            if(currentScreen != Screens.NoScreen) UIScreens[currentScreen].OnExit(false);
            currentScreen = previousScreen;
            previousScreen = default;
            shouldResetState = false;

        }
        else
        {
            if (currentScreen != Screens.NoScreen) UIScreens[currentScreen].OnExit(UIScreens[nextScreen].IsOverlay);
            previousScreen = currentScreen;
            currentScreen = nextScreen;
        }

        UIScreens[currentScreen].OnEnter(shouldResetState);
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

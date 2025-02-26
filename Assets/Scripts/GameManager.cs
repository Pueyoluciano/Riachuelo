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
    [SerializeField] ConfirmPictureScreen confirmPictureScreen;
    [SerializeField] LogbookScreen logbookScreen;
    [SerializeField] GalleryScreen galleryScreen;

    [Header("Polaroid Controller")]
    [SerializeField] PolaroidController polaroidController;

    [Header("Messages Controller")]
    [SerializeField] MessagesController messagesController;
    
    [Header("Actions Controller")]
    [SerializeField] ActionsController actionsController;

    [Header("Conversations Manager")]
    [SerializeField] ConversationManager conversationManger;

    [Header("Conversations Manager")]
    [SerializeField] AudioManager audioManager;

    [Header("Visual Debug Mode")]
    public bool visualDebugEnabled = false;

    Screens currentScreen;
    Screens previousScreen;
    Dictionary<Screens, UIScreen> UIScreens;

    public bool IsPaused { get => isPaused; }
    bool isPaused = false;
    public static GameManager Instance { get; private set; }
    public PerspectiveScreen PerspectiveScreen { get => perspectiveScreen; }
    public TakingPictureScreen TakingPictureScreen { get => takingPictureScreen; }
    public LogbookScreen LogbookScreen { get => logbookScreen; set => logbookScreen = value; }
    public GalleryScreen GalleryScreen { get => galleryScreen; set => galleryScreen = value; }
    public PolaroidController PolaroidController { get => polaroidController; }
    public ConversationManager ConversationManger { get => conversationManger; }
    public ActionsController ActionsController { get => actionsController; }
    public MessagesController MessagesController { get => messagesController; }
    public AudioManager AudioManager { get => audioManager; }
    public ConfirmPictureScreen ConfirmPictureScreen { get => confirmPictureScreen; }

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
            { Screens.ConfirmPicture, confirmPictureScreen },
            { Screens.Logbook, logbookScreen },
            { Screens.Gallery, galleryScreen },
        };

        SetActiveScreen(Screens.Perspective);

        foreach (var UIScreen in UIScreens.Values)
        {
            UIScreen.Init();
        }

        audioManager.PlaySound(audioManager.audios.LocationMusic);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIScreen : MonoBehaviour
{
    private Screens nextScreen;

    public Screens NextScreen { get => nextScreen; set => nextScreen = value; }
    public abstract bool IsOverlay { get; }

    protected virtual void Awake()
    {
        nextScreen = Screens.NoScreen;
    }
    public abstract void Init();
    public virtual void OnEnter(bool resetState)
    {
        gameObject.SetActive(true);
    }
    public virtual void OnExit(bool isNextScreenOverlay)
    {
        if(!isNextScreenOverlay)
            gameObject.SetActive(false);
        nextScreen = Screens.NoScreen;
    }

    public abstract void GetInput();
    public virtual Screens Execute()
    {
        GetInput();
        return nextScreen;
    }
}

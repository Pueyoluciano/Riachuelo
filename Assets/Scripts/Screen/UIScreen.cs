using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIScreen : MonoBehaviour
{
    private Screens nextScreen;

    public Screens NextScreen { get => nextScreen; set => nextScreen = value; }

    protected virtual void Awake()
    {
        nextScreen = Screens.NoScreen;
    }
    public abstract void Init();
    public virtual void OnEnter()
    {
        gameObject.SetActive(true);
    }
    public virtual void OnExit()
    {
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

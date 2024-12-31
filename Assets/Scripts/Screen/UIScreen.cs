using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIScreen : MonoBehaviour
{
    public abstract void Init();
    public virtual void Enable()
    {
        gameObject.SetActive(true);
    }
    public virtual void Disable()
    {
        gameObject.SetActive(false);
    }

    public abstract void GetInput();
    public virtual void Execute()
    {
        GetInput();
    }
}

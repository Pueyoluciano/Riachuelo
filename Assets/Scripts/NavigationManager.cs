using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] Scenes scenes;

    public void NewGame()
    {
        SceneManager.LoadScene(scenes.Location.SceneName);
    }
    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(scenes.MainMenu.SceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script will be used in the main menu to select scenes to load
/// by clicking a button
/// 
/// Author: Ben Hoffman
/// </summary>
public class MainMenu : MonoBehaviour {

    public Canvas aboutCanvas;

    private void Start()
    {
        aboutCanvas.gameObject.SetActive(false);
    }

    public void ToggleAboutCanvas()
    {
        aboutCanvas.gameObject.SetActive(!aboutCanvas.gameObject.activeInHierarchy);
    }

    public void OpenWebsite()
    {
        Application.OpenURL("http://ergowitness.info");
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);     
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

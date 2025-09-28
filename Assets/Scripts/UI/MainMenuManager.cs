using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;

    public void Start()
    {
        mainMenu.SetActive(true);
        GameManager.Instance.mainMenuOpen = true;
    }

    public void StartButton()
    {
        PlayerStats.Instance.ResetPlayerStats();
        LightDetector.Instance.ResetLights();
        Interact.Instance.ResetInteract();

        AudioManager.Instance.StopMusic();
        GameManager.Instance.sceneToSceneID = "mainmenu_aptjohnsapt"; // Change this
        HideMenu();
        SceneManager.LoadSceneAsync(1); // Change this
    }

    public void ContinueButton()
    {
        AudioManager.Instance.StopMusic();
        if (!File.Exists(GameManager.Instance.saveFilePath))
        {
            return;
        }
        else
        {
            HideMenu();
            GameManager.Instance.LoadButton();
        }
    }

    public void SettingsButton()
    {

    }

    public void ExitButton()
    {
        AudioManager.Instance.StopMusic();
        Application.Quit();
    }

    public void HideMenu()
    {
        GameManager.Instance.mainMenuOpen = false;
        mainMenu.SetActive(false);
    }
}

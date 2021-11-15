using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuButtons;
    public GameObject SettingsButtons;
    public GameObject controlScheme;
    public void ExitGame()
    {
        Application.Quit();
    }
    public void Settings()
    {
        mainMenuButtons.SetActive(false);
        SettingsButtons.SetActive(true);
    }
    public void Controls()
    {
        mainMenuButtons.SetActive(false);
        controlScheme.SetActive(true);
    }
    public void Menu()
    {
        mainMenuButtons.SetActive(true);
        SettingsButtons.SetActive(false);
        controlScheme.SetActive(false);
    }
    public void LevelSelect()
    {
        //needs to interact with scene manager script
    }    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : LocationManager
{
    public GameObject fileSelectMenu;
    public GameObject mainMenu;
    public GameObject levelSelectMenu;
    public GameObject optionsMenu;
    public GameObject comfortMenu;
    public GameObject soundMenu;
    public GameObject controlsMenu;
    public GameObject confirmResetMenu;

    private void Start()
    {
        if (GameManager.Instance.fileSelected)
        {
            DisplayMain();
        }
        else
        {
            DisplayFileSelect();
        }
    }

    public void DisplayMain()
    {
        mainMenu.SetActive(true);

        fileSelectMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
        optionsMenu.SetActive(false);
        comfortMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsMenu.SetActive(false);
        confirmResetMenu.SetActive(false);
    }

    public void DisplayFileSelect()
    {
        fileSelectMenu.SetActive(true);

        mainMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
        optionsMenu.SetActive(false);
        comfortMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsMenu.SetActive(false);
        confirmResetMenu.SetActive(false);
    }

    public void DisplayLevelSelect()
    {
        levelSelectMenu.SetActive(true);

        mainMenu.SetActive(false);
        fileSelectMenu.SetActive(false);
        optionsMenu.SetActive(false);
        comfortMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsMenu.SetActive(false);
        confirmResetMenu.SetActive(false);
    }

    public void DisplayOptions()
    {
        optionsMenu.SetActive(true);

        mainMenu.SetActive(false);
        fileSelectMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
        comfortMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsMenu.SetActive(false);
        confirmResetMenu.SetActive(false);
    }

    public void DisplayComfortOptions()
    {
        comfortMenu.SetActive(true);

        mainMenu.SetActive(false);
        fileSelectMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
        optionsMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsMenu.SetActive(false);
        confirmResetMenu.SetActive(false);
    }

    public void DisplaySoundOptions()
    {
        soundMenu.SetActive(true);

        mainMenu.SetActive(false);
        fileSelectMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
        comfortMenu.SetActive(false);
        optionsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        confirmResetMenu.SetActive(false);
    }

    public void DisplayControls()
    {
        controlsMenu.SetActive(true);

        mainMenu.SetActive(false);
        fileSelectMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
        optionsMenu.SetActive(false);
        comfortMenu.SetActive(false);
        soundMenu.SetActive(false);
        confirmResetMenu.SetActive(false);
    }

    public void DisplayConfirm()
    {
        confirmResetMenu.SetActive(true);

        mainMenu.SetActive(false);
        fileSelectMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
        optionsMenu.SetActive(false);
        comfortMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsMenu.SetActive(false);
    }

    public void SwapFile(int fileNumber)
    {
        GameManager.Instance.fileSelected = true;
        GameSaveManager.Instance.ChangeProfile((byte)fileNumber);
    }
    public void ResetFile()
    {
        GameSaveManager.Instance.ResetFile();
    }
    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
    public override void LoadMainMenu()
    {
        return;
    }

    public override void RespawnPlayer()
    {
        return;
    }
    public override void KillPlayer()
    {
        return;
    }
    public override void LoadNextLevel()
    {
        if (GameManager.Instance.profile.unlockedLevels.Contains(0))
        {
            SceneLoader.Instance.LoadLevel(0);
        }
        else
        {
            SceneLoader.Instance.LoadLevel(GameManager.Instance.profile.unlockedLevels[GameManager.Instance.profile.unlockedLevels.Count - 1]);
        }
    }
}

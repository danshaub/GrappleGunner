using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuManager : LocationManager
{
    // Submenues
    public GameObject disclaimer;
    public GameObject fileSelectMenu;
    public GameObject mainMenu;
    public GameObject levelSelectMenu;
    public GameObject optionsMenu;
    public GameObject comfortMenu;
    public GameObject soundMenu;
    public GameObject controlsMenu;
    public GameObject confirmResetMenu;

    // Text objects
    public TextMeshPro turnProvider;
    public TextMeshPro turnAmount;
    public TextMeshPro speedLineStatus;
    public List<TextMeshPro> levelText;
    public TextMeshPro musicVolume;
    public TextMeshPro sfxVolume;
    public TextMeshPro voiceVolume;
    public TextMeshPro ambientVolume;

    // Materials
    public List<MeshRenderer> levelButtonVisuals;
    public Material lockedLevelMaterial;
    public Material unlockedLevelMaterial;

    private bool musicPlaying = false;
    protected void Start()
    {
        if (GameManager.Instance.fileSelected)
        {
            DisplayMain();
        }
        else
        {
            DisplayDisclaimer();
        }
    }

    #region display_functions

    public void DisplayDisclaimer()
    {
        disclaimer.SetActive(true);

        mainMenu.SetActive(false);
        fileSelectMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
        optionsMenu.SetActive(false);
        comfortMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsMenu.SetActive(false);
        confirmResetMenu.SetActive(false);
    }
    public void DisplayMain()
    {
        mainMenu.SetActive(true);

        disclaimer.SetActive(false);
        fileSelectMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
        optionsMenu.SetActive(false);
        comfortMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsMenu.SetActive(false);
        confirmResetMenu.SetActive(false);

        if (!musicPlaying)
        {
            SFXManager.Instance.PlayMusic(music);
            musicPlaying = true;
        }
    }

    public void DisplayFileSelect()
    {
        fileSelectMenu.SetActive(true);

        disclaimer.SetActive(false);
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

        disclaimer.SetActive(false);
        mainMenu.SetActive(false);
        fileSelectMenu.SetActive(false);
        optionsMenu.SetActive(false);
        comfortMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsMenu.SetActive(false);
        confirmResetMenu.SetActive(false);

        UpdateLevelButtons();
    }

    public void DisplayOptions()
    {
        optionsMenu.SetActive(true);

        disclaimer.SetActive(false);
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

        disclaimer.SetActive(false);
        mainMenu.SetActive(false);
        fileSelectMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
        optionsMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsMenu.SetActive(false);
        confirmResetMenu.SetActive(false);

        UpdateComfortMenuVisual();
    }

    public void DisplaySoundOptions()
    {
        soundMenu.SetActive(true);

        disclaimer.SetActive(false);
        mainMenu.SetActive(false);
        fileSelectMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
        comfortMenu.SetActive(false);
        optionsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        confirmResetMenu.SetActive(false);

        UpdateAudioMenu();
    }

    public void DisplayControls()
    {
        controlsMenu.SetActive(true);

        disclaimer.SetActive(false);
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

        disclaimer.SetActive(false);
        mainMenu.SetActive(false);
        fileSelectMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
        optionsMenu.SetActive(false);
        comfortMenu.SetActive(false);
        soundMenu.SetActive(false);
        controlsMenu.SetActive(false);
    }

    #endregion

    #region comfort_menu_functions

    public void UpdateComfortMenuVisual()
    {
        if (GameManager.Instance.options.snapTurn)
        {
            turnProvider.text = "Snap";
            turnAmount.text = "Snap Angle: " + ComfortManager.snapValues[GameManager.Instance.options.snapValue].ToString() + "??";
        }
        else
        {
            turnProvider.text = "Continuous";
            turnAmount.text = "Turn Speed: " + GameManager.Instance.options.continuousTrunSpeed.ToString();
        }

        if (GameManager.Instance.options.useSpeedLines)
        {
            speedLineStatus.text = "On";
        }
        else
        {
            speedLineStatus.text = "Off";
        }
    }

    public void ToggleTurnProvider()
    {
        ComfortManager.Instance.ToggleTurnProvider();
        UpdateComfortMenuVisual();
    }
    public void ToggleSpeedLines()
    {
        ComfortManager.Instance.ToggleSpeedLines();
        UpdateComfortMenuVisual();
    }
    public void Increment()
    {
        ComfortManager.Instance.Increment();
        UpdateComfortMenuVisual();
    }
    public void Decrement()
    {
        ComfortManager.Instance.Decrement();
        UpdateComfortMenuVisual();
    }

    #endregion

    #region sound_options
    public void UpdateAudioMenu()
    {
        musicVolume.text = "Music " + ((int)(SFXManager.Instance.GetVolume("MusicVolume") + 80)).ToString();
        sfxVolume.text = "SFX " + ((int)(SFXManager.Instance.GetVolume("SFXVolume") + 80)).ToString();
        voiceVolume.text = "Voice " + ((int)(SFXManager.Instance.GetVolume("VoiceVolume") + 80)).ToString();
        ambientVolume.text = "Ambient " + ((int)(SFXManager.Instance.GetVolume("AmbientVolume") + 80)).ToString();
    }

    public void VolumeIncreaseSFX()
    {
        SFXManager.Instance.VolumeIncrease("SFXVolume");
        UpdateAudioMenu();
    }
    public void VolumeIncreaseMusic()
    {
        SFXManager.Instance.VolumeIncrease("MusicVolume");
        UpdateAudioMenu();
    }
    public void VolumeIncreaseVoice()
    {
        SFXManager.Instance.VolumeIncrease("VoiceVolume");
        UpdateAudioMenu();
    }
    public void VolumeIncreaseAmbient()
    {
        SFXManager.Instance.VolumeIncrease("AmbientVolume");
        UpdateAudioMenu();
    }

    public void VolumeDecreaseSFX()
    {
        SFXManager.Instance.VolumeDecrease("SFXVolume");
        UpdateAudioMenu();
    }
    public void VolumeDecreaseMusic()
    {
        SFXManager.Instance.VolumeDecrease("MusicVolume");
        UpdateAudioMenu();
    }
    public void VolumeDecreaseVoice()
    {
        SFXManager.Instance.VolumeDecrease("VoiceVolume");
        UpdateAudioMenu();
    }
    public void VolumeDecreaseAmbient()
    {
        SFXManager.Instance.VolumeDecrease("AmbientVolume");
        UpdateAudioMenu();
    }

    public void ResetVolume()
    {
        SFXManager.Instance.SetVolume("MusicVolume", 0f);
        SFXManager.Instance.SetVolume("SFXVolume", 0f);
        SFXManager.Instance.SetVolume("VoiceVolume", 0f);
        SFXManager.Instance.SetVolume("AmbientVolume", 0f);

        UpdateAudioMenu();
    }
    #endregion

    #region misc
    public void UpdateLevelButtons()
    {
        for (int i = 0; i < SceneLoader.Instance.directory.levelNames.Count; i++)
        {
            if (GameManager.Instance.profile.unlockedLevels.Contains(i))
            {
                levelButtonVisuals[i].material = unlockedLevelMaterial;
                levelText[i].text = i == 0 ? "Playground" : "Level " + i.ToString();
            }
            else
            {
                levelButtonVisuals[i].material = lockedLevelMaterial;
                levelText[i].text = "Locked";
            }
        }
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
    #endregion

    #region overrides
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

    public void LoadLevelIndex(int index){
        base.LoadLevel(index);
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

    public override void Debug()
    {
        GameManager.Instance.profile.unlockedLevels.Add(0);
        GameManager.Instance.profile.unlockedLevels.Add(1);
        GameManager.Instance.profile.unlockedLevels.Add(2);
        GameManager.Instance.profile.unlockedLevels.Add(3);
        GameManager.Instance.profile.unlockedLevels.Add(4);
        var set = new HashSet<int>(GameManager.Instance.profile.unlockedLevels);
        GameManager.Instance.profile.unlockedLevels = new List<int>(set);
        GameManager.Instance.profile.unlockedLevels.Sort();
        GameSaveManager.Instance.SaveGame();

        UpdateLevelButtons();
    }
    #endregion
}
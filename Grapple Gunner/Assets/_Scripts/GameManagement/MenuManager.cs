using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class MenuManager : Singleton<MenuManager>
{
    private bool _menuLocked;
    public bool menuLocked
    {
        get { return _menuLocked; }
        set
        {
            if (!_menuLocked && value)
            {
                HideMenu();
            }

            _menuLocked = value;
        }
    }

    // Mount transforms
    public Transform controllerMountPoint;
    public Transform controllerMounterTransform;
    // Submenues
    public GameObject background;
    public GameObject homeMenu;
    public GameObject levelSelect;
    public GameObject controlsMenu;
    public GameObject comfortMenu;
    public GameObject audioMenu1;
    public GameObject audioMenu2;
    // Text Objects
    public List<TextMeshPro> levelTexts;
    public TextMeshPro speedLinesText;
    public TextMeshPro turnProviderStatusText;
    public TextMeshPro musicVolume;
    public TextMeshPro sfxVolume;
    public TextMeshPro voiceVolume;
    public TextMeshPro ambientVolume;
    // Materials
    public List<MeshRenderer> levelButtonVisuals;
    public Material lockedMaterial;
    public Material unlockedMaterial;

    private void Update()
    {
        controllerMounterTransform.rotation = controllerMountPoint.rotation;
        controllerMounterTransform.position = controllerMountPoint.position;
    }

    public void ShowMenu()
    {
        background.SetActive(true);
        GrappleManager.Instance.DisableReticle(0);
        DisplayHome();
    }
    public void HideMenu()
    {
        background.SetActive(false);
        GrappleManager.Instance.EnableReticle(0);
    }

    public void DisplayHome()
    {
        homeMenu.SetActive(true);

        levelSelect.SetActive(false);
        controlsMenu.SetActive(false);
        comfortMenu.SetActive(false);
        audioMenu1.SetActive(false);
        audioMenu2.SetActive(false);
    }

    public void DisplayLevelSelect()
    {
        levelSelect.SetActive(true);

        homeMenu.SetActive(false);
        controlsMenu.SetActive(false);
        comfortMenu.SetActive(false);
        audioMenu1.SetActive(false);
        audioMenu2.SetActive(false);

        UpdateLevelButtons();
    }

    public void DisplayControls()
    {
        controlsMenu.SetActive(true);

        homeMenu.SetActive(false);
        levelSelect.SetActive(false);
        comfortMenu.SetActive(false);
        audioMenu1.SetActive(false);
        audioMenu2.SetActive(false);
    }

    public void DisplayComfortMenu()
    {
        comfortMenu.SetActive(true);

        homeMenu.SetActive(false);
        levelSelect.SetActive(false);
        controlsMenu.SetActive(false);
        audioMenu1.SetActive(false);
        audioMenu2.SetActive(false);


        UpdateComfortMenu();
    }

    public void DisplayAudioMenu1()
    {
        audioMenu1.SetActive(true);

        homeMenu.SetActive(false);
        levelSelect.SetActive(false);
        controlsMenu.SetActive(false);
        comfortMenu.SetActive(false);
        audioMenu2.SetActive(false);

        UpdateAudioMenu1();
    }

    public void DisplayAudioMenu2()
    {
        audioMenu2.SetActive(true);

        homeMenu.SetActive(false);
        levelSelect.SetActive(false);
        controlsMenu.SetActive(false);
        comfortMenu.SetActive(false);
        audioMenu1.SetActive(false);

        UpdateAudioMenu2();
    }

    public void MainMenu()
    {
        SceneLoader.Instance.LoadMainMenu(true);
    }

    public void LoadLevel(int levelIndex)
    {
        SceneLoader.Instance.LoadLevel(levelIndex);
        HideMenu();
    }

    public void RespawnPlayer()
    {
        LocationManager.Instance.RespawnPlayer();
        HideMenu();
    }

    public void UpdateLevelButtons()
    {
        for (int i = 0; i < SceneLoader.Instance.directory.levelNames.Count; i++)
        {
            if (GameManager.Instance.profile.unlockedLevels.Contains(i))
            {
                levelButtonVisuals[i].material = unlockedMaterial;
                levelTexts[i].text = i == 0 ? "Playground" : "Level " + i.ToString();
            }
            else
            {
                levelButtonVisuals[i].material = lockedMaterial;
                levelTexts[i].text = "Locked";
            }
        }
    }

    public void ToggleTurnProvider()
    {
        ComfortManager.Instance.ToggleTurnProvider();
        UpdateComfortMenu();
    }
    public void ToggleSpeedLines()
    {
        ComfortManager.Instance.ToggleSpeedLines();
        UpdateComfortMenu();
    }
    public void Increment()
    {
        ComfortManager.Instance.Increment();
        UpdateComfortMenu();
    }
    public void Decrement()
    {
        ComfortManager.Instance.Decrement();
        UpdateComfortMenu();
    }

    public void VolumeIncreaseSFX()
    {
        SFXManager.Instance.VolumeIncrease("SFXVolume");
        UpdateAudioMenu1();
    }
    public void VolumeIncreaseMusic()
    {
        SFXManager.Instance.VolumeIncrease("MusicVolume");
        UpdateAudioMenu1();
    }
    public void VolumeIncreaseVoice()
    {
        SFXManager.Instance.VolumeIncrease("VoiceVolume");
        UpdateAudioMenu2();
    }
    public void VolumeIncreaseAmbient()
    {
        SFXManager.Instance.VolumeIncrease("AmbientVolume");
        UpdateAudioMenu2();
    }

    public void VolumeDecreaseSFX()
    {
        SFXManager.Instance.VolumeDecrease("SFXVolume");
        UpdateAudioMenu1();
    }
    public void VolumeDecreaseMusic()
    {
        SFXManager.Instance.VolumeDecrease("MusicVolume");
        UpdateAudioMenu1();
    }
    public void VolumeDecreaseVoice()
    {
        SFXManager.Instance.VolumeDecrease("VoiceVolume");
        UpdateAudioMenu2();
    }
    public void VolumeDecreaseAmbient()
    {
        SFXManager.Instance.VolumeDecrease("AmbientVolume");
        UpdateAudioMenu2();
    }
    public void UpdateComfortMenu()
    {
        if (GameManager.Instance.options.snapTurn)
        {
            turnProviderStatusText.text = "Snap\nAngle: " + ComfortManager.snapValues[GameManager.Instance.options.snapValue].ToString() + "°";
        }
        else
        {
            turnProviderStatusText.text = "Continuous\nSpeed: " + GameManager.Instance.options.continuousTrunSpeed.ToString();
        }

        if (GameManager.Instance.options.useSpeedLines)
        {
            speedLinesText.text = "Speed Lines \u25A1";
        }
        else
        {
            speedLinesText.text = "Speed Lines X";
        }
    }

    public void UpdateAudioMenu1()
    {
        musicVolume.text = ((int)(SFXManager.Instance.GetVolume("MusicVolume") + 80)).ToString();
        sfxVolume.text = ((int)(SFXManager.Instance.GetVolume("SFXVolume") + 80)).ToString();
    }

    public void UpdateAudioMenu2()
    {
        voiceVolume.text = ((int)(SFXManager.Instance.GetVolume("VoiceVolume") + 80)).ToString();
        ambientVolume.text = ((int)(SFXManager.Instance.GetVolume("AmbientVolume") + 80)).ToString();
    }
}

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
    public GameObject homeMenu;
    public GameObject comfortMenu;
    public GameObject controlsMenu;
    public GameObject background;
    public TMP_Text indicatorArrow;
    public TMP_Text turnSpeedIndicator;
    public Transform controllerMountPoint;
    public Transform controllerMounterTransform;
    public ActionBasedContinuousTurnProvider continuousTurnProvider;
    public ActionBasedSnapTurnProvider snapTurnProvider;

    private int[] snapValues = new int[] { 5, 6, 9, 10, 15, 18, 30, 45 };
    private int snapValue = 4;

    private void Update()
    {
        controllerMounterTransform.rotation = controllerMountPoint.rotation;
        controllerMounterTransform.position = controllerMountPoint.position;
    }

    public void ShowMenu()
    {
        background.SetActive(true);
        HomeMenu();
    }
    public void HideMenu()
    {
        background.SetActive(false);
    }

    public void MainMenu()
    {
        SceneLoader.Instance.LoadMainMenu(true);
    }

    public void ControlsMenu()
    {
        homeMenu.SetActive(false);
        comfortMenu.SetActive(false);

        controlsMenu.SetActive(true);
    }

    public void ComfortMenu()
    {
        homeMenu.SetActive(false);
        controlsMenu.SetActive(false);

        comfortMenu.SetActive(true);

        if (snapTurnProvider.enabled)
        {
            SetSnap();
        }
        else
        {
            SetCont();
        }
    }

    public void HomeMenu()
    {
        controlsMenu.SetActive(false);
        comfortMenu.SetActive(false);

        homeMenu.SetActive(true);
    }

    public void SetSnap()
    {
        continuousTurnProvider.enabled = false;
        snapTurnProvider.enabled = true;

        indicatorArrow.text = "<---";
        turnSpeedIndicator.text = snapTurnProvider.turnAmount.ToString() + "°";
    }

    public void SetCont()
    {
        continuousTurnProvider.enabled = true;
        snapTurnProvider.enabled = false;

        indicatorArrow.text = "--->";
        turnSpeedIndicator.text = continuousTurnProvider.turnSpeed.ToString();
    }

    public void Increment()
    {
        if (snapTurnProvider.enabled)
        {
            snapValue = (snapValue + 1) % 8;
            snapTurnProvider.turnAmount = snapValues[snapValue];
            turnSpeedIndicator.text = snapTurnProvider.turnAmount.ToString() + "°";
        }
        else
        {
            continuousTurnProvider.turnSpeed += 15;
            continuousTurnProvider.turnSpeed = continuousTurnProvider.turnSpeed > 120 ? 120 : continuousTurnProvider.turnSpeed;
            turnSpeedIndicator.text = continuousTurnProvider.turnSpeed.ToString();
        }
    }

    public void Decrement()
    {
        if (snapTurnProvider.enabled)
        {
            snapValue = (snapValue - 1);
            snapValue = snapValue < 0 ? 7 : snapValue;
            snapTurnProvider.turnAmount = snapValues[snapValue];
            turnSpeedIndicator.text = snapTurnProvider.turnAmount.ToString() + "°";
        }
        else
        {
            continuousTurnProvider.turnSpeed -= 15;
            continuousTurnProvider.turnSpeed = continuousTurnProvider.turnSpeed < 30 ? 30 : continuousTurnProvider.turnSpeed;
            turnSpeedIndicator.text = continuousTurnProvider.turnSpeed.ToString();
        }
    }
}

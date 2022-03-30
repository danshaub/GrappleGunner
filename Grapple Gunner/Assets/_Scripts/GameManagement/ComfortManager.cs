using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ComfortManager : SingletonPersistent<ComfortManager>
{
    private ActionBasedContinuousTurnProvider continuousTurnProvider;
    private ActionBasedSnapTurnProvider snapTurnProvider;
    public static int[] snapValues = new int[] { 5, 6, 9, 10, 15, 18, 30, 45 };
    protected override void Awake()
    {
        base.Awake();

        continuousTurnProvider = PlayerManager.Instance.movementController.GetComponent<ActionBasedContinuousTurnProvider>();
        snapTurnProvider = PlayerManager.Instance.movementController.GetComponent<ActionBasedSnapTurnProvider>();
    }

    public void ApplyOptions()
    {
        OptionsData options = GameManager.Instance.options;

        VFXManager.Instance.speedlines.active = options.useSpeedLines;

        if (options.snapTurn)
        {
            snapTurnProvider.enabled = true;
            continuousTurnProvider.enabled = false;
        }
        else
        {
            continuousTurnProvider.enabled = true;
            snapTurnProvider.enabled = false;
        }

        continuousTurnProvider.turnSpeed = options.continuousTrunSpeed;
        snapTurnProvider.turnAmount = snapValues[options.snapValue];

        GameSaveManager.Instance.SaveGame();
    }

    public void ToggleTurnProvider()
    {
        GameManager.Instance.options.snapTurn = !GameManager.Instance.options.snapTurn;

        ApplyOptions();
    }

    public void ToggleSpeedLines()
    {
        GameManager.Instance.options.useSpeedLines = !GameManager.Instance.options.useSpeedLines;

        ApplyOptions();
    }

    public void Increment()
    {
        if (GameManager.Instance.options.snapTurn)
        {
            GameManager.Instance.options.snapValue = (GameManager.Instance.options.snapValue + 1) % 8;
        }
        else
        {
            GameManager.Instance.options.continuousTrunSpeed += 15;
            GameManager.Instance.options.continuousTrunSpeed = Mathf.Clamp(GameManager.Instance.options.continuousTrunSpeed, 30, 120);
        }

        ApplyOptions();
    }

    public void Decrement()
    {
        if (GameManager.Instance.options.snapTurn)
        {
            int newValue = GameManager.Instance.options.snapValue - 1;
            newValue = newValue < 0 ? 7 : newValue;
            GameManager.Instance.options.snapValue =  newValue;
        }
        else
        {
            GameManager.Instance.options.continuousTrunSpeed -= 15;
            GameManager.Instance.options.continuousTrunSpeed = Mathf.Clamp(GameManager.Instance.options.continuousTrunSpeed, 30, 120);
        }

        ApplyOptions();
    }
}

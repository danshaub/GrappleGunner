using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DevCanvas : SingletonPersistent<DevCanvas>
{
    public TextMeshProUGUI unlockedLevels;
    public TextMeshProUGUI snapTurn;
    public TextMeshProUGUI speedlines;

    // Update is called once per frame
    void Update()
    {
        unlockedLevels.text = string.Concat("Unlocked Levels: ", string.Join(", ", GameManager.Instance.profile.unlockedLevels));
        snapTurn.text = string.Concat("snap turn: ", GameManager.Instance.options.snapTurn.ToString());
        speedlines.text = string.Concat("speed lines: ", GameManager.Instance.options.useSpeedLines.ToString());
    }
}

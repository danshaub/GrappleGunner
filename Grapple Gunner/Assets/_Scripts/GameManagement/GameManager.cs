using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonPersistent<GameManager>
{
    public OptionsData options;
    public ProfileData profile;

    private void Start() {
        GameSaveManager.Instance.LoadGame();
    }
}

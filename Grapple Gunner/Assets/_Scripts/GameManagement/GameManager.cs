using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonPersistent<GameManager>
{
    public OptionsData options;
    public ProfileData profile;
    public bool fileSelected = false;

    private void Start()
    {
        GameSaveManager.Instance.LoadGame();
        ComfortManager.Instance.ApplyOptions();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL(webplayerQuitURL);
#else
        Application.Quit();
#endif
    }
}

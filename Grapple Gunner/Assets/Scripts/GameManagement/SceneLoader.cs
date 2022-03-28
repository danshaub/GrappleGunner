using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : SingletonPersistent<SceneLoader>
{
    public LevelDirectory directory;
    public Scene GetCurrentScene()
    {
        return SceneManager.GetActiveScene();
    }
    public void LoadMainMenu(bool useTransition)
    {
        StartCoroutine(LoadLevelCoroutine(directory.GetMainMenu(), useTransition));
    }
    public void LoadLevel(int levelIndex)
    {
        if (directory.ValidateLevelIndex(levelIndex))
        {
            StartCoroutine(LoadLevelCoroutine(directory.GetLevelName(levelIndex), true));
        }
    }
    public IEnumerator LoadLevelCoroutine(string levelName, bool useTransition)
    {

        AsyncOperation oper = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
        oper.allowSceneActivation = false;
        while (oper.progress < 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }
        oper.allowSceneActivation = true;

        yield return new WaitForSeconds(Time.deltaTime * 2);

        PlayerManager.Instance.TeleportPlayer(LevelManager.Instance?.playerStartTransform);

    }
}

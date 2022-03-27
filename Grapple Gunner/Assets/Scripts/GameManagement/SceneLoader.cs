using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : SingletonPersistent<SceneLoader>
{
    public AsyncOperation LoadMainMenu()
    {
        return SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }
}

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
    private IEnumerator LoadLevelCoroutine(string levelName, bool useTransition)
    {
        SFXManager.Instance.FadeOutMusic(1f);
        SFXManager.Instance.StopAllVoiceClips();
        SFXManager.Instance.StopAllLoopingSounds();
        
        if(useTransition){
            VFXManager.Instance.transitionSystem.SetParticleColor(VFXManager.Instance.defaultTransitionColor);
            VFXManager.Instance.transitionSystem.StartTransition();
            yield return new WaitForSeconds(1f);
        }
        PlayerManager.Instance.movementController.enabled = false;
        SceneManager.LoadScene(levelName);

        StartCoroutine(FinishLoadingLevel());
    }

    private IEnumerator FinishLoadingLevel(){
        yield return new WaitForSeconds(0.5f);
        PlayerManager.Instance.TeleportPlayer(LocationManager.Instance?.playerStartTransform);

        PlayerManager.Instance.movementController.enabled = true;
    }
}

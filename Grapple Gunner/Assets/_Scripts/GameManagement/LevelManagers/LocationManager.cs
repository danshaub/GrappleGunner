using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LocationManager : Singleton<LocationManager>
{

    public Transform playerStartTransform;
    public Sound music;

    public bool LoadLevel(int levelIndex)
    {
        if (GameManager.Instance.profile.unlockedLevels.Contains(levelIndex))
        {
            SceneLoader.Instance.LoadLevel(levelIndex);
            return true;
        }
        return false;
    }
    public virtual void LoadMainMenu()
    {
        SceneLoader.Instance.LoadMainMenu(true);
    }

    public void PlayVoiceClip(string clipName)
    {
        SFXManager.Instance.PlayVoiceClip(clipName);
    }

    private void OnDrawGizmos()
    {
        if (playerStartTransform != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(playerStartTransform.position, 0.2f);
            Gizmos.DrawLine(playerStartTransform.position, (playerStartTransform.position + (0.4f * playerStartTransform.forward)));
        }
    }

    public abstract void RespawnPlayer();
    public abstract void KillPlayer();
    public abstract void LoadNextLevel();
    public abstract void Debug();
}

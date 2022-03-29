using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    // TODO remove from this class and include in child classes
    public int levelIndex = -1;
    public Transform playerStartTransform;
    public Transform playerDeathTransform;

    protected override void Awake()
    {
        base.Awake();

        if (levelIndex >= 0 && !GameManager.Instance.profile.unlockedLevels.Contains(levelIndex))
        {
            GameManager.Instance.profile.unlockedLevels.Add(levelIndex);
            GameManager.Instance.profile.unlockedLevels.Sort();
            GameSaveManager.Instance.SaveGame();
        }
    }

    public virtual void LoadLevel(int levelIndex)
    {
        SceneLoader.Instance.LoadLevel(levelIndex);
    }
    public virtual void LoadMainMenu()
    {
        SceneLoader.Instance.LoadMainMenu(true);
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

    public virtual void RespawnPlayer()
    {
        PlayerManager.Instance.TeleportPlayer(playerStartTransform);
    }

    public virtual void KillPlayer()
    {
        PlayerManager.Instance.TeleportPlayer(playerDeathTransform);
    }

    // TODO: Make abstract and implement in subsequent levels
    public virtual void LoadNextLevel()
    {
        if (levelIndex < 0) return;

        SceneLoader.Instance.LoadLevel((levelIndex + 1) % SceneLoader.Instance.directory.levelNames.Count);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : LocationManager
{
    public int levelIndex = -1;
    public Transform playerDeathTransform;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start() {
        if (levelIndex >= 0 && !GameManager.Instance.profile.unlockedLevels.Contains(levelIndex))
        {
            GameManager.Instance.profile.unlockedLevels.Add(levelIndex);
            GameManager.Instance.profile.unlockedLevels.Sort();
            GameSaveManager.Instance.SaveGame();
        }
    }

    public override void LoadNextLevel()
    {
        if (levelIndex < 0) return;

        SceneLoader.Instance.LoadLevel((levelIndex + 1) % SceneLoader.Instance.directory.levelNames.Count);
    }

    public override void RespawnPlayer()
    {
        VFXManager.Instance.transitionSystem.SetParticleColor(VFXManager.Instance.defaultTransitionColor);
        VFXManager.Instance.transitionSystem.StartTransition();
        PlayerManager.Instance.TeleportAfter(playerStartTransform, 0.25f);
    }

    public override void KillPlayer()
    {
        VFXManager.Instance.transitionSystem.SetParticleColor(VFXManager.Instance.deathTransitionColor);
        VFXManager.Instance.transitionSystem.StartTransition();
        PlayerManager.Instance.TeleportAfter(playerDeathTransform, 0.1f);
    }
}
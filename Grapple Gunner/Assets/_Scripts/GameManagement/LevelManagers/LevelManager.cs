using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : LocationManager
{
    public int levelIndex = -1;
    public Transform playerDeathTransform;
    public Transform playerRespawnTransform;
    public List<Transform> movingObjects;
    private List<Vector3> movingObjectPositions;
    private List<Quaternion> movingObjectRotations;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (levelIndex >= 0 && !GameManager.Instance.profile.unlockedLevels.Contains(levelIndex))
        {
            GameManager.Instance.profile.unlockedLevels.Add(levelIndex);
            GameManager.Instance.profile.unlockedLevels.Sort();
            GameSaveManager.Instance.SaveGame();
        }


    }

    public void MakeCheckpoint(Transform respawnTransform)
    {
        playerRespawnTransform = respawnTransform;

        movingObjectPositions = new List<Vector3>();
        movingObjectRotations = new List<Quaternion>();
        foreach (Transform t in movingObjects)
        {
            movingObjectPositions.Add(t.position);
            movingObjectRotations.Add(t.rotation);

            t.GetComponent<OrangePoint>()?.SaveState();
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

        for(int i = 0; i < movingObjects.Count; i++){
            movingObjects[i].GetComponent<OrangePoint>()?.LoadState();

            movingObjects[i].position = movingObjectPositions[i];
            movingObjects[i].rotation = movingObjectRotations[i];
        }

        PlayerManager.Instance.TeleportAfter(playerRespawnTransform, 0.1f);
    }
}
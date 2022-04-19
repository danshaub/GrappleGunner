using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : LocationManager
{
    public int levelIndex = -1;
    public Transform playerDeathTransform;
    public Transform playerRespawnTransform;
    public List<Transform> movingObjects;
    private List<Vector3> movingObjectPositions;
    private List<Quaternion> movingObjectRotations;
    public UnityEvent onLevelStart;

    private UnityEvent onRespawn;

    protected override void Awake()
    {
        base.Awake();
    }

    override protected void Start()
    {
        base.Start();

        if (levelIndex >= 0 && !GameManager.Instance.profile.unlockedLevels.Contains(levelIndex))
        {
            GameManager.Instance.profile.unlockedLevels.Add(levelIndex);
            GameManager.Instance.profile.unlockedLevels.Sort();
            GameSaveManager.Instance.SaveGame();
        }

        MakeCheckpoint(playerStartTransform, onLevelStart);
    }

    public void MakeCheckpoint(Transform respawnTransform, UnityEvent onPlayerRespawn)
    {
        playerRespawnTransform = respawnTransform;

        movingObjectPositions = new List<Vector3>();
        movingObjectRotations = new List<Quaternion>();
        foreach (Transform t in movingObjects)
        {
            movingObjectPositions.Add(t.position);
            movingObjectRotations.Add(t.rotation);

            t.GetComponent<ISaveState>()?.SaveState();
        }

        onRespawn = onPlayerRespawn;
    }

    private void UseCheckpoint(){
        for (int i = 0; i < movingObjects.Count; i++)
        {
            movingObjects[i].GetComponent<ISaveState>()?.LoadState();

            movingObjects[i].position = movingObjectPositions[i];
            movingObjects[i].rotation = movingObjectRotations[i];
        }
    }

    public override void LoadNextLevel()
    {
        if (levelIndex < 0) return;

        SceneLoader.Instance.LoadLevel((levelIndex + 1) % SceneLoader.Instance.directory.levelNames.Count);
    }

    public override void RespawnPlayer()
    {
        UseCheckpoint();

        VFXManager.Instance.transitionSystem.SetParticleColor(VFXManager.Instance.defaultTransitionColor);
        VFXManager.Instance.transitionSystem.StartTransition();
        PlayerManager.Instance.TeleportAfter(playerRespawnTransform, 0.25f);

        Invoke("InvokeOnRespawn", .5f);
    }

    private void InvokeOnRespawn(){
        SFXManager.Instance.PlayMusic(music);
        onRespawn.Invoke();
    }

    public override void KillPlayer()
    {
        CancelInvoke("InvokeOnRespawn");
        VFXManager.Instance.transitionSystem.SetParticleColor(VFXManager.Instance.deathTransitionColor);
        VFXManager.Instance.transitionSystem.StartTransition();

        PlayerManager.Instance.TeleportAfter(playerDeathTransform, 0.25f);

        SFXManager.Instance.PlaySFX("PlayerDeath");
        SFXManager.Instance.StopAllVoiceClips();
        SFXManager.Instance.StopAllLoopingSounds();
        SFXManager.Instance.FadeOutMusic(.5f);
    }
}
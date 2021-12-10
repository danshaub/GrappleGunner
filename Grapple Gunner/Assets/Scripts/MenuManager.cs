using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject homeMenu;
    public GameObject comfortMenu;
    public GameObject controlsMenu;
    public Transform controllerMountPoint;
    public Transform controllerMounterTransform;
    public ActionBasedContinuousTurnProvider continuousTurnProvider;
    public ActionBasedSnapTurnProvider snapTurnProvider;

    private void Update() {
        controllerMounterTransform.rotation = controllerMountPoint.rotation;
        controllerMounterTransform.position = controllerMountPoint.position;
    }
}

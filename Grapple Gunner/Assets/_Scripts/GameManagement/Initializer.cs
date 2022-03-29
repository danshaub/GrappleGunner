using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : SingletonPersistent<Initializer>
{
    public bool startupMainMenu = true;
    public List<GameObject> objectsToCreate;
    public List<GameObject> objectsToDestroy;
    protected override void Awake()
    {
        base.Awake();
        if (Instance.gameObject.Equals(gameObject))
        {
            foreach (GameObject gameObject in objectsToCreate)
            {
                Instantiate(gameObject);
            }
        }

    }
    private void Start()
    {
        StartCoroutine(FinishInitializing());
    }

    private IEnumerator FinishInitializing()
    {
        yield return new WaitForEndOfFrame();
        if (startupMainMenu)
        {
            SceneLoader.Instance.LoadMainMenu(false);
        }
        else{
            PlayerManager.Instance.TeleportPlayer(LocationManager.Instance?.playerStartTransform);
        }

        yield return new WaitForEndOfFrame();

        foreach (GameObject gameObject in objectsToDestroy)
        {
            Destroy(gameObject);
        }
    }
}

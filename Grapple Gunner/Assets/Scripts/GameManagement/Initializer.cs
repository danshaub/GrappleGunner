using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : Singleton<Initializer>
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
            AsyncOperation oper = SceneLoader.Instance.LoadMainMenu();
            oper.allowSceneActivation = false;
            while (oper.progress < 0.9f)
            {
                yield return new WaitForEndOfFrame();
            }
            oper.allowSceneActivation = true;
        }

        yield return new WaitForEndOfFrame();

        foreach (GameObject gameObject in objectsToDestroy)
        {
            Destroy(gameObject);
        }

        Destroy(this.gameObject);
    }
}

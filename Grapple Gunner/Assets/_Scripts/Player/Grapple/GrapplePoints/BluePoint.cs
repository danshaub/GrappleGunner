using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BluePoint : GrapplePoint
{
    public Vector3 worldRespawnPosition;
    public Transform pointVisual;
    public bool canStore = true;
    private Collider pointCollider;

    [HideInInspector] public bool blockHeld;
    [HideInInspector] public bool queueDestroyBlock = false;
    [HideInInspector] public bool colliding { get; private set; } = false;
    [HideInInspector] public Vector3 collisionNormal { get; private set; }
    [HideInInspector] public bool storageLockedByCooldown { get; private set; } = true;
    private bool showingMiniPoint = false;
    private bool destroying = false;
    public bool validTakeOutLocation { get; private set; } = true;
    private Vector3 targetLocalPosition;
    private float targetLocalScale;
    private float lerpValue;

    override protected void Awake()
    {
        base.Awake();
        type = GrappleType.Blue;
        useRaycastPosition = true;

        pointCollider = GetComponent<Collider>();
    }
    override public void OnPointHit()
    {
        storageLockedByCooldown = true;
        StartCoroutine(UnlockBlock());
        pointVisual.parent = transform;
        return;
    }

    override public void OnPointReleased()
    {
        storageLockedByCooldown = true;
        StopCoroutine(UnlockBlock());
        pointVisual.parent = transform;

        GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, GrappleManager.Instance.blueOptions.maxHookVelocity * .75f);
        return;
    }

    private void Update()
    {
        if (destroying)
        {
            Debug.Log("Destroying");
            pointVisual.localScale = Vector3.Lerp(pointVisual.localScale, Vector3.zero, lerpValue);
            Debug.Log(pointVisual.localScale);
        }
        else if (showingMiniPoint)
        {
            pointVisual.localPosition = Vector3.Lerp(pointVisual.localPosition, targetLocalPosition, lerpValue);
            pointVisual.localScale = Vector3.Lerp(pointVisual.localScale, targetLocalScale * Vector3.one, lerpValue);
            if (validTakeOutLocation)
            {
                pointVisual.rotation = Quaternion.Slerp(pointVisual.rotation, Random.rotation, lerpValue);
            }
        }
        else
        {
            pointVisual.localPosition = Vector3.Lerp(pointVisual.localPosition, Vector3.zero, lerpValue);
            pointVisual.localScale = Vector3.Lerp(pointVisual.localScale, Vector3.one, lerpValue);
            pointVisual.localRotation = Quaternion.Slerp(pointVisual.localRotation, Quaternion.identity, lerpValue);
        }
    }

    public void ShowMiniPoint(Transform parent, Vector3 targetPosition, float targetScale, float interpolationValue)
    {
        if (pointCollider.GetType() == typeof(SphereCollider))
        {
            ((SphereCollider)pointCollider).radius = .25f;
        }
        else if (pointCollider.GetType() == typeof(BoxCollider))
        {
            ((BoxCollider)pointCollider).size = Vector3.one * .5f;
        }
        targetLocalPosition = targetPosition;
        targetLocalScale = targetScale;
        lerpValue = interpolationValue;

        showingMiniPoint = true;

        pointVisual.position = transform.position;
        pointVisual.rotation = transform.rotation;
        pointVisual.localScale = Vector3.one;

        pointVisual.parent = parent;

        pointCollider.isTrigger = true;

    }

    public void HideMiniPoint()
    {
        if (pointCollider.GetType() == typeof(SphereCollider))
        {
            ((SphereCollider)pointCollider).radius = .5f;
        }
        else if (pointCollider.GetType() == typeof(BoxCollider))
        {
            ((BoxCollider)pointCollider).size = Vector3.one;
        }

        showingMiniPoint = false;
        pointVisual.parent = transform;

        pointCollider.isTrigger = false;
    }

    public void ApplyLaunchForce(Vector3 force)
    {
        StartCoroutine(LaunchForceCoroutine(force));
    }

    private IEnumerator LaunchForceCoroutine(Vector3 force)
    {
        yield return new WaitForFixedUpdate();

        GetComponent<Rigidbody>().AddForce(force);
    }

    private void OnTriggerEnter(Collider other)
    {
        validTakeOutLocation = false;
    }

    private void OnTriggerExit(Collider other)
    {
        validTakeOutLocation = true;
    }

    private void OnCollisionStay(Collision other)
    {
        colliding = true;
        collisionNormal = other.contacts[0].normal;
    }

    private void OnCollisionExit(Collision other)
    {
        colliding = false;
    }

    public void DestroyBlock()
    {
        if (destroying) return;

        type = GrappleType.DestroyingBlue;

        lerpValue = GrappleManager.Instance.blueOptions.interpolationValue;
        pointCollider.isTrigger = true;

        queueDestroyBlock = false;
        destroying = true;

        Transform ps = Instantiate(GrappleManager.Instance.blueOptions.destructionPS, pointVisual.position, pointVisual.rotation).transform;
        ps.localScale = pointVisual.lossyScale;

        Invoke("RespawnBlock", GrappleManager.Instance.blueOptions.destructionTime);

    }

    private void RespawnBlock()
    {
        type = GrappleType.Blue;
        Transform ps = Instantiate(GrappleManager.Instance.blueOptions.respawnPS, worldRespawnPosition, Quaternion.identity).transform;
        ps.localScale = transform.lossyScale;
        pointCollider.isTrigger = false;

        transform.position = worldRespawnPosition;
        transform.rotation = Quaternion.identity;

        Rigidbody pointRB = GetComponent<Rigidbody>();

        pointRB.velocity = Vector3.zero;
        pointRB.angularVelocity = Vector3.zero;

        destroying = false;
    }

    private IEnumerator UnlockBlock()
    {
        yield return new WaitForSeconds(GrappleManager.Instance.blueOptions.storeBlockCooldown);
        storageLockedByCooldown = false;
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        Color c = Color.blue;
        c.a = .25f;
        Gizmos.color = c;
        Gizmos.DrawWireSphere(worldRespawnPosition, .25f);
        base.OnDrawGizmos();
    }
#endif
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangePoint : GrapplePoint
{

    public Material disabledMaterial;
    private Material originalMaterial;
    public bool infiniteUses = true;
    public int numberUses;
    public Rigidbody blockRigidbody { get; private set; }
    public Rigidbody blockTargetRigidbody { get; private set; }
    public Rigidbody playerTargetRigidbody { get; private set; }
    private CapsuleCollider playerTargetCollider;
    private Vector3 blockTargetPosition = Vector3.zero;
    private Vector3 playerTargetPosition = Vector3.zero;

    private bool hookConnected = false;
    private int remainingUses;

    override protected void Awake()
    {
        base.Awake();
        useRaycastPosition = true;
        type = GrappleType.Orange;
    }

    private void Start()
    {
        gameObject.tag = "Hookable";
        originalMaterial = GetComponent<MeshRenderer>().sharedMaterial;

        blockRigidbody = GetComponent<Rigidbody>();

        Transform child = transform.GetChild(0);
        playerTargetRigidbody = child.GetComponent<Rigidbody>();
        playerTargetCollider = child.GetComponent<CapsuleCollider>();

        child = transform.GetChild(1);
        blockTargetRigidbody = child.GetComponent<Rigidbody>();

        playerTargetRigidbody.gameObject.SetActive(false);
        blockTargetRigidbody.gameObject.SetActive(false);

        remainingUses = numberUses;
    }

    private void FixedUpdate()
    {
        if (hookConnected)
        {
            SetTargetPositions();
        }
    }

    private void CalculatePlayerTargetCenter()
    {
        // Set collider Height and center
        playerTargetCollider.height = PlayerManager.Instance.movementController.playerCollider.height + 
                                      PlayerManager.Instance.movementController.options.rideHeight;
        playerTargetCollider.center = Vector3.up * playerTargetCollider.height * 0.5f;

        // Calculate target Position
        playerTargetPosition = transform.position;
        playerTargetPosition.y -= playerTargetCollider.height * 0.5f;

        Vector3 playerTargetDistance = playerTargetRigidbody.transform.position - playerTargetPosition;

        if (Vector3.Magnitude(playerTargetDistance) > 1f)
        {
            playerTargetRigidbody.MovePosition(playerTargetPosition);
        }
        else
        {
            Vector3 springForce = (-playerTargetDistance * 500) - (playerTargetRigidbody.velocity * 10);
            playerTargetRigidbody.AddForce(springForce);
        }
    }

    private void CalculateBlockTargetCenter()
    {
        blockTargetPosition = PlayerManager.Instance.movementController.transform.TransformPoint(PlayerManager.Instance.movementController.playerCollider.center);

        blockTargetPosition.y -= PlayerManager.Instance.movementController.options.rideHeight * .5f;

        Vector3 blockTargetDistance = blockTargetRigidbody.transform.position - blockTargetPosition;
        if (Vector3.Magnitude(blockTargetDistance) > 1f)
        {
            blockTargetRigidbody.MovePosition(blockTargetPosition);
        }
        else
        {
            Vector3 springForce = (-blockTargetDistance * 500) - (blockTargetRigidbody.velocity * 10);
            blockTargetRigidbody.AddForce(springForce);
        }

        blockTargetRigidbody.MoveRotation(transform.rotation);
    }

    private void SetTargetPositions()
    {
        CalculatePlayerTargetCenter();
        CalculateBlockTargetCenter();
    }

    override public void OnPointHit()
    {
        hookConnected = true;
        playerTargetRigidbody.gameObject.SetActive(true);
        blockTargetRigidbody.gameObject.SetActive(true);
    }

    override public void OnPointReleased()
    {
        hookConnected = false;
        playerTargetRigidbody.gameObject.SetActive(false);
        blockTargetRigidbody.gameObject.SetActive(false);
    }


    public void DecrementUses()
    {
        if (infiniteUses)
        {
            return;
        }
        else
        {
            remainingUses--;
            if (remainingUses == 0)
            {
                type = GrappleType.OrangeDisabled;
                GetComponent<MeshRenderer>().sharedMaterial = disabledMaterial;
            }
        }
    }

    public void ResetBlock()
    {
        remainingUses = numberUses;
        type = GrappleType.Orange;
        GetComponent<MeshRenderer>().sharedMaterial = originalMaterial;
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        base.OnDrawGizmos();
    }
#endif
}
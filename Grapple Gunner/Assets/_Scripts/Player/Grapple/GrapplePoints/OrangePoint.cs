using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OrangePoint : GrapplePoint, ISaveState
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

    public bool active = true;

    private bool hookConnected = false;
    private int remainingUses;

    private OrangeInfo savedInfo;

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

    public void SaveState()
    {
        savedInfo = new OrangeInfo();
        savedInfo.remainingUses = remainingUses;
        savedInfo.material = GetComponent<MeshRenderer>().sharedMaterial;
        savedInfo.type = type;
    }

    public void LoadState()
    {
        remainingUses = savedInfo.remainingUses;
        GetComponent<MeshRenderer>().sharedMaterial = savedInfo.material;
        type = savedInfo.type;
    }

    public void DestroyBlock()
    {
        active = false;
        Transform ps = Instantiate(GrappleManager.Instance.orangeOptions.destructionPS, transform).transform;
        GetComponentInChildren<Animator>().SetTrigger("Destroy");
    }

    public void RespawnBlock(Vector3 position, Vector3 rotation)
    {
        transform.position = position;
        transform.eulerAngles = rotation;
        active = true;
        Transform ps = Instantiate(GrappleManager.Instance.orangeOptions.respawnPS, transform.position, transform.rotation).transform;
        ps.localScale = Vector3.one;
        GetComponentInChildren<Animator>().SetTrigger("Respawn");
    }


    private struct OrangeInfo
    {
        public int remainingUses;
        public Material material;
        public GrapplePoint.GrappleType type;
    }

    public bool bonked = false;
    private void OnCollisionEnter(Collision other)
    {
        if (bonked || other.gameObject.layer == 8) return;


        BlueOptions options = GrappleManager.Instance.blueOptions;

        float hitVelocity = Vector3.Dot(other.relativeVelocity, other.contacts[0].normal);

        if (hitVelocity > options.bonkThreshold)
        {
            bonked = true;
            GetComponent<AudioSource>().volume = options.bonkVolume.Evaluate(hitVelocity);
            if (other.gameObject.layer == 17)
            {
                GetComponent<AudioSource>().PlayOneShot(options.bonkBarrierSound);
            }
            else if (other.gameObject.layer == 13 || other.gameObject.layer == 14)
            {
                GetComponent<AudioSource>().volume *= .8f;
                GetComponent<AudioSource>().PlayOneShot(options.bonkSound);
            }
            else
            {
                GetComponent<AudioSource>().PlayOneShot(options.bonkSound);
            }
            StartCoroutine(BonkCooldown(options.bonkCooldown));
        }
    }

    private IEnumerator BonkCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        bonked = false;
    }


#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        Color c = Color.yellow;
        c.a = .25f;
        Gizmos.color = c;
        base.OnDrawGizmos();
    }
#endif
}
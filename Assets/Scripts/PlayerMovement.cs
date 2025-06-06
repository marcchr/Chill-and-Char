using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : NetworkBehaviour
{
    public float walkSpeed;
    public float xAddedSpeed;
    private float moveInput;
    public bool isGrounded;
    private Rigidbody2D rb;
    public LayerMask groundMask;

    public PhysicsMaterial2D bounceMat, normalMat;
    public bool canJump;
    public float jumpValue = 0.0f;
    public float maxJumpValue = 20f;
    public float jumpValueRate = 0.1f;

    public Vector3 resetPosition;

    private Animator animator;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    private NetworkVariable<bool> flipX = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
        );

    //public struct MyCustomData : INetworkSerializable
    //{
    //    public Vector2 movement;

    //    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    //    {
    //        serializer.SerializeValue(ref movement);
    //    }
    //}

    //public override void OnNetworkSpawn()
    //{
    //    position.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
    //    {
    //        Debug.Log(OwnerClientId + "; " + newValue.movement);
    //    };
    //}
    public override void OnNetworkSpawn()
    {
        flipX.OnValueChanged += (bool previousValue, bool newValue) =>
        {
            spriteRenderer.flipX = newValue;
        };

        if (!IsOwner)
        {
            return;
        }

        CinemachineCamera playerCam = FindFirstObjectByType<CinemachineCamera>();

        if (playerCam != null)
        {
            playerCam.Follow = transform;
        }
        else
        {
            Debug.LogWarning("No CinemachineCamera found.");
        }
    }


    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        spriteRenderer.flipX = flipX.Value;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        moveInput = Input.GetAxisRaw("Horizontal");

        if (jumpValue == 0f && isGrounded)
        {
            rb.linearVelocity = new Vector2(moveInput * walkSpeed, rb.linearVelocity.y);

        }

        //position.Value = new MyCustomData
        //{
        //    movement = rb.linearVelocity
        //};

        isGrounded = Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - boxCollider.size.y * 0.5f + boxCollider.offset.y), new Vector2(0.95f, 0.3f), 0f, groundMask);

        if (!isGrounded)
        {
            rb.sharedMaterial = bounceMat;
        }
        else
        {
            rb.sharedMaterial = normalMat;
        }

        if (Input.GetKey(KeyCode.Space) && isGrounded && canJump)
        {
            
            jumpValue += 0.1f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && canJump)
        {
            animator.SetBool("isAboutToJump", true);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        if (jumpValue > maxJumpValue && isGrounded)
        {
            float tempx = moveInput * (walkSpeed + xAddedSpeed);
            float tempy = jumpValue;
            rb.linearVelocity = new Vector2(tempx, tempy);
            Invoke("ResetJump", 0.1f);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            animator.SetBool("isAboutToJump", false);
            animator.SetBool("isJumping", false);
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(moveInput * (walkSpeed + xAddedSpeed), jumpValue);
                jumpValue = 0f;
            }
            canJump = true;
        }

        if (rb.linearVelocity.x != 0f && rb.linearVelocity.y == 0f)
        {
            animator.SetBool("isRunning", true);
            if (rb.linearVelocity.x < 0f)
            {
                flipX.Value = true;
            }
            else if (rb.linearVelocity.x > 0f)
            {
                flipX.Value = false;
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        spriteRenderer.flipX = flipX.Value;

        if (rb.linearVelocity.y > 0f && !isGrounded)
        {
            animator.SetBool("isJumping", true);
            animator.SetBool("isRunning", false);
        }
        else if (rb.linearVelocity.y < 0f)
        {
            animator.SetBool("isFalling", true);
            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", false);
        }
        else
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision detected");
        if (collision.gameObject.CompareTag("Hazard"))
        {
            ResetPosition();
        }

        NetworkObject networkObject = collision.gameObject.GetComponent<NetworkObject>();

        if (networkObject != null && !networkObject.IsOwner && !collision.gameObject.CompareTag("Player"))
        {
            TransferOwnershipServerRpc(collision.gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TransferOwnershipServerRpc(NetworkObjectReference collision)
    {
        NetworkObject collisionObject = collision;
        collisionObject.ChangeOwnership(OwnerClientId);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard"))
        {
            ResetPosition();
        }
    }

    void ResetJump()
    {
        canJump = false;
        jumpValue = 0f;
    }

    void ResetPosition()
    {
        rb.linearVelocity = Vector3.zero;
        transform.position = resetPosition;
        animator.SetBool("isJumping", false);
        animator.SetBool("isRunning", false); 
        animator.SetBool("isAboutToJump", false);
        animator.SetBool("isRunning", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y - boxCollider.size.y * 0.5f + boxCollider.offset.y), new Vector3(0.95f, 0.3f));
    }
}

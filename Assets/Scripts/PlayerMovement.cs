using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : NetworkBehaviour
{
    public float walkSpeed;
    private float moveInput;
    public bool isGrounded;
    private Rigidbody2D rb;
    public LayerMask groundMask;

    public PhysicsMaterial2D bounceMat, normalMat;
    public bool canJump;
    public float jumpValue = 0.0f;
    public float maxJumpValue = 20f;

    private Animator animator;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    //private NetworkVariable<MyCustomData> position = new NetworkVariable<MyCustomData>(
    //    new MyCustomData
    //    {
    //        movement = new Vector2(0,0),
    //    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
    //    );

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
        //if (IsServer)
        //{
        //    SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, 0);
        //}
        //else
        //{
        //    SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, 1);
        //}

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

        isGrounded = Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - boxCollider.size.y * 0.5f + boxCollider.offset.y), new Vector2(0.9f, 0.3f), 0f, groundMask);

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
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        if (jumpValue > maxJumpValue && isGrounded)
        {
            float tempx = moveInput * walkSpeed;
            float tempy = jumpValue;
            rb.linearVelocity = new Vector2(tempx, tempy);
            Invoke("ResetJump", 0.1f);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(moveInput * walkSpeed, jumpValue);
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
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y - boxCollider.size.y * 0.5f + boxCollider.offset.y), new Vector3(0.9f, 0.3f));
    }
}

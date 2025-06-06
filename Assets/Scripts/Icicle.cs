using Unity.Netcode;
using UnityEngine;

public class Icicle : NetworkBehaviour
{
    [SerializeField] Transform iciclePrefab;
    public Transform iciclePosition;
    public float cooldownDuration;
    private bool isSpawningIcicle = false;

    public LayerMask playerDetectionLayer;

    private Animator animator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("icicle_grow") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            animator.SetBool("isAttacking", false);
            isSpawningIcicle = false;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("icicle_wait"))
        {
            CheckIfPlayerInRange();
        }
    }

    private void CheckIfPlayerInRange()
    {
        if (isSpawningIcicle)
        {
            animator.SetBool("isAttacking", true);
            return;
        }

        Collider2D hit = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - 4.5f), new Vector2(4f, 10f), 0f, playerDetectionLayer);

        if (hit != null)
        {
            Attack();
            isSpawningIcicle = true;
            animator.SetBool("isAttacking", true);
        }
    }
    private void Attack()
    {
        Transform icicle = Instantiate(iciclePrefab, iciclePosition.position, transform.rotation);
        icicle.GetComponent<NetworkObject>().Spawn(true);

        Debug.Log("icicle spawned");
        isSpawningIcicle = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(transform.position.x, transform.position.y - 4.5f), new Vector2(4f, 10f));
    }
}

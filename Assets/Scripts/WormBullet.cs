using Unity.Netcode;
using UnityEngine;

public class WormBullet : NetworkBehaviour
{
    private Vector3 target;
    public float targetDistance;

    private Rigidbody2D rb2D;
    public float speed;

    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        target = new Vector3(transform.position.x + targetDistance, transform.position.y);

        Debug.Log(target);

        Vector3 direction = target - transform.position;
        rb2D.linearVelocity = new Vector2(direction.x, direction.y).normalized * speed;

        Debug.Log(direction);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        animator.SetTrigger("isShot");
        float delay = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, delay);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

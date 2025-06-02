using Unity.Netcode;
using UnityEngine;

public class Worm : NetworkBehaviour
{
    [SerializeField] Transform bulletPrefab;
    public Transform bulletPosition;
    public float cooldownDuration;
    private float timer = 0f;

    public LayerMask playerDetectionLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
        {
            return;
        }

        timer += Time.deltaTime;

        if(timer > cooldownDuration)
        {
            timer = 0f;
            
            CheckIfPlayerInRange();
        }
    }

    private void CheckIfPlayerInRange()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, new Vector3(40f, 2f), 0f, playerDetectionLayer);

        if (hit != null)
        {
            Attack();
        }
    }
    private void Attack()
    {
        Transform bullet = Instantiate(bulletPrefab, bulletPosition.position, Quaternion.identity);
        bullet.GetComponent<NetworkObject>().Spawn(true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(40f, 2f));
    }
}

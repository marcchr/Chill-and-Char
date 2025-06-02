using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

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
            CheckIfPlayerInRange();
            timer = 0f;

            //AttackClientRpc();
        }
    }

    private void CheckIfPlayerInRange()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, new Vector2(40f, 4f), 0f, playerDetectionLayer);

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

    //void Attack()
    //{
    //    spawnedObjectTransform = Instantiate(bulletPrefab, bulletPosition.position, Quaternion.identity, bulletPosition);
    //    spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
    //}
}

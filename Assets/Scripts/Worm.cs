using Unity.Netcode;
using UnityEngine;

public class Worm : NetworkBehaviour
{
    [SerializeField] Transform bulletPrefab;
    public Transform bulletPosition;
    public float cooldownDuration;
    private float timer = 0f;

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
            Attack();

            //AttackClientRpc();
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

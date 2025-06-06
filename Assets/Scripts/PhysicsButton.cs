using Unity.Netcode;
using UnityEngine;

public class PhysicsButton : NetworkBehaviour
{
    public float maxY;
    public float minY;
    public float activatedY;

    public bool isActivated;
    private Rigidbody2D rb2D;

    //private NetworkVariable<bool> isActivated = new NetworkVariable<bool>(
    //       false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
    //       );

    //public override void OnNetworkSpawn()
    //{
    //    isActivated.OnValueChanged += (bool previousValue, bool newValue) =>
    //    {
            
    //    };
    //} 
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        maxY = transform.position.y;
        minY = transform.position.y - 0.5f;
        activatedY = transform.position.y - 0.25f;
    }

    // Update is called once per frame
    void Update()
    {
        //if (!IsOwner)
        //{
        //    return;
        //}

        if (transform.position.y >= maxY)
        {
            rb2D.linearVelocityY = 0f;
            transform.position = new Vector2(transform.position.x, maxY);
        }
        if (transform.position.y <= minY)
        {
            rb2D.linearVelocityY = 0f;
            transform.position = new Vector2(transform.position.x, minY);
        }
        if (transform.position.y <= activatedY)
        {
            isActivated = true;
        }
        else
        {
            isActivated = false;
        }
    }

    //[ServerRpc(RequireOwnership = false)]
    //public void ChangeButtonStateServerRpc(bool previousValue, bool newValue)
    //{

    //}
}

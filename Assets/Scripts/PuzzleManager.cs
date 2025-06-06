using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : NetworkBehaviour
{
    [SerializeField] PhysicsButton buttonA;
    [SerializeField] PhysicsButton buttonB;
    [SerializeField] Transform door;
    [SerializeField] Transform box;
    public Transform doorStartPos;
    public Transform doorEndPos;
    public Transform boxSpawnPos;

    private bool openDoor;
    private NetworkObject networkObject;
    private NetworkObject doorNetworkObj;
    private SpriteRenderer doorSprite;

    private bool lastButtonAState = false;
    private bool lastButtonBState = false;

    public override void OnNetworkSpawn()
    {
        //if (IsServer)
        //{
        //    Transform puzzleBox = Instantiate(box, boxSpawnPos.position, Quaternion.identity);
        //    puzzleBox.GetComponent<NetworkObject>().Spawn(true);

        //    Debug.Log("spawning box");
        //}

        //Debug.Log(NetworkManager.LocalClientId);
        //Debug.Log(OwnerClientId);
        //Debug.Log(box);

        //if (OwnerClientId != 0)
        //{
        //    DespawnObjectServerRpc(box.gameObject);
        //}
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        networkObject = GetComponent<NetworkObject>();
        buttonA.GetComponent<NetworkObject>();
        buttonB.GetComponent<NetworkObject>();
        box.GetComponent<NetworkObject>();
        doorNetworkObj = door.GetComponent<NetworkObject>();
        doorSprite = door.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!IsOwner)
        //{
        //    return;
        //}

        if (buttonA.isActivated != lastButtonAState)
        {
            lastButtonAState = buttonA.isActivated;
            HandleButtonStateChange(buttonA, buttonA.isActivated);
        }

        if (buttonB.isActivated != lastButtonBState)
        {
            lastButtonBState = buttonB.isActivated;
            HandleButtonStateChange(buttonB, buttonB.isActivated);
        }

        if (buttonA.isActivated || buttonB.isActivated)
        {
            openDoor = true;
            doorSprite.color = Color.yellow;
        }
        else
        {
            openDoor = false;
            doorSprite.color = Color.white;
        }

        if (openDoor && door.transform.position.y > doorEndPos.position.y)
        {
            door.transform.position = Vector2.MoveTowards(door.transform.position, doorEndPos.position, 0.01f);
        }
        else
        {
            door.transform.position = Vector2.MoveTowards(door.transform.position, doorStartPos.position, 0.01f);
        }

    }

    private void HandleButtonStateChange(PhysicsButton button, bool isActivated)
    {
        if (isActivated)
        {
            TransferOwnershipServerRpc(button.OwnerClientId);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void TransferOwnershipServerRpc(ulong clientId)
    {
        networkObject.ChangeOwnership(clientId);
        doorNetworkObj.ChangeOwnership(clientId);
    }

    //[ServerRpc(RequireOwnership = false)]
    //public void DespawnObjectServerRpc(NetworkObjectReference networkObject)
    //{
    //    NetworkObject boxNetObj = networkObject;
    //    boxNetObj.Despawn();
    //}

}

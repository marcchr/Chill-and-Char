using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class RelayManager : MonoBehaviour
{
    [SerializeField] Button hostButton;
    [SerializeField] Button joinButton;
    [SerializeField] TMP_InputField joinInput;
    [SerializeField] TextMeshProUGUI codeText;

    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        hostButton.onClick.AddListener(CreateRelay);
        joinButton.onClick.AddListener(() => JoinRelay(joinInput.text));
    }

    private async void CreateRelay()
    {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log(joinCode);

            codeText.text = "Code: \n" + joinCode;

            var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            hostButton.gameObject.SetActive(false);
            joinButton.gameObject.SetActive(false);
            joinInput.gameObject.SetActive(false);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);

            hostButton.gameObject.SetActive(true);
            joinButton.gameObject.SetActive(true);
            joinInput.gameObject.SetActive(true);
        }
    }

    async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with " + joinCode);
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();

            hostButton.gameObject.SetActive(false);
            joinButton.gameObject.SetActive(false);
            joinInput.gameObject.SetActive(false);
            codeText.gameObject.SetActive(false);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);

            hostButton.gameObject.SetActive(true);
            joinButton.gameObject.SetActive(true);
            joinInput.gameObject.SetActive(true);
            codeText.gameObject.SetActive(true);

        }
    }
}

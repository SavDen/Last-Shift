using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UINetworkManager : MonoBehaviour
{
    [SerializeField] private Button host, client, server;

    private void Start()
    {
        host.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
        client.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
        server.onClick.AddListener(() => NetworkManager.Singleton.StartServer());
    }
}

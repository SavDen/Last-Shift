using FishNet.Managing;
using FishNet.Transporting;
using UnityEngine;

public class ServerStarter : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;

    private void OnEnable()
    {
        if (networkManager != null)
        {
            networkManager.ServerManager.OnServerConnectionState += OnServerStateChanged;
        }
    }

    private void Start()
    {
        if (networkManager == null)
        {
            networkManager = FindObjectOfType<NetworkManager>();
        }

        if (networkManager != null)
        {
            networkManager.ServerManager.StartConnection();
        }
        else
        {
            Debug.LogError("NetworkManager не найден!");
        }
    }

    private void OnServerStateChanged(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            Debug.Log("✅ Сервер запущен!");
        }
    }

    private void OnDisable()
    {
        if (networkManager != null)
        {
            networkManager.ServerManager.OnServerConnectionState -= OnServerStateChanged;
        }
    }
}
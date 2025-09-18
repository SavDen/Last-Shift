using FishNet.Managing;

using UnityEngine;

public class RemoteConnector : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;


    public void ConnectToRemoteServer()
    {
        // Подключаемся к вашему серверу
        if(networkManager.ClientManager.StartConnection("77.105.168.43", 7777))
        {
            Debug.Log($"Connecting to remote server: 77.105.168.43:7777");
        }


    }
}
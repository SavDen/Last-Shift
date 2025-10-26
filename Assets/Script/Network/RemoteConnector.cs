using FishNet.Managing;
using FishNet;
using UnityEngine;
using FishNet.Object;

public class RemoteConnector : MonoBehaviour
{
    private NetworkManager networkManager;

    private void Start()
    {
        networkManager = FindAnyObjectByType<NetworkManager>();
    }

    public void Connect()
    {
        // Подключаемся к вашему серверу
        if(networkManager.ClientManager.StartConnection())
        {
            print("подклоючение есть");
            SceneManagers.LoadGame();
        }
        else
        {
            print("подключения нет");
            //return false;
        }



    }
}
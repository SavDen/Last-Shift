using FishNet.Managing;
using FishNet;
using UnityEngine;

public class RemoteConnector : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;


    public bool Connect()
    {
        // Подключаемся к вашему серверу
        if(networkManager.ClientManager.StartConnection("77.105.168.43", 7777))
        {
            print("подклоючение есть");
            return true;
        }
        else
        {
            print("подключения нет");
            return false;
        }



    }
}
using Unity.Netcode;
using UnityEngine;

public class ClientConnect : MonoBehaviour
{
    public void Client()
    {
        NetworkManager.Singleton.StartClient();
    }
}

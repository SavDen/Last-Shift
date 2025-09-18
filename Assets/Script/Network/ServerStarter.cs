using FishNet.Managing;
using UnityEngine;

public class ServerStarter : MonoBehaviour
{
    public NetworkManager networkManager;

    void Start()
    {
        var transport = networkManager.TransportManager.Transport;
        transport.SetServerBindAddress("0.0.0.0", FishNet.Transporting.IPAddressType.IPv4);
        transport.SetPort(7777);

        networkManager.ServerManager.StartConnection();
    }
}

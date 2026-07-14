using FishNet;
using UnityEngine;

public class GameNetworkManager : MonoBehaviour // ← Простой MonoBehaviour, не NetworkBehaviour
{
    public void ExitGame()
    {
        // if (InstanceFinder.IsServer)
        // {
        //     // Хост/сервер - отключаем всех
        //     InstanceFinder.ServerManager.StopConnection(true);
        // }
        // else if (InstanceFinder.IsClient)
        // {
        //     // Клиент - отключаем только себя
        //     InstanceFinder.ClientManager.StopConnection();
        // }
        
        SteamLobbyManager.LeaveLobby();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Component.Spawning;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using Steamworks;
using TMPro;
using UnityEngine;


public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyNameText;
    [SerializeField] private List<Transform> podiumPlace;
    [SerializeField] private NetworkObject playerPrefab;
    
    // private readonly List<NetworkConnection> _playerConnection = new();
    // private readonly Dictionary<NetworkConnection, NetworkConnection> _lobbyPlayers = new();

    private readonly List<LobbyPlayerState> _players = new();

    #region Subscriptions

    public override void OnStartServer()
    {
        base.OnStartServer();
        print("сервер старт");
        InstanceFinder.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState; //подписка на удаленное подключение 

        foreach (NetworkConnection connection  //поиск по уже готовым подключениям (хост)
                 in InstanceFinder.ServerManager.Clients.Values)
        {
            RegisterPlayer(connection);
        }
    }
    
    private void OnDisable()
    {
        InstanceFinder.ServerManager.OnRemoteConnectionState -= OnRemoteConnectionState;
    }

    private void OnRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs stateArgs)
    {
        if (stateArgs.ConnectionState == RemoteConnectionState.Started)
        {
            RegisterPlayer(conn);
        }
        
        if (stateArgs.ConnectionState == RemoteConnectionState.Stopped)
        {
            DespawnPlayer(conn);
        }
    }


    #endregion
    
    public void StartGame()
    {
        if (IsServer)
        {
            print("starting game");
            LoadGameScene();
        }
    }
    
    private void LoadGameScene()
    {
        foreach (var conn in _players)
        {
            if (conn.LobbyObject != null)
            {
                InstanceFinder.ServerManager.Despawn(conn.LobbyObject);
            }
        }
        SceneLoadData sceneLoadData = new SceneLoadData("Game")
        {
            ReplaceScenes = ReplaceOption.All
        };
        
        InstanceFinder.SceneManager.LoadGlobalScenes(sceneLoadData);

    }
    
    private void RegisterPlayer(NetworkConnection connection)
    {
        if (connection == null)
            return;
        
        if(HasPlayer(connection))
            return;
        
        SpawnPlayer(connection);
    }


    private void SpawnPlayer(NetworkConnection conn)
    {
        int podiumIndex = FindPodiumIndex();

        if (podiumIndex < 0)
        {
            Debug.LogWarning("Нет свободного подиума.");
            return;
        }
        
        var spwnPos = podiumPlace[podiumIndex].position;

        NetworkObject playerObj = Instantiate(playerPrefab, spwnPos, Quaternion.identity);
    
        InstanceFinder.ServerManager.Spawn(playerObj, conn);
        
        AddPlayer(conn, playerObj, podiumIndex);

    }
    
    private void DespawnPlayer(NetworkConnection conn) //dis дисконект от сессии 
    {
        LobbyPlayerState player = FindPlayer(conn);

        if (player == null)
            return;
        
        if (player.LobbyObject != null)
        {
            InstanceFinder.ServerManager.Despawn(player.LobbyObject);
        }

        _players.Remove(player);
    }

    #region Network

    private void AddPlayer(NetworkConnection conn, NetworkObject player, int podiumIndex)
    {
        _players.Add(new LobbyPlayerState(conn, player, podiumIndex));
    }
    
    public void Leavelobby()
    {
        SteamLobbyManager.LeaveLobby();
    }

    public void InviteSteam()
    {
        SteamFriends.ActivateGameOverlay("Friends");
    }

    //[ServerRpc(RequireOwnership = false)]
    public void SelectClass(PlayerData playerData, NetworkConnection conn = null)
    {
        // if (conn != null && conn.FirstObject != null)
        // {
        //     
        //     conn.FirstObject.GetComponent<PlayerController>().InitPlayer(playerData);
        //     
        //     InstanceFinder.ServerManager.Despawn(conn.FirstObject);
        //     
        //     int playerIndex = _playerConnection.FindIndex(c => c.ClientId == conn.ClientId);
        //     var spwnPos = podiumPlace[playerIndex].position;
        //
        //     NetworkObject playerObj = Instantiate(playerData, spwnPos, Quaternion.identity);
        //    
        //     InstanceFinder.ServerManager.Spawn(playerObj, conn);
        //     
        // }
    }

    #endregion
    
    #region Tools

    private LobbyPlayerState FindPlayer(NetworkConnection connection)
    {
        return _players.Find(player => player.Connection == connection);
    }

    private bool HasPlayer(NetworkConnection connection)
    {
        return _players.Exists(player => player.Connection == connection);
    }
    
    private int FindPodiumIndex()
    {
        for (int i = 0; i < podiumPlace.Count; i++)
        {
            bool isOccupied = _players.Exists(player => player.PodiumIndex == i);

            if (!isOccupied)
                return i;
        }

        return -1;
    }

    #endregion
}
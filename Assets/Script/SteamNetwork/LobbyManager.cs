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
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyNameText;
    [SerializeField] private List<Transform> podiumPlace;
    [SerializeField] private NetworkObject playerPrefab;
    
    private readonly List<NetworkConnection> _playerConnection = new();

    public override void OnStartServer()
    {
        base.OnStartServer();
        print("сервер старт");
        InstanceFinder.ServerManager.OnRemoteConnectionState += OnClientConnection;
        SpawnPlayer(InstanceFinder.ServerManager.Clients.Values.First());
    }

    private void OnDisable()
    {
        InstanceFinder.ServerManager.OnRemoteConnectionState -= OnClientConnection;
    }

    private void OnClientConnection(NetworkConnection conn, RemoteConnectionStateArgs stateArgs)
    {
        if (stateArgs.ConnectionState == RemoteConnectionState.Started)
        {
            SpawnPlayer(conn);
        }
        
        if (stateArgs.ConnectionState == RemoteConnectionState.Stopped)
        {
            DespawnPlayer(conn);
        }
    }

    public void StartGame()
    {
        if (IsServer)
        {
            print("starting game");
            StartGameServerRPC();
        }
    }
    
    private void StartGameServerRPC(NetworkConnection conn = null)
    {
        LoadGameScene();
    }
    
    private void LoadGameScene()
    {
        foreach (var conn in _playerConnection.ToList())
        {
            if (conn.FirstObject != null)
            {
                InstanceFinder.ServerManager.Despawn(conn.FirstObject);
            }
        }
        SceneLoadData sceneLoadData = new SceneLoadData("Game")
        {
            ReplaceScenes = ReplaceOption.All
        };
        
        InstanceFinder.SceneManager.LoadGlobalScenes(sceneLoadData);

    }


    private void SpawnPlayer(NetworkConnection conn)
    {
        AddPlayer(conn);
        int playerIndex = _playerConnection.FindIndex(c => c.ClientId == conn.ClientId);
        var spwnPos = podiumPlace[playerIndex].position;

        NetworkObject playerObj = Instantiate(playerPrefab, spwnPos, Quaternion.identity);
    
        Debug.Log($"До спавна: conn.ClientId = {conn.ClientId}, playerObj.Owner = {playerObj.Owner?.ClientId}");
    
        InstanceFinder.ServerManager.Spawn(playerObj, conn);
        
        print($"{InstanceFinder.ServerManager.Clients.Count}");
    
        Debug.Log($"После спавна: conn.ClientId = {conn.ClientId}, playerObj.Owner = {playerObj.Owner?.ClientId}");

    }
    
    private void DespawnPlayer(NetworkConnection conn)
    {
        LeavePlayer(conn);
        InstanceFinder.ServerManager.Despawn(conn.FirstObject);
    }

    private void AddPlayer(NetworkConnection conn)
    {
        if (!_playerConnection.Contains(conn))
            _playerConnection.Add(conn);
    }

    private void LeavePlayer(NetworkConnection conn)
    {
        _playerConnection.Remove(conn);
    }
   
    public void Leavelobby()
    {
        BootstrapManager.LeaveLobby();
    }

    public void InviteSteam()
    {
        SteamFriends.ActivateGameOverlay("Friends");
    }
}
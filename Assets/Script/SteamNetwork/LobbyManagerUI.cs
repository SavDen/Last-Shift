using System;
using FishNet.Object;
using UnityEngine;

public class LobbyManagerUI : NetworkBehaviour
{
    private LobbyManager _lobbyManager;

    private void Awake()
    {
        _lobbyManager = GetComponent<LobbyManager>();
    }

    public void SelectClass(int id)
    {
        _lobbyManager.SelectClass(id);
    }
    
}

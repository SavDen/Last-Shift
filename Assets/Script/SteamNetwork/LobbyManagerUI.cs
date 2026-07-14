using System;
using UnityEngine;

public class LobbyManagerUI : MonoBehaviour
{
    private LobbyManager _lobbyManager;

    private void Awake()
    {
        _lobbyManager = GetComponent<LobbyManager>();
    }

    public void SelectPlayer(PlayerData playerData)
    {
        _lobbyManager.SelectClass(playerData);
    }
    
}

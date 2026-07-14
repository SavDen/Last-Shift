using System;
using FishNet.Managing;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamLobbyManager : MonoBehaviour
{
    public static SteamLobbyManager instance;
    
    [SerializeField] private NetworkManager _networkManager;
    [SerializeField] private FishySteamworks.FishySteamworks _steamworks;  
    
    private Callback<LobbyCreated_t> LobbyCreated;
    private Callback<GameLobbyJoinRequested_t> JoinRequest;
    private Callback<LobbyEnter_t> LobbyEnter;
    
    public static ulong CurrentLobbyID;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("DUPLICATE SteamLobbyManager destroyed!");
            Destroy(gameObject);
            return;
        }
    
        instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("SteamLobbyManager made persistent");
    }

    private void Start()
    { 
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
    }
    
    public static void LeaveLobby()
    {
        instance.OnLeaveLobby();
    }

    private void OnLeaveLobby()
    {
        SteamMatchmaking.LeaveLobby(new CSteamID(CurrentLobbyID));
        CurrentLobbyID = 0;
 
        
        _steamworks.StopConnection(false);
        if(_networkManager.IsServer)
            _steamworks.StopConnection(true);

        LoadScene("Menu");
    }
    
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        print("Start Lobby Creation: " + callback.m_eResult);

        CurrentLobbyID = callback.m_ulSteamIDLobby;
        SteamMatchmaking.SetLobbyData(new CSteamID(CurrentLobbyID), "Host", SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(CurrentLobbyID), "name", SteamFriends.GetPersonaName().ToString() + "s lobby");
        _steamworks.SetClientAddress(SteamUser.GetSteamID().ToString());
        _steamworks.StartConnection(true);
        print("Lobby created OK");
        
        LoadScene("Lobby");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        
        _steamworks.SetClientAddress(SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "Host"));
        _steamworks.StartConnection(false);
        
        
        if(_networkManager.IsServer) return;
        print("Lobby Enter Success");
        LoadScene("Lobby");
    }
}

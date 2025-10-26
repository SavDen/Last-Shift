using System;
using FishNet.Managing;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapManager : MonoBehaviour
{
    public static BootstrapManager instance;
    
    [SerializeField] private string _nameScene;
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
            Debug.Log("DUPLICATE BootstrapManager destroyed!");
            Destroy(gameObject);
            return;
        }
    
        instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("BootstrapManager made persistent");
    }

    private void Start()
    { 
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    public void GotoMenu()
    {
        SceneManager.LoadScene(_nameScene); 
    }
    
    private void GoToOnlineLobby()
    {
        SceneManager.LoadScene("Lobby");
        
    }

    public static void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
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
        
        GoToOnlineLobby();
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
        
        GoToOnlineLobby();
    }

}

using System;
using System.Collections.Generic;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRoomManager : NetworkBehaviour
{
    [Header("NetworkManager")]
    public NetworkManager networkManager;

    [Header("Data")]
    public List<RoomData> activeRooms = new();

    private void Start()
    {
        print("Room instal");
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreateRoom()
    {
        var roomCode = GenerateRommCode();

        var newRoom = new RoomData(roomCode);

        activeRooms.Add(newRoom);

        EventBusRooms.OnRoomCreated?.Invoke(roomCode);

        SceneManagers.LoadLobby();

    }

    private string GenerateRommCode()
    {
        return Random.Range(1000, 9999).ToString();
    }

    [ServerRpc(RequireOwnership = false)]
    public void JoinRoom(string roomCode)
    {
        var roomJoin = activeRooms.Find(r => r.roomCode == roomCode);

        if (roomJoin == null)
        {
            print($"Комната с кодом {roomCode} не найдена");
            return;
        }

        if(roomJoin.currentPlayers >= roomJoin.maxPlayers)
        {
            print("Комната переполнена");
            return;
        }

        if(roomJoin.isGameStarted)
        {
            print("Игра в комнате уже началась");
            return;
        }

        roomJoin.currentPlayers++;

        EventBusRooms.OnRoomJoined?.Invoke(roomCode);


    }


}

//using System.Linq;
//using System.Collections.Generic;
//using FishNet.Managing.Server;
//using FishNet.Object;
//using FishNet.Object.Synchronizing;
//using UnityEngine;
//using System;

//public class VRManager : NetworkBehaviour
//{
//    [Header("Settings Rooms")]
//    public int maxRooms = 10;
//    public int defaultMaxPlayers = 5;

//    [Header("List Data")]
//    public List<RoomData> activeRooms = new List<RoomData>();
//    public Dictionary<int, List<NetworkObject>> roomPlayers = new();
//    public Dictionary<NetworkObject, int> playerRooms = new Dictionary<NetworkObject, int>(); // В какой комнате находится игрок


//    public List<NetworkObject> allPlayers = new();

//    private int nextRoomId = 1;

//    private void Start()
//    {
//        Debug.Log("VirtualRoomManager инициализирован на клиенте");
//    }

//    [ServerRpc(RequireOwnership = false)]
//    public void CreateRoom(string roomName, string password, string hostName)
//    {
//        if (activeRooms.Count >= maxRooms)
//        {
//            print("MaxRooms");
//            return;

//        }

//        if (activeRooms.Any(room => room.roomName == roomName))
//        {
//            print($"A room with the name '{roomName}' already exists!");
//            return;
//        }

//        RoomData newRoom = new RoomData(roomName, password, hostName, nextRoomId++);

//        activeRooms.Add(newRoom);

//        UpdateRoomList();

//        EventBusRooms.InvokeRoomCreated(newRoom);

//        Debug.Log($"Создана комната: '{newRoom.roomName}' (ID: {newRoom.roomId})");

//    }

//    [ServerRpc(RequireOwnership = false)]
//    public void JoiunRoom(int roomId, string password, string playerName)
//    {
//        RoomData room = activeRooms.Find(r => r.roomId == roomId);

//        if (room == null)
//        {
//            Debug.Log($"Комната с ID {roomId} не найдена!");
//            return;
//        }

//        // Проверяем, не заполнена ли комната
//        if (room.currentPlayers >= room.maxPlayers)
//        {
//            Debug.Log($"Комната '{room.roomName}' заполнена!");
//            return;
//        }

//        // Проверяем, не началась ли уже игра
//        if (room.isGameStarted)
//        {
//            Debug.Log($"Игра в комнате '{room.roomName}' уже началась!");
//            return;
//        }

//        // Проверяем пароль для приватных комнат
//        if (room.isPrivate && room.password != password)
//        {
//            Debug.Log($"Неверный пароль для комнаты '{room.roomName}'!");
//            return;
//        }

//        // Получаем NetworkObject игрока
//        NetworkObject playerObject = GetPlayerNetworkObject();
//        if (playerObject == null)
//        {
//            Debug.Log("Не удалось получить NetworkObject игрока!");
//            return;
//        }

//        // Проверяем, не находится ли игрок уже в какой-то комнате
//        if (playerRooms.ContainsKey(playerObject))
//        {
//            Debug.Log($"Игрок уже находится в комнате {playerRooms[playerObject]}!");
//            return;
//        }

//        // Добавляем игрока в комнату
//        roomPlayers[roomId].Add(playerObject);
//        playerRooms[playerObject] = roomId;

//        // Обновляем данные комнаты
//        room.currentPlayers++;
//        room.isFull = (room.currentPlayers >= room.maxPlayers);

//        UpdateRoomList();

//        EventBusRooms.InvokeRoomJoined(room);
//        EventBusRooms.InvokePlayerJoinedRoom(playerObject, roomId);

//        Debug.Log($"Игрок '{playerName}' подключился к комнате '{room.roomName}'");


//    }

//    [ServerRpc(RequireOwnership = false)]
//    public void LeaveRoom(string playerName)
//    {
//        NetworkObject playerObject = GetPlayerNetworkObject();
//        if (playerObject == null) return;

//        if (!playerRooms.ContainsKey(playerObject))
//        {
//            Debug.Log($"Игрок '{playerName}' не находится ни в одной комнате!");
//            return;
//        }

//        int roomId = playerRooms[playerObject];
//        playerRooms.Remove(playerObject);

//        RoomData room = activeRooms.Find(r => r.roomId == roomId);

//        if (room == null) return;

//        // Удаляем игрока из комнаты
//        roomPlayers[roomId].Remove(playerObject);
//        playerRooms.Remove(playerObject);

//        // Обновляем данные комнаты
//        room.currentPlayers--;
//        room.isFull = false;

//        // Если комната пустая, удаляем её
//        if (room.currentPlayers <= 0)
//        {
//            activeRooms.Remove(room);
//            roomPlayers.Remove(roomId);
//            EventBusRooms.InvokeRoomDeleted(room);
//            Debug.Log($"Комната '{room.roomName}' удалена (пустая)");
//        }
//        else
//        {
//            // Обновляем список комнат
//            UpdateRoomList();
//        }

//        // Уведомляем о покидании комнаты
//        EventBusRooms.InvokeRoomLeft(room);
//        EventBusRooms.InvokePlayerLeftRoom(playerObject, roomId);

//        Debug.Log($"Игрок '{playerName}' покинул комнату '{room.roomName}'");
//    }

//    // Начало игры в комнате (вызывается клиентом)
//    [ServerRpc(RequireOwnership = false)]
//    public void StartGame(int roomId)
//    {
//        RoomData room = GetRoomById(roomId);
//        if (room == null) return;


//        // Проверяем, что игра еще не началась
//        if (room.isGameStarted)
//        {
//            Debug.Log("Игра уже началась!");
//            return;
//        }

//        // Отмечаем, что игра началась
//        room.isGameStarted = true;

//        // Уведомляем всех клиентов
//        RpcNotifyGameStarted(roomId);

//        // Уведомляем о начале игры
//        EventBusRooms.InvokeGameStarted(room);

//        Debug.Log($"Игра началась в комнате '{room.roomName}'");
//    }

//    private void RpcNotifyGameStarted(int roomId)
//    {
//        Debug.Log($"Игра началась в комнате {roomId}");
//    }

//    private void UpdateRoomList()
//    {
//        // Отправляем обновленный список всем клиентам
//        RpcUpdateRoomList(activeRooms.ToArray());
//    }

//    // RPC для отправки списка комнат клиентам
//    [ObserversRpc]
//    private void RpcUpdateRoomList(RoomData[] rooms)
//    {
//        activeRooms.Clear();

//        // Добавляем новые комнаты
//        activeRooms.AddRange(rooms);

//        // Уведомляем UI об обновлении
//        EventBusRooms.InvokeRoomListUpdated(activeRooms);

//        Debug.Log($"Список комнат обновлен. Всего комнат: {activeRooms.Count}");
//    }

//    // Получение списка всех комнат
//    public List<RoomData> GetAllRooms()
//    {
//        return activeRooms;
//    }

//    // Поиск комнаты по ID
//    public RoomData GetRoomById(int roomId)
//    {
//        return activeRooms.Find(room => room.roomId == roomId);
//    }

//    // Поиск комнаты по названию
//    public RoomData GetRoomByName(string roomName)
//    {
//        return activeRooms.Find(room => room.roomName == roomName);
//    }

//    // Получение NetworkObject игрока
//    private NetworkObject GetPlayerNetworkObject()
//    {
//        // Ищем NetworkObject среди всех игроков
//        foreach (var player in allPlayers)
//        {
//            if (player != null && player.IsOwner)
//            {
//                return player;
//            }
//        }

//        // Если не нашли среди всех игроков, ищем в текущем объекте
//        NetworkObject currentObject = GetComponent<NetworkObject>();
//        if (currentObject != null && currentObject.IsOwner)
//        {
//            return currentObject;
//        }

//        return null;
//    }

//    // Регистрация игрока в системе (вызывается при подключении)
//    [ServerRpc(RequireOwnership = false)]
//    public void RegisterPlayer(NetworkObject playerObject)
//    {
//        if (!allPlayers.Contains(playerObject))
//        {
//            allPlayers.Add(playerObject);
//            EventBusRooms.InvokePlayerRegistered(playerObject);
//        }
//    }

//    [ServerRpc(RequireOwnership = false)]
//    public void UnregisterPlayer(NetworkObject playerObject)
//    {
//        if (allPlayers.Contains(playerObject))
//        {
//            allPlayers.Remove(playerObject);

//            // Если игрок был в комнате, удаляем его оттуда
//            if (playerRooms.ContainsKey(playerObject))
//            {
//                int roomId = playerRooms[playerObject];
//                roomPlayers[roomId].Remove(playerObject);
//                playerRooms.Remove(playerObject);

//                RoomData room = GetRoomById(roomId);
//                if (room != null)
//                {
//                    room.currentPlayers--;
//                    room.isFull = false;

//                    if (room.currentPlayers <= 0)
//                    {
//                        activeRooms.Remove(room);
//                        roomPlayers.Remove(roomId);
//                        EventBusRooms.InvokeRoomDeleted(room);
//                    }
//                }

//                UpdateRoomList();
//            }

//            EventBusRooms.InvokePlayerUnregistered(playerObject);
//        }
//    }
//}

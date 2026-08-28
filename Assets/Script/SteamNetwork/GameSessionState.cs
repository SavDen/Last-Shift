using System.Collections.Generic;
using FishNet.Connection;
using UnityEngine;

public class GameSessionState: MonoBehaviour
    {
        public static GameSessionState instance;

        public void Awake()
        {
            instance = this;
        }

        private readonly Dictionary<NetworkConnection, PlayerData> _classPlayer = new();

        public void SetClass(NetworkConnection conn, PlayerData playerData)
        {
            if (conn == null || playerData == null)
            {
                print($@"Is Null Error. \n {conn} , {playerData}");
                return;
            }
            
            _classPlayer[conn] =  playerData;
        }

        public PlayerData GetClass(NetworkConnection conn)
        { 
            _classPlayer.TryGetValue(conn, out var playerData);
            return playerData;
        }
        
        public void DeleteClass(NetworkConnection conn)
        {
            _classPlayer.Remove(conn);
        }
        
    }

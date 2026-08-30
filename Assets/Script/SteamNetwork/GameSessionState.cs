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

        private readonly Dictionary<int, int> _classPlayer = new();

        public void SetClass(int conn, int playerData)
        {
            if (conn == null || playerData == null)
            {
                print($@"Is Null Error. \n {conn} , {playerData}");
                return;
            }
            
            _classPlayer[conn] =  playerData;
        }

        public int GetClass(int conn)
        { 
            _classPlayer.TryGetValue(conn, out var getClass);
            return getClass;
        }
        
        public void DeleteClass(int conn)
        {
            _classPlayer.Remove(conn);
        }
        
    }

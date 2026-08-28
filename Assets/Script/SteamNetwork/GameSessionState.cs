using System.Collections.Generic;
using FishNet.Connection;

    public class GameSessionState
    {
        private readonly Dictionary<int, int> _classId = new();

        private void SetClass(NetworkConnection conn, int classIndex)
        {
            _classId[conn.ClientId] =  classIndex;
        }
        
    }

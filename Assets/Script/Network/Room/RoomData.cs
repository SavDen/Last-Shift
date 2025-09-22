using System;

[Serializable]
public class RoomData
{
    public string roomCode;
    public int maxPlayers = 4;
    public int currentPlayers = 1;
    public bool isGameStarted = false;

    public RoomData()
    {

    }

    public RoomData(string roomCode): this()
    {
        this.roomCode = roomCode;
    }
}

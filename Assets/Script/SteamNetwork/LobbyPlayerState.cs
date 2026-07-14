using FishNet.Connection;
using FishNet.Object;

public sealed class LobbyPlayerState
{
      public NetworkConnection Connection { get; }

      public NetworkObject LobbyObject;
      
      public int PodiumIndex { get; }
      public int ClassIndex;
      public bool IsReady;

      public LobbyPlayerState(NetworkConnection conn, NetworkObject player,  int podiumIndex)
      {
            Connection = conn;
            LobbyObject = player;
            PodiumIndex = podiumIndex;
      }
}
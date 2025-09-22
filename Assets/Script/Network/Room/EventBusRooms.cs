using System;

public static class EventBusRooms
{
    public static Action<string> OnRoomCreated;
    public static Action<string> OnRoomJoined;
}
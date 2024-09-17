using System.Collections.Generic;
using Photon.Realtime;

public static class RuntimeVariables
{
    public static bool PhotonConnected { get; set; }
    public static List<RoomInfo> Rooms { get; } = new();
}
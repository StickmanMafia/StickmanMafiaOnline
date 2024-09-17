using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomMonitor : MonoBehaviourPunCallbacks
{
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RuntimeVariables.Rooms.Clear();
        RuntimeVariables.Rooms.AddRange(roomList);
        GameObject.Find("UIDocument").GetComponent<MainMenuBehaviour>().FillRooms();
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MainLocation");
    }
}
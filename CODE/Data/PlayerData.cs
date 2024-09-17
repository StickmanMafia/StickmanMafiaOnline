using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerData
{
    public GameObject GameObject { get; set; }
    public PhotonView PhotonView { get; set; }
    public PlayerController Controller { get; set; }
    public Player PhotonPlayer { get; set; }
    public int ActorNumber { get; set; }
    public bool IsMine => PhotonView.IsMine;
    public string Nickname => PhotonPlayer.NickName;
    public string id => PhotonPlayer.NickName;
}

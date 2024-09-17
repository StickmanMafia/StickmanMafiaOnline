using Photon.Pun;
using UnityEngine;

public class PlayerParentFetchService
{
    public static GameObject Fetch() {
        return GameObject.Find("boat").transform.Find($"table-{PhotonNetwork.CurrentRoom.MaxPlayers}-seats").gameObject;
    }
}

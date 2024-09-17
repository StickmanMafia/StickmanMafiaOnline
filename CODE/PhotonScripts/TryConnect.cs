using Photon.Pun;
using UnityEngine;

public class TryConnect : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting connection");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        RuntimeVariables.PhotonConnected = true;
        Debug.Log("Connected!");
    }
}
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
[System.Serializable]
public class PlayerActivityData
{
    public int ActorNumber;
    public bool Active;

    public PlayerActivityData(int actorNumber, bool active)
    {
        ActorNumber = actorNumber;
        Active = active;
    }
}

public class ActivityChecker : MonoBehaviourPunCallbacks
{
    private float checkInterval = 2f; // Интервал проверки активности игроков
    private float lastCheckTime = 0f;
    public List<PlayerActivityData> AllPlayers = new List<PlayerActivityData>();

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                AllPlayers.Add(new PlayerActivityData(player.ActorNumber, false));
            }

            StartCoroutine(Routine());
        }
    }

    public IEnumerator Routine()
    {
        foreach (PlayerActivityData playerData in AllPlayers)
        {
            playerData.Active = false;
        }

        Debug.Log("Coroutine started");

        photonView.RPC("RequestActivityCheck", RpcTarget.All);

        yield return new WaitForSeconds(2);
        CheckPlayerActivity();
        yield return new WaitForSeconds(1);
        StartCoroutine(Routine());
    }

    [PunRPC]
    private void RequestActivityCheck()
    {
        photonView.RPC("SendActivityResponse", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    private void SendActivityResponse(int actorNumber)
    {
       // Debug.Log("ОТВЕТИЛ " + actorNumber);
        // Обработка ответа от игрока
        if (PhotonNetwork.IsMasterClient)
        {
            PlayerActivityData playerData = AllPlayers.Find(p => p.ActorNumber == actorNumber);
            if (playerData != null)
            {
                playerData.Active = true;
            }
        }
    }

    private void CheckPlayerActivity()
    {
        for (int i = 0; i < AllPlayers.Count; i++)
        {
            if (!AllPlayers[i].Active)
            {
                OnPlayerLeft(FindPlayerByActorNumber(AllPlayers[i].ActorNumber), i);
            }
        }
    }

    private Player FindPlayerByActorNumber(int actorNumber){
        foreach(Player player in PhotonNetwork.PlayerList){
            if(player.ActorNumber == actorNumber){
                return player;
            }
        }

        return null;
    }
    public void OnPlayerLeft(Player otherPlayer, int id)
    {
        AllPlayers.RemoveAt(id);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // When a new player enters the room, add a new PlayerData to the list
        AllPlayers.Add(new PlayerActivityData(newPlayer.ActorNumber, true));
    }
  

}

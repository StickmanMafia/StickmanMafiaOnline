using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public static class PlayerFetchService
{
    public static IEnumerable<PlayerData> FetchPlayerData(GameObject parent)
    {
        

        var allGameObjects = parent.transform.Cast<Transform>()
            .Where(n => n.gameObject.CompareTag("player-object"))
            .Select(child => child.gameObject).ToList();
        var collection = new List<PlayerData>();

        foreach (var player in allGameObjects)
        {
            var newPlayerData = new PlayerData();
            var photonView = player.GetComponent<PhotonView>();
            newPlayerData.GameObject = player;
            newPlayerData.PhotonView = photonView;
            newPlayerData.PhotonPlayer = photonView.Owner;
            newPlayerData.ActorNumber = photonView.Owner.ActorNumber;
            newPlayerData.Controller = player.GetComponent<PlayerController>();
            
            collection.Add(newPlayerData);
        }

        return collection;
    }
}

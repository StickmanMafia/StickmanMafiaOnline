using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

public abstract class PUNController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    protected LocalizedStringTable LocalizedStringTable;

    protected GameObject PlayerParent;
    protected IEnumerable<PlayerData> Players;

    protected PlayerData Me => Players.First(n => n.IsMine);
    protected PlayerData Master => Players.First(n => n.PhotonPlayer.IsMasterClient);

    protected virtual void Start() => PlayerParent = PlayerParentFetchService.Fetch();

    public string GetLocalizedString(string key) => LocalizedStringTable.GetTable().GetEntry(key).Value;
    
    protected void FetchPlayers() => Players = PlayerFetchService.FetchPlayerData(PlayerParent);
}
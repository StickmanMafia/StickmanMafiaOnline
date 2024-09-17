using System.Linq;
using Photon.Pun;
using UnityEngine;

public sealed class Revenge : Perk
{
    private GameObject _playerParent;
    private PlayerData[] _players;

    protected override bool ConditionSatisfied =>
        _players.Any(n => n.Controller.VotingActorNumbers.Contains(PhotonNetwork.LocalPlayer.ActorNumber));
    
    protected override void Execute()
    {
        var target = _players.First(n => n.Controller.VotingActorNumbers.Contains(PhotonNetwork.LocalPlayer.ActorNumber));
        target.PhotonView.RpcSecure("SetParalyzedByRevenge", RpcTarget.AllBuffered, true, true);
        target.PhotonView.RpcSecure("SetActionImpactPrompted", RpcTarget.AllBuffered, true, true);
        Completed = true;
        base.Execute();
    }

    protected override void Start()
    {
        _playerParent = PlayerParentFetchService.Fetch();
        _players = PlayerFetchService.FetchPlayerData(_playerParent).ToArray();
        Completed = false;
    }

    protected override void Update()
    {
        if (!Completed)
            base.Update();
    }
}
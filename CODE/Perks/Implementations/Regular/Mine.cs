using System.Linq;
using Photon.Pun;
using UnityEngine;

public class Mine : Perk
{
    private GameObject _playerParent;
    private PlayerData[] _players;

    protected override bool ConditionSatisfied =>
        _players.Any(n => n.Controller.VotingActorNumbers.Contains(PhotonNetwork.LocalPlayer.ActorNumber));
    
    protected override void Execute()
    {
        var target = _players.First(n => n.Controller.VotingActorNumbers.Contains(PhotonNetwork.LocalPlayer.ActorNumber));
        var me = _players.First(n => n.IsMine);
        
        if (target.Controller.HasMineDetector)
        {
            me.Controller.DisplayMineDetectorMessage(target.Nickname);
            target.PhotonView.RpcSecure("DisplayMineDetectorProtectionMessage", target.PhotonPlayer, true);
            target.PhotonView.RpcSecure("SetHasMineDetector", target.PhotonPlayer, true, false);
            Destroy(gameObject);
            return;
        }
        
        target.PhotonView.RpcSecure("StopVoteStopwatch", target.PhotonPlayer, true);
        target.PhotonView.RpcSecure("SetKilled", RpcTarget.AllBuffered, true, true);
        target.PhotonView.RpcSecure("SetActionImpactPrompted", RpcTarget.AllBuffered, true, true);

        foreach (var player in _players)
            player.PhotonView.RpcSecure("DisplayMineExplodedMessage", player.PhotonPlayer, true, target.Nickname);

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
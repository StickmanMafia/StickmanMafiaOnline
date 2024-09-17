using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class VotingController : Controller
{
    [SerializeField] 
    private GameObject _gameplayControllerObject;

    private bool _voting;

    private GameplayController _gameplayController;

    private IEnumerable<PlayerData> _suitablePlayers;

    protected override void Start()
    {
        base.Start();
        _gameplayController = _gameplayControllerObject.GetComponent<GameplayController>();
        _voting = false;
    }
    
    public void StartVoting()
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            return;
        
        FetchPlayers();
        _suitablePlayers = Players.Where(n => n.Controller.Intact);
        
        if (!_voting)
            _voting = true;
    }

    private void Update()
    {
        if (!_voting) return;
        FetchPlayers();
        _suitablePlayers = Players.Where(n => n.Controller.Intact);
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            return;
        
        if (!VotingFinished()) return;
        
        var mostVotedPlayer = GetMostVotedPlayer();
        Debug.Log(mostVotedPlayer);   //тот за кого проголосовали йоу
        if (mostVotedPlayer == null)
            _gameplayController.ToggleNightForAllPlayers();
        else
            TakeActionOnMostVotedPlayer(mostVotedPlayer);

        _voting = false;
    }

    public bool AnyBadPlayersLeft()
    {
        FetchPlayers();
        return Players.Any(n => n.Controller is {Role: Role.Mafia, Alive: true} or {Role: Role.MafiaBoss, Alive: true});
    }

    private bool VotingFinished()
    {
        foreach (var player in _suitablePlayers.ToList())
        {
            if (!player.Controller.Voting && !player.Controller.VotingActorNumbers.Contains(Numerics.NoId))
                continue;
            
            return false;
        }

        return true;
    }

    private PlayerData GetMostVotedPlayer()
    {
        Debug.Log("эм");
        FetchPlayers();
        var playerTable = Players.ToDictionary(player => player, _ => 0);

        foreach (var (player, _) in playerTable.ToList())
        {
            var allPlayersVotedForThis = new List<GameObject>();

            foreach (var voter in _suitablePlayers.ToList())
            {
                if (voter.Controller.VotingActorNumbers.Contains(player.ActorNumber))
                    allPlayersVotedForThis.Add(voter.GameObject);
            }

            playerTable[player] = allPlayersVotedForThis.Count; 
        }

        var playersOrdered = playerTable.OrderByDescending(n => n.Value);

        for (var i = 0; i < playersOrdered.Count(); i++)
        {
            if (Equals(playersOrdered.ElementAt(i), playersOrdered.First())) continue;
            
            if (playersOrdered.ElementAt(i).Value == playersOrdered.First().Value)
                return null;
        }
        
        return playersOrdered.First().Key;
    }
    
    private void TakeActionOnMostVotedPlayer(PlayerData targetPlayer)
    {
        FetchPlayers();
        
        /*
         * Checked and used in case the player was
         * exploded with a mine perk
         */
        if (!targetPlayer.Controller.Alive)
        {
            _gameplayController.ToggleNightForAllPlayers();
            return;
        }
        
        if (targetPlayer.Controller.HasHelicopter)
        {
            foreach (var player in Players)
                player.PhotonView.RpcSecure("DisplayHelicopterMessage", player.PhotonPlayer, true, targetPlayer.Nickname);

            var anyMafiasLeft = AnyBadPlayersLeft();
            
            if (!anyMafiasLeft)
            {
                foreach (var player in Players) 
                    player.PhotonView.RpcSecure("SetGameOver", RpcTarget.AllBuffered, true, true);
            }
            else
            {
                _gameplayController.ToggleNightForAllPlayers();
            }
            
            return;
        }
        
        targetPlayer.PhotonView.RpcSecure("RevealRole", RpcTarget.AllBuffered, true, targetPlayer.ActorNumber);
    }
}
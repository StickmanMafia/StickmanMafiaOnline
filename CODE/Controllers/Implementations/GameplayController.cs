using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class GameplayController : Controller
{
    [SerializeField]
    private GameObject _cameraObject;

    [SerializeField]
    private GameObject _boatObject;
    
    private BoatMovement _boatMovement;
    private MainCameraController _mainCameraController;
    private StopwatchController _stopwatchController;
    private GameplayUIBehaviour _uiScript;
    
    public bool GameRunning { get; private set; }

    protected override void Start()
    {
        base.Start();
        GameRunning = false;
        _boatMovement = _boatObject.GetComponent<BoatMovement>();
        _mainCameraController = _cameraObject.GetComponent<MainCameraController>();
        _stopwatchController = GameObject.Find("StopwatchController").GetComponent<StopwatchController>();
        _uiScript = GameObject.Find("ui-document").GetComponent<GameplayUIBehaviour>();
    }
    
    public void StartGame()
    {
        GameRunning = true;
        _boatMovement.SpeedUp();
        _mainCameraController.StartFollowingBoat();
        GameObject.Find("ui-document").GetComponent<GameplayUIBehaviour>().UpdateGoodVsBadPanelContents();
        GameObject.Find("ui-document").GetComponent<GameplayUIBehaviour>().GoodVsBadPanelVisible = true;
    }

    // Reserved for the future
    public void AbortGame()
    {
        GameRunning = false;
        GameObject.Find("ui-document").GetComponent<GameplayUIBehaviour>().UpdateGoodVsBadPanelContents();
        GameObject.Find("ui-document").GetComponent<GameplayUIBehaviour>().GoodVsBadPanelVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = true;
    }
    
    public void Kill(PlayerData sender, PlayerData receiver)
    {
        FetchPlayers();
        
        if (PhotonNetwork.CurrentRoom.MaxPlayers == 10)
        {
            var actionQueueInfo = Me.Controller.ActionQueueInfo;
            
            if (actionQueueInfo.AllIds.ElementAt(0) == sender.ActorNumber)
            {
                sender.PhotonView.RpcSecure("SetKilledActorNumber", RpcTarget.AllBuffered, true, receiver.ActorNumber);
                GoQueue();
                return;
            }
            
            if (actionQueueInfo.AllIds.ElementAt(1) == sender.ActorNumber)
            {
                var firstMafia = Players.First(n => n.ActorNumber == actionQueueInfo.AllIds.First());
                
                if (receiver.Controller.Disguised)
                {
                    AbortKill(receiver, new[] { Me, firstMafia });
                    return;
                }
                
                if (firstMafia.Controller.KilledActorNumber != receiver.ActorNumber)
                {
                    firstMafia.PhotonView.RpcSecure("SetKilledActorNumber", RpcTarget.AllBuffered, true,Numerics.NoId);
                    GoQueue();
                    return;
                }
            }
        }
        
        if (PhotonNetwork.CurrentRoom.MaxPlayers == 12)
        {
            var actionQueueInfo = Me.Controller.ActionQueueInfo;
            
            if (actionQueueInfo.AllIds.ElementAt(0) == sender.ActorNumber || actionQueueInfo.AllIds.ElementAt(1) == sender.ActorNumber)
            {
                sender.PhotonView.RpcSecure("SetKilledActorNumber", RpcTarget.AllBuffered, true, receiver.ActorNumber);
                GoQueue();
                return;
            }
            
            if (actionQueueInfo.AllIds.ElementAt(2) == sender.ActorNumber)
            {
                var firstMafia = Players.First(n => n.ActorNumber == actionQueueInfo.AllIds.First());
                var secondMafia = Players.First(n => n.ActorNumber == actionQueueInfo.AllIds.ElementAt(1));
                
                if (receiver.Controller.Disguised)
                {
                    AbortKill(receiver, new[] { Me, firstMafia, secondMafia });
                    return;
                }
                
                if (!(firstMafia.Controller.KilledActorNumber == secondMafia.Controller.KilledActorNumber 
                      && secondMafia.Controller.KilledActorNumber == receiver.ActorNumber))
                {
                    firstMafia.PhotonView.RpcSecure("SetKilledActorNumber", RpcTarget.AllBuffered, true,Numerics.NoId);
                    secondMafia.PhotonView.RpcSecure("SetKilledActorNumber", RpcTarget.AllBuffered, true,Numerics.NoId);
                    GoQueue();
                    return;
                }
            }
        }
        
        _stopwatchController.StopActionStopwatch();
        
        if (receiver.Controller.Disguised)
        {
            AbortKill(receiver, new[] { Me });
            return;
        }
        
        receiver.PhotonView.RpcSecure("SetKilled", RpcTarget.AllBuffered, true, true);
        receiver.PhotonView.RpcSecure("SetActionImpactPrompted", RpcTarget.AllBuffered, true, true);
        
        if (receiver.Controller.Witnessed)
        {
            var witness = Players.First(n => n.Controller.Role == Role.Witness);
            witness.PhotonView.RpcSecure("DisplayWitnessReportMessage", witness.PhotonPlayer, true, sender.Nickname);
        }
    }

    public void Heal(PlayerData receiver)
    {
        _stopwatchController.StopActionStopwatch();
        receiver.PhotonView.RpcSecure("SetHealed", RpcTarget.AllBuffered, true, true);
        receiver.PhotonView.RpcSecure("SetActionImpactPrompted", RpcTarget.AllBuffered, true, true);
    }

    public void Paralyze(PlayerData receiver)
    {
        _stopwatchController.StopActionStopwatch();
        receiver.PhotonView.RpcSecure("SetParalyzed", RpcTarget.AllBuffered, true, true);
        receiver.PhotonView.RpcSecure("SetActionImpactPrompted", RpcTarget.AllBuffered, true, true);
    }

    public void Witness(PlayerData receiver)
    {
        _stopwatchController.StopActionStopwatch();
        receiver.PhotonView.RpcSecure("SetWitnessed", RpcTarget.AllBuffered, true, true);
    }

    public void Investigate(PlayerData receiver)
    {
        _stopwatchController.StopActionStopwatch();
        FetchPlayers();
        
        if (receiver.Controller.Disguised)
        {
            Me.Controller.DisplayDisguisedMessage(receiver.Nickname, "player_disguised_police");
            GoQueue();
            return;
        }
        
        if (Players.Count(n => n.Controller.Alive) == 2 && Players.Any(n => n.Controller.Role == Role.Mafia))
            receiver.PhotonView.RpcSecure("RevealRole", RpcTarget.AllBuffered, true, receiver.ActorNumber);
        else
            receiver.Controller.RevealRoleTemporary();
    }

    public void Vote(PlayerData sender, PlayerData receiver)
{
    var playerExists = receiver.PhotonView != null;

    if (!playerExists)
    {
        sender.PhotonView.RpcSecure("SetVotedActorNumbers", RpcTarget.AllBuffered, true, new[] { Numerics.PlayerNotExists });
        Debug.Log("отсюда");
        sender.PhotonView.RpcSecure("SetVoting", RpcTarget.AllBuffered, true, false);
    }
    else
    {
        /*
         * VotingActorNumber will always have at least 1 item in it.
         * This conditions processes 1 item being in the array.
         */
        if (sender.Controller.VotingActorNumbers.First() == Numerics.NoId)
        {
            sender.PhotonView.RpcSecure("SetVotedActorNumbers", RpcTarget.AllBuffered, true, new[] { receiver.ActorNumber });

            if (!sender.Controller.CanVoteTwice){
                Debug.Log("отсюда");
                sender.PhotonView.RpcSecure("SetVoting", RpcTarget.AllBuffered, true, false);

            }
        }
        /*
         * This condition processes 2 items being in the array.
         * Only players with CanVoteTwice = true (DoubleVoice perk)
         * can reach this statement.
         */
        else
        {
            var currentlyAddedActorNumber = sender.Controller.VotingActorNumbers.First();
            sender.PhotonView.RpcSecure("SetVotedActorNumbers", RpcTarget.AllBuffered, true, new[] { currentlyAddedActorNumber, receiver.ActorNumber });
            Debug.Log("отсюда");
            sender.PhotonView.RpcSecure("SetVoting", RpcTarget.AllBuffered, true, false);
        }

        _stopwatchController.StopVoteStopwatch();
    }
}

    public void ToggleNightForAllPlayers()
{
    FetchPlayers();

    foreach (var player in Players)
    {
        
        
        player.PhotonView.RpcSecure("SetNightSwitchPrompted", player.PhotonPlayer, true, true);
        player.PhotonView.RpcSecure("SetDaySwitchPrompted", player.PhotonPlayer, true, false);
        player.PhotonView.RpcSecure("SetVotedActorNumbers", RpcTarget.AllBuffered, true, new[] { Numerics.NoId });
//        Debug.Log("ЧТО ЭТООО " + player.ActorNumber);

        if (player.Controller.CanVoteTwice)
            player.PhotonView.RpcSecure("SetCanVoteTwice", RpcTarget.AllBuffered, true, false);

        if (player.Controller.Paralyzed)
            player.PhotonView.RpcSecure("SetParalyzed", RpcTarget.AllBuffered, true, false);

        if (player.Controller.ParalyzedByRevenge)
            player.PhotonView.RpcSecure("SetParalyzedByRevenge", RpcTarget.AllBuffered, true, false);
    }
}

    [MasterOnly]
    private void ToggleDayForAllPlayers()
    {
        FetchPlayers();
        
        foreach (var player in Players)
        {
            player.PhotonView.RpcSecure("SetDaySwitchPrompted", player.PhotonPlayer, true, true);
            player.PhotonView.RpcSecure("SetNightSwitchPrompted", player.PhotonPlayer, true, false);
        }
    }
    
    /*
     * As GameplayController iterates through the list,
     * this index is updated. When the action queue
     * reaches the end, this index is reset to -1.
     */
    [MasterOnly]
    public void ActionQueueForward()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        FetchPlayers();
        var actionQueueInfo = Me.Controller.ActionQueueInfo;
        
        // If the queue is over
        if (actionQueueInfo.IsLast)
        {
            foreach (var player in Players)
                player.PhotonView.RpcSecure("SetActionQueueInfo", RpcTarget.AllBuffered, true, Strings.NoActiveQueues);
            
            ToggleDayForAllPlayers();
            return;
        }
        
        actionQueueInfo.MoveForward();
        
        var targetPlayerActorNumber = actionQueueInfo.CurrentId;
        var targetPlayer = Players.First(n => n.ActorNumber == targetPlayerActorNumber);
        
        if (!targetPlayer.Controller.Alive)
        {
            foreach (var player in Players)
                player.PhotonView.RpcSecure("SetActionQueueInfo", RpcTarget.AllBuffered, true, actionQueueInfo.ConvertIntoString());
            
            Me.PhotonView.RpcSecure("ActionQueueForward", RpcTarget.MasterClient, true);
            return;
        }

        targetPlayer.PhotonView.RpcSecure("SetActionPrompted", targetPlayer.PhotonPlayer, true, true);
    
        foreach (var player in Players)
            player.PhotonView.RpcSecure("SetActionQueueInfo", RpcTarget.AllBuffered, true, actionQueueInfo.ConvertIntoString());
    }
    
    [MasterOnly]
    public void InitializeActionQueue()
    {
        /*
         * This method will be invoked by DayTimeController on day time change
         * in every client instance in the room,
         * but queue info should be generated by MasterClient
         * (same as with roles). This check is necessary in order
         * to not everybody generate their own queue info and put
         * it into everybody's Photon custom properties.
         * ONLY MASTER CAN USE THIS METHOD.
         */
        
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            return;

        FetchPlayers();
        
        /*
         * This is the list of players who
         * are automatically chosen to take
         * part in night - time actions
         * (like killing, inspecting, etc.)
         */
        if (Players.All(n => !n.Controller.SuitableForActions))
        {
            foreach (var player in Players)
                player.PhotonView.RpcSecure("SetActionQueueInfo", RpcTarget.AllBuffered, true, Strings.NoActiveQueues);
            
            ToggleDayForAllPlayers();
            
            return;
        }
        
        var suitablePlayers = Players.Where(n => n.Controller.SuitableForActions);
        var suitablePlayersSorted = SortActionQueuePlayersByRole(suitablePlayers);
        var suitablePlayerIds = suitablePlayersSorted.Select(n => n.GetComponent<PhotonView>().Owner.ActorNumber);

        var newActionQueueInfo = new ActionQueueInfo(suitablePlayerIds, suitablePlayerIds.First());

        foreach (var player in Players)
            player.PhotonView.RpcSecure("SetActionQueueInfo", RpcTarget.AllBuffered, true, newActionQueueInfo.ConvertIntoString());
        
        var firstQueuePlayer = suitablePlayersSorted.First();
        firstQueuePlayer.GetComponent<PhotonView>().RpcSecure("SetActionPrompted", RpcTarget.AllBuffered, true, true);

        foreach (var otherPlayer in Players.Where(n => !Equals(n, firstQueuePlayer) && n.Controller.Intact))
            otherPlayer.PhotonView.RpcSecure("ShowWaitTile", otherPlayer.PhotonPlayer, true);
    }

    [MasterOnly]
    private static IEnumerable<GameObject> SortActionQueuePlayersByRole(IEnumerable<PlayerData> players)
    {
        var newPlayerList = new List<GameObject>();

        var gameObjects = players as PlayerData[] ?? players.ToArray();
        newPlayerList.AddRange(GetAllPlayersWithSpecifiedRole(gameObjects.Select(n => n.GameObject), Role.Mafia));
        newPlayerList.AddRange(GetAllPlayersWithSpecifiedRole(gameObjects.Select(n => n.GameObject), Role.MafiaBoss));
        newPlayerList.AddRange(GetAllPlayersWithSpecifiedRole(gameObjects.Select(n => n.GameObject), Role.Maniac));
        newPlayerList.AddRange(GetAllPlayersWithSpecifiedRole(gameObjects.Select(n => n.GameObject), Role.Doctor));
        newPlayerList.AddRange(GetAllPlayersWithSpecifiedRole(gameObjects.Select(n => n.GameObject), Role.PoliceOfficer));
        newPlayerList.AddRange(GetAllPlayersWithSpecifiedRole(gameObjects.Select(n => n.GameObject), Role.Witness));

        return newPlayerList;
    }
    
    [MasterOnly]
    private static IEnumerable<GameObject> GetAllPlayersWithSpecifiedRole(IEnumerable<GameObject> collection, Role role) => 
        collection.Where(n => n.GetComponent<PlayerController>().Role == role);
    
    private void AbortKill(PlayerData receiver, IEnumerable<PlayerData> mafias)
    {
        var args = ChatMessageArgumentsService.GetDisguisedMessageArguments(receiver.Nickname, "player_disguised_mafia");
        
        foreach (var mafia in mafias)
            mafia.PhotonView.RpcSecure("DisplayDisguisedMessage", mafia.PhotonPlayer, true, args);
        
        receiver.PhotonView.RpcSecure("SetDisguised", RpcTarget.AllBuffered, true, false);
        GoQueue();
    }
}
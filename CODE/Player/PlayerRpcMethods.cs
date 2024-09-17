using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public abstract class PlayerRpcMethods : PlayerLogicMethods
{
    /*
     * This section contains complex multifunctional RPC methods
     * that serve various purposes at the same time. They are
     * mostly related to the visual part of the game.
     */
    [PunRPC]
    public void ResetPosition(int actorNumber)
    {
        var players = PlayerFetchService.FetchPlayerData(PlayerParent);
        var target = players.First(n => n.ActorNumber == actorNumber);
        var targetPositionIndex = (int)target.PhotonPlayer.CustomProperties[Strings.PositionPropName];
        var positions = Resources.LoadAll<SpawnPosition>("ScriptableObjects/Coordinates/PlayerPositionsNew6");
        
        target.GameObject.transform.localPosition = positions[targetPositionIndex].Position;
        target.GameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
        target.GameObject.transform.Rotate(0, positions[targetPositionIndex].Rotation.y, 0);
    }
    
    [PunRPC]
    public void RevealRole(int actorNumber)
    {
        var players = PlayerFetchService.FetchPlayerData(PlayerParent);
        var targetPlayer = players.First(n => n.ActorNumber == actorNumber);
        
        if (PhotonView.IsMine)
        {
            PhotonView.RpcSecure("SetAlive", RpcTarget.AllBuffered, true, false);
            PhotonView.RpcSecure("LiftToTransformPosition", RpcTarget.AllBuffered, true, PhotonView.Owner.ActorNumber);
        }
        
        GetComponent<Animator>().Play("Transformation");
        Outfit.GetComponent<Animator>().Play("Transformation");

        foreach (var player in players.Where(n => n.ActorNumber != actorNumber))
            player.PhotonView.RpcSecure("PlayAnimation", RpcTarget.All, true, "SitIdle");

        MainCameraController.StartShowingTransformation(targetPlayer.GameObject);
        
        foreach (var player in players)
            player.PhotonView.RpcSecure("DisableBigText", RpcTarget.AllBuffered, true);
    }
    
    [PunRPC]
    public void PlayBeforeDeathTransformation(int actorNumber)
    {
        var players = PlayerFetchService.FetchPlayerData(PlayerParent);
        var targetPlayer = players.First(n => n.ActorNumber == actorNumber);
        
        if (PhotonView.IsMine)
            PhotonView.RpcSecure("LiftToTransformPosition", RpcTarget.AllBuffered, true, PhotonView.Owner.ActorNumber);
        
        GetComponent<Animator>().Play("TransformationBeforeDeath");
        Outfit.GetComponent<Animator>().Play("TransformationBeforeDeath");

        MainCameraController.StartShowingTransformation(targetPlayer.GameObject);
        
        foreach (var player in players)
            player.PhotonView.RpcSecure("DisableBigText", RpcTarget.AllBuffered, true);
    }

    [PunRPC]
    public void PlayHealedAnimation()
    {
        SetMeshVisibility(true);
        RevealRoleTemporary();
        UIScript.EnableBigText($"{GetLocalizedString("healed_by_doctor_big")}");
        
        if (!Alive) 
            SetOutfitVisibility();
        
        ResetPosition();
        
        if (PhotonView.IsMine) 
            PhotonView.RpcSecure("SetAlive", RpcTarget.AllBuffered, true, true);
    }

  [PunRPC]
public IEnumerator LoadCustomization(int actorNumber, string jsonConfig)
{
      yield return new WaitForSeconds(1f); 
    if(PlayerParent==null)
        PlayerParent = PlayerParentFetchService.Fetch();
    IEnumerable<PlayerData> players = PlayerFetchService.FetchPlayerData(PlayerParent);

    var targetPlayer = players.First(n => n.ActorNumber == actorNumber);

    CustomizerLogic.LoadConfig(targetPlayer.GameObject.transform.Find("Mesh").Find("Basic_Stickman").gameObject,
        targetPlayer.GameObject.transform.Find("Hip")
            .Find("Spine")
            .Find("Spine_1")
            .Find("Spine_2")
            .Find("Neck")
            .Find("HeadMain").gameObject, jsonConfig);

    
      
    
}

   
    
    [PunRPC]
    public void LiftToTransformPosition(int actorNumber)
    {
        var players = PlayerFetchService.FetchPlayerData(PlayerParent);
        var targetPlayer = players.First(n => n.ActorNumber == actorNumber);

        var position = targetPlayer.GameObject.transform.position;
        position = new Vector3(position.x, position.y + 0.9f, position.z);
        targetPlayer.GameObject.transform.position = position;
        
        SoundController.PlayHeartBeatSound();
    }

    
    /*
     * This section contains RPC methods that mainly work with
     * other classes and controllers and redirect various
     * tasks sent via RPC to them.
     */
    [PunRPC]
    public void StopVoteStopwatch() => StopwatchController.StopVoteStopwatch();
    
    [PunRPC]
    public void ShowWaitTile()
    {
        UIScript.DisableAllInteractionButtons();
        UIScript.OnAwaiting();
    }
    
    [PunRPC]
    public void FollowBoat() => MainCameraController.StartFollowingBoat();
    
    [PunRPC]
    public void StartVoting() => VotingController.StartVoting();
        
    [PunRPC]
    public void EnableBigText(string message) => UIScript.EnableBigText(message);

    [PunRPC]
    public void DisableBigText() => UIScript.BigTextVisible = false;
    
    [PunRPC]
    public void PlayAnimation(string animationName)
    {
        GetComponent<Animator>().Play(animationName);
        
        if (Outfit != null)
            Outfit.GetComponent<Animator>().Play(animationName);
    }

    [PunRPC]
    public void ShowOutfit() => SetOutfitVisibility();

    [PunRPC]
    public void HideOutfit() => SetOutfitVisibility(false);
    
    [PunRPC]
    public void ActionQueueForward() => GameplayController.ActionQueueForward();
    
    [PunRPC]
    public void ActionQueueInfoMoveForward() => ActionQueueInfo.MoveForward();
    
    [PunRPC]
    public void AddPoints(int pointsToAdd) => PointsScored += pointsToAdd;
    
    [PunRPC] 
    public void StartGame() => GameplayController.StartGame();
    
    
    /*
     * This section contains methods related to chat and
     * everything that can be sent to it.
     */
    [PunRPC]
    public void DisplayMessage(string assetName, string header, string message) => 
        UIScript.ShowNewChatMessage(assetName, header, message);

    [PunRPC]
    public void DisplayActionImpactMessage(string nickname, string messageLocalizedKey) =>
        UIScript.ShowNewChatMessage("chat-message-game-host", 
                                    GetLocalizedString("game_host"), 
                                    nickname + " " + GetLocalizedString(messageLocalizedKey));
    
    [PunRPC]
    public void DisplayWitnessReportMessage(string nickname) => UIScript.ShowNewChatMessage(
        "chat-message-game-host", 
        GetLocalizedString("game_host"), 
        GetLocalizedString("visited_player_murdered") + " " + nickname);

    [PunRPC]
    public void DisplayItWasntMeMessage(string nickname) =>
        UIScript.ShowNewChatMessage("chat-message", nickname, GetLocalizedString("it_wasnt_me_quick"));
    
    [PunRPC]
    public void DisplayThinkMafiaIsMessage(string mafiaIsNickname, string senderNickname) =>
        UIScript.ShowNewChatMessage("chat-message", senderNickname, GetLocalizedString("mafia_is_quick_message") + " " + mafiaIsNickname + ".");
    
    [PunRPC]
    public void DisplaySuspiciousMessage(string suspiciousNickname, string senderNickname) => 
        UIScript.ShowNewChatMessage("chat-message", senderNickname, suspiciousNickname + " " + GetLocalizedString("action_suspicious_quick_message"));

    [PunRPC]
    public void DisplayDisguisedMessage(string nickname, string localizedMessage) => UIScript.ShowNewChatMessage(
        "chat-message-game-host",
        GetLocalizedString("game_host"),
        nickname + " " + GetLocalizedString(localizedMessage));

    [PunRPC]
    public void DisplayHelicopterMessage(string nickname) => UIScript.ShowNewChatMessage(
        "chat-message-game-host",
        GetLocalizedString("game_host"),
        GetLocalizedString("you_tried_expose") + " " + nickname + ", " + GetLocalizedString("but_helicopter"));

    [PunRPC]
    public void DisplayMineDetectorProtectionMessage() => UIScript.ShowNewChatMessage(
        "chat-message-game-host",
        GetLocalizedString("game_host"),
        GetLocalizedString("tried_to_kill_mine_detector"));

    [PunRPC]
    public void DisplayMineExplodedMessage(string nickname) => UIScript.ShowNewChatMessage(
        "chat-message-game-host",
        GetLocalizedString("game_host"),
        nickname + " " + GetLocalizedString("exploded_with_mine"));

    /*
     * This section contains basic "setter" RPC methods,
     * some of which serve some more purposes other
     * than setting a value (e.g. updating some visual
     * content related to that value).
     */
    [PunRPC]
    public void SetRole(Role newRole)
    {
        if (newRole == Role.None)
        {
            Destroy(Outfit);
            Outfit = null;
            Role = newRole;
            return;
        }
        
        var roleBeforeAssignment = Role;

        if (roleBeforeAssignment != Role.None)
        {
            OldOutfit = Outfit;
            Destroy(OldOutfit);
        }
        
        var outfit = Resources.Load<GameObject>($"Transformation/{newRole}");
        Outfit = Instantiate(outfit, gameObject.transform);
        
        SetOutfitVisibility(false);
        
        Role = newRole;
    }

    [PunRPC]
    public void SetOutfitVisible(bool value) => OutfitVisible = value;
    
    [PunRPC]
    public void SetGameOver(bool value) => GameOver = value;
    
    [PunRPC]
    public void SetActionQueueInfo(string actionQueueInfo) => ActionQueueInfo = new ActionQueueInfo(actionQueueInfo);
    
    [PunRPC]
    public void SetVotedActorNumbers(int[] numbers) {
        string AllNumbersString = "";
        foreach (var number in numbers){
            AllNumbersString += number + " ";
        }
        Debug.Log(AllNumbersString + " :::" + numbers[0]);
        VotingActorNumbers = numbers;
    } 

    [PunRPC]
    public void SetKilledActorNumber(int number) => KilledActorNumber = number;
    
    [PunRPC]
    public void SetVotedOut(bool value) => VotedOut = value;
    
    [PunRPC]
    public void SetVoting(bool value)
    {
        Voting = value;
        
        if (value)
            UIScript.ShowWarning(GetLocalizedString("time_to_vote"));
    }

    [PunRPC]
    public void SetActionExecuted(bool value) => ActionExecuted = value;

    [PunRPC]
    public void SetHasRadio(bool value) => HasRadio = value;

    [PunRPC]
    public void SetHasHelicopter(bool value) => HasHelicopter = value;
    
    [PunRPC]
    public void SetActiveRoleRequired(bool value) => ActiveRoleRequired = value;

    [PunRPC]
    public void SetCanVoteTwice(bool value) => CanVoteTwice = value;

    [PunRPC]
    public void SetDisguised(bool value) => Disguised = value;

    [PunRPC]
    public void SetHasMineDetector(bool value) => HasMineDetector = value;
    
    [PunRPC]
    public void SetNightSwitchPrompted(bool value) => NightSwitchPrompted = value;

    [PunRPC]
    public void SetDaySwitchPrompted(bool value) => DaySwitchPrompted = value;
    
    [PunRPC]
    public void SetActionImpactPrompted(bool value) => ActionImpactPrompted = value;
    
    [PunRPC]
    public void SetActionPrompted(bool value) => ActionPrompted = value;
    
    [PunRPC]
    public void SetParalyzed(bool value) => Paralyzed = value;

    [PunRPC]
    public void SetParalyzedByRevenge(bool value) => ParalyzedByRevenge = value;

    [PunRPC] 
    public void SetHealed(bool value) => Healed = value;
    
    [PunRPC]
    public void SetWitnessed(bool value) => Witnessed = value;

    [PunRPC]
    public void SetInvestigated(bool value) => Investigated = value;

    [PunRPC]
    public void SetKilled(bool value) => Killed = value; 
        
    [PunRPC]
    public void SetAlive(bool value)
    {
        Alive =  value;
        UIScript.UpdateGoodVsBadPanelContents();
    }
    
    [PunRPC]
    public void AddWiretappingAttempt()
    {
        if (!HasRadio)
            return;

        var targetGameObject = GameObject.Find(nameof(Radio));
        var targetController = targetGameObject.GetComponent<Radio>();
        targetController.WiretappingAttempts += 1;
    }
}
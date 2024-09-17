using System.Linq;
using Photon.Pun;
using UnityEngine;

public class PlayerAuxillaryMethods : PlayerRuntimeProperties
{
    
    public void ResetPosition()
    {
        var targetPositionIndex = (int)PhotonView.Owner.CustomProperties[Strings.PositionPropName];
        var positions = Resources.LoadAll<SpawnPosition>($"ScriptableObjects/Coordinates/PlayerPositions{PhotonNetwork.CurrentRoom.MaxPlayers}");
        
        transform.localPosition = positions[targetPositionIndex].Position;
        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.Rotate(0, positions[targetPositionIndex].Rotation.y, 0);
    }
    
    // Used locally by Players who are presented with
    // role revelation that only they can see.
    public void RevealRoleTemporary()
    {
        GetComponent<Animator>().Play("GreetingReveal");
        Outfit.GetComponent<Animator>().Play("GreetingReveal");
        UIScript.EnableBigText($"{GetLocalizedString("role_is_temporary")} {GetLocalizedString(RoleLocalizedStrings[Role])}.");
        SetOutfitVisibility();
    }
    
    public void DisplayGameHostMessage(string messageLocalizedKey) =>
        UIScript.ShowNewChatMessage("chat-message-game-host", GetLocalizedString("game_host"), GetLocalizedString(messageLocalizedKey));
    
    // Used in various parts of the game both locally and via RPC methods. 
    public void SetOutfitVisibility(bool value = true) 
    {
        var mesh = Outfit.transform.Find("Mesh");

        foreach (Transform meshChild in mesh.transform)
            meshChild.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = value;
    }
    
    // Used in scenarios when a substitute for main mesh is being used (e.g. death animation).
    protected void SetMeshVisibility(bool value)
    {
        transform.Find("Hip")
            .Find("Spine")
            .Find("Spine_1")
            .Find("Spine_2")
            .Find("Neck")
            .Find("HeadMain").gameObject.SetActive(value);
        transform.Find("Mesh").Find("Basic_Stickman").gameObject.GetComponent<SkinnedMeshRenderer>().enabled = value;
        transform.Find("Mesh").Find("Eye").gameObject.GetComponent<SkinnedMeshRenderer>().enabled = value;
        transform.Find("Mesh").Find("Pupil").gameObject.GetComponent<SkinnedMeshRenderer>().enabled = value;
        transform.Find("Nickname").gameObject.SetActive(value);
        transform.Find($"{Role}(Clone)").gameObject.SetActive(value);
    }
      
    // Used only in constructor.
    protected void InstantiateSwirl()
    {
        var parent = transform.Find("Mesh").Find("Basic_Stickman").transform;
        const float xOffset = 1.47f;
        const float yOffset = 1.34f;
        const float zOffset = 1.41f;
        
        var instance = Instantiate(Swirl);
        instance.transform.SetParent(parent, false);
        // instance.transform.position = new Vector3(parent.transform.position.x + xOffset, parent.transform.position.y + yOffset, parent.transform.position.z + zOffset);
        // instance.transform.position = new Vector3(myPosition.x, myPosition.y, myPosition.z);
    }

    // Used only in constructor.
    protected void FindControllers()
    {
        StopwatchController = GameObject.Find("StopwatchController").GetComponent<StopwatchController>();
        DayTimeController = GameObject.Find("DayTimeController").GetComponent<DayTimeController>();
        VotingController = GameObject.Find("VotingController").GetComponent<VotingController>();
        UIScript = GameObject.Find("ui-document").GetComponent<GameplayUIBehaviour>();
        MainCameraController = GameObject.Find("MainCamera").GetComponent<MainCameraController>();
        GameplayController = GameObject.Find("GameplayController").GetComponent<GameplayController>();
        SoundController = GameObject.Find("FXSoundController").GetComponent<FXSoundController>();
        PlayerParent = PlayerParentFetchService.Fetch();
    }
    
    // Used only in constructor.
    protected void SetParent() => 
        transform.SetParent(GameObject.Find("boat").transform.Find($"table-{PhotonNetwork.CurrentRoom.MaxPlayers}-seats"), false);

    // Used only in constructor.
    protected void SetInitialPropertyValues()
    {
        Role = Role.None;
        Alive = true;
        Killed = false;
        Healed = false;
        Investigated = false;
        Witnessed = false;
        Paralyzed = false;
        ParalyzedByRevenge = false;
        ActionPrompted = false;
        ActionImpactPrompted = false;
        NightSwitchPrompted = false;
        DaySwitchPrompted = false;
        OutfitVisible = false;
        VotingPending = false;
        Voting = false;
        VotedOut = false;
        ActionExecuted = false;
        HasRadio = false;
        Disguised = false;
        HasHelicopter = false;
        ActiveRoleRequired = false;
        VotingActorNumbers = new[] { Numerics.NoId };
        KilledActorNumber = Numerics.NoId;
        ActionQueueInfo = new ActionQueueInfo();
    }
    
    // Used only in constructor.
    protected void LoadCustomizationLocal()
    {
        var actorNumber = PhotonView.Owner.ActorNumber;
        var jsonString = SaveManagerStatic.Load();

        if (PhotonView.IsMine)
            PhotonView.RpcSecure("LoadCustomization", RpcTarget.AllBuffered, true, new object[] { actorNumber, jsonString });
    }
    
    // Used only in constructor.
    protected void CheckStartCountdown()
    {
        if (InRoomCallbackListener.IsMaxPlayerQuantityReached())
            StopwatchController.StartCountdownStopwatch();
    }
    
    protected void NotifyOMyActionImpact(string messageLocalizedKey)
    {
        FetchPlayers();
        var args = ChatMessageArgumentsService.GetActionImpactMessageArguments(PhotonView.Owner.NickName, messageLocalizedKey);

        foreach (var player in Players) 
            player.PhotonView.RpcSecure("DisplayActionImpactMessage", player.PhotonPlayer, true, args);
    }
    
    protected void PlayDeathAnimation()
    {
        var index = (int)PhotonView.Owner.CustomProperties[Strings.PositionPropName];
        
        SetMeshVisibility(false);
        var positions = Resources.LoadAll<SpawnPosition>($"ScriptableObjects/Coordinates/DeathPositions{PhotonNetwork.CurrentRoom.MaxPlayers}/");
        DeathAnimationInstance = Instantiate(DeathSharkPrefab, positions[index].Position, positions[index].Rotation);
        
        DeathAnimationInstance.transform.SetParent(PlayerParent.transform, false);
        
        DeathAnimationInstance.transform.localScale = positions[index].Scale;
        DeathAnimationInstance.transform.Rotate(0, positions[index].Rotation.y + 180, 0);
        MainCameraController.StartShowingDeath(DeathAnimationInstance);
    }
        
    protected void MoveToCage()
    {
        var cagePositions = Resources.LoadAll<SpawnPosition>("ScriptableObjects/Coordinates/CagePositions");
        FetchPlayers();
        
        for (int i = 0; i < cagePositions.Length; i++)
        {
            if (Players.Any(n => n.GameObject.transform.localPosition == cagePositions.ElementAt(i).Position)) continue;       

            transform.localPosition = cagePositions[i].Position; 
            transform.rotation = new Quaternion(0, 0, 0, 0);
            transform.Rotate(0, cagePositions[i].Rotation.y, 0);
            transform.localScale = cagePositions[i].Scale;

            GetComponent<Animator>().Play("SitBeforeCage");
            Outfit.GetComponent<Animator>().Play("SitBeforeCage");

            return;
        }
    }
        
    // Used when Mafia dies and at least one 
    // mafia boss is still alive.
    protected void PassRoleToMafiaBoss(PlayerData boss)
    {
        boss.PhotonView.RpcSecure("SetRole", RpcTarget.AllBuffered, true, Role.Mafia);
        boss.PhotonView.RpcSecure("EnableBigText", boss.PhotonPlayer, true, $"{GetLocalizedString("mafia_is_dead")} \n" +
                                                                            $"{GetLocalizedString("you_take_place")}");
    }
}
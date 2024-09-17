using System.Linq;
using Photon.Pun;

public abstract class PlayerAnimationEvents : PlayerRpcMethods
{
    public void OnTransformationAnimationOver()
    {
        ResetPosition();
        MoveToCage();
        
        if (!PhotonView.IsMine) return;
        
        UIScript.DisableAllInteractionButtons();
        UIScript.AddDeadMessage();
        
        SoundController.Stop();
    }
    
    public void OnDeathTransformationAnimationOver()
    {
        PlayDeathAnimation();
        HideOutfit();

        if (!PhotonView.IsMine) return;
        
        UIScript.DisableAllInteractionButtons();
        UIScript.AddDeadMessage();

        DisableBigText();
        SoundController.Stop();
    }
    
    public void OnWeepCageAnimationOver()
    {
        MainCameraController.StartFollowingBoat();
        SoundController.Stop();

        if (!PhotonView.IsMine) return;
        
        FetchPlayers();
        var anyMafiasLeft = VotingController.AnyBadPlayersLeft();
            
        if (!anyMafiasLeft)
        {
            foreach (var player in Players) 
                player.PhotonView.RpcSecure("SetGameOver", RpcTarget.AllBuffered, true, true);
        }
        else
        {
            if (!Players.Any(n => n.Controller is {Role: Role.Mafia, Alive: true}))
            {
                var boss = Players.First(n => n.Controller.Role == Role.MafiaBoss);
                PassRoleToMafiaBoss(boss);
            }

            GameplayController.ToggleNightForAllPlayers();
        }
    }
    
    public void OnShowOutfit()
    {
        ShowOutfit();
        EnableBigText($"{GetLocalizedString("role_is")} " + $"{GetLocalizedString(RoleLocalizedStrings[Role])}.");
        OutfitVisible = true;
    }

    public void OnWeepCageAnimationStart()
    {
        SoundController.PlayCry();
        MainCameraController.StartShowingCage(gameObject);
    }

    public void OnWakeUpStart() => EnableBigText($"{GetLocalizedString("night_over")} \n" + $"{GetLocalizedString("city_awaken")}");

    public void OnWakeUpOver()
    {
        ActionExecuted = false;
        VotingPending = true;
        
        DisableBigText();
    }
    
    public void OnFallAsleepStart()
    {
        if (PhotonView.IsMine)
            SetOutfitVisibility();
        
        EnableBigText($"{GetLocalizedString("its_nighttime")} \n" + $"{GetLocalizedString("mafia_awaken")}");
    }
    
    public void OnFallAsleepOver() => DisableBigText();

    public void OnSitIdleStart() => DisableBigText();

    public void OnGreetingRevealStart() => MainCameraController.StartShowingGreetingReveal(gameObject);

    public void OnGreetingRevealOver()
    {
        UIScript.BigTextVisible = false;
        SetOutfitVisibility(false);
        MainCameraController.StartFollowingBoat();
        FetchPlayers();
        GoQueue();
    }
}
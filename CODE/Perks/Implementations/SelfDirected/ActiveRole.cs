using Photon.Pun;

public sealed class ActiveRole : SelfDirectedPerk
{
    private void DisplayUnavailableMessage()
    {
        TargetPlayer.Controller.DisplayGameHostMessage(GameplayController.GetLocalizedString("role_already_assigned"));
        Completed = true;
    }
    
    protected override string SetterRpcName => "SetActiveRoleRequired";

    protected override void Execute(string setterRpcName, bool value = true)
    {
        TargetPlayer.PhotonView.RpcSecure(setterRpcName, RpcTarget.AllBuffered, true, value);
        Completed = true;
    }
    
    protected override void Update()
    {   
        if (TargetPlayerExists && TargetPlayer.Controller.Role != Role.None && !Completed)
        {
            DisplayUnavailableMessage();
            return;
        }
        
        base.Update();
    }
}
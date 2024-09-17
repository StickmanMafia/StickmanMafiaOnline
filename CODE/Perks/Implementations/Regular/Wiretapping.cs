public sealed class Wiretapping : Perk
{
    private bool _targetCommittedAction;
    private bool _tracking;

    // base.Update() is not used due to perk complexity.
    protected override bool ConditionSatisfied => false;
    
    protected override void Execute()
    {
        Owner.Controller.DisplayGameHostMessage(_targetCommittedAction
            ? GameplayController.GetLocalizedString("tracked_taken_action")
            : GameplayController.GetLocalizedString("tracked_not_take_action"));
        
        base.Execute();
    }
    
    private void ExecuteRefused()
    {
        Owner.Controller.DisplayGameHostMessage(GameplayController.GetLocalizedString("player_has_radio"));
        base.Execute();
    }

    protected override void Start()
    {
        _targetCommittedAction = false;
        _tracking = false;
    }

    protected override void Update()
    {
        /*
         * Tracking can only start when target Player is Assigned in a logic method of owner.
         * This exists due to a possible delay between instantiating perk object and assigning TargetPlayer.
         */ 
        if (TargetPlayerExists && !_tracking)
            _tracking = true;
        
        if (!_tracking) return;
        
        if (TargetPlayer.Controller.Role == Role.Civilian)
            Execute();
        
        if (TargetPlayer.Controller.HasRadio) 
            ExecuteRefused();

        var actionQueueInfo = TargetPlayer.Controller.ActionQueueInfo;

        if (!actionQueueInfo.AnyPlayersWithActorId(TargetPlayer.ActorNumber))
        {
            _tracking = false;
            Execute();
            return;
        }

        if (actionQueueInfo.GetCurrentPlayerPosition() > actionQueueInfo.GetPlayerPosition(TargetPlayer.ActorNumber) 
            && !TargetPlayer.Controller.ActionExecuted)
        {
            _tracking = false;
            Execute();
            return;
        }
        
        if (actionQueueInfo.GetCurrentPlayerPosition() > actionQueueInfo.GetPlayerPosition(TargetPlayer.ActorNumber) 
            && TargetPlayer.Controller.ActionExecuted)
        {
            _tracking = false;
            _targetCommittedAction = true;
            Execute();
        }
    }
}
using UnityEngine;

public sealed class LieDetector : Perk
{
    private GameplayUIBehaviour _uiScript;
    private DayTimeController _dayTimeController;
    
    private int _messageCount;
    private int _overallMessageCount;
    private int _daysPassed;
    
    private const int MaxMessagesAmount = 3;
    
    protected override bool ConditionSatisfied => _messageCount >= MaxMessagesAmount && TargetPlayerExists;
    
    protected override void Execute()
    {
        Owner.Controller.DisplayGameHostMessage(TargetPlayer.Nickname + " " + 
                                                GameplayController.GetLocalizedString("talking_too_much") + " " + 
                                                PlayerRuntimeFields.RoleLocalizedStrings[TargetPlayer.Controller.Role]);
        
        base.Execute();
    }

    protected override void Start()
    {
        _dayTimeController = GameObject.Find("DayTimeController").GetComponent<DayTimeController>();
        _uiScript = GameObject.Find("ui-document").GetComponent<GameplayUIBehaviour>();
        
        _overallMessageCount = _uiScript.GetPlayerMessageAmount(TargetPlayer.ActorNumber);
        _daysPassed = _dayTimeController.DaysPassed;
    }

    protected override void Update()
    {
        if (_dayTimeController.DaysPassed > _daysPassed)
        {
            _daysPassed = _dayTimeController.DaysPassed;
            _messageCount = 0;
        }
        
        var currentMessageCount = _uiScript.GetPlayerMessageAmount(TargetPlayer.ActorNumber);

        if (currentMessageCount > _overallMessageCount)
        {
            _messageCount += currentMessageCount - _overallMessageCount;
            _overallMessageCount = currentMessageCount;
        }
                   
        base.Update();
    }
}
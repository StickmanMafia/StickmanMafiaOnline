using System;
using Photon.Pun;
using UnityEngine;

public class PlayerLogicMethods : PlayerAuxillaryMethods
{
    public void DisplayMineDetectorMessage(string nickname)
    {
        UIScript.ShowNewChatMessage(
            "chat-message-game-host",
            GetLocalizedString("game_host"),
            nickname + " " + GetLocalizedString("has_mine_detector"));
    }
    
    // Used whenever a primary "prompt" flag is raised.
    protected void AnswerPrimaryPrompts()
    {
        if (GameOver)
        {
            UIScript.ShowGameOver();
            GameOver = false;
        }
        
        if (VotingPending)
        {
            VotingPending = false;
            
            FetchPlayers();
            
            MainCameraController.StartFollowingBoat();
            
            if (Intact)
            {
                ResetPosition();
                PhotonView.RpcSecure("SetVoting", RpcTarget.AllBuffered, true, true);
                UIScript.ShowInteractionBarDiscussionButtons();
                StopwatchController.StartVoteStopwatch();
                VotingController.StartVoting();
            }
        }
    }
    
    // Used whenever a secondary "prompt" flag is raised.
    protected void AnswerSecondaryPrompts()
    {
        if (VotedOut)
        {
            PhotonView.RpcSecure("SetAlive", RpcTarget.AllBuffered, true, false);
            PhotonView.RpcSecure("SetVotedOut", RpcTarget.AllBuffered, true, false);
            PhotonView.RpcSecure("MoveToCage", RpcTarget.AllBuffered, true, 1);
            PhotonView.RpcSecure("PlayAnimation", RpcTarget.AllBuffered, true, "CryInCage");
            return;
        }

        if (DaySwitchPrompted)
        {
            PhotonView.RpcSecure("SetDaySwitchPrompted", RpcTarget.AllBuffered, true, false);
            ToggleDaytimeSwitch(TimeOfDay.Day);
        } 
        else if (NightSwitchPrompted)
        {
            PhotonView.RpcSecure("SetNightSwitchPrompted", RpcTarget.AllBuffered, true, false);
            ToggleDaytimeSwitch(TimeOfDay.Night);
        }
        else if (ActionPrompted)
        {
            PhotonView.RpcSecure("SetActionPrompted", RpcTarget.AllBuffered, true, false);
            UIScript.ShowActionButtons();
        }  
        else if (ActionImpactPrompted)
        {
            PhotonView.RpcSecure("SetActionImpactPrompted", RpcTarget.AllBuffered, true, false);
            GetActionImpact();
        }
    }
    
    // Used whenever ActionImpactPrompted is raised.
    // Can be used in both times of day.
    private void GetActionImpact()
    {
        if (Killed)
        {
            if (DayTimeController.CurrentDayTime == TimeOfDay.Night)
            {
                NotifyOMyActionImpact("has_been_killed");    
            }
            
            PhotonView.RpcSecure("SetAlive", RpcTarget.AllBuffered, true, false);
            PhotonView.RpcSecure("SetKilled", RpcTarget.AllBuffered, true, false);
            PhotonView.RpcSecure("PlayBeforeDeathTransformation", RpcTarget.AllBuffered, true, PhotonView.Owner.ActorNumber);
             
            UIScript.AddDeadMessage();
        }
        
        else if (Healed)
        {
            NotifyOMyActionImpact("healed_by_doctor");
            PhotonView.RpcSecure("PlayHealedAnimation", RpcTarget.AllBuffered, true);
            PhotonView.RpcSecure("SetHealed", RpcTarget.AllBuffered, true, false);
        }
        
        /*
         * This condition can happen ONLY on the side of police officer.
         * Thus no RPC methods are required.
         */
        else if (Investigated)
        {
            Investigated = false;
            RevealRoleTemporary();
        }
        else if (Paralyzed || ParalyzedByRevenge)
        {
            NotifyOMyActionImpact("paralyzed_cant_act");
            UIScript.AddParalyzedMessage();
        }

        if (Witnessed)
        {
            NotifyOMyActionImpact("witnessed_by_another");
        }
    }

    // Used only by AnswerSecondaryPrompts locally. 
    // All online DayTimeSwitch actions are  
    // transmitted via RPC method to flags.
    private void ToggleDaytimeSwitch(TimeOfDay daytime)
    {
        MainCameraController.StartFollowingBoat();
        
        switch (daytime)
        {
            case TimeOfDay.Night:
                DayTimeController.StartSwitchToNight();
                PhotonView.RpcSecure("PlayAnimation", RpcTarget.AllBuffered, true, "FallAsleep");
                break;
            
            case TimeOfDay.Day:
                DayTimeController.StartSwitchToDay();
                PhotonView.RpcSecure("PlayAnimation", RpcTarget.AllBuffered, true, "WakeUp");
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(daytime), daytime, null);
        }
    }
}
using System;
using System.Diagnostics;
using Photon.Pun;
using UnityEngine;

public class StopwatchController : Controller
{
    [SerializeField]
    private GameObject _uiDocument;

    [SerializeField]
    private GameObject _roleControllerObject;

    [SerializeField]
    private GameObject _gameplayControllerObject;

    private GameplayUIBehaviour _uiScript;
    private bool _rolesAssigned;
    private int _countDownDigit;
    
    private RoleController _roleController;
    private GameplayController _gameplayController;
    
    private Stopwatch _countdownStopwatch;
    private Stopwatch _roleCardStopwatch;
    private Stopwatch _meetupStopwatch;
    private Stopwatch _actionStopwatch;
    private Stopwatch _votingStopwatch;

    public bool PreGameProceduresRunning =>
        _countdownStopwatch.IsRunning || _roleCardStopwatch.IsRunning || _meetupStopwatch.IsRunning;

    protected override void Start()
    {
        base.Start();
        _uiScript = _uiScript = _uiDocument.GetComponent<GameplayUIBehaviour>();
        
        _countDownDigit = Numerics.CountdownTime;
        _roleController = _roleControllerObject.GetComponent<RoleController>();
        _gameplayController = _gameplayControllerObject.GetComponent<GameplayController>();
        
        _countdownStopwatch = new Stopwatch();
        _roleCardStopwatch = new Stopwatch();
        _meetupStopwatch = new Stopwatch();
        _actionStopwatch = new Stopwatch();
        _votingStopwatch = new Stopwatch();
    }

    public void StartCountdownStopwatch() => StartStopwatch(_countdownStopwatch, Numerics.CountdownTime, "countdown_in");

    private void StartMeetupStopwatch() => StartStopwatch(_meetupStopwatch, Numerics.MeetupTime, "night_starting_shortly");

    public void StartActionStopwatch() => StartStopwatch(_actionStopwatch, Numerics.ActionTime, "complete_action");

    public void StartVoteStopwatch() => StartStopwatch(_votingStopwatch, Numerics.VoteTime, "complete_action");

    public void StopActionStopwatch() => StopStopwatch(_actionStopwatch);

    public void StopVoteStopwatch() => StopStopwatch(_votingStopwatch);
    
   private void Update()
{
    if (_countdownStopwatch != null && _countdownStopwatch.IsRunning)
        UpdateCountdownStopwatch();
    else if (_roleCardStopwatch != null && _roleCardStopwatch.IsRunning)
        UpdateRoleCardStopwatch();
    else if (_meetupStopwatch != null && _meetupStopwatch.IsRunning)
        UpdateMeetupStopwatch();
    else if (_actionStopwatch != null && _actionStopwatch.IsRunning)
        UpdateActionStopwatch();
    else if (_votingStopwatch != null && _votingStopwatch.IsRunning)
        UpdateVoteStopwatch();
}

    private void UpdateCountdownStopwatch()
    {
        if (_countdownStopwatch.Elapsed.TotalSeconds >= Numerics.CountdownTime)
        {
            FetchPlayers();
            StopStopwatch(_countdownStopwatch);

            ShowRoleCard(Me);
            Me.Controller.SetOutfitVisibility();
            _uiScript.HideWarning();
            _uiScript.StopPerkPulsing();
            _uiScript.PerksAcknowledged = true;

            _roleCardStopwatch.Start();
            
            return;
        }
        
        UpdateProgressBar(_countdownStopwatch);
        
        if (_countdownStopwatch.Elapsed.TotalSeconds >= Numerics.RoleAssignmentTime && !_rolesAssigned)
        {
            FetchPlayers();
            
            if (Equals(Me, Master))
            {
                _roleController.AssignRoles();
            }

            _rolesAssigned = true;
        }
    }

    private void UpdateRoleCardStopwatch()
    {
        if (_roleCardStopwatch.Elapsed.TotalSeconds < Numerics.RoleShowTime)
            return;
        
        StopStopwatch(_roleCardStopwatch);
        _uiScript.RolePanelVisible = false;

        _meetupStopwatch.Start();
        StartMeetupStopwatch();
    }

    private void UpdateMeetupStopwatch()
    {
        if (_meetupStopwatch.Elapsed.TotalSeconds < Numerics.MeetupTime)
        {
            UpdateProgressBar(_meetupStopwatch);
        }
        else 
        {
            StopStopwatch(_meetupStopwatch);
            _gameplayController.ToggleNightForAllPlayers();
        } 
    }

    private void UpdateActionStopwatch()
    {
        if (_actionStopwatch.Elapsed.TotalSeconds < Numerics.ActionTime) 
        {
            UpdateProgressBar(_actionStopwatch);
        }
        else 
        {
            FetchPlayers();
            StopStopwatch(_actionStopwatch);
            _uiScript.DisableAllInteractionButtons();
            _uiScript.OnAwaiting();
            _uiScript.StopActionPulsing();
            _uiScript.HideWarning();
            UnityEngine.Debug.Log("запрашиваю экшон");
            Master.PhotonView.RpcSecure("ActionQueueForward", Master.PhotonPlayer, true);
        }
    }

    private void UpdateVoteStopwatch()
    {
        if (_votingStopwatch.Elapsed.TotalSeconds < Numerics.VoteTime)
        {
            UpdateProgressBar(_votingStopwatch);
        }
        else
        {
            FetchPlayers();
            StopStopwatch(_votingStopwatch);
            _uiScript.DisableAllInteractionButtons();
            _uiScript.OnAwaiting();
            _uiScript.StopVotingPulsing();
            _uiScript.HideWarning();
             UnityEngine.Debug.Log("отсюда"); 
            Me.PhotonView.RpcSecure("SetVoting", RpcTarget.AllBuffered, true, false);
            Me.PhotonView.RpcSecure("SetVotedActorNumbers", RpcTarget.AllBuffered, true, new[] { Numerics.VotingTimeExpired });
        }
    }

    private static void ResetStopwatch(Stopwatch stopwatch)
    {
        stopwatch.Stop();
        stopwatch.Reset();
    }
        
    private void StartStopwatch(Stopwatch stopwatch, int time, string localizedMessage)
    {
        stopwatch.Start();
        ShowProgressBar(localizedMessage, time);
    }
    
    private void StopStopwatch(Stopwatch stopwatch)
    {
        ResetStopwatch(stopwatch);
        _uiScript.ProgressBarVisible = false;
    }
    
    private void ShowProgressBar(string localizedStringName, int time)
    {
        _uiScript.ProgressBarMessage = GetLocalizedString(localizedStringName);
        _uiScript.ProgressBarMaxValue = time * 1000;
        _uiScript.ProgressBarVisible = true;
    }

    private void ShowRoleCard(PlayerData me)
    {
        _uiScript.RolePanelCard = me.Controller.Role;
        _uiScript.RolePanelVisible = true;
    }
    
    private void UpdateProgressBar(Stopwatch stopwatch) => 
        _uiScript.ProgressBarCurrentValue = Convert.ToSingle(stopwatch.Elapsed.TotalMilliseconds);
}
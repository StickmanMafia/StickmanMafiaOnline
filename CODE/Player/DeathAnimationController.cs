using System.Linq;
using Photon.Pun;
using UnityEngine;

public sealed class DeathAnimationController : Controller
{
    private FXSoundController _soundController;
    private DayTimeController _dayTimeController;
    private MainCameraController _mainCameraController;

    protected override void Start()
    {
        base.Start();
        _soundController = GameObject.Find("FXSoundController").GetComponent<FXSoundController>();
        _dayTimeController = GameObject.Find("DayTimeController").GetComponent<DayTimeController>();
        _mainCameraController = GameObject.Find("MainCamera").GetComponent<MainCameraController>();
    }

    public void OnPlayScreamSound() => _soundController.PlayDeathScream();

    public void OnPlaySplashSound() => _soundController.PlaySplash();
    
    public void OnDeathAnimationOver()
    {
        _mainCameraController.StartFollowingBoat();
        
        FetchPlayers();
        
        if (!Players.Any(n => n.Controller is {Alive: true, Role: not Role.Mafia and not Role.MafiaBoss})
            || !Players.Any(n => n.Controller is {Alive: true, Role: Role.Mafia}))
        {
            Me.Controller.SetGameOver(true);
        }

        if (_dayTimeController.CurrentDayTime == TimeOfDay.Day)
        {
             Debug.Log("отсюда");
            Me.PhotonView.RpcSecure("SetVoting", RpcTarget.AllBuffered, true, false);
            return;
        }
        
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        GoQueue();
    }
}
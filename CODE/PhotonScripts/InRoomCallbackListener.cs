using System.Collections;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class InRoomCallbackListener : PUNController
{
    [SerializeField]
    private GameObject _gameControllerObject;

    [SerializeField]
    private GameObject _countdownControllerObject;

    [SerializeField]
    private GameObject _playerParent;
    
    private GameplayController _gameplayController;
    private StopwatchController _stopwatchController;

    private GameplayUIBehaviour _uiScript;
    
    protected override void Start()
    {
        base.Start();
        _gameplayController = _gameControllerObject.GetComponent<GameplayController>();
        _stopwatchController = _countdownControllerObject.GetComponent<StopwatchController>();
        _uiScript = GameObject.Find("ui-document").GetComponent<GameplayUIBehaviour>();
    }

    public override void OnLeftRoom()
    {
        var newProps = new Hashtable {{Strings.PositionPropName, Numerics.NoId}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(newProps);
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (IsMaxPlayerQuantityReached() && !_gameplayController.GameRunning && PhotonNetwork.CurrentRoom.IsOpen)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            _stopwatchController.StartCountdownStopwatch();
        }
    }
    public void LeaveRoom(Player player){
        StartCoroutine(DelayedPlayerQuitAction(player));
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // FetchPlayers();
        //
        // foreach (var player in Players)
        // {
        //     if (player.PhotonPlayer.UserId != otherPlayer.UserId) continue;
        //     Destroy(player.GameObject);
        //     break;
        // }
            
           
            
        
        StartCoroutine(DelayedPlayerQuitAction(otherPlayer));
    }
    
    public static bool IsMaxPlayerQuantityReached() =>
        PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
    
    private IEnumerator DelayedPlayerQuitAction(Player otherPlayer)
    {
        yield return new WaitForSeconds(1);
                
        FetchPlayers();
        
        if (_gameplayController.GameRunning)
        {
            _uiScript.UpdateGoodVsBadPanelContents();
            
            if (Players.All(n => n.Controller.Role != Role.Mafia) || Players.Count() < 2)
            {
                if (!Equals(Me, Master)) yield break;
                
                foreach (var player in Players)
                    player.PhotonView.RpcSecure("SetGameOver", RpcTarget.AllBuffered, true, true);
            }
        }
        else
        {
            // foreach (var player in Players)
            // {
            //     if (player.PhotonPlayer.UserId != otherPlayer.UserId) continue;
            //     PhotonNetwork.Destroy(player.GameObject);
            // }
        }
    }
}
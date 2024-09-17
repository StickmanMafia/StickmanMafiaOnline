using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerQuantityController : Controller
{
    [SerializeField]
    private GameObject _uiDocument;
    
    private List<PlayerData> _playersInPreviousIteration;
    private GameplayUIBehaviour _uiScript;

    protected override void Start()
    {
        base.Start();
        _uiScript = _uiDocument.GetComponent<GameplayUIBehaviour>();
        _playersInPreviousIteration = new List<PlayerData>();
    }

    private void Update()
    {
        FetchPlayers();
        
        if (_playersInPreviousIteration.Count != Players.Count()) 
            _uiScript.UpdateAwaitPlayerList();

        _playersInPreviousIteration.Clear();
        _playersInPreviousIteration.AddRange(Players);
    }
}
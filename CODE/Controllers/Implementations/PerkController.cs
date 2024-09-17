#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PerkController : Controller
{
    private GameplayUIBehaviour _uiScript;

    private FXSoundController _soundController;

    private List<GameObject> _allPerks;

    protected override void Start()
    {
        base.Start();
        _uiScript = GameObject.Find("ui-document").GetComponent<GameplayUIBehaviour>();
        _soundController = GameObject.Find("FXSoundController").GetComponent<FXSoundController>();
        _allPerks = new List<GameObject>();
    }

    public void Add(Type perkType, PlayerData owner, PlayerData targetPlayer, Dictionary<Type, string> perkTypeLocalizedStrings)
    {
        if (CheckIfPerkExists(perkType, targetPlayer))
        {
            NotifyPerkRefusal(owner);
            return;
        }
        
        var perkGameObject = new GameObject(perkType.Name);
        perkGameObject.AddComponent(perkType);
        perkGameObject.GetComponent<Perk>().Owner = owner;
        perkGameObject.GetComponent<Perk>().TargetPlayer = targetPlayer;
        
        _allPerks.Add(perkGameObject);
        
        _uiScript.AddNewPerkMiniIcon(targetPlayer.Nickname, perkType.Name);
        
        _soundController.PlayPerkSound();
    }
    
    private bool CheckIfPerkExists(Type perkType, PlayerData targetPlayer) =>
        _allPerks.Any(n => n.GetComponent<Perk>().GetType() == perkType && n.GetComponent<Perk>().TargetPlayer.PhotonView == targetPlayer.PhotonView);
    
    private void NotifyPerkRefusal(PlayerData owner) => 
        owner.Controller.DisplayGameHostMessage(GetLocalizedString("perk_already_used"));
}
using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class RoleController : Controller
{
    private GameplayController _gameplayController;
    
    protected override void Start()
    {
        base.Start();
        _gameplayController = GameObject.Find("GameplayController").GetComponent<GameplayController>();
    }
    
    /*
     * WARNING TO THE DEVELOPER!
     * This method currently doesn't handle a situation where 3 or 2 players are left. It can lead to
     * having no mafia in the game and thus having the game stuck
     * in a continuous night - day cycle (with occasional police officer's / somebody else's action).
     * Since we don't have a proper approach to handling a leaving players amidst
     * role assignment / countdownStopwatch being active, I decided to not integrate
     * that feature. Yet this needs attention to be in resolved either way. 
     */
    public void AssignRoles()
    {
        FetchPlayers();
        
        var requiredRoles = GetRequiredRoles();
        
        foreach (var player in Players)
        {
            while (true)
            {
                /*
                 * I don't normally use try-catch blocks a lot
                 * in unity but this case is different, since it
                 * can stop other players from getting a role,
                 * in case someone leaves the room amidst role assignment
                 */
                try
                {
                    var roleInteger = player.Controller.ActiveRoleRequired && requiredRoles.Any(n => n.Key != Role.Civilian && n.Value > 0)
                        ? Random.Range(1, 8)
                        : Random.Range(0, 8);
            
                    var roleEnum = (Role)roleInteger;

                    if (!requiredRoles.Any(n => n.Key == roleEnum && n.Value > 0)) continue;
            
                    requiredRoles[roleEnum] -= 1;
                    player.PhotonView.RpcSecure("SetRole", RpcTarget.AllBuffered, true, roleEnum);
            
                    break;        
                }
                catch (Exception)
                {
                    break;
                }
            }
        }
        
        // This solution is kind of wicked and needs replacement
        // I will come back to it someday soon
        foreach (var player in Players) 
            player.PhotonView.RpcSecure("StartGame", player.PhotonPlayer, true);
    }

    // Format: <Role required, Quantity required>
    private static Dictionary<Role, int> GetRequiredRoles() => PhotonNetwork.CurrentRoom.MaxPlayers switch
    {
        4 => new Dictionary<Role, int>  
        {
            { Role.Civilian, 2 }, 
            { Role.Mafia, 1},
            { Role.PoliceOfficer, 1 }
        },
        
        6 => new Dictionary<Role, int>
        {
            { Role.Civilian, 2 }, 
            { Role.Mafia, 1 },
            { Role.PoliceOfficer, 1 },
            { Role.Doctor, 1 },
            { Role.MafiaBoss, 1 },
        },
        
        8 => new Dictionary<Role, int>
        {
            { Role.Civilian, 2 }, 
            { Role.Mafia, 1 },
            { Role.PoliceOfficer, 1 },
            { Role.Doctor, 1 },
            { Role.MafiaBoss, 1 },
            { Role.Witness, 1 },
            { Role.Maniac, 1 },
        },
        
        10 => new Dictionary<Role, int>
        {
            { Role.Civilian, 3 }, 
            { Role.Mafia, 2 },
            { Role.PoliceOfficer, 1 },
            { Role.Doctor, 1 },
            { Role.MafiaBoss, 1 },
            { Role.Witness, 1 },
            { Role.Maniac, 1 },
        },
        
        12 => new Dictionary<Role, int>
        {
            { Role.Civilian, 4 }, 
            { Role.Mafia, 3 },
            { Role.PoliceOfficer, 1 },
            { Role.Doctor, 1 },
            { Role.MafiaBoss, 1 },
            { Role.Witness, 1 },
            { Role.Maniac, 1 },
        },
        
        _ => new Dictionary<Role, int>()
    };
}
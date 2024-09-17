using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * This class is the first and lowest layer of PlayerController
 * class family. The idea to split PlayerController
 * into several abstract classes before PlayerController is instantiated
 * came up after PlayerController grew so big that it was hard to navigate.
 * PlayerRuntimeFields contains protected fields
 * that are used locally within PlayerController.
 */
public abstract class PlayerRuntimeFields : Controller
{
    [SerializeField]
    protected GameObject Swirl;

    [SerializeField]
    protected GameObject DeathSharkPrefab;

    [SerializeField]
    protected TextMeshPro Nickname;
    
    protected GameplayUIBehaviour UIScript;
    
    protected DayTimeController DayTimeController;
    protected StopwatchController StopwatchController;
    protected VotingController VotingController;
    protected MainCameraController MainCameraController;
    protected GameplayController GameplayController;
    protected FXSoundController SoundController;
    
    protected GameObject DeathAnimationInstance;
    public GameObject PlayerParents;
    protected GameObject OldOutfit;
    protected GameObject Outfit;

    public static readonly Dictionary<Role, string> RoleLocalizedStrings = new()
    {
        { Role.Civilian, "role_civilian" },
        { Role.PoliceOfficer, "role_police" },
        { Role.Mafia, "role_mafia" },
        { Role.Maniac, "role_maniac" },
        { Role.MafiaBoss, "role_mafiaboss" },
        { Role.Doctor, "role_doctor" },
        { Role.Witness, "role_witness" },
    };
}
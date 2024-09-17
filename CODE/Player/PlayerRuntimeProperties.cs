using Photon.Pun;

/*
 * This class is the second layer of PlayerController class family.
 * It contains PlayerProperties that are used in many different places
 * across the whole code base. They can be only modified outside of the class
 * if using RPC methods that are presented on the 5th level in PlayerRpcMethods class.
 */
public abstract class PlayerRuntimeProperties : PlayerRuntimeFields
{
    public PhotonView PhotonView { get; protected set; }
    public bool GameOver { get; protected set; }
    public bool Healed { get; protected set; }
    public bool Alive { get; protected set; }
    public bool Killed { get; protected set; }
    public bool Witnessed { get; protected set; }
    public bool Paralyzed { get; protected set; }
    public bool ParalyzedByRevenge { get; protected set; }
    public bool ActionPrompted { get; protected set; }
    public bool ActionImpactPrompted { get; set; }
    public bool NightSwitchPrompted { get; protected set; }
    public bool DaySwitchPrompted { get; protected set; }
    public bool Voting { get; protected set; }
    public bool VotedOut { get; protected set; }
    
    // The following properties are used in perks
    public bool ActionExecuted { get; protected set; }
    public bool HasRadio { get; protected set; }
    public bool CanVoteTwice { get; protected set; }
    public bool Disguised { get; protected set; }
    public bool HasHelicopter { get; protected set; }
    public bool HasMineDetector { get; protected set; }
    
    // It's not necessary to set this property back to "false",
    // when its used in RoleController, thus it stays "true"
    // till the end of the game.
    public bool ActiveRoleRequired { get; protected set; }
    
    // Those 2 methods below differ in terms of encapsulation, 
    // because they might occasionally be used by GameplayController
    // locally if the player's photon view !IsMine to display some
    // animations only visible to one player and not everybody.
    public bool Investigated { get; set; }
    public bool VotingPending { get; set; }
    
    public bool OutfitVisible { get; protected set; }
    public int[] VotingActorNumbers { get; protected set; }
    public int KilledActorNumber { get; protected set; }
    public int PointsScored { get; protected set; }
    public Role Role { get; protected set; }
    public ActionQueueInfo ActionQueueInfo { get; protected set; }

    public bool Intact => Alive && !Paralyzed && !ParalyzedByRevenge;
    public bool SuitableForActions => Role != Role.Civilian && Intact;
}
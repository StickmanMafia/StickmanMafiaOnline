using UnityEngine;

public abstract class Perk : MonoBehaviour
{
    public PlayerData TargetPlayer { get; set; }
    
    public PlayerData Owner { protected get; set; }

    protected GameplayController GameplayController;
    
    protected bool TargetPlayerExists => TargetPlayer.PhotonView != null
                                         && TargetPlayer.PhotonPlayer != null
                                         && TargetPlayer.Controller != null
                                         && TargetPlayer.GameObject != null;
    
    // Gonna replace wanky private fields that are called differently
    // but generally do the same thing in every perk child
    protected bool Completed { get; set; }
    
    protected abstract bool ConditionSatisfied { get; }
    
    protected virtual void Execute() => Destroy(gameObject);

    protected virtual void Start() => GameplayController = GameObject.Find("GameplayController").GetComponent<GameplayController>();

    protected virtual void Update()
    {
        if (ConditionSatisfied)
            Execute();
    }
}
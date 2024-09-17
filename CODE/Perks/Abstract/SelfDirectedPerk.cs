using Photon.Pun;

/*
 * SelfDirectedPerk really came into my head when I was
 * implementing Disguise and DoubleVoice. I noticed
 * way too many similarities and code repetition between
 * aforementioned perks and also ActiveRole & Radio,
 * so I decided to put their common logic into a separate class.
 */
public abstract class SelfDirectedPerk : Perk
{
    protected override bool ConditionSatisfied => TargetPlayerExists;
    
    protected abstract string SetterRpcName { get; }
    
    protected virtual void Execute(string setterRpcName, bool value = true)
    {
        TargetPlayer.PhotonView.RpcSecure(setterRpcName, RpcTarget.AllBuffered, true, value);
        Completed = true;
        Destroy(gameObject);
    }
    
    protected virtual void Start()
    {
        base.Start();
        Completed = false;
    }

    protected virtual void Update()
    {
        if (!Completed && ConditionSatisfied) 
            Execute(SetterRpcName);
    }
}
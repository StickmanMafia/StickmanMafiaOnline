using Photon.Pun;

public sealed class Radio : SelfDirectedPerk
{
    public int WiretappingAttempts { get; set; }
    
    protected override string SetterRpcName => "SetHasRadio";

    private const int MaxWiretappingAttempts = 3;
    
    protected override void Start()
    {
        base.Start();
        WiretappingAttempts = 0;
    }

    protected override void Execute(string setterRpcName, bool value = true)
    {
        TargetPlayer.PhotonView.RpcSecure(setterRpcName, RpcTarget.AllBuffered, true, value);
        Completed = true;
    }
    
    protected override void Update()
    {
        if (WiretappingAttempts == MaxWiretappingAttempts)
        {
            Execute(SetterRpcName, false);
            Destroy(gameObject);
            return;
        }

        base.Update();
    }
}
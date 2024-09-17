using Photon.Pun;
using UnityEngine;

public sealed class PlayerController : PlayerAnimationEvents
{
    protected override void Start()
    {
        base.Start();
        
        PhotonView = GetComponent<PhotonView>();
        Nickname.text = PhotonView.Owner.NickName;
        
        FindControllers();
        SetInitialPropertyValues();
        SetParent();
        CheckStartCountdown();
        // InstantiateSwirl();
        
        if (Equals(GetComponent<PhotonView>().Owner, PhotonNetwork.LocalPlayer))
            GetComponent<PhotonView>().RpcSecure("PlayAnimation", RpcTarget.AllBuffered, true, "SitDown");
      LoadCustomizationLocal();
        
    }
    

    private void Update()
    {
        
        if (!PhotonView.IsMine)
            return;
        AnswerPrimaryPrompts();
        AnswerSecondaryPrompts();
    }
}
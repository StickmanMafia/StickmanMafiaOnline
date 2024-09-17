using System.Collections;
using System.Collections.Generic;
using Models;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class NickChanger : MonoBehaviour
{
    public User currentUser;
   public InputField field;
    public DataBaseManager dataBase;

    // Update is called once per frame
    public void ChangeNick(){

        currentUser.username= field.text;
        PhotonNetwork.NickName = field.text;

        dataBase.SaveDataURL(currentUser);
    }
}

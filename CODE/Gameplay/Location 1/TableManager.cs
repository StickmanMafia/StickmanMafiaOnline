using Photon.Pun;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    private void Awake()
    {
        DisableAllTables();
        GameObject.Find("boat").transform.Find($"table-{PhotonNetwork.CurrentRoom.MaxPlayers}-seats").gameObject.SetActive(true);
    }
    
    private void DisableAllTables()
    {
        for (var i = 4; i <= 12; i += 2) 
            GameObject.Find("boat").transform.Find($"table-{i}-seats").gameObject.SetActive(false);
    }
}

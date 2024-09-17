using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public abstract class Controller : MonoBehaviour
{
    [SerializeField]
    protected LocalizedStringTable LocalizedStringTable;

    protected GameObject PlayerParent;
    protected IEnumerable<PlayerData> Players;

    protected PlayerData Me => Players?.FirstOrDefault(n => n.IsMine);
    protected PlayerData Master => Players?.FirstOrDefault(n => n.PhotonPlayer.IsMasterClient);

    protected virtual void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainLocation")
        {
            Debug.Log(LocalizationSettings.AvailableLocales.Locales.Count + " всего имеем");
            Debug.Log(PlayerPrefs.GetInt("language") + " текущая локализация");
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[PlayerPrefs.GetInt("language")];
        }

        PlayerParent = PlayerParentFetchService.Fetch();
    }

    public string GetLocalizedString(string key) => LocalizedStringTable.GetTable().GetEntry(key).Value;

    protected void FetchPlayers()
    {
        if (PlayerParent != null)
        {
            Players = PlayerFetchService.FetchPlayerData(PlayerParent);
        }
        else
        {
            Debug.LogError("PlayerParent is not initialized.");
        }
    }

    protected void GoQueue()
    {
        if (Master != null)
        {
            Master.PhotonView.RpcSecure("ActionQueueForward", RpcTarget.MasterClient, true);
        }
        else
        {
            Debug.LogError("Master is not initialized.");
        }
    }
}

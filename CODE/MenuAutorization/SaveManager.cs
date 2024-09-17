using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System;

public class SaveManager : MonoBehaviour
{
    private string UserID;
    public DataBaseManager manager;

    void Start()
    {
        UserID = PlayerPrefs.GetString("UserID");
    }

    public async Task<bool> Save(PlayerCustomization customizationConfig)
    {
        User user = await manager.ReadDataURLAsync(UserID);

        if (user != null)
        {
            user.customization = customizationConfig;
            PlayerPrefs.SetString("Customization", JsonUtility.ToJson(customizationConfig));
            manager.SaveDataURL(user);
            return true;
        }
        return false;
    }

    public PlayerCustomization Load()
    {
       return JsonUtility.FromJson<PlayerCustomization>(PlayerPrefs.GetString("Customization"));
       // User user = manager.ReadDataURL(UserName);
       // if (user != null)
       // {
       //     return user.customization;
      //  }
       // return null;
    }

    public async Task<string> Load(bool stringed)
    {
        User user =await manager.ReadDataURLAsync(UserID);
        if (user != null)
        {
            return JsonUtility.ToJson(user.customization);
        }
        return null;
    }
}
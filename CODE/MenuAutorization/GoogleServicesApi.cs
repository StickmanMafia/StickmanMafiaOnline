#if UNITY_ANDROID

using System.Collections;
using System.Collections.Generic;
 using GooglePlayGames;
 using GooglePlayGames.BasicApi;
using UnityEngine;

public class GoogleServicesApi : MonoBehaviour
{
    public DataBaseManager dataBaseManager;
    
    private void Start() {
      PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

     [System.Obsolete]
     internal void ProcessAuthentication(SignInStatus status) {
       if (status == SignInStatus.Success) {
        string userID = PlayGamesPlatform.Instance.localUser.userName;
 ;
         dataBaseManager.CheckUser(userID);
       }
        else
        {
        
         PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
       }
    
 }


}
#elif UNITY_STANDALONE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class NotApi : MonoBehaviour

{
    public DataBaseManager dataBaseManager;

   private void Start() {
    dataBaseManager.CheckUser(Random.Range(1,1000).ToString());
    }
}
public class WEBGL : MonoBehaviour
{
    public DataBaseManager dataBaseManager;

    [DllImport("__Internal")]
    private static extern void SetLocalStorageItem(string key, string value);

    [DllImport("__Internal")]
    private static extern string GetLocalStorageItem(string key);

    private void Start()
    {
        string uniqueId = GetLocalStorageItem("UniqueId");
        if (string.IsNullOrEmpty(uniqueId))
        {
            uniqueId = System.Guid.NewGuid().ToString();
            SetLocalStorageItem("UniqueId", uniqueId);
        }

        Debug.Log("Unique ID: " + uniqueId);

        dataBaseManager.CheckUser(uniqueId);
    }
}

#else
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TelegramAuth : MonoBehaviour
{
    private string botId = "2"; 
    private string apiUrl = "https://tg-game-bot.tonrare.com/game-api/auth/telegram-webapp";
   // public DataBaseManager DataBase;
    public DataBaseManager dataBaseManager;
     private void Start() {
        StartGame();
    }
   private void StartGame(){
   
            StartAuth();
       
   }
  
    bool IsMobileDevice()
    {
        // Проверка типа устройства
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            return true;
        }

        // Проверка разрешения экрана
        if (Screen.width <= 800 && Screen.height <= 600)
        {
            return true;
        }

        // Проверка наличия сенсорного экрана
        if (Input.touchSupported)
        {
            return true;
        }

        return false;
    }
   
    void StartAuth()
    {

          
            string authData = Application.absoluteURL.Split(new[] { '#' }, StringSplitOptions.None)[1];
            StartCoroutine(GetUserId(authData));
        
       
    }
   
    IEnumerator GetUserId(string authData)
    {
        WWWForm form = new WWWForm();
        form.AddField("bot_id", botId);
        form.AddField("auth_data", authData);
        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
        
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
            }
            else
            {
                string response = www.downloadHandler.text;
                var authResponse = JsonUtility.FromJson<AuthResponse>(response);
                string token = authResponse.token;

                StartCoroutine(GetUserData(token));
            }
        }
    }

    IEnumerator GetUserData(string token)
    {
        string userDataUrl = "https://tg-game-bot.tonrare.com/game-api/auth/user";

        using (UnityWebRequest www = UnityWebRequest.Get(userDataUrl))
        {
            www.SetRequestHeader("Authorization", "Bearer " + token);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
            }
            else
            {
                string response = www.downloadHandler.text;
                var userResponse = JsonUtility.FromJson<UserResponse>(response);
                string userId = userResponse.user.id;
                dataBaseManager.CheckUser(userId);
            }
        }
    }

    [Serializable]
    private class AuthResponse
    {
        public string token;
    }

    [Serializable]
    private class UserResponse
    {
        public User user;
    }

    [Serializable]
    private class User
    {
        public string id;
        public string username;
        public string name;
        public string created_at;
        public string referral_by_id;
    }
}
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using DG.Tweening;
using Proyecto26;
using UnityEngine.Networking;
using Sirenix.OdinInspector;

public class DataBaseManager : MonoBehaviour
{
    public string database_url = "";
    public ManagerAuth manager;
    public TMP_InputField userNameINPUT;
    private string rememberedID;
    private bool IsnewUser = false;
     public Color AnimColor;
   public Color AnimColor2;
   public InputField window;
   public Transform windowField;
   
    private void Start() {
        
    }
    public void EnerInPC(){
        CheckUser(window.text);
        window.gameObject.SetActive(false);

    }   
        [Button("войти")]

    public async void CheckUser(string userID)
    {
        User user = await ReadDataURLAsync(userID);

        if (user != null)
        {
           PlayerPrefs.SetString("UserID", userID);
           PlayerPrefs.SetInt("money",user.money);
           PlayerPrefs.SetString("Customization",JsonUtility.ToJson(user.customization));
           PlayerPrefs.SetInt("gems",user.gems);
           manager.AuthEnded(user.username);
            
        }
        else
        {
            rememberedID = userID;
            IsnewUser = true;
            windowField.DOScale(1,0.7f);

        }
    }

    public void CreateUser()
    {
        string userName = userNameINPUT.text;
        if (IsnewUser)
        {
            User newUser = new User(rememberedID, userName, new PlayerCustomization(),100,new AllPerksStorage(),10);
            SaveDataURL(newUser);
            PlayerPrefs.SetString("UserID", rememberedID);
            PlayerPrefs.SetInt("money",100);
             PlayerPrefs.SetInt("gems",10);
            PlayerPrefs.SetString("Customization", JsonUtility.ToJson(new PlayerCustomization()));
            manager.AuthEnded(userName);
            IsnewUser = false;
        }
    }

    public async Task<User> ReadDataURLAsync(string userID)
    {
         UnityWebRequest request = UnityWebRequest.Get(database_url + "/Users/" + userID + ".json");
        await request.SendWebRequest();

    while (!request.isDone)
    {
        
    }

    if (request.result == UnityWebRequest.Result.Success)
    {
        if(request.downloadHandler.text == "null") return null;
        else return JsonUtility.FromJson<User>(request.downloadHandler.text);
        
    }
    else
    {
        Debug.LogError("Failed to get user data: " + request.error);
        return null;
    }
    }
   


    public void SaveDataURL(User user)
    {
        RestClient.Put(database_url + "/Users/" + user.id + ".json", user);
    }
    
    public void AnimateButton(Transform me)
    {
        float scaleMe = me.transform.localScale.x;
        me.DOScale(scaleMe * 1.1f, 0.15f)
            .OnComplete(() => me.DOScale(scaleMe, 0.075f));
    }

    public void AnimateColor(Image me)
    {
        float scaleMe = me.transform.localScale.x;
        me.DOColor(AnimColor2, 0.15f).OnComplete(() => me.DOColor(AnimColor, 0.05f));
    }
}

[System.Serializable]
public class User
{
    public string id;
    public string username;
    public PlayerCustomization customization;
    public int money;
    public AllPerksStorage perks;
    public int gems;

    public User(string id, string username, PlayerCustomization customization, int money, AllPerksStorage perks,int gems)
    {
        this.id = id;
        this.username = username;
        this.customization = customization;
        this.money = money;
        this.perks = perks;
        this.gems = gems;

    }
}


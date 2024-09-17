using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BetGame : MonoBehaviour
{
     public User currentUser;
    public DataBaseManager dataBase;
    public int CurrentBet;
    async void Start()
    {
        CurrentBet = PlayerPrefs.GetInt("MyBet");
        await LoadUserDataAsync();
        
        }
    
    async Task LoadUserDataAsync()
    {
        string userId = PlayerPrefs.GetString("UserID");

        currentUser =await dataBase.ReadDataURLAsync(userId);
    }
     
    public void WinGame(int players){
        
        PlayerPrefs.SetInt("WinBet",Mathf.RoundToInt(players*CurrentBet*0.95f));
        PlayerPrefs.SetInt("CaseRecieve",1);
        PlayerPrefs.SetInt("MyBet",0);
    }
    public void LoseGame(){
        PlayerPrefs.SetInt("MyBet",0);
    }
    public void DontStarted(){
        currentUser.money += PlayerPrefs.GetInt("MyBet");
        dataBase.SaveDataURL(currentUser);
        PlayerPrefs.SetInt("MyBet",0);
    }

    // Update is called once per frame
    
}

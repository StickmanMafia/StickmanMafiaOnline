using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MoneyGet : MonoBehaviour
{
    public User currentUser;
    public DataBaseManager dataBase;
    public Image[] moneyImages;
    public managerMenu menu;

    private void Start() {
        int streak = PlayerPrefs.GetInt("CurrentStreak");
        if(streak > 4)
            streak = 1;
        if(streak>0 && PlayerPrefs.GetInt("Recieved")==0){
            moneyImages[streak-1].gameObject.GetComponent<Button>().interactable = true;
        }
    }
    
    public async void GetMoneyAsync(int money){
         await LoadUserDataAsync();
         currentUser.money+=money;
         dataBase.SaveDataURL(currentUser);
         menu.money.text = currentUser.money.ToString();
         PlayerPrefs.SetInt("Recieved",1);
         PlayerPrefs.SetInt("CurrentStreakKey",0);

         foreach(Image item in moneyImages){
             item.gameObject.GetComponent<Button>().interactable = false;
         }

    }
     async Task LoadUserDataAsync()
    {
        string userId = PlayerPrefs.GetString("UserID");

        currentUser =await dataBase.ReadDataURLAsync(userId);
    }
}

    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
public class managerMenu : MonoBehaviour

{
    public DataBaseManager dataBase;
    public  MainMenuBehaviour menu;
    public Text money;
    public User currentUser;
    public Text gems;
    public ShopPerks perks;
    public BetMoney bet;
    public NickChanger nickChanger;
    public CaseLogic caseLogic;
    private async void Start() {
       await LoadUserDataAsync();
       
       //if(PlayerPrefs.GetInt("language")==0){
      //      lang.sprite = Resources.Load<Sprite>("russian");
      //  }
      //  else{
      //      lang.sprite = Resources.Load<Sprite>("english");
     //   } 
    }
    public async void Load(){
        await LoadUserDataAsync();
    }
     async Task LoadUserDataAsync()
    {
        string userId = PlayerPrefs.GetString("UserID");

        currentUser =await dataBase.ReadDataURLAsync(userId);
        LoadPlayer();
    }
    public void LoadPlayer(){
        perks.currentUser = currentUser;
        perks.LoadPerks();
        nickChanger.currentUser = currentUser;
        bet.currentUser = currentUser;
        caseLogic.currentUser = currentUser;
        bet.ready();
        CheckMoney();
        CheckGems();
    }
    public void CheckMoney(){
        money.text = currentUser.money.ToString();
    }
    public void CheckGems(){
        gems.text = currentUser.gems.ToString();
    }
    public void ShowItem(Transform item){
        item.DOScale(1,0.3f);

    }
    public void LoadCustomization(){
        SceneManager.LoadScene("Customization");
    }
    public void HideItem(Transform item){
        item.DOScale(0,0.3f);
    }
    public void ChangeOrderToTop(Canvas canvas){
        canvas.sortingOrder = 1;
    }
    public void ChangeOrderToBottom(Canvas canvas){
        canvas.sortingOrder = -1;
    } 
    public void Exit(){
        Application.Quit(); 
    }
   
}

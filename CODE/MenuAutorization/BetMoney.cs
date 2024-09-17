using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BetMoney : MonoBehaviour
{
    private Slider me;

    public User currentUser;
    public DataBaseManager dataBase;
    public Text text;
    public int money;
    public managerMenu menu;
    
    public void VKL(){
        me = GetComponent<Slider>();
        me.value = 50;
    }
   
        
        public void ready(){
            if (PlayerPrefs.GetInt("MyBet") != 0)
        {
            currentUser.money += PlayerPrefs.GetInt("MyBet");
            Save();
            PlayerPrefs.SetInt("MyBet",0);
            
        }
        else if (PlayerPrefs.GetInt("WinBet") != 0)
        {
            currentUser.money += PlayerPrefs.GetInt("WinBet");
            Save();
            PlayerPrefs.SetInt("WinBet",0);
        }
        
        CheckMoney();
        }
    
   
    public void CheckMoney(){
       
       me = GetComponent<Slider>();
       text.text = me.value+"";
       me.value = currentUser.money/10;
     //  me.maxValue = currentUser.money/2;
    
       
    }

    public void Save(){
     
     dataBase.SaveDataURL(currentUser);
     menu.money.text = currentUser.money.ToString();

    }
    public void Change(){
        me.value = Mathf.Round( me.value / 50) * 50;
        text.text = me.value.ToString();
        
    }
    public void EndBet(){
        PlayerPrefs.SetInt("MyBet",(int)me.value);
        currentUser.money -= (int)me.value;
        menu.money.text = currentUser.money.ToString();
        dataBase.SaveDataURL(currentUser);
    }
    
    
}

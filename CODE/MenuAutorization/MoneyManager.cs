using System.Threading.Tasks;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    private User currentUser;
    private DataBaseManager databaseManager;

    async void Start()
    {
        databaseManager = GetComponent<DataBaseManager>();

        await LoadUserDataAsync();
    }

    async Task LoadUserDataAsync()
    {
        string userId = PlayerPrefs.GetString("UserID");

        currentUser = await databaseManager.ReadDataURLAsync(userId);
    }

    void SaveUserData()
    {
        databaseManager.SaveDataURL(currentUser);
    }

    public void PlaceBet(int betAmount)
    {
        if (betAmount <= currentUser.money)
        {
            currentUser.money -= betAmount;
            SaveUserData();
        }
        
    }

    public void WinBet(int playerCount, int betAmount)
    {
        int winAmount = (int)(playerCount * betAmount * 0.95f);
        currentUser.money += winAmount;
        SaveUserData();
    }
    public void PlusMoney(int money){
        currentUser.money += money;
        SaveUserData();
    }
}

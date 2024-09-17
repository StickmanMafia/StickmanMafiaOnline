using System;
using UnityEngine;

public class DayCounter : MonoBehaviour
{
    private const string LastLoginKey = "LastLogin";
    private const string CurrentStreakKey = "CurrentStreak";

    void Awake()
    {
        string lastLoginString = PlayerPrefs.GetString(LastLoginKey, DateTime.MinValue.ToString());
        DateTime lastLogin = DateTime.Parse(lastLoginString);

        DateTime now = DateTime.Now;

        if (now.Date == lastLogin.Date)
        {
            if(PlayerPrefs.GetInt("Recieved")==0)
                PlayerPrefs.SetInt(CurrentStreakKey, 1);
            else
                PlayerPrefs.SetInt(CurrentStreakKey,0);

        }
        else if (now.Date == lastLogin.Date.AddDays(1))
        {
            PlayerPrefs.SetInt("Recieved",0);
            int currentStreak = PlayerPrefs.GetInt(CurrentStreakKey, 0) + 1;
            PlayerPrefs.SetInt(CurrentStreakKey, currentStreak);
            PlayerPrefs.SetString(LastLoginKey, now.ToString());
        }
        else
        {
            PlayerPrefs.SetInt("Recieved",0);
            PlayerPrefs.SetInt(CurrentStreakKey, 1);
            PlayerPrefs.SetString(LastLoginKey, now.ToString());
        }
    }
}
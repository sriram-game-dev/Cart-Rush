
using System;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    public static PlayerPrefsManager instance;

    private const string LoginKey = "LoginStatus";
    private const string LastDateKey = "Daily_LastDate";
    private const string DailyGroceriesKey = "Daily_GroceriesCollected";
    private const string DailySavingsKey = "Daily_SavingsCollected";

    public string LoginStatus { get; private set; }

    public int DailyGroceriesProgress { get; private set; }
    public int DailySavingsProgress { get; private set; }

    public const int DailyGroceriesTarget = 20;
    public const int DailySavingsTarget = 5;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadLoginStatus();
        CheckAndInitDailyObjectives();
    }

    public void LoadLoginStatus()
    {
        LoginStatus = PlayerPrefs.GetString(
            LoginKey,
            "Entry_LoginFirstTime"
        );
    }

    public void SaveLoginStatus(string status)
    {
        LoginStatus = status;

        PlayerPrefs.SetString(LoginKey, status);
        PlayerPrefs.Save();
    }

    public void ResetLoginStatus()
    {
        PlayerPrefs.DeleteKey(LoginKey);
        PlayerPrefs.Save();

        LoginStatus = "Entry_LoginFirstTime";
    }

    public void CheckAndInitDailyObjectives()
    {
        string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        string lastDate = PlayerPrefs.GetString(LastDateKey, string.Empty);

        if (lastDate != today)
        {
            // New day or first time! Reset daily progress
            PlayerPrefs.SetString(LastDateKey, today);
            PlayerPrefs.SetInt(DailyGroceriesKey, 0);
            PlayerPrefs.SetInt(DailySavingsKey, 0);
            PlayerPrefs.Save();

            DailyGroceriesProgress = 0;
            DailySavingsProgress = 0;
        }
        else
        {
            DailyGroceriesProgress = PlayerPrefs.GetInt(DailyGroceriesKey, 0);
            DailySavingsProgress = PlayerPrefs.GetInt(DailySavingsKey, 0);
        }
    }

    public void AddDailyGroceries(int amount)
    {
        DailyGroceriesProgress = Mathf.Clamp(DailyGroceriesProgress + amount, 0, DailyGroceriesTarget);
        PlayerPrefs.SetInt(DailyGroceriesKey, DailyGroceriesProgress);
        PlayerPrefs.Save();
    }

    public void AddDailySavings(int amount)
    {
        DailySavingsProgress = Mathf.Clamp(DailySavingsProgress + amount, 0, DailySavingsTarget);
        PlayerPrefs.SetInt(DailySavingsKey, DailySavingsProgress);
        PlayerPrefs.Save();
    }

    public bool IsDailyObjectivesCompleted()
    {
        return DailyGroceriesProgress >= DailyGroceriesTarget && DailySavingsProgress >= DailySavingsTarget;
    }
}

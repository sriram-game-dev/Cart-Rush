using System;
using System.Globalization;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    public static PlayerPrefsManager instance;

    private const string LoginKey = "LoginStatus";
    private const string LastDateKey = "Daily_LastDate";
    private const string DailyGroceriesKey = "Daily_GroceriesCollected";
    private const string DailySavingsKey = "Daily_SavingsCollected";
    private const string DefaultLoginStatus = "Entry_LoginFirstTime";

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

    private void OnApplicationFocus(bool hasFocus)
    {
        // Refresh when returning from the background (handles midnight rollover)
        if (hasFocus) CheckAndInitDailyObjectives();
    }

    // ---------------- Login ----------------

    public void LoadLoginStatus()
    {
        LoginStatus = PlayerPrefs.GetString(LoginKey, DefaultLoginStatus);
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

        LoginStatus = DefaultLoginStatus;
    }

    // ---------------- Daily objectives ----------------

    public void CheckAndInitDailyObjectives()
    {
        string today = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
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
        CheckAndInitDailyObjectives();
        DailyGroceriesProgress = Mathf.Clamp(DailyGroceriesProgress + amount, 0, DailyGroceriesTarget);
        PlayerPrefs.SetInt(DailyGroceriesKey, DailyGroceriesProgress);
        PlayerPrefs.Save();
    }

    public void AddDailySavings(int amount)
    {
        CheckAndInitDailyObjectives();
        DailySavingsProgress = Mathf.Clamp(DailySavingsProgress + amount, 0, DailySavingsTarget);
        PlayerPrefs.SetInt(DailySavingsKey, DailySavingsProgress);
        PlayerPrefs.Save();
    }

    public bool IsDailyObjectivesCompleted()
    {
        CheckAndInitDailyObjectives();
        return DailyGroceriesProgress >= DailyGroceriesTarget
            && DailySavingsProgress >= DailySavingsTarget;
    }

    // ---------------- Resets ----------------

    public void ResetDailyObjectives()
    {
        PlayerPrefs.DeleteKey(LastDateKey);
        PlayerPrefs.DeleteKey(DailyGroceriesKey);
        PlayerPrefs.DeleteKey(DailySavingsKey);
        PlayerPrefs.Save();

        DailyGroceriesProgress = 0;
        DailySavingsProgress = 0;
    }

    public void ResetAll()
    {
        ResetLoginStatus();
        ResetDailyObjectives();
    }

    [ContextMenu("Reset Daily Objectives")]
    private void DebugResetDaily() => ResetDailyObjectives();
}
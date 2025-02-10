using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class DailyLoginSystem : MonoBehaviour
{
    public GameObject dailyLoginPopup; // The UI popup
    public TextMeshProUGUI rewardText; // Text to display reward message
    public Button claimButton; // Button to claim reward
    private DateTime lastLogin;

    void Start()
    {
        CheckDailyLogin();
        claimButton.onClick.AddListener(ClaimDailyReward);
    }

    void CheckDailyLogin()
    {
        // Retrieve last login time, or default to a past date
        string lastLoginStr = PlayerPrefs.GetString("LastLoginDate", "2000-01-01");
        lastLogin = DateTime.Parse(lastLoginStr);

        // Check if a new day has started
        if (DateTime.Now.Date > lastLogin.Date)
        {
            ShowDailyLoginPopup();
        }
    }

    void ShowDailyLoginPopup()
    {
        dailyLoginPopup.SetActive(true);
        rewardText.text = "Daily Reward Available!";
    }

    void ClaimDailyReward()
    {
        // Give the reward (customize based on your game)
        Debug.Log("Daily Reward Claimed!");

        // Update the last login date
        PlayerPrefs.SetString("LastLoginDate", DateTime.Now.ToString("yyyy-MM-dd"));
        PlayerPrefs.Save();

        // Hide the popup
        dailyLoginPopup.SetActive(false);
    }
}
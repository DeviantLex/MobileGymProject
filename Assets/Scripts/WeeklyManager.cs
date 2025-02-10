using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginTracker : MonoBehaviour
{
    public RectTransform[] dayPanels; // Array of panels representing each day of the week
    public TextMeshProUGUI[] dayLabels; // Text labels to display login counts
    public int[] loginCounts = new int[7]; // Array to track logins for each day
    public float maxHeight = 200f; // Maximum height for the bar graph
    private void Start() {
        TrackLogin();
        UpdateGraph();
    }
    private void TrackLogin() {
        // Get current day index (0 = Sunday, 1 = Monday, ..., 6 = Saturday)
        int todayIndex = (int)DateTime.Now.DayOfWeek;

        // Increase login count for today
        loginCounts[todayIndex]++;

        // Save updated login data
        SaveLoginData();
    }
    private void UpdateGraph() {
        for (int i = 0; i < 7; i++) {
            // Update panel height based on login count
            float height = (loginCounts[i] / (float)GetMaxLoginCount()) * maxHeight;
            dayPanels[i].sizeDelta = new Vector2(dayPanels[i].sizeDelta.x, Mathf.Max(height, 10)); // Ensure minimum height

            // Update label text to show login count
            dayLabels[i].text = $"{(DayOfWeek)i}\n{loginCounts[i]}";
        }
    }
    private int GetMaxLoginCount() {
        int max = 1;
        foreach (int count in loginCounts) {
            if (count > max) max = count;
        }
        return max;
    }
    private void SaveLoginData() {
        for (int i = 0; i < 7; i++)
        {
            PlayerPrefs.SetInt($"LoginCount_{i}", loginCounts[i]);
        }
        PlayerPrefs.Save();
    }

    private void LoadLoginData() {
        for (int i = 0; i < 7; i++) {
            loginCounts[i] = PlayerPrefs.GetInt($"LoginCount_{i}", 0);
        }
    }
}
using UnityEngine;
using TMPro; // For UI text

public class TimerController : MonoBehaviour
{
    public float timeRemaining = 10f; // Set timer duration
    public bool timerRunning = false; // Control timer state
    public TextMeshProUGUI timerText; // UI Text to display time

    void Start()
    {
        timerRunning = true; // Start the timer
    }

    void Update()
    {
        if (timerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime; // Reduce time
                UpdateTimerUI();
            }
            else
            {
                timeRemaining = 0;
                timerRunning = false;
                Debug.Log("Timer finished!");
            }
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = timeRemaining.ToString("F2"); // Display time with 2 decimal places
        }
    }
}
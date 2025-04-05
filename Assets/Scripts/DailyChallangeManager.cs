using UnityEngine;
using UnityEngine.UI;
using System;

public class DailyChallengeManager : MonoBehaviour
{
    public Animator animator;
    public Button slideButton;
    private bool isVisible = false;

    public Sprite checkSprite; // ✅ Completed challenge sprite
    public Sprite emptySprite; // 🔲 Default empty sprite

    public DungeonGenerator dungeonGenerator;
    public ExercisePanelManager exercisePanelManager;
    public ShopManager shopManager;
    public DailyLoginSystem dailyLoginSystem;

    public Image dayCheckImage;
    public Image dungeonCheckImage;
    public Image shopCheckImage;
    public Image exerciseCheckImage;

    private bool dayChallengeComplete;
    private bool dungeonChallengeComplete;
    private bool shopChallengeComplete;
    private bool exerciseChallengeComplete;

    void Start() {
        if (slideButton == null || animator == null) {
            Debug.LogError("⚠️ Button or Animator is not assigned in the Inspector!");
            return;
        }

        animator.Play("HiddenPanel");
        slideButton.onClick.AddListener(ToggleSlide);

        CheckDailyReset(); // ⏳ Check if a reset is needed
        LoadChallengeStates(); // 🔄 Load previous states if today’s challenges were completed
    }

    public void ToggleSlide() {
        isVisible = !isVisible;
        animator.SetBool("IsVisible", isVisible);
    }

    private void CheckDailyReset() {  // Check if it's a new day and reset challenges if necessary
        string lastCompletedDate = PlayerPrefs.GetString("LastCompletedDate", "");
        string today = DateTime.Now.ToString("yyyy-MM-dd");

        if (lastCompletedDate != today) {
            ResetChallenges();
            PlayerPrefs.SetString("LastCompletedDate", today);
            PlayerPrefs.Save();
        }
    }
    private void LoadChallengeStates()
    {
        // Load stored challenge completion states
        dayChallengeComplete = PlayerPrefs.GetInt("DayChallenge", 0) == 1;
        dungeonChallengeComplete = PlayerPrefs.GetInt("DungeonChallenge", 0) == 1;
        shopChallengeComplete = PlayerPrefs.GetInt("ShopChallenge", 0) == 1;
        exerciseChallengeComplete = PlayerPrefs.GetInt("ExerciseChallenge", 0) == 1;

        // Update the UI accordingly
        UpdateSprite(dayCheckImage, dayChallengeComplete);
        UpdateSprite(dungeonCheckImage, dungeonChallengeComplete);
        UpdateSprite(shopCheckImage, shopChallengeComplete);
        UpdateSprite(exerciseCheckImage, exerciseChallengeComplete);

        // ✅ Ensure already completed challenges stay marked as completed
        Debug.Log($"Challenges loaded: Day={dayChallengeComplete}, Dungeon={dungeonChallengeComplete}, Shop={shopChallengeComplete}, Exercise={exerciseChallengeComplete}");
    }

    // 🔹 Reset all challenges (for new day)
    private void ResetChallenges()
    {
        dayChallengeComplete = false;
        dungeonChallengeComplete = false;
        shopChallengeComplete = false;
        exerciseChallengeComplete = false;

        PlayerPrefs.SetInt("DayChallenge", 0);
        PlayerPrefs.SetInt("DungeonChallenge", 0);
        PlayerPrefs.SetInt("ShopChallenge", 0);
        PlayerPrefs.SetInt("ExerciseChallenge", 0);
        PlayerPrefs.Save();

        UpdateSprite(dayCheckImage, false);
        UpdateSprite(dungeonCheckImage, false);
        UpdateSprite(shopCheckImage, false);
        UpdateSprite(exerciseCheckImage, false);

        Debug.Log("🌅 New day detected! Challenges reset.");
    }

    // ✅ Mark individual challenge as completed and save progress
    private void CheckAndUpdate(ref bool challengeComplete, Image checkImage, string challengeKey, string challengeName)
    {
        if (!challengeComplete)
        {
            challengeComplete = true;
            PlayerPrefs.SetInt(challengeKey, 1);
            PlayerPrefs.Save();
            UpdateSprite(checkImage, true);
            Debug.Log($"✅ Challenge Completed: {challengeName}");
        }
    }

    public void CheckChallenges()
    {
        if (!dayChallengeComplete && dailyLoginSystem.claimButton)
            CheckAndUpdate(ref dayChallengeComplete, dayCheckImage, "DayChallenge", "Daily Login");

        if (!dungeonChallengeComplete && dungeonGenerator.exitButton)
            CheckAndUpdate(ref dungeonChallengeComplete, dungeonCheckImage, "DungeonChallenge", "Dungeon Exit");

        if (!shopChallengeComplete && shopManager.itemPurchased)
            CheckAndUpdate(ref shopChallengeComplete, shopCheckImage, "ShopChallenge", "Shop Purchase");

        if (!exerciseChallengeComplete && exercisePanelManager.finishButton)
            CheckAndUpdate(ref exerciseChallengeComplete, exerciseCheckImage, "ExerciseChallenge", "Exercise Completion");
    }

    // 🖼️ Helper function to update challenge UI
    private void UpdateSprite(Image checkImage, bool isCompleted)
    {
        checkImage.sprite = isCompleted ? checkSprite : emptySprite;
    }
}
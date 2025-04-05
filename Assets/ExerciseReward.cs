using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ExerciseReward : MonoBehaviour {
    public Transform exerciseRewardPanelParent;
    public GameObject rewardPrefab;
    public ExercisePanelManager exercisePanelManager;
    public TextMeshProUGUI expRewardAmount;
    public PlayerLifeStats playerLifeStats;
    public ExerciseManager exerciseManager;
    

  public void UpdateRewardUI(List<ExerciseData> savedExercises) {

    GameObject newRewardPanel = Instantiate(rewardPrefab, exerciseRewardPanelParent);
    newRewardPanel.SetActive(true);

    SetRewardContent(newRewardPanel, savedExercises);
}
    
    private void SetRewardContent(GameObject rewardPanel, List<ExerciseData> exercises) {
        TextMeshProUGUI rewardExerciseSets = rewardPanel.transform.Find("rewardSets")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI rewardDateText = rewardPanel.transform.Find("rewardDate")?.GetComponent<TextMeshProUGUI>();

        int totalReward = exercises.Sum(e => {
        var selection = exerciseManager.GetExerciseSelection(e.exerciseName);
        return int.TryParse(selection?.exerciseReward, out int exp) ? exp : 0;
    });
        playerLifeStats.currentLevelExp += totalReward;
        expRewardAmount.text = "You gained: " + totalReward + " exp. Current Exp: " + playerLifeStats.currentLevelExp;


        // 🔹 Set the date text
        if (rewardDateText && exercisePanelManager?.dayText != null) {
            rewardDateText.text = $"<b>{exercisePanelManager.dayText.text}</b>\n\n"; 
        }

        if (rewardExerciseSets) {
            string summaryHistory = "";

            foreach (var exercise in exercises) {
                if (exercise.sets.Count == 0) continue; // Skip exercises with no sets

                int totalSets = exercise.sets.Count;
                float maxWeight = 0;
                int maxReps = 0;

                // Find max weight & reps
                foreach (var set in exercise.sets) {
                    if (float.TryParse(set.weight, out float weight) && int.TryParse(set.reps, out int reps)) {
                        if (weight > maxWeight || (weight == maxWeight && reps > maxReps)) {
                            maxWeight = weight;
                            maxReps = reps;
                        }
                    }
                }

                // 🔹 Summary format
                summaryHistory += $"{totalSets} x <b>{exercise.exerciseName}</b>\n";
                summaryHistory += $" Best Set: {maxWeight} lbs x {maxReps} reps\n\n";
            }
            rewardExerciseSets.text = summaryHistory;
        }
    }
}
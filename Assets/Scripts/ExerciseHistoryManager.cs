using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ExerciseHistoryManager : MonoBehaviour {
    public Transform historyPanelParent;
    public GameObject historyPrefab;
    public ExercisePanelManager exercisePanelManager;

    public void UpdateHistoryUI(List<ExerciseData> savedExercises) {
        Debug.Log("Adding new exercise history panel...");

        // 🔹 Check if historyPrefab is assigned
        if (historyPrefab == null) {
            Debug.LogError("History Prefab is not assigned in ExerciseHistoryManager!");
            return;
        }

        // 🔹 Instantiate a new history panel for each save
        GameObject newHistoryPanel = Instantiate(historyPrefab, historyPanelParent);
        newHistoryPanel.SetActive(true);

        SetHistoryContent(newHistoryPanel, savedExercises);
    }

   private void SetHistoryContent(GameObject historyPanel, List<ExerciseData> exercises) {
    TextMeshProUGUI historyExerciseSets = historyPanel.transform.Find("historySets")?.GetComponent<TextMeshProUGUI>();
    TextMeshProUGUI historyDateText = historyPanel.transform.Find("historyDate")?.GetComponent<TextMeshProUGUI>();

    if (historyDateText && exercisePanelManager?.dayText != null) {
        historyDateText.text = $"<b>{exercisePanelManager.dayText.text}</b>\n\n"; // Adds space after date
    }

    if (historyExerciseSets) {
        string setHistory = "";

        foreach (var exercise in exercises) {
            setHistory += $"<b>{exercise.exerciseName}</b>\n"; 
            foreach (var set in exercise.sets) {
                setHistory += $"  • Set {set.setNumber}: {set.weight} lbs x {set.reps} reps\n";
            }
            setHistory += "\n"; // Extra space between exercises
        }

        historyExerciseSets.text = setHistory;
    }
}
}
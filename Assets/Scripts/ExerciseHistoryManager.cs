using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ExerciseHistoryManager : MonoBehaviour {
    public Transform historyPanelParent;
    public GameObject historyPrefab;

    private GameObject historyPanelInstance; // 🔹 Store a single history panel

    public void UpdateHistoryUI(List<ExerciseData> savedExercises) {
        Debug.Log("Updating Exercise History UI...");

        // 🔹 Check if historyPrefab is assigned
        if (historyPrefab == null) {
            Debug.LogError("History Prefab is not assigned in ExerciseHistoryManager!");
            return;
        }

        // 🔹 Clear previous panel if it exists to prevent duplicates
        if (historyPanelInstance != null) {
            Destroy(historyPanelInstance);
        }

        // 🔹 Create a single panel to hold all history
        historyPanelInstance = Instantiate(historyPrefab, historyPanelParent);
        historyPanelInstance.SetActive(true);

        SetHistoryContent(historyPanelInstance, savedExercises);
    }

    private void SetHistoryContent(GameObject historyPanel, List<ExerciseData> exercises) {
        TextMeshProUGUI historyExerciseSets = historyPanel.transform.Find("historySets")?.GetComponent<TextMeshProUGUI>();

        if (historyExerciseSets) {
            string setHistory = "";

            // 🔹 Iterate over all exercises and append data
            foreach (var exercise in exercises) {
                setHistory += $"<b>{exercise.exerciseName}</b>\n"; // 🔹 Bold exercise name
                foreach (var set in exercise.sets) {
                    setHistory += $"  • Set {set.setNumber}: {set.weight} lbs x {set.reps} reps\n";
                }
                setHistory += "\n"; // 🔹 Add spacing between exercises
            }

            historyExerciseSets.text = setHistory;
        }
    }
}
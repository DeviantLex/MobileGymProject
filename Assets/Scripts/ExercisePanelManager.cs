using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.IO;

public class ExercisePanelManager : MonoBehaviour
{
    public GameObject setPrefab, exercisePrefab;
    public Transform contentPanel;
    public Button addSetButton, deleteExercsiseButton, finishButton;
    public float setSpacing = 10f, exerciseSpacing;
    public ExerciseManager exerciseManager;
    public ExerciseHistoryManager historyManager;
    public Transform exerciseContent;
    public GameObject exerciseButtonPrefab;
    public PlayerLifeStats playerLifeStats;
    public int ExpRewards = 50;

    private int exerciseNum = 0;
    private string previousSet = "100 x 15";
    private List<ExerciseData> exercises = new List<ExerciseData>();
    private string saveFilePath;

    void Start() {
        saveFilePath = Path.Combine(Application.persistentDataPath, "exerciseData.json");

        addSetButton?.onClick.AddListener(() => AddSet(exercises.Count > 0 ? exercises[^1] : null));
        deleteExercsiseButton?.onClick.AddListener(RemoveLastExercise);
        finishButton?.onClick.AddListener(SaveData);
        LoadData();

        foreach (Transform child in exerciseContent) {   // Clear existing history to prevent duplicates
            Destroy(child.gameObject);
        }
    }
    public void AddExercise() {
    GameObject newExercise = Instantiate(exercisePrefab, contentPanel);
    exerciseNum++;

    string exerciseName = exerciseManager.exerciseSelectedName; // Get selected exercise name
    SetText(newExercise, "ExerciseNameText", exerciseName);

    ExerciseData exerciseData = new ExerciseData(exerciseNum, newExercise, exerciseName);
    exercises.Add(exerciseData);

    newExercise.transform.SetAsLastSibling(); // 🔹 Move new exercise to the bottom
    AdjustContentPanelSize();
}
    public void AddSet(ExerciseData parentExercise) {
        if (parentExercise != null) {
            GameObject newSet = Instantiate(setPrefab, contentPanel);
            int setNum = parentExercise.sets.Count + 1;

            SetText(newSet, "SetNumberText", $"Set {setNum}");
            SetText(newSet, "PreviousSetText", $"Previous: {previousSet}");

            TMP_InputField weightInput = FindInputField(newSet, "WeightInput");
            TMP_InputField repsInput = FindInputField(newSet, "RepsInput");

            if (weightInput && repsInput) {
                SetData setData = new SetData(setNum, previousSet);
                parentExercise.sets.Add(setData);

                weightInput.onValueChanged.AddListener((value) => {
                    setData.weight = value;
                    UpdatePreviousSet(setData, value, repsInput.text);
                });
                repsInput.onValueChanged.AddListener((value) => {
                    setData.reps = value;
                    UpdatePreviousSet(setData, weightInput.text, value);
                });
            }
            AdjustContentPanelSize();
        }
        else {
            Debug.LogWarning("Cannot add a set because no parent exercise exists.");
        }
    }
    public void RemoveLastExercise() {
        if (exercises.Count > 0) {
            ExerciseData lastExercise = exercises[^1];
            Destroy(lastExercise.exerciseObject);
            exercises.RemoveAt(exercises.Count - 1);
            AdjustContentPanelSize();
        }
    }
    private void UpdatePreviousSet(SetData setData, string weight, string reps) {
        if (!string.IsNullOrEmpty(weight) && !string.IsNullOrEmpty(reps)) {
            previousSet = $"{weight} x {reps}";
            setData.previousSet = previousSet;
        }
    }
    private void AdjustContentPanelSize() {
    LayoutRebuilder.ForceRebuildLayoutImmediate(contentPanel.GetComponent<RectTransform>());

    RectTransform contentRect = contentPanel.GetComponent<RectTransform>();

    // 🔹 Calculate new height based on exercises and sets
    float totalHeight = 0;
    foreach (var exercise in exercises) {
        totalHeight += exerciseSpacing; // Add exercise spacing
        totalHeight += exercise.sets.Count * setSpacing; // Add space for sets
    }
    contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight); // 🔹 Expand downwards only

    // 🔹 Ensure the content stays anchored at the top while expanding downward
    contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, 0);
}
    private void SaveData() {
    // 🔹 Save workout data
    string json = JsonUtility.ToJson(new ExerciseSaveData(exercises), true);
    File.WriteAllText(saveFilePath, json);
    Debug.Log("Workout data saved!");

    // 🔹 Update history UI
    if (historyManager != null) {
        historyManager.UpdateHistoryUI(exercises);
    } else {
        Debug.LogError("ExerciseHistoryManager is not assigned in ExercisePanelManager!");
    }

    // 🔹 Clear saved file so next session starts fresh
    File.Delete(saveFilePath);
    Debug.Log("Previous workout data deleted!");
    playerLifeStats.GainExperience(ExpRewards);

    // 🔹 Reset workout UI
    ResetWorkout();
}
    private void LoadData() {
    if (File.Exists(saveFilePath)) {
        string json = File.ReadAllText(saveFilePath);
        ExerciseSaveData loadedData = JsonUtility.FromJson<ExerciseSaveData>(json);

        // 🔹 Clear previous exercises and UI before loading new data
        foreach (Transform child in exerciseContent) {
            Destroy(child.gameObject);
        }
        exercises.Clear();  // Clear the list of stored exercises

        foreach (var loadedExercise in loadedData.exercises) {
            AddExercise();
            exercises[^1].exerciseName = loadedExercise.exerciseName; // 🔹 Restore name

            foreach (var loadedSet in loadedExercise.sets) {
                AddSet(exercises[^1]);
            }
        }
        AdjustContentPanelSize();
    } 
}
    private void ResetWorkout() {
    Debug.Log("Resetting workout session...");

    foreach (Transform child in exerciseContent) {  // Destroy all exercise objects in the UI
            Destroy(child.gameObject);
        }
    exercises.Clear(); 
    exerciseNum = 0; 
    AdjustContentPanelSize(); 
}
    private void SetText(GameObject obj, string childName, string text) {
        TMP_Text tmpText = obj.transform.Find(childName)?.GetComponent<TMP_Text>();
        if (tmpText) tmpText.text = text;
    }

    private TMP_InputField FindInputField(GameObject obj, string childName) {
        return obj.transform.Find(childName)?.GetComponent<TMP_InputField>();
    }
}
[System.Serializable] // Update ExerciseData Class
public class ExerciseData {
    public int exerciseNumber;
    public string exerciseName; // Store name explicitly
    public GameObject exerciseObject;
    public List<SetData> sets = new();

    public ExerciseData(int number, GameObject obj, string name) {
        exerciseNumber = number;
        exerciseObject = obj;
        exerciseName = name;
    }
}

[System.Serializable]
public class SetData {
    public int setNumber;
    public string previousSet, weight, reps;

    public SetData(int number, string prev) {
        setNumber = number;
        previousSet = prev;
        weight = "";
        reps = "";
    }
}

[System.Serializable]
public class ExerciseSaveData {
    public List<ExerciseData> exercises = new();
    public ExerciseSaveData(List<ExerciseData> data) => exercises = data;
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System;
using System.Collections;
using System.Linq;

public class ExercisePanelManager : MonoBehaviour {
    [Header("UI References")]
    public Transform contentPanel;
    public GameObject setPrefab, exercisePrefab, exerciseButtonPrefab;
    public Button addSetButton, removeSetButton, deleteExerciseButton, finishButton, addExerciseButton;
    public TMP_Text dayText, timerText;

    [Header("Dependencies")]
    public ExerciseManager exerciseManager;
    public ExerciseHistoryManager historyManager;
    public ExerciseReward expManager;
    public PlayerLifeStats playerLifeStats;
    public PanelManager panelManager;

    [Header("Settings")]
    public float setSpacing = 10f, exerciseSpacing = 10f;

    private List<ExerciseData> exercises = new();
    private string saveFilePath;
    public int exerciseNum = 0;
    private bool isTiming = false;
    private float elapsedTime = 0f;

    void Start() {

       
        saveFilePath = Path.Combine(Application.persistentDataPath, "exerciseData.json");

        addSetButton?.onClick.AddListener(() => AddSet(exercises.Count > 0 ? exercises[^1] : null));
        deleteExerciseButton?.onClick.AddListener(RemoveLastExercise);
        finishButton?.onClick.AddListener(SaveData);
        removeSetButton?.onClick.AddListener(RemoveLastSet);

        UpdateDayText();
        LoadData(); 
        ClearSpecificPanel(contentPanel);
    }

    private void UpdateDayText() {
        if (dayText) dayText.text = $"  {DateTime.Now.DayOfWeek}";
    }

    private void StartTimerIfNeeded() {
        if (!isTiming) StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine() {
        isTiming = true;
        while (isTiming) {
            elapsedTime += Time.deltaTime;
            UpdateTimerText();
            yield return null;
        }
    }

    private void UpdateTimerText() {
        if (timerText) {
            TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);
            timerText.text = $"  Time: {timeSpan:hh\\:mm\\:ss}";
        }
    }

    public void AddExercise() {
        string exerciseName = exerciseManager.exerciseSelectedName;
        if (string.IsNullOrEmpty(exerciseName)) return;

        GameObject newExercise = Instantiate(exercisePrefab, contentPanel);
        SetText(newExercise, "ExerciseNameText", exerciseName);

        ExerciseData exerciseData = new(exerciseNum++, newExercise, exerciseName);
        exercises.Add(exerciseData);

        newExercise.transform.SetAsLastSibling();
        AdjustContentPanelSize();
        StartTimerIfNeeded();
    }

    public void AddSet(ExerciseData parentExercise) {
    if (parentExercise == null) return;

    GameObject newSet = Instantiate(setPrefab, contentPanel);
    int setNum = parentExercise.sets.Count + 1;

    // Retrieve previous set data
    string previousSetData = GetPreviousSet(parentExercise.exerciseName);
    string[] splitData = previousSetData.Split('x');
    string previousWeight = splitData.Length > 0 ? splitData[0].Trim() : "0";
    string previousReps = splitData.Length > 1 ? splitData[1].Trim() : "0";

    SetText(newSet, "SetNumberText", $"Set {setNum}");
    SetText(newSet, "PreviousSetText", $"{previousWeight} x {previousReps}");

    TMP_InputField weightInput = FindInputField(newSet, "WeightInput");
    TMP_InputField repsInput = FindInputField(newSet, "RepsInput");

    // Store previous set data properly
    SetData setData = new(setNum, $"Previous: {previousWeight} x {previousReps}") {
        weight = previousWeight,
        reps = previousReps
    };
    parentExercise.sets.Add(setData);

    weightInput?.onValueChanged.AddListener(value => {
        UpdatePreviousSet(setData, value, setData.reps);
    });

    repsInput?.onValueChanged.AddListener(value => {
        UpdatePreviousSet(setData, setData.weight, value);
    });

    AdjustContentPanelSize();
}
    
    public void RemoveLastSet() {
    if (exercises.Count == 0) return; // No exercises available

    ExerciseData lastExercise = exercises[^1]; // Get the last exercise
    if (lastExercise.sets.Count == 0) return; // No sets to remove

    lastExercise.sets.RemoveAt(lastExercise.sets.Count - 1); // Remove last set

    // Destroy the last instantiated set GameObject
    Transform lastSetTransform = contentPanel.GetChild(contentPanel.childCount - 1);
    Destroy(lastSetTransform.gameObject);

    AdjustContentPanelSize(); // Update the UI layout
}

    public void RemoveLastExercise() {
        if (exercises.Count == 0) return;

        Destroy(exercises[^1].exerciseObject);
        exercises.RemoveAt(exercises.Count - 1);
        AdjustContentPanelSize();
    }

    private void UpdatePreviousSet(SetData setData, string weight, string reps) {
    if (!string.IsNullOrEmpty(weight) && !string.IsNullOrEmpty(reps)) {
        setData.weight = weight;
        setData.reps = reps;
        setData.previousSet = $"{weight} x {reps}";
    }
}

    private void AdjustContentPanelSize() {
        RectTransform contentRect = contentPanel.GetComponent<RectTransform>();
        float totalHeight = exercises.Count * exerciseSpacing + exercises.Sum(e => e.sets.Count * setSpacing);
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
        contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, 0);
    }

    private void SaveData() {
        List<ExerciseSaveData> allSessions = new();

        // Load existing history if available
        if (File.Exists(saveFilePath)) {
            string json = File.ReadAllText(saveFilePath);
            ExerciseHistory existingHistory = JsonUtility.FromJson<ExerciseHistory>(json);
            if (existingHistory != null) {
                allSessions = existingHistory.sessions;
            }
        }

        // Prepare current session data directly from the exercises list
        List<ExerciseData> savedExercises = exercises.Select(exercise => {
            List<SetData> savedSets = exercise.sets.Select(set => new SetData(set.setNumber, $"{set.weight} x {set.reps}") {
                weight = set.weight,
                reps = set.reps
            }).ToList();

            return new ExerciseData(exercise.exerciseNumber, null, exercise.exerciseName) {
                sets = savedSets
            };
        }).ToList();

        // Add to session history
        allSessions.Add(new ExerciseSaveData(savedExercises));

        // Save to file
        string updatedJson = JsonUtility.ToJson(new ExerciseHistory(allSessions), true);
        File.WriteAllText(saveFilePath, updatedJson);

        // Update UI & stats
        expManager?.UpdateRewardUI(exercises);
        historyManager?.UpdateHistoryUI(exercises);
        panelManager.OpenPanel(19);
        playerLifeStats.UpdateUI();

        isTiming = false;
        ResetWorkout();
    }

    private void LoadData() {
    if (!File.Exists(saveFilePath)) return; // No save file

    string json = File.ReadAllText(saveFilePath);
    ExerciseHistory history = JsonUtility.FromJson<ExerciseHistory>(json);
    
    if (history == null || history.sessions.Count == 0) return; // No sessions to load

    ExerciseSaveData lastSession = history.sessions[^1]; // Get the most recent session

    foreach (var savedExercise in lastSession.exercises) {
        GameObject newExercise = Instantiate(exercisePrefab, contentPanel);
        SetText(newExercise, "ExerciseNameText", savedExercise.exerciseName);
        
        ExerciseData exerciseData = new(savedExercise.exerciseNumber, newExercise, savedExercise.exerciseName);
        exercises.Add(exerciseData);

        foreach (var savedSet in savedExercise.sets) {
            GameObject newSet = Instantiate(setPrefab, contentPanel);
            SetText(newSet, "SetNumberText", $"Set {savedSet.setNumber}");
            SetText(newSet, "PreviousSetText", $"{savedSet.weight} x {savedSet.reps}");

            TMP_InputField weightInput = FindInputField(newSet, "WeightInput");
            TMP_InputField repsInput = FindInputField(newSet, "RepsInput");

            if (weightInput != null) weightInput.text = savedSet.weight;
            if (repsInput != null) repsInput.text = savedSet.reps;


            exerciseData.sets.Add(new SetData(savedSet.setNumber, $"{savedSet.weight} x {savedSet.reps}") {
                weight = savedSet.weight,
                reps = savedSet.reps
            });
        }

        newExercise.transform.SetAsLastSibling();
    }

    AdjustContentPanelSize();
}

   private string GetPreviousSet(string exerciseName) {
    if (!File.Exists(saveFilePath)) return "No set saved"; 

    string json = File.ReadAllText(saveFilePath);
    ExerciseHistory history = JsonUtility.FromJson<ExerciseHistory>(json);
    if (history == null || history.sessions.Count == 0) return "No history";

    for (int i = history.sessions.Count - 1; i >= 0; i--) { // Iterate backwards for most recent session
        foreach (var exercise in history.sessions[i].exercises) {
            if (exercise.exerciseName == exerciseName && exercise.sets.Count > 0) {
                SetData lastSet = exercise.sets[^1];

                // Ensure weight and reps aren't blank
                string weight = string.IsNullOrEmpty(lastSet.weight) ? "0" : lastSet.weight;
                string reps = string.IsNullOrEmpty(lastSet.reps) ? "0" : lastSet.reps;

                return $"{weight} x {reps}";
            }
        }
    }
    return "   -   "; 
}

    private void ResetWorkout() {
        ClearSpecificPanel(contentPanel);
        exercises.Clear();
        exerciseNum = 0;
        AdjustContentPanelSize();
    }

    private void ClearSpecificPanel(Transform targetPanel) {
    if (targetPanel == null) return;

    foreach (Transform child in targetPanel) {
        Destroy(child.gameObject);
    }
    }

    private void SetText(GameObject obj, string childName, string text) {
        TMP_Text tmpText = obj.transform.Find(childName)?.GetComponent<TMP_Text>();
        if (tmpText) tmpText.text = text;
    }

    private TMP_InputField FindInputField(GameObject obj, string childName) {
        return obj.transform.Find(childName)?.GetComponent<TMP_InputField>();
    }
}
[System.Serializable]
public class ExerciseHistory {
    public List<ExerciseSaveData> sessions = new();

    public ExerciseHistory(List<ExerciseSaveData> data) => sessions = data;
}

[System.Serializable]
public class ExerciseData {
    public int exerciseNumber;
    public string exerciseName;
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
    public string previousSet, weight = "", reps = "";



    public SetData(int number, string prev) {
        setNumber = number;
        previousSet = prev;
    }
}

[System.Serializable]
public class ExerciseSaveData {
    public List<ExerciseData> exercises = new();
    public ExerciseSaveData(List<ExerciseData> data) => exercises = data;
}
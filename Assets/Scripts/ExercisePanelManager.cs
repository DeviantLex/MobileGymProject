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
    public Transform contentPanel, exerciseContent;
    public GameObject setPrefab, exercisePrefab, exerciseButtonPrefab;
    public Button addSetButton, deleteExerciseButton, finishButton, addExerciseButton;
    public TMP_Text dayText, timerText;

    [Header("Dependencies")]
    public ExerciseManager exerciseManager;
    public ExerciseHistoryManager historyManager;
    public PlayerLifeStats playerLifeStats;

    [Header("Settings")]
    public float setSpacing = 10f, exerciseSpacing = 10f;

    private List<ExerciseData> exercises = new();
    private string saveFilePath;
    private int exerciseNum = 0;
    private string previousSet = "100 x 15";
    private bool isTiming = false;
    private float elapsedTime = 0f;

    void Start() {
        saveFilePath = Path.Combine(Application.persistentDataPath, "exerciseData.json");

        addSetButton?.onClick.AddListener(() => AddSet(exercises.Count > 0 ? exercises[^1] : null));
        deleteExerciseButton?.onClick.AddListener(RemoveLastExercise);
        finishButton?.onClick.AddListener(SaveData);
        addExerciseButton?.onClick.AddListener(StartTimerIfNeeded);

        LoadData();
        UpdateDayText();
        ClearExerciseContent();
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
    }

    public void AddSet(ExerciseData parentExercise) {
        if (parentExercise == null) return;

        GameObject newSet = Instantiate(setPrefab, contentPanel);
        int setNum = parentExercise.sets.Count + 1;

        SetText(newSet, "SetNumberText", $"Set {setNum}");
        SetText(newSet, "PreviousSetText", $"Previous: {previousSet}");

        TMP_InputField weightInput = FindInputField(newSet, "WeightInput");
        TMP_InputField repsInput = FindInputField(newSet, "RepsInput");

        SetData setData = new(setNum, previousSet);
        parentExercise.sets.Add(setData);

        weightInput?.onValueChanged.AddListener(value => UpdatePreviousSet(setData, value, repsInput?.text));
        repsInput?.onValueChanged.AddListener(value => UpdatePreviousSet(setData, weightInput?.text, value));

        AdjustContentPanelSize();
    }

    public void RemoveLastExercise() {
        if (exercises.Count == 0) return;

        Destroy(exercises[^1].exerciseObject);
        exercises.RemoveAt(exercises.Count - 1);
        AdjustContentPanelSize();
    }

    private void UpdatePreviousSet(SetData setData, string weight, string reps) {
        if (!string.IsNullOrEmpty(weight) && !string.IsNullOrEmpty(reps)) {
            previousSet = $"{weight} x {reps}";
            setData.previousSet = previousSet;
        }
    }

    private void AdjustContentPanelSize() {
        RectTransform contentRect = contentPanel.GetComponent<RectTransform>();
        float totalHeight = exercises.Count * exerciseSpacing + exercises.Sum(e => e.sets.Count * setSpacing);
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
        contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, 0);
    }

    private void SaveData() {
        string json = JsonUtility.ToJson(new ExerciseSaveData(exercises), true);
        File.WriteAllText(saveFilePath, json);
        historyManager?.UpdateHistoryUI(exercises);

        int totalReward = exercises.Sum(e => {
        var selection = exerciseManager.GetExerciseSelection(e.exerciseName);
        return int.TryParse(selection?.exerciseReward, out int exp) ? exp : 0;
    });
        Debug.Log("Reward is " + totalReward + " xp.");
        
        playerLifeStats.currentLevelExp += totalReward;
        playerLifeStats.UpdateUI();
        isTiming = false;
        File.Delete(saveFilePath);
        ResetWorkout();
    }

    private void LoadData() {
        if (!File.Exists(saveFilePath)) return;

        string json = File.ReadAllText(saveFilePath);
        ExerciseSaveData loadedData = JsonUtility.FromJson<ExerciseSaveData>(json);
        
        ClearExerciseContent();
        exercises.Clear();

        foreach (var loadedExercise in loadedData.exercises) {
            AddExercise();
            exercises[^1].exerciseName = loadedExercise.exerciseName;
            foreach (var _ in loadedExercise.sets) AddSet(exercises[^1]);
        }
        AdjustContentPanelSize();
    }

    private void ResetWorkout() {
        ClearExerciseContent();
        exercises.Clear();
        exerciseNum = 0;
        AdjustContentPanelSize();
    }

    private void ClearExerciseContent() {
        foreach (Transform child in exerciseContent) Destroy(child.gameObject);
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
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ExerciseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelPrefab;
    public Transform panelParent;
    public GameObject descriptionPanel;
    public TextMeshProUGUI descriptionText;
    public Image descriptionImage;

    [Header("Dependencies")]
    public PanelManager panelManager;
    public ExercisePanelManager exercisePanelManager;

    [Header("Data")]
    public ExerciseSelection[] exerciseSelection;
    public List<ExerciseSelection> availableExercises = new();
    
    public string exerciseSelectedName;

    void Start() {
        if (!panelPrefab || !panelParent || !descriptionPanel || !descriptionText) {
            Debug.LogError("Missing UI references! Assign them in the Inspector.");
            return;
        }
        GenerateExerciseButtons();
    }

    void GenerateExerciseButtons() {
        if (exerciseSelection == null || exerciseSelection.Length == 0) {
            Debug.LogWarning("Exercise selection is empty!");
            return;
        }

        foreach (var exercise in exerciseSelection) {
            if (exercise == null) continue;
            GameObject newButton = Instantiate(panelPrefab, panelParent);
            SetupExerciseButton(newButton, exercise);
        }
    }

    void SetupExerciseButton(GameObject buttonObj, ExerciseSelection exercise) {
        // Set image & text
        SetUIElement(buttonObj, "ExerciseImage", exercise.exerciseImage);
        SetUIElement(buttonObj, "ExerciseName", exercise.exerciseName);
        SetUIElement(buttonObj, "ExerciseReward", exercise.exerciseReward);

        // Info button - shows description
        Button infoButton = GetButton(buttonObj, "InfoButton");
        infoButton?.onClick.AddListener(() => ShowDescription(exercise));

        // Add Exercise button
        Button addButton = GetButton(buttonObj, "AddExerciseButton");
        addButton?.onClick.AddListener(() => AddExercise(exercise));
    }

    void ShowDescription(ExerciseSelection exercise) {
        descriptionText.text = exercise.exerciseDescription;
        descriptionImage.sprite = exercise.exerciseImage;
        panelManager.OpenPanel(3); // Assuming 2 is the description panel index
        panelManager.ClosePanel(1); // Assuming 0 is the main exercise selection panel
    }

    void AddExercise(ExerciseSelection exercise) {
        exerciseSelectedName = exercise.exerciseName;
        if (exercisePanelManager) {
            exercisePanelManager.AddExercise();
        }
        panelManager.OpenPanel(2); // Assuming 1 is the exercise detail panel
        panelManager.ClosePanel(1);
    }

    public ExerciseSelection GetExerciseSelection(string name) {
        return availableExercises.Find(e => e.exerciseName == name);
    }

    // Utility Methods
    void SetUIElement(GameObject obj, string childName, string text) {
        TextMeshProUGUI tmpText = obj.transform.Find(childName)?.GetComponent<TextMeshProUGUI>();
        if (tmpText) tmpText.text = text;
    }

    void SetUIElement(GameObject obj, string childName, Sprite sprite) {
        Image image = obj.transform.Find(childName)?.GetComponent<Image>();
        if (image) image.sprite = sprite;
    }

    Button GetButton(GameObject obj, string childName) {
        return obj.transform.Find(childName)?.GetComponent<Button>();
    }
}
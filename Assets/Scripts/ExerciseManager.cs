using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExerciseManager : MonoBehaviour
{
    public GameObject panelPrefab; // Prefab for the exercise button
    public Transform panelParent; // Parent transform for buttons
    public ExerciseSelection[] exerciseSelection; // Array of exercise dataß
    public GameObject descriptionPanel; // Reference to the separate description panel
    public PanelManager panelManager; // Reference to PanelManager
    public TextMeshProUGUI descriptionText; // Text component in the description panel
    public Image descriptionImage;
    private Button exerciseButton;
    public ExercisePanelManager exercisePanelManager;
    public string exerciseSelectedName;
    public int exercisePanelIndex;
    public int descriptionPanelIndex; // Index of the description panel in PanelManager
    
    void Start() {
        if (!panelPrefab || !panelParent || !descriptionPanel || !descriptionText) {
            Debug.LogError("Missing exercise references! Assign them in the Inspector.");
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
            SetButtonContent(newButton, exercise);
        }
    }
    public void SetButtonContent(GameObject buttonObj, ExerciseSelection exercise) {
    // Set button image
    Image exerciseImage = buttonObj.transform.Find("ExerciseImage")?.GetComponent<Image>();
    if (exerciseImage != null) exerciseImage.sprite = exercise.exerciseImage;

    // Set button text
    TextMeshProUGUI exerciseNameText = buttonObj.transform.Find("ExerciseName")?.GetComponent<TextMeshProUGUI>();
    if (exerciseNameText != null) exerciseNameText.text = exercise.exerciseName;

    TextMeshProUGUI exerciseRewardText = buttonObj.transform.Find("ExerciseReward")?.GetComponent<TextMeshProUGUI>();
    if (exerciseRewardText != null) exerciseRewardText.text = exercise.exerciseReward;

    Button infoButton = buttonObj.transform.Find("InfoButton")?.GetComponent<Button>(); // Displays exercise info 
    if (infoButton != null) {
        infoButton.onClick.AddListener(() => ShowDescription(exercise.exerciseDescription, exercise.exerciseImage));
    }
    exerciseButton = buttonObj.transform.Find("AddExerciseButton")?.GetComponent<Button>(); // Modify ExerciseButton to both set name and add exercise
    if (exerciseButton != null) {
        exerciseButton.onClick.AddListener(() => {
            if (!string.IsNullOrEmpty(exercise.exerciseName)) {
                SetExerciseName(exercise.exerciseName);
            } 
            if (exercisePanelManager != null) {
                exercisePanelManager.AddExercise();
                panelManager.OpenPanel(exercisePanelIndex);
            }          
        });
    }
}
    void ShowDescription(string description, Sprite exerciseSprite) {
        descriptionText.text = description;
        descriptionImage.sprite = exerciseSprite; //Update the image
        panelManager.OpenPanel(descriptionPanelIndex);
    }
    public void SetExerciseName(string exerciseName) {
    exerciseSelectedName = exerciseName; // Store the selected name
    TextMeshProUGUI selectedExerciseNameText = exercisePanelManager.exercisePrefab.transform.Find("ExercisePrefab")?.GetComponent<TextMeshProUGUI>();

    if (selectedExerciseNameText != null) {
        selectedExerciseNameText.text = exerciseSelectedName;
        Debug.Log("Current Exercise Set To: " + exerciseSelectedName);
    } else {
        //Debug.LogWarning("Exercise button text not found!");
    }
}
}
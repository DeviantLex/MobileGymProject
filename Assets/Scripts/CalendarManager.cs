using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GymAttendanceGraph : MonoBehaviour
{
    public GameObject barPrefab; // Assign a Panel prefab in the inspector
    public Transform barContainer; // Parent container with HorizontalLayoutGroup
    public List<int> gymAttendance; // List of visits per week (last 5 weeks)
    public int maxVisits = 7; // Max visits per week for scaling
    void Start() {
        GenerateGraph();
    }
    void GenerateGraph() {
        foreach (Transform child in barContainer) {// Clear old bars before generating new ones
            Destroy(child.gameObject);
        }      
        foreach (int visits in gymAttendance) { // Generate bars based on gym attendance data
            GameObject newBar = Instantiate(barPrefab, barContainer);
            RectTransform barRect = newBar.GetComponent<RectTransform>();
        
            TextMeshProUGUI VisitNum = newBar.transform.Find("VisitText").GetComponent<TextMeshProUGUI>(); 
            VisitNum.text = $"{visits}";   

            float normalizedHeight = Mathf.Clamp01((float)visits / maxVisits); // Scale height based on visits (normalized by maxVisits)
            barRect.sizeDelta = new Vector2(barRect.sizeDelta.x, normalizedHeight * 800); // Adjust 200 to desired max height
        }
    }
    public void UpdateGraph(List<int> newAttendance) {  // Call this method when attendance data updates
        gymAttendance = newAttendance;
        GenerateGraph();
    }
}
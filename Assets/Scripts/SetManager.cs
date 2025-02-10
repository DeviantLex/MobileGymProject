using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SetManager : MonoBehaviour
{
    public GameObject setPrefab; // Prefab for a single set
    public Transform contentPanel; // Reference to the content panel in the Scroll View
    public Button addSetButton; // Button to add a new set
    public float setSpacing = 10f; // Spacing between sets
    public TextMeshProUGUI setText; // Text for global set count (optional)

    private List<GameObject> sets = new List<GameObject>(); // List to hold all sets
    private int setNum = 0; // Tracks the total number of sets added

    void Start()
    {
        // Ensure the Add Set button has a listener
        if (addSetButton != null)
        {
            addSetButton.onClick.AddListener(AddSet);
        }
    }

    void Update()
    {
        // Optionally display the total number of sets globally
        if (setText != null)
        {
            setText.text = "Total Sets: " + setNum;
        }
    }

    public void AddSet()
    {
        if (setPrefab != null && contentPanel != null)
        {
            // Instantiate a new set and parent it to the content panel
            GameObject newSet = Instantiate(setPrefab, contentPanel);

            // Increment and assign the set number
            setNum++;
            TMP_Text setNumberText = newSet.GetComponentInChildren<TMP_Text>();
            if (setNumberText != null)
            {
                setNumberText.text = "Set #" + setNum;
            }

            // Add the new set to the list
            sets.Add(newSet);

            // Adjust the content panel size
            AdjustContentPanelSize();
        }
    }

    private void AdjustContentPanelSize()
    {
        // Calculate the new height for the content panel
        float totalHeight = sets.Count * (setPrefab.GetComponent<RectTransform>().sizeDelta.y + setSpacing);

        // Adjust the content panel's height
        RectTransform contentRect = contentPanel.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);

        Debug.Log($"Content panel resized to {contentRect.sizeDelta.y}");
    }
}
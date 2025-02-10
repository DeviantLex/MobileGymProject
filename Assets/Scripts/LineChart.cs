using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LineChart : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public RectTransform chartContainer;
    public TextMeshProUGUI xAxisLabelPrefab, yAxisLabelPrefab;
    public Transform xAxisParent, yAxisParent;

    private List<Vector2> dataPoints = new List<Vector2>();
    private int maxDays = 7;  // 🔹 Number of days
    private float maxWeight = 200f; // 🔹 Max weight for scaling

    void Start()
    {
        // 🔹 Ensure the LineRenderer is visible
        lineRenderer.startWidth = 5f;
        lineRenderer.endWidth = 5f;
        lineRenderer.useWorldSpace = false; 

        GenerateRandomTestData();
    }

    // 🔹 Generate random weight data for testing
    void GenerateRandomTestData()
    {
        dataPoints.Clear();
        for (int i = 0; i < maxDays; i++)
        {
            float randomWeight = Random.Range(100f, maxWeight);
            AddDataPoint(randomWeight, i);
        }
    }

    // 🔹 Add a new weight entry (e.g., daily progress)
    public void AddDataPoint(float weight, int day)
    {
        dataPoints.Add(new Vector2(day, weight));
        UpdateChart();
    }

    // 🔹 Update and fix chart alignment issues
    void UpdateChart()
    {
        if (dataPoints.Count < 2) return; // Need at least two points to draw a line

        lineRenderer.positionCount = dataPoints.Count;

        float chartWidth = chartContainer.rect.width;
        float chartHeight = chartContainer.rect.height;
        float xStep = chartWidth / (maxDays - 1);  // 🔹 Space out points correctly
        float yMax = Mathf.Max(maxWeight, 1);  // 🔹 Prevent division by zero

        for (int i = 0; i < dataPoints.Count; i++)
        {
            float normalizedX = i * xStep;  // 🔹 Ensures correct spacing on X-axis
            float normalizedY = (dataPoints[i].y / yMax) * chartHeight;

            Vector3 localPosition = new Vector3(normalizedX - (chartWidth / 2), normalizedY - (chartHeight / 2), 0);
            lineRenderer.SetPosition(i, localPosition); // 🔹 FIXED!
        }

        UpdateLabels();
    }

    // 🔹 Generates X (days) & Y (weight) labels
    void UpdateLabels()
    {
        foreach (Transform child in xAxisParent) Destroy(child.gameObject);
        foreach (Transform child in yAxisParent) Destroy(child.gameObject);

        for (int i = 0; i < maxDays; i++)
        {
            TextMeshProUGUI xLabel = Instantiate(xAxisLabelPrefab, xAxisParent);
            xLabel.text = $"Day {i + 1}";
            xLabel.rectTransform.anchoredPosition = new Vector2(i * (chartContainer.rect.width / (maxDays - 1)) - (chartContainer.rect.width / 2), -20);
        }

        for (int i = 0; i <= 5; i++)
        {
            float weightStep = (maxWeight / 5) * i;
            TextMeshProUGUI yLabel = Instantiate(yAxisLabelPrefab, yAxisParent);
            yLabel.text = $"{weightStep:F0} lbs";
            yLabel.rectTransform.anchoredPosition = new Vector2(-40, (i / 5f) * chartContainer.rect.height - (chartContainer.rect.height / 2));
        }
    }
}
using UnityEngine;
using System.Collections.Generic;

public class PanelManager : MonoBehaviour
{
    [System.Serializable]
    public class Panel
    {
        public GameObject panelObject;
        public GameObject[] enableUI;
        public GameObject[] disableUI;
    }

    public Panel[] panels;
    private Dictionary<int, bool> panelStates = new Dictionary<int, bool>(); // Cache panel states

    void Start()
    {
      
    Application.targetFrameRate = 60; // Force 60 FPS
    QualitySettings.vSyncCount = 0;   // Disable VSync to avoid conflicts

        // Initialize panel states
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i].panelObject)
                panelStates[i] = panels[i].panelObject.activeSelf;
        }
    }

    public void OpenPanel(int index)
    {
        if (!IsValidIndex(index) || panelStates[index]) return; // Skip if already open

        panelStates[index] = true;
        panels[index].panelObject.SetActive(true);
        SetUIState(panels[index].enableUI, true);
        SetUIState(panels[index].disableUI, false);
    }

    public void ClosePanel(int index)
    {
        if (!IsValidIndex(index) || !panelStates[index]) return; // Skip if already closed

        panelStates[index] = false;
        panels[index].panelObject.SetActive(false);
    }

    public void TogglePanel(int index)
    {
        if (!IsValidIndex(index)) return;

        panelStates[index] = !panelStates[index];
        panels[index].panelObject.SetActive(panelStates[index]);
    }

    private bool IsValidIndex(int index) => index >= 0 && index < panels.Length && panels[index].panelObject;

    private void SetUIState(GameObject[] elements, bool state)
    {
        foreach (var element in elements)
        {
            if (element && element.activeSelf != state) // Only change if necessary
                element.SetActive(state);
        }
    }
}
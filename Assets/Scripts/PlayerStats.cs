using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public PlayerLifeStats playerLifeStats;
    public TextMeshProUGUI playerEx;
    public TextMeshProUGUI playerCoins;

    void Start() {
        if (playerEx || playerCoins != null) {
            UpdatePlayerStatsDisplay();
        }
        else {
            Debug.LogWarning("UI Text reference is missing!");
        }
    }
    void UpdatePlayerStatsDisplay() {
        playerEx.text = $"{playerLifeStats.currentPlayerLevel}";     
        playerCoins.text = $"{playerLifeStats.currentCoins}";
    }

    public void OnStatsChanged() {  // Call when stats change in PlayerLifeStats
        UpdatePlayerStatsDisplay();
    }
}
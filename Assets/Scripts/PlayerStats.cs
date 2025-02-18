using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public PlayerLifeStats playerLifeStats;
    public TextMeshProUGUI playerEx;
    public TextMeshProUGUI playerCoins;

    void Start() {
        if (playerEx != null && playerCoins != null) {
            Update();
        }
    }
    void Update() {
        if (playerLifeStats != null) {  // Ensure playerLifeStats is assigned
            playerEx.text = $"{playerLifeStats.currentPlayerLevel}";     
            playerCoins.text = $"{playerLifeStats.currentCoins}";
        }
    }
    public void OnStatsChanged() {  // Call when stats change in PlayerLifeStats
        Update();
    }
}
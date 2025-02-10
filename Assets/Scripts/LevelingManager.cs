using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelingManager : MonoBehaviour
{
    public PlayerLifeStats playerLifeStats;
    public Button HealthUpButton, ManaUpButton, SpeedUpButton, StrengthUpButton, LevelUpButton;
    public int healthIncrease, manaIncrease, speedIncrease, strengthIncrease, lastSkillPoints; //Track previous skill points to detect changes
    public TextMeshProUGUI healthUpText, ManaUpText, SpeedUpText, StrengthUpText;
    void Start() {
        HealthUpButton.onClick.AddListener(HealthPowerUp);
        ManaUpButton.onClick.AddListener(ManaPowerUp);
        SpeedUpButton.onClick.AddListener(SpeedPowerUp);
        StrengthUpButton.onClick.AddListener(StrenthPowerUp);

        lastSkillPoints = playerLifeStats.skillPoints;
        UpdateUI();
    }
    void Update() {
        if (playerLifeStats.skillPoints != lastSkillPoints) { // Only update the UI when skill points change
            lastSkillPoints = playerLifeStats.skillPoints;
            UpdateUI();
        }
    }
    void UpdateUI() { 
        if (playerLifeStats == null) return;
        
        //Enables the Level Up button
        LevelUpButton.gameObject.SetActive(playerLifeStats.skillPoints > 0);
       
        //Updates text for the button 
        healthUpText.text = "Health Up + " + healthIncrease + " Current Health: " + playerLifeStats.maxPlayerHealth;
        ManaUpText.text = "Mana Up + " + manaIncrease + " Current Health: " + playerLifeStats.maxPlayerMana; 
        SpeedUpText.text = "Mana Up + " + speedIncrease + " Current Health: " + playerLifeStats.currentPlayerSpeed; 
        StrengthUpText.text = "Mana Up + " + strengthIncrease + " Current Health: " + playerLifeStats.currentPlayerStrength; 
    }

    void HealthPowerUp() {
        if (SpendSkillPoint()) {
            playerLifeStats.maxPlayerHealth += healthIncrease;
            Debug.Log($"Player's health increased by {healthIncrease}. Current max health: {playerLifeStats.maxPlayerHealth}");
            UpdateUI();

        }
    }
    void ManaPowerUp() {
        if (SpendSkillPoint()) {
            playerLifeStats.maxPlayerMana += manaIncrease;
            Debug.Log($"Player's mana increased by {manaIncrease}. Current max mana: {playerLifeStats.maxPlayerMana}");
            UpdateUI();

        }
    }
    void SpeedPowerUp() {
        if (SpendSkillPoint()) {
            playerLifeStats.currentPlayerSpeed += speedIncrease;
            Debug.Log($"Player's speed increased by {speedIncrease}. Current speed: {playerLifeStats.currentPlayerSpeed}");
            UpdateUI();

        }
    }
    void StrenthPowerUp() {
        if (SpendSkillPoint()) {
            playerLifeStats.currentPlayerStrength += strengthIncrease;
            Debug.Log($"Player's strength increased by {strengthIncrease}. Current strength: {playerLifeStats.currentPlayerStrength}");
            UpdateUI();
        }
    }
    bool SpendSkillPoint() {
        if (playerLifeStats.skillPoints > 0) {
            playerLifeStats.skillPoints--;
            UpdateUI();
            return true;
        }
        return false;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

[System.Serializable]
public class PlayerData
{
    public int currentPlayerHealth, currentPlayerMana, currentCoins, currentLevelExp, currentPlayerLevel, currentPlayerStrength, currentPlayerSpeed;
}
public class PlayerLifeStats : MonoBehaviour
{
    public int maxPlayerHealth = 100, maxPlayerMana = 100, maxLevelExp = 100, maxHealthPotions = 10, playerAttackPoints = 10;  
    private bool isDefending = false;

    public int currentPlayerHealth, currentPlayerMana, currentPlayerStrength, currentPlayerSpeed, healthPotionCount, currentCoins = 0;
    public int currentLevelExp = 0;
    public int currentPlayerLevel = 0;
    public int skillPoints = 0;
    
    public Slider healthBar, manaBar, expBar;

    private string saveFilePath;

    void Start() {
        saveFilePath = Application.persistentDataPath + "/playerdata.json";
        LoadPlayerData(); // Load data when the game starts

        // Initialize UI elements
        healthBar.maxValue = maxPlayerHealth;
        manaBar.maxValue = maxPlayerMana;
        expBar.maxValue = maxLevelExp;
        currentCoins = 10000;
        currentPlayerLevel = 1;
        UpdateUI();
    }

    void Update() {
        if(currentLevelExp >= maxLevelExp) {
            currentLevelExp = 0; // Reset XP
            currentPlayerLevel++;
            skillPoints++;
            OnStatsChanged();
        }
    }  

    IEnumerator SmoothBarTransition(Slider bar, float targetValue, float duration = 0.5f)
    {
    float startValue = bar.value;
    float time = 0f;

    while (time < duration)
    {
        time += Time.deltaTime;
        bar.value = Mathf.Lerp(startValue, targetValue, time / duration);
        yield return null;
    }

    bar.value = targetValue; // Ensure it reaches exactly the target value
    }

    public void TakeDamage(int damage) {
        if (isDefending) {
            damage /= 2;
        }

        currentPlayerHealth = Mathf.Clamp(currentPlayerHealth - damage, 0, maxPlayerHealth);
        Debug.Log($"Player took {damage} damage. Current health: {currentPlayerHealth}");
        UpdateUI();  
    }

    public void PlayerAttack(EnemyController enemy) {
        enemy.TakeDamage(playerAttackPoints);
    }

    public void Defend() {
        isDefending = true;
    }

    public void Heal() {
        currentPlayerHealth = Mathf.Clamp(currentPlayerHealth + 20, 0, maxPlayerHealth);
        UpdateUI();
        //SavePlayerData();
    }

    public void Run() {
        currentPlayerHealth -= 10;
        UpdateUI();
        SavePlayerData();
    }

    public void GainExperience(int exp) {
        currentLevelExp = Mathf.Clamp(currentLevelExp + exp, 0, maxLevelExp);
        Debug.Log($"Gained {exp} XP. Total XP: {currentLevelExp}");
        UpdateUI();
        SavePlayerData();
    }

    void UpdateUI() {
    StartCoroutine(SmoothBarTransition(healthBar, currentPlayerHealth));
    StartCoroutine(SmoothBarTransition(manaBar, currentPlayerMana));
    StartCoroutine(SmoothBarTransition(expBar, currentLevelExp));
    }

    public void OnStatsChanged() {
        // Assuming PlayerStats is attached to the same GameObject
        var playerStats = GetComponent<PlayerStats>();
        if (playerStats != null) {
            playerStats.OnStatsChanged();
        }
    }
    public void SavePlayerData() {
        PlayerData data = new PlayerData {
            currentPlayerHealth = currentPlayerHealth,
            currentPlayerMana = currentPlayerMana,
            currentCoins = currentCoins,
            currentLevelExp = currentLevelExp,
            currentPlayerLevel = currentPlayerLevel
        };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved!");
    }

    public void LoadPlayerData() {
        if (File.Exists(saveFilePath)) {
            string json = File.ReadAllText(saveFilePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            //All Values here levelExp, playerLevel, Strength, and Speed need to be updated

            //currentPlayerHealth = data.currentPlayerHealth;
            currentPlayerHealth =maxPlayerHealth;
            currentPlayerMana = data.currentPlayerMana;
            currentCoins = data.currentCoins;
            currentPlayerLevel = 1;
            currentLevelExp = 0;
            //currentLevelExp = data.currentLevelExp;
            //currentPlayerLevel = data.currentPlayerLevel;
            currentPlayerStrength = 0;
            currentPlayerSpeed = 0;
            //currentPlayerStrength = data.currentPlayerStrength;
            //currentPlayerSpeed = data.currentPlayerSpeed;

            Debug.Log("Game Loaded!");
        }
        else {
            Debug.Log("No save file found. Starting new game.");
            currentPlayerHealth = maxPlayerHealth;
            currentPlayerMana = maxPlayerMana;
            currentCoins = 10000;
            currentLevelExp = 0;
            currentPlayerLevel = 1;
            currentPlayerStrength = 0;
            currentPlayerSpeed = 0;
        }
        UpdateUI();
    }
      public bool OnPlayerDefeat() {
        return currentPlayerHealth <= 0;
    }
}
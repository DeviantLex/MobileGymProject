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
    public int currentLevelExp = 0, currentPlayerLevel = 1, skillPoints = 0;

    public Slider healthBar, manaBar, expBar;
    private string saveFilePath;

    void Awake() {
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerdata.json");
        LoadPlayerData();
    }
    void Start() {
        healthBar.maxValue = maxPlayerHealth;
        manaBar.maxValue = maxPlayerMana;
        expBar.maxValue = maxLevelExp;
        UpdateUI();
    }
    void Update() {
        if (currentLevelExp >= maxLevelExp) {
            currentLevelExp = 0;
            currentPlayerLevel++;
            skillPoints++;
            OnStatsChanged();
        }
    }
    IEnumerator SmoothBarTransition(Slider bar, float targetValue, float duration = 0.5f) {
        float startValue = bar.value, time = 0f;
        while (time < duration) {
            time += Time.deltaTime;
            bar.value = Mathf.Lerp(startValue, targetValue, time / duration);
            yield return null;
        }
        bar.value = targetValue;
    }

    public void TakeDamage(int damage) {
        if (isDefending) damage /= 2;
        currentPlayerHealth = Mathf.Clamp(currentPlayerHealth - damage, 0, maxPlayerHealth);
        Debug.Log($"Player took {damage} damage. Current health: {currentPlayerHealth}");
        UpdateUI();
    }

    public void PlayerAttack(EnemyController enemy) => enemy.TakeDamage(playerAttackPoints);
    public void Defend() => isDefending = true;
    public void Heal() => ModifyHealth(40);
    public void Run() => ModifyHealth(-10);
    public void GainCoins(int coinAmount) => ModifyExp(coinAmount);

    void ModifyHealth(int amount) {
        currentPlayerHealth = Mathf.Clamp(currentPlayerHealth + amount, 0, maxPlayerHealth);
        UpdateUI();
    }
     public void ModifyCoins(int amount)
    {
        currentCoins += amount;
        OnStatsChanged();  // Update the UI whenever coins change
    }

    void ModifyExp(int amount) {
        currentLevelExp = Mathf.Clamp(currentLevelExp + amount, 0, maxLevelExp);
        UpdateUI();
        SavePlayerData();
    }
    public void UpdateUI() {
        StartCoroutine(SmoothBarTransition(healthBar, currentPlayerHealth));
        StartCoroutine(SmoothBarTransition(manaBar, currentPlayerMana));
        StartCoroutine(SmoothBarTransition(expBar, currentLevelExp));
        OnStatsChanged();
    }
    public void OnStatsChanged() {
        var playerStats = GetComponent<PlayerStats>();
        playerStats?.OnStatsChanged();
    }
    public void SavePlayerData() {
        PlayerData data = new PlayerData {
            currentPlayerHealth = currentPlayerHealth,
            currentPlayerMana = currentPlayerMana,
            currentCoins = currentCoins,
            currentLevelExp = currentLevelExp,
            currentPlayerLevel = currentPlayerLevel,
            currentPlayerStrength = currentPlayerStrength,
            currentPlayerSpeed = currentPlayerSpeed
        };
        File.WriteAllText(saveFilePath, JsonUtility.ToJson(data, true));
        Debug.Log("Game Saved!");
    }
    public void LoadPlayerData() {
        if (File.Exists(saveFilePath)) {
            PlayerData data = JsonUtility.FromJson<PlayerData>(File.ReadAllText(saveFilePath));
            currentPlayerHealth = maxPlayerHealth;
            currentPlayerMana = data.currentPlayerMana;
            currentCoins = data.currentCoins;
            currentPlayerLevel = 1;
            currentLevelExp = 0;
            currentPlayerStrength = 0;
            currentPlayerSpeed = 0;
            Debug.Log("Game Loaded!");
        } else {
            Debug.Log("No save file found. Starting new game.");
            currentPlayerHealth = maxPlayerHealth;
            currentPlayerMana = maxPlayerMana;
            currentCoins = 100;
            currentLevelExp = 1;
            currentPlayerLevel = 1;
            currentPlayerStrength = 0;
            currentPlayerSpeed = 0;
        }
        UpdateUI();
    }
    public void DeleteSaveFile() {
        if (File.Exists(saveFilePath)) {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted.");
        } else {
            Debug.Log("No save file found.");
        }
    }

    public bool OnPlayerDefeat() => currentPlayerHealth <= 0;
}
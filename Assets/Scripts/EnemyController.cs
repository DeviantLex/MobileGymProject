using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public delegate void EnemyDefeatDelegate(EnemyController defeatedEnemy);
    public event EnemyDefeatDelegate OnEnemyDefeated;
    public int currentEnemyHealth;
    int minEnemyHealth = 50;
    int maxEnemyHealth = 100;
    public int minEnemyAttackPoints = 5;
    public int maxEnemyAttackPoints = 10;
    public Slider enemyHealthBar;
    public PlayerLifeStats playerLifeStats;

    private bool isDefeated = false;
    public void Initialize(PlayerLifeStats player, int health)
{
    this.playerLifeStats = player;
    this.currentEnemyHealth = health;

    if (enemyHealthBar != null)
    {
        enemyHealthBar.maxValue = health;
        enemyHealthBar.value = health;
    }
}

    void Start()
    {
     if (playerLifeStats == null)
     {
        playerLifeStats = FindFirstObjectByType<PlayerLifeStats>();
        if (playerLifeStats == null)
        {
            Debug.LogError("PlayerLifeStats not found!");
        }

        currentEnemyHealth = Random.Range(minEnemyHealth, maxEnemyHealth + 1);
        enemyHealthBar.maxValue = currentEnemyHealth; 
        enemyHealthBar.value = currentEnemyHealth;

        Debug.Log($"Enemy initialized with {currentEnemyHealth} health.");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDefeated) return;

        currentEnemyHealth -= damage;
        currentEnemyHealth = Mathf.Clamp(currentEnemyHealth, 0, maxEnemyHealth);
        enemyHealthBar.value = currentEnemyHealth;

        Debug.Log($"Enemy took {damage} damage. Current health: {currentEnemyHealth}");

        if (currentEnemyHealth <= 0 && !isDefeated)
        {
            isDefeated = true;
            Debug.Log("Enemy is defeated.");
            OnEnemyDefeat();
        }
    }

    public void EnemyAttack(PlayerLifeStats player)
    {
        if (isDefeated) return;

        int currentAttackPoints = Random.Range(minEnemyAttackPoints, maxEnemyAttackPoints + 1);
        Debug.Log($"Enemy attacks with {currentAttackPoints} damage!");
        player.TakeDamage(currentAttackPoints);
    }

    public bool OnEnemyDefeat()
    {
        if (currentEnemyHealth <= 0)
        {
            OnEnemyDefeated?.Invoke(this);
            Debug.Log("Enemy defeat logic triggered.");
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
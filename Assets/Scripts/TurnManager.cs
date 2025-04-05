using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public PlayerLifeStats player;
    public Transform enemyContainer; // Parent object for organizing enemies
    public List<GameObject> enemyPrefabs;
    public TextMeshProUGUI turnNum;
    public Button attackButton, defendButton, healButton, runButton;
    public Button fireButton, waterButton, earthButton, airButton;
    public CanvasGroup playerUI;
    public PanelManager panelManager;
    public int dungeonMapIndex;
    public int fightIndex;

    private EnemyController.Element selectedElement = EnemyController.Element.Neutral;
    private List<EnemyController> enemies = new();
    private int currentTurn = 1, currentEnemyIndex = 0;
    private bool isPlayerTurn = true;
    public int numberOfEnemies = 3, runDamage = 10;
    private EnemyController currentEnemy;

    void Start()
    {
        attackButton.onClick.AddListener(() => PlayerAction(() => player.PlayerAttack(GetCurrentEnemy(), selectedElement)));
        defendButton.onClick.AddListener(() => PlayerAction(player.Defend));
        healButton.onClick.AddListener(() => PlayerAction(player.Heal));
        runButton.onClick.AddListener(RunAway);

        fireButton.onClick.AddListener(() => SetAttackElement(EnemyController.Element.Fire));
        waterButton.onClick.AddListener(() => SetAttackElement(EnemyController.Element.Water));
        earthButton.onClick.AddListener(() => SetAttackElement(EnemyController.Element.Earth));
        airButton.onClick.AddListener(() => SetAttackElement(EnemyController.Element.Air));

        SpawnNextEnemy(); // Spawn the first enemy
        StartTurn();
    }

    void Update() => turnNum.text = $"Turn: {currentTurn}";

    private void SetAttackElement(EnemyController.Element element)
    {
        selectedElement = element;
        Debug.Log($"Selected attack element: {selectedElement}");
    }

    private void SpawnNextEnemy()
    {
        if (currentEnemyIndex >= numberOfEnemies) // End game if all enemies are defeated
        {
            EndGame(true);
            return;
        }

        if (currentEnemy != null)
        {
            Destroy(currentEnemy.gameObject); // Remove previous enemy
        }

        GameObject enemyObj = Instantiate(
            enemyPrefabs[Random.Range(0, enemyPrefabs.Count)],
            enemyContainer // Parent to the enemy container
        );

        enemyObj.transform.SetParent(enemyContainer, false); // Ensure correct UI positioning

        RectTransform rectTransform = enemyObj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = Vector2.zero; // Center inside the UI container
        }
        else
        {
            enemyObj.transform.localPosition = Vector3.zero; // Adjust for world-space objects
        }

        if (enemyObj.TryGetComponent(out EnemyController enemy))
        {
            currentEnemy = enemy;
            currentEnemy.Initialize(player, Random.Range(50, 100));
            currentEnemy.OnEnemyDefeated += HandleEnemyDefeat;
            enemies.Add(currentEnemy);
        }

        Debug.Log($"Spawned enemy {currentEnemyIndex + 1}/{numberOfEnemies} inside {enemyContainer.name}");
    }

    private void HandleEnemyDefeat(EnemyController defeatedEnemy)
    {
        enemies.Remove(defeatedEnemy);
        currentEnemyIndex++;

        Destroy(defeatedEnemy.gameObject); // Remove defeated enemy
        SpawnNextEnemy(); // Spawn the next one
    }

    private void StartTurn()
    {
        if (IsGameOver()) return;
        TogglePlayerUI(isPlayerTurn);
        if (!isPlayerTurn) EnemyTurn();
    }

    private void PlayerAction(System.Action action)
    {
        action.Invoke();
        EndPlayerTurn();
    }

    private void EndPlayerTurn()
    {
        isPlayerTurn = false;
        StartTurn();
    }

    private void EnemyTurn()
    {
        EnemyController enemy = GetCurrentEnemy();
        if (enemy != null)
        {
            enemy.EnemyAttack(player);
            if (enemy.currentEnemyHealth <= 0)
            {
                HandleEnemyDefeat(enemy);
            }
        }
        NextTurn();
    }

    private void RunAway()
    {
        player.TakeDamage(runDamage);
        Debug.Log($"Player took {runDamage} damage while running away.");
        EndGame(false);
    }

    private void TogglePlayerUI(bool enable) => playerUI.alpha = enable ? 1 : 0;

    private void NextTurn()
    {
        isPlayerTurn = true;
        currentTurn++;
        StartTurn();
    }

    private bool IsGameOver()
    {
        if (player.OnPlayerDefeat())
        {
            EndGame(false);
            return true;
        }
        return currentEnemyIndex >= numberOfEnemies;
    }

    private void EndGame(bool playerWon)
    {
        TogglePlayerUI(false);
        if (playerWon) player.currentCoins += 100;
        if (playerWon) panelManager.OpenPanel(dungeonMapIndex); 
        if (playerWon) panelManager.ClosePanel(fightIndex);

        Debug.Log(playerWon ? "You win!" : "You lose.");
    }

    public void ResetEnemies()
    {
        currentEnemyIndex = 0; // Reset enemy index
        enemies.Clear(); // Clear existing enemy list
        numberOfEnemies = Random.Range(2, 5); // Randomize the number of enemies per fight
        SpawnNextEnemy(); // Start a new fight
        currentTurn = 1; // Reset turn count
        isPlayerTurn = true;
        StartTurn(); // Restart turn cycle
    }

    private EnemyController GetCurrentEnemy() => currentEnemy;
}
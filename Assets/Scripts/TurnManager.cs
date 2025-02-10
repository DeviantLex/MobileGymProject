using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public PlayerLifeStats player;
    public Transform enemySpawnPoint;
    public List<GameObject> enemyPrefabs; 
    public TextMeshProUGUI turnNum;
    
    public Button attackButton, defendButton, healButton, runButton;
    
    private List<GameObject> enemies = new();
    private int currentTurn = 1, currentEnemyIndex = 0;
    private bool isPlayerTurn = true;
    public int numberOfEnemies = 3, runDamage = 10;
    public float enemySpreadDistance = 50f;


    void Start()
    {
        attackButton.onClick.AddListener(() => PlayerAction(() => player.PlayerAttack(GetCurrentEnemy())));
        defendButton.onClick.AddListener(() => PlayerAction(player.Defend));
        healButton.onClick.AddListener(() => PlayerAction(player.Heal));
        runButton.onClick.AddListener(RunAway);

        SpawnEnemy();
        StartTurn();
    }
    

    void Update() => turnNum.text = $"Turn: {currentTurn}";

    void SpawnEnemy()
    {
        if (currentEnemyIndex >= numberOfEnemies) return;

        GameObject enemyObj = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], enemySpawnPoint.position, Quaternion.identity, enemySpawnPoint);
        enemyObj.transform.localPosition = new Vector3(currentEnemyIndex * enemySpreadDistance, 0, 0);
        enemies.Add(enemyObj);

        if (enemyObj.TryGetComponent(out EnemyController enemy))
        {
            enemy.Initialize(player, Random.Range(50, 100));
            enemy.OnEnemyDefeated += HandleEnemyDefeat;
        }
    }

    void HandleEnemyDefeat(EnemyController defeatedEnemy)
    {
        currentEnemyIndex++;
        if (currentEnemyIndex < numberOfEnemies)
            SpawnEnemy();
        else
            EndGame(true);
    }

    void StartTurn()
    {
        if (IsGameOver()) return;
        EnablePlayerUI(isPlayerTurn);
        if (!isPlayerTurn) EnemyTurn();
    }

    void PlayerAction(System.Action action)
    {
        action.Invoke();
        EndPlayerTurn();
    }

    void EndPlayerTurn()
    {
        isPlayerTurn = false;
        StartTurn();
    }

    void EnemyTurn()
    {
        GetCurrentEnemy()?.EnemyAttack(player);
        if (GetCurrentEnemy()?.OnEnemyDefeat() == true)
        {
            HandleEnemyDefeat(GetCurrentEnemy());
            return;
        }

        NextTurn();
    }

    void RunAway()
    {
        player.TakeDamage(runDamage);
        Debug.Log($"Player took {runDamage} damage while running away.");
        EnablePlayerUI(false);
    }

    void EnablePlayerUI(bool enable)
    {
        attackButton.gameObject.SetActive(enable);
        defendButton.gameObject.SetActive(enable);
        healButton.gameObject.SetActive(enable);
        runButton.gameObject.SetActive(enable);
    }

    void NextTurn()
    {
        isPlayerTurn = true;
        currentTurn++;
        StartTurn();
    }

    bool IsGameOver()
    {
        if (player.OnPlayerDefeat())
        {
            EndGame(false);
            return true;
        }
        return currentEnemyIndex >= enemies.Count;
    }

    void EndGame(bool playerWon)
    {
        EnablePlayerUI(false);
        Debug.Log(playerWon ? "You win!" : "You lose.");
    }

    EnemyController GetCurrentEnemy() => (currentEnemyIndex < enemies.Count) ? enemies[currentEnemyIndex].GetComponent<EnemyController>() : null;
}
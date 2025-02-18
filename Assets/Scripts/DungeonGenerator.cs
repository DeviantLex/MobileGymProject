using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonGenerator : MonoBehaviour
{
    public int gridWidth = 10, gridHeight = 10;
    public int roomCount = 5;
    public Vector2Int roomSizeMin = new Vector2Int(2, 2);
    public Vector2Int roomSizeMax = new Vector2Int(4, 4);
    public GameObject panelPrefab;
    public GridLayoutGroup gridLayout;
    public PanelManager panelManager; // ✅ PanelManager reference

    private DungeonCell[,] grid;
    private List<RectInt> rooms = new List<RectInt>();
    private Vector2Int playerPosition;
    private HashSet<Vector2Int> enemyPositions = new();
    private HashSet<Vector2Int> rewardPositions = new();

    public int rewardPanelIndex;
    public int enemyRoomIndex;
    public int dungeonRoomIndex;

    public int numEnemies = 3;
    public int numRewards = 2;

    void Start() {
        GenerateGrid();
        GenerateRooms();
        ConnectRooms();
        PlacePlayer();
        PlaceEnemiesAndRewards();
        VisualizeDungeon();
    }

    void GenerateGrid()
    {
        grid = new DungeonCell[gridWidth, gridHeight];
        
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject panel = Instantiate(panelPrefab, gridLayout.transform);
                panel.name = $"Cell ({x}, {y})"; 
                grid[x, y] = new DungeonCell(panel);
            }
        }
    }

    void GenerateRooms()
    {
        for (int i = 0; i < roomCount; i++)
        {
            int roomWidth = Random.Range(roomSizeMin.x, roomSizeMax.x);
            int roomHeight = Random.Range(roomSizeMin.y, roomSizeMax.y);
            int posX = Random.Range(1, gridWidth - roomWidth - 1);
            int posY = Random.Range(1, gridHeight - roomHeight - 1);

            RectInt newRoom = new RectInt(posX, posY, roomWidth, roomHeight);

            bool overlaps = false;
            foreach (var room in rooms)
            {
                if (newRoom.Overlaps(room))
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps)
            {
                rooms.Add(newRoom);
                MarkRoom(newRoom);
            }
        }
    }

    void ConnectRooms()
    {
        for (int i = 1; i < rooms.Count; i++)
        {
            Vector2Int start = Vector2Int.RoundToInt(rooms[i - 1].center);
            Vector2Int end = Vector2Int.RoundToInt(rooms[i].center);
            CreateLShapedHallway(start, end);
        }
    }

    void CreateLShapedHallway(Vector2Int start, Vector2Int end)
    {
        Vector2Int current = start;

        while (current.x != end.x)
        {
            grid[current.x, current.y].isRoom = true;
            current.x += current.x < end.x ? 1 : -1;
        }

        while (current.y != end.y)
        {
            grid[current.x, current.y].isRoom = true;
            current.y += current.y < end.y ? 1 : -1;
        }
    }

    void MarkRoom(RectInt room) {
        for (int y = room.y; y < room.yMax; y++) {
            for (int x = room.x; x < room.xMax; x++)
            {
                grid[x, y].isRoom = true;
            }
        }
    }

    void PlacePlayer() {
        playerPosition = new Vector2Int(Mathf.FloorToInt(rooms[0].center.x), Mathf.FloorToInt(rooms[0].center.y));
        UpdatePlayerVisual();
    }

    void PlaceEnemiesAndRewards()
    {
        List<Vector2Int> availablePositions = new();

        foreach (var room in rooms)
        {
            for (int y = room.y; y < room.yMax; y++)
            {
                for (int x = room.x; x < room.xMax; x++)
                {
                    Vector2Int pos = new(x, y);
                    if (pos != playerPosition) availablePositions.Add(pos);
                }
            }
        }

        for (int i = 0; i < availablePositions.Count; i++)
        {
            Vector2Int temp = availablePositions[i];
            int randomIndex = Random.Range(i, availablePositions.Count);
            availablePositions[i] = availablePositions[randomIndex];
            availablePositions[randomIndex] = temp;
        }

        for (int i = 0; i < numEnemies && i < availablePositions.Count; i++)
        {
            enemyPositions.Add(availablePositions[i]);
        }

        for (int i = numEnemies; i < numEnemies + numRewards && i < availablePositions.Count; i++)
        {
            rewardPositions.Add(availablePositions[i]);
        }
    }

    public void MoveUp() { MovePlayer(Vector2Int.down); }
    public void MoveDown() { MovePlayer(Vector2Int.up); }
    public void MoveLeft() { MovePlayer(Vector2Int.left); }
    public void MoveRight() { MovePlayer(Vector2Int.right); }

    void MovePlayer(Vector2Int direction) {
        Vector2Int newPosition = playerPosition + direction;

        if (newPosition.x >= 0 && newPosition.x < gridWidth &&
            newPosition.y >= 0 && newPosition.y < gridHeight &&
            grid[newPosition.x, newPosition.y].isRoom)
        {
            playerPosition = newPosition;

            if (enemyPositions.Contains(newPosition))
            {
                panelManager.OpenPanel(enemyRoomIndex); 
                panelManager.ClosePanel(dungeonRoomIndex); 
            }
            else if (rewardPositions.Contains(newPosition))
            {
                panelManager.OpenPanel(rewardPanelIndex); 
                //panelManager.ClosePanel(dungeonRoomIndex); 
            }

            UpdatePlayerVisual();
        }
    }

    void UpdatePlayerVisual() {
        for (int y = 0; y < gridHeight; y++)  {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);
                Image panelImage = grid[x, y].panel.GetComponent<Image>();

                if (currentPos == playerPosition)
                {
                    panelImage.color = Color.green; // Player
                }
                else if (enemyPositions.Contains(currentPos))
                {
                    panelImage.color = Color.red; // Enemy
                }
                else if (rewardPositions.Contains(currentPos))
                {
                    panelImage.color = Color.yellow; // Reward
                }
                else if (grid[x, y].isRoom)
                {
                    panelImage.color = Color.white; // Normal rooms
                }
                else
                {
                    panelImage.color = Color.black; // Walls
                }
            }
        }
    }

    void VisualizeDungeon() {
        UpdatePlayerVisual();
    }

    class DungeonCell
    {
        public GameObject panel;
        public bool isRoom = false;

        public DungeonCell(GameObject panel)
        {
            this.panel = panel;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public const int Width = 20;
    public const int Height = 20;

    public GameObject tilePrefab;

    public Transform tileParent;
    

    public static List<List<TileScript>> Tiles;

    public List<LevelScript> levels;

    private LevelScript _currentLevel;

    public static List<EnemyScript> enemiesAlive = new List<EnemyScript>();

    public static List<EnemyScript> enemiesToBeRemoved = new List<EnemyScript>();

    public PlayerController pc;

    public GameObject kingPrefab;
    public GameObject knightPrefab;
    public GameObject rookPrefab;
    public GameObject bishopPrefab;
    public GameObject supermanPrefab;

    [Space(20)]
    public Camera mainCamera;

    public float maxCameraHeight = 24f;
    public float minCameraHeight = 10f;

    public float maxHorizontal = 10f;
    public float minHorizontal = -10f;

    public float maxVertical = 8f;
    public float minVertical = -26f;

    public static int Phase = 2;

    private static float MovementSpeed = 30f;
    private static float ScrollingSpeed = 500f;
    void Start()
    {
        Tiles = new List<List<TileScript>>();
        for (int z = 0; z < Height; z++)
        {
            List<TileScript> row = new List<TileScript>();
            Tiles.Add(row);
            for (int x = 0; x < Width; x++)
            {
                TileScript tile = Instantiate(tilePrefab, new Vector3(x * 2 - Width + 1, 0, z * 2 - Height + 1), Quaternion.identity, tileParent).GetComponent<TileScript>();
                tile.SetCoords(x, z);
                row.Add(tile);
            } 
        }

        Tiles[0][10].SetFog(3);
        
        _currentLevel = levels[0];
        _currentLevel.StartFirstWave();
    }

    void Update()
    {
        if (Phase == 1)
        {
            _currentLevel.GetCurrentWave().SpawnNextEnemies();
            foreach (var enemy in enemiesAlive)
            {
                enemy.ChooseNextMove();
            }
            
            Phase += 1;
        }

        HandleCamera();
    }

    public static void RemoveEnemy(GameObject enemy)
    {
        enemiesToBeRemoved.Add(enemy.GetComponent<EnemyScript>());
        enemy.SetActive(false);
    }

    private void HandleCamera()
    {        
        float horizontalAxis = Input.GetAxisRaw("Horizontal") * MovementSpeed * Time.deltaTime;
        float verticalAxis = Input.GetAxisRaw("Vertical") * MovementSpeed * Time.deltaTime;
        float scrollAxis = Input.GetAxisRaw("Mouse ScrollWheel") * ScrollingSpeed * Time.deltaTime;

        Vector3 newPosition = mainCamera.transform.position;
        
        newPosition.x += horizontalAxis;
        newPosition.y = Mathf.Clamp(newPosition.y - scrollAxis, minCameraHeight, maxCameraHeight);
        newPosition.z = Mathf.Clamp(newPosition.z + verticalAxis + scrollAxis, minVertical, maxVertical);

        float horizontalModifier = 1 - (newPosition.y - minCameraHeight) / (maxCameraHeight - minCameraHeight);
        newPosition.x = Mathf.Clamp(newPosition.x, minHorizontal * horizontalModifier, maxHorizontal * horizontalModifier);
        
        mainCamera.transform.position = newPosition;
    }

    public void MoveEnemies()
    {
        enemiesToBeRemoved = new List<EnemyScript>();
        
        foreach (EnemyScript enemy in enemiesAlive)
        { 
            enemy.Move();
        }

        foreach (EnemyScript enemy in enemiesToBeRemoved)
        {
            enemiesAlive.Remove(enemy);
            Destroy(enemy);
        }
        enemiesToBeRemoved.Clear();


        Phase += 1;
        pc.MarkFog();
        Phase += 1;
        
        // todo choose player movement and killing and stuff
        // todo when complete set phase to 1
        
        TileScript.AgeAllFogTiles();
        
        Phase = 1;
    }
}

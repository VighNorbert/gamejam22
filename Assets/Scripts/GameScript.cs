using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    public const int Width = 20;
    public const int Height = 20;

    public GameObject tilePrefab;

    public Transform tileParent;
    

    public static List<List<TileScript>> Tiles;

    public List<LevelScript> levels;

    private LevelScript _currentLevel;
    private int _currentLevelIndex;

    public static List<EnemyScript> enemiesAlive = new List<EnemyScript>();

    public static List<EnemyScript> enemiesToBeRemoved = new List<EnemyScript>();

    public PlayerController pc;

    public GameObject kingPrefab;
    public GameObject knightPrefab;
    public GameObject rookPrefab;
    public GameObject bishopPrefab;
    public GameObject supermanPrefab;
    public GameObject infoText;

    [Space(20)]
    public Camera mainCamera;

    public float maxCameraHeight = 22f;
    public float minCameraHeight = 5f;

    public float maxHorizontal = 15f;
    public float minHorizontal = -15f;

    public float maxVertical = 20f;
    public float minVertical = -18f;

    public static int Phase = 2;

    private static float MovementSpeed = 30f;
    private static float ScrollingSpeed = 500f;

    public GameObject inventory;

    [HideInInspector]
    public int enemiesMoved = 0;

    public TMPro.TextMeshProUGUI levelText;

    public GameObject winScreen;
    
    void Start()
    {
        Phase = 2;
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

        pc.SpawnPlayer(10, 0);
        Tiles[0][10].SetFog(3);
        
        _currentLevel = levels[_currentLevelIndex];
        _currentLevel.StartFirstWave();
        if (levelText != null)
        {
            levelText.fontSize = 56;
            levelText.text = "Level " + (_currentLevelIndex + 1);
            levelText.gameObject.SetActive(true);
            StartCoroutine(DisableLevelText());
        }
    }

    private IEnumerator DisableLevelText()
    {
        yield return new WaitForSeconds(2f);
        levelText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Phase == 1)
        {
            bool enemiesRemaining = _currentLevel.GetCurrentWave().SpawnNextEnemies();

            foreach (var enemy in enemiesAlive)
            {
                enemy.ChooseNextMove();
            }

            if (!enemiesRemaining && enemiesAlive.Count == 0)
            {
                if (!_currentLevel.StartNextWave())
                {
                    Debug.Log("Level Complete");
                    _currentLevelIndex++;
                    if (_currentLevelIndex == levels.Count)
                    {
                        if (winScreen)
                        {
                            winScreen.SetActive(true);
                        }
                        Phase = 6;
                    }
                    else
                    {
                        pc.RefillHealth();
                        pc.ResetScore();
                        pc.NewLevel();
                        _currentLevel = levels[_currentLevelIndex];
                        _currentLevel.StartFirstWave();

                        if (levelText != null)
                        {
                            levelText.fontSize = 56;
                            levelText.text = "Level " + (_currentLevelIndex + 1);
                            levelText.gameObject.SetActive(true);
                            StartCoroutine(DisableLevelText());
                        }
                    }
                }
                else if (levelText != null)
                {
                    levelText.fontSize = 36;
                    levelText.text = "Wave " + (_currentLevel.GetCurrentWaveIndex() + 1);
                    levelText.gameObject.SetActive(true);
                    StartCoroutine(DisableLevelText());
                }
            }
            
            Phase += 1;
            ToggleInventory();
            infoText.GetComponent<InfotextController>().UpdateInfoText("Choose shape and place it");
        }

        if (enemiesMoved == enemiesAlive.Count && Phase == 3)
        {
            Phase += 1;
            infoText.GetComponent<InfotextController>().UpdateInfoText("Fog!");
            enemiesMoved = 0;
        }

        if (Phase == 4)
        {
            pc.MarkFog();
            TileScript.AgeAllFogTiles();
            Phase += 1;
            Tiles[PlayerController.PlayerTileCoords.y][PlayerController.PlayerTileCoords.x].hasPlayer = false;
            infoText.GetComponent<InfotextController>().UpdateInfoText("Choose where you want to go");
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
        newPosition.z = Mathf.Clamp(newPosition.z + verticalAxis + scrollAxis, minVertical - newPosition.y, maxVertical - newPosition.y);

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
    }

    public void ToggleInventory()
    {
        for (int i = 0; i < inventory.transform.childCount; i++)
        {
            inventory.transform.GetChild(i).gameObject.GetComponent<Button>().interactable = 
                !inventory.transform.GetChild(i).gameObject.GetComponent<Button>().interactable;
        }
    }
}

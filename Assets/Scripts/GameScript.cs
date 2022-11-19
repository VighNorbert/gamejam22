using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public const int Width = 20;
    public const int Height = 20;

    public GameObject tilePrefab;

    public Transform tileParent;
    
    public Camera mainCamera;

    public static List<List<TileScript>> Tiles;

    public List<LevelScript> levels;

    private LevelScript _currentLevel;

    public List<EnemyScript> enemiesAlive;

    public PlayerController pc;

    public GameObject kingPrefab;
    public GameObject knightPrefab;
    public GameObject rookPrefab;
    public GameObject bishopPrefab;
    public GameObject supermanPrefab;


    public static int Phase = 2;

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
    }

    public void MoveEnemies()
    {
        foreach (EnemyScript enemy in enemiesAlive)
        { 
            enemy.Move();
        }

        Phase += 1;
        pc.MarkFog();
        Phase += 1;
        
        // todo choose player movement and killing and stuff
        // todo when complete set phase to 1
        
        TileScript.AgeAllFogTiles();
        
        Phase = 1;
    }
}

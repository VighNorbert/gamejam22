using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public static int width = 20;
    public static int height = 20;
    
    public GameObject tilePrefab;

    public Transform tileParent;
    
    public Camera mainCamera;

    public static List<List<TileScript>> tiles;

    public List<LevelScript> levels;

    private LevelScript currentLevel;

    public List<EnemyScript> enemiesAlive;

    public PlayerController _pc;

    public GameObject kingPrefab;
    public GameObject knightPrefab;
    public GameObject rookPrefab;
    public GameObject bishopPrefab;
    public GameObject supermanPrefab;


    public static int phase = 2;

    void Start()
    {

        tiles = new List<List<TileScript>>();
        for (int z = 0; z < height; z++)
        {
            List<TileScript> row = new List<TileScript>();
            tiles.Add(row);
            for (int x = 0; x < width; x++)
            {
                TileScript tile = Instantiate(tilePrefab, new Vector3(x * 2 - width + 1, 0, z * 2 - height + 1), Quaternion.identity, tileParent).GetComponent<TileScript>();
                tile.SetCoords(x, z);
                row.Add(tile);
            } 
        }

        tiles[0][10].SetFog(true);
        
        // camera

        currentLevel = levels[0];
        currentLevel.StartFirstWave();
    }

    void Update()
    {
        if (phase == 1)
        {
            currentLevel.GetCurrentWave().SpawnNextEnemies();
            foreach (var enemy in enemiesAlive)
            {
                enemy.ChooseNextMove();
            }

            phase += 1;
        }
    }

    public void MoveEnemies()
    {
        foreach (EnemyScript enemy in enemiesAlive)
        {
            enemy.Move();
        }

        phase += 1;
        _pc.MarkFog();
        phase += 1;
        
        // todo choose player movement and killing and stuff
        // todo when complete set phase to 1
        phase = 1;
    }
}

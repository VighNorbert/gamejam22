using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameScript : MonoBehaviour
{
    public static int WIDTH = 20;
    public static int HEIGHT = 20;
    
    public GameObject tilePrefab;
    // public int width;
    // public int height;
    public Camera camera;

    public List<List<TileScript>> tiles;

    void Start()
    {
        tiles = new List<List<TileScript>>();
        for (int z = 0; z < HEIGHT; z++)
        {
            List<TileScript> row = new List<TileScript>();
            tiles.Add(row);
            for (int x = 0; x < WIDTH; x++)
            {
                TileScript tile = Instantiate(tilePrefab, new Vector3(x * 2 - WIDTH + 1, 0, z * 2 - HEIGHT + 1), Quaternion.identity).GetComponent<TileScript>();
                tile.SetCoords(x, z);
                row.Add(tile);
            } 
        }
        
        // camera
    }

    void Update()
    {
        
    }
}

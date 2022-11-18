using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class GameScript : MonoBehaviour
{
    public int width = 20;
    public int height = 20;
    
    public GameObject tilePrefab;

    public Transform tileParent;
    
    public Camera camera;

    public List<List<TileScript>> tiles;

    public static int phase = 1;

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
        
        // camera
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (phase < 5)
            {
                phase += 1;
            }
            else
            {
                phase = 1;
            }
        }
    }
}

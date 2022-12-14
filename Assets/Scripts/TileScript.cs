using System;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool hasPlayer;
    public bool hasEnemy;
    public bool hasDeadEnemy;
    private int _fogState;
    public bool isFinishingTile;
    public Vector2Int coords;

    public bool isFogConnectedToPlayer;
    public TileScript cameFrom;
    public int pathCost;
    
    private Color _basicColor;
    private Color _fogColor;

    public GameObject fog;
    public GameObject initFog;

    private ParticleSystem _ps;

    [HideInInspector] 
    public GameObject enemy;

    private Renderer _renderer;

    void Start()
    {
        transform.tag = "Tile";
    }


    public void SetCoords(int x, int z)
    {
        
        if ((x + z) % 2 == 0)
        {
            _basicColor = new Color(47f/255f, 70f/255f, 24f/255f, 255f/255f);
        }
        else
        {
            _basicColor = new Color(98f/255f, 130f/255f, 60f/255f, 255f/255f);
        }
        _renderer = GetComponent<Renderer>();
        _renderer.material.SetColor("_BaseColor", _basicColor);
        
        coords = new Vector2Int(x, z);
    }

    public void SetFog(int state = 4)
    {
        _fogState = state;
        if (state > 0) {
            fog.SetActive(true);
            
            _fogColor = new Color(1, 1, 1, 1f);
            fog.transform.GetComponent<Renderer>().material.color = _fogColor;
        }
        else
        {
            fog.SetActive(false);
            initFog.SetActive(false);
        }
    }

    public void SetInitFog()
    {
        initFog.SetActive(true);
    }


    public bool GetHasFog()
    {
        return _fogState > 0;
    }

    private void OnMouseExit()
    {
        ResetAllColors();
    }

    public static void ResetAllColors()
    {
        foreach (var tile in GameScript.Tiles)
        {
            foreach (var t in tile)
            {
                t.ResetColor();
                t.ResetFogColor();
            }
        }
    }

    private void ResetColor()
    {
        _renderer.material.color = _basicColor;
    }

    private void ResetFogColor()
    {
        fog.GetComponent<Renderer>().material.color = _fogColor;
    }
    
    private void AgeTheFog()
    {
        if (_fogState == 3)
        {
            initFog.SetActive(false);
        }

        if (_fogState > 0)
        {
            if (_fogState != 1 || !hasPlayer)
            {
                _fogState--;
            }

            if (_fogState == 0)
            {
                fog.SetActive(false);
            }
            else if (_fogState <= 2)
            {
                _fogColor = new Color(1, 1, 1, _fogState * .25f);
                fog.transform.GetComponent<Renderer>().material.color = _fogColor;
            }

        }
    }
    
    public static void AgeAllFogTiles()
    {
        foreach (var tile in GameScript.Tiles)
        {
            foreach (var t in tile)
            {
                t.AgeTheFog();
                t.isFogConnectedToPlayer = false;
                t.cameFrom = null;
                t.pathCost = Int32.MaxValue;
            }
        }
        VisitTile(PlayerController.PlayerTileCoords.x, PlayerController.PlayerTileCoords.y);
    }

    public static void VisitTile(int x, int y)
    {
        TileScript ts = GameScript.Tiles[y][x];
        if (ts.isFogConnectedToPlayer == false && ts.GetHasFog()) {
            ts.isFogConnectedToPlayer = true;
            if (x > 0) VisitTile(x - 1, y);
            if (x < GameScript.Width - 1) VisitTile(x + 1, y);
            if (y > 0) VisitTile(x, y - 1);
            if (y < GameScript.Height - 1) VisitTile(x, y + 1);
        }
    }
}

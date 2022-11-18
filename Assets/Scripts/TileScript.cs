using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool hasEnemy = false;
    public bool hasDeadEnemy = false;
    private bool hasFog = false;
    public bool isFinishingTile;
    private Vector2Int coords;

    public GameObject fog;

    public void SetCoords(int x, int z)
    {
        coords = new Vector2Int(x, z);
        if (z == 0)
        {
            isFinishingTile = true;
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    public void SetFog(bool fogState)
    {
        hasFog = fogState;
        fog.SetActive(fogState);
    }

    public bool GetHasFog()
    {
        return hasFog;
    }
}

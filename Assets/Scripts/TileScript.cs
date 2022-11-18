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
    public Vector2Int coords;

    private Color basicColor = Color.black;

    public GameObject fog;

    private Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.material.color = basicColor;
        transform.tag = "Tile";
    }


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

    private void OnMouseExit()
    {
        renderer.material.color = basicColor;
    }
}

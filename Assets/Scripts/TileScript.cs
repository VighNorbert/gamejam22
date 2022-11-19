using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool hasEnemy;
    public bool hasDeadEnemy;
    private bool _hasFog;
    public bool isFinishingTile;
    public Vector2Int coords;

    private readonly Color _basicColor = Color.black;

    public GameObject fog;

    private Renderer _renderer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.material.color = _basicColor;
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
        _hasFog = fogState;
        fog.SetActive(fogState);
    }

    public bool GetHasFog()
    {
        return _hasFog;
    }

    private void OnMouseExit()
    {
        for (int x = 0; x < GameScript.Width; x++)
        {
            for (int y = 0; y < GameScript.Height; y++)
            {
                GameScript.Tiles[y][x].GetComponent<Renderer>().material.color = GameScript.Tiles[y][x]._basicColor;
            }
        }
        _renderer.material.color = _basicColor;
    }
}

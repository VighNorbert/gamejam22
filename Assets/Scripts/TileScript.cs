using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool hasEnemy;
    public bool hasDeadEnemy;
    private bool _hasFog;
    public bool isFinishingTile;
    public Vector2Int coords;

    private Color _basicColor;

    public GameObject fog;

    private Renderer _renderer;

    void Start()
    {
        transform.tag = "Tile";
    }


    public void SetCoords(int x, int z)
    {
        if ((x + z) % 2 == 0)
        {
            _basicColor = new Color {r = 0f, g = 0.4f, b = 0f};
        }
        else
        {
            _basicColor = new Color {r = 0f, g = 0.5f, b = 0f};
        }
        _renderer = GetComponent<Renderer>();
        _renderer.material.color = _basicColor;
        
        coords = new Vector2Int(x, z);
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
        ResetAllColors();
    }

    public static void ResetAllColors()
    {
        foreach (var tile in GameScript.Tiles)
        {
            foreach (var t in tile)
            {
                t.ResetColor();
            }
        }
    }

    public void ResetColor()
    {
        _renderer.material.color = _basicColor;
    }
}

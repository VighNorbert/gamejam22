using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool hasPlayer;
    public bool hasEnemy;
    public bool hasDeadEnemy;
    private int _fogState;
    public bool isFinishingTile;
    public Vector2Int coords;

    private Color _basicColor;
    private Color _fogColor;

    public GameObject fog;

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

    public void SetFog(int state = 4)
    {
        _fogState = state;
        fog.SetActive(true);
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
        fog.GetComponent<Renderer>().material.color = Color.white;
    }
    
    private void AgeTheFog()
    {
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
        }
    }
    
    public static void AgeAllFogTiles()
    {
        foreach (var tile in GameScript.Tiles)
        {
            foreach (var t in tile)
            {
                t.AgeTheFog();
            }
        }
    }
}

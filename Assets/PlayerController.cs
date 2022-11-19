using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public GameObject healthUI;
    [HideInInspector]
    public GameObject currShape;
    private GameObject[] _currTiles;
    private Color _color = Color.green;
    private int _currShapeIndex = 0;

    private List<Vector2Int> _shapeToUse = new List<Vector2Int>();

    private Vector2Int _currTileCoords;
    
    public GameScript gs;

    public InventoryController ic;

    public int totalShapes;

    private int _xFlip = 1;
    private int _yFlip = 1;
    private bool _swap = false;
    private int _angle = 0;

    public static int Health = 5;
    
    void Start()
    {
        totalShapes = transform.Find("Shapes").transform.childCount; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _angle = (_angle + 270) % 360;
            HandleAngles();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _angle = (_angle + 90) % 360;
            HandleAngles();
        }


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log(_currShapeIndex);

            currShape = transform.Find("Shapes").transform.GetChild(_currShapeIndex).gameObject;
            if (_currShapeIndex < totalShapes - 1)
            {
                _currShapeIndex += 1;
            }
            else
            {
                _currShapeIndex = 0;
            }
        }
        
        if (GameScript.phase == 2)
        {
            if (currShape)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                // Casts the ray and get the first game object hit
                Physics.Raycast(ray, out hit);
                if (hit.transform != null)
                {
                    if (hit.transform.CompareTag("Tile"))
                    {
                        ShapeController sc = currShape.GetComponent<ShapeController>();
                        hit.transform.GetComponent<Renderer>().material.color = Color.red;
                        _color = isNeigbouring(sc, hit) ? Color.green : Color.red;

                        Debug.Log("new");
                        
                        foreach (var p in sc.points)
                        {
                            int pointsY = p.y;
                            int pointsX = p.x;

                            Debug.Log("a, x " + p.x + " y " + p.y);
                            
                            if (_swap)
                            {
                                (pointsX, pointsY) = (pointsY, pointsX);
                            }

                            pointsX = _xFlip * pointsX;
                            pointsY = _yFlip * pointsY;

                            int y = hit.transform.GetComponent<TileScript>().coords.y + pointsY;
                            int x = hit.transform.GetComponent<TileScript>().coords.x + pointsX;
                            
                            if (sc.points.Count > _shapeToUse.Count)
                            {
                                _shapeToUse.Add(new Vector2Int(pointsX, pointsY));
                            }
                            
                            if (x < GameScript.width && y < GameScript.height && x >= 0 && y >= 0)
                            {
                                GameScript.tiles[y][x].transform.GetComponent<Renderer>().material.color = _color;
                            }
                        }
                        
                        if (Input.GetMouseButtonDown(0) && _color == Color.green)
                        {
                            _currTileCoords = new Vector2Int(hit.transform.GetComponent<TileScript>().coords.x, hit.transform.GetComponent<TileScript>().coords.y);
                            
                            SwapShape();
                            
                            currShape = null;
                            GameScript.phase = 3;
                            gs.MoveEnemies();
                        }

                    }
                }
            }
           
        }   
    }

    private bool isNeigbouring(ShapeController sc, RaycastHit hit)
    {
        foreach (var p in sc.points)
        {
            int pointsY = p.y;
            int pointsX = p.x;

            if (_swap)
            {
                (pointsX, pointsY) = (pointsY, pointsX);
            }

            pointsX = _xFlip * pointsX;
            pointsY = _yFlip * pointsY;
                            
            int y = hit.transform.GetComponent<TileScript>().coords.y + pointsY;
            int x = hit.transform.GetComponent<TileScript>().coords.x + pointsX;

            if (GameScript.width <= x || GameScript.height <= y || 0 > x || 0 > y)
            {
                continue;
            }
            if ((InBounds(y + 1, x) && GameScript.tiles[y + 1][x].GetHasFog()) ||
                (InBounds(y - 1, x) && GameScript.tiles[y - 1][x].GetHasFog()) ||
                (InBounds(y, x + 1) && GameScript.tiles[y][x + 1].GetHasFog()) ||
                (InBounds(y, x - 1) && GameScript.tiles[y][x - 1].GetHasFog()))
            {
                _color = Color.green;
                return true;
            }
        }

        return false;
    }

    public void MarkFog()
    {
        Debug.Log(_shapeToUse.Count);
        for (int i = 0; i < _shapeToUse.Count; i++)
        {
            int y = _currTileCoords.y + _shapeToUse[i].y;
            int x = _currTileCoords.x + _shapeToUse[i].x;
            Debug.Log("x " + _shapeToUse[i].x + " y " + _shapeToUse[i].y);
            if (GameScript.width <= x || GameScript.height <= y || 0 > x || 0 > y)
            {
                continue;
            }
            GameScript.tiles[y][x].GetComponent<TileScript>().SetFog(true);
        }
        _shapeToUse.Clear();
    }

    public static bool InBounds(int x, int y)
    {
        return (x >= 0 && y >= 0 && x < GameScript.width && y < GameScript.height);
    }

    public void SwapShape()
    {
        int randInt = Random.Range(3, totalShapes);
        int sibIndex = transform.Find("Shapes").transform.GetChild(randInt).gameObject.transform.GetSiblingIndex();
        transform.Find("Shapes").transform.GetChild(randInt).gameObject.transform.SetSiblingIndex(currShape.GetComponent<ShapeController>().index);
        currShape.transform.SetSiblingIndex(sibIndex);

        for (int i=0; i<totalShapes; i++)
        {
            transform.Find("Shapes").transform.GetChild(i).GetComponent<ShapeController>().index = transform.Find("Shapes").transform.GetChild(i).GetComponent<ShapeController>().transform.GetSiblingIndex();
        }

        ic.UpdateInventory();
    }


    public void HandleAngles()
    {
        _shapeToUse.Clear();
        if (_angle == 0)
        {
            _xFlip = 1;
            _yFlip = 1;
            _swap = false;
        }
        else if (_angle == 90)
        {
            _xFlip = 1;
            _yFlip = -1;
            _swap = true;
        }
        else if (_angle == 180)
        {
            _xFlip = -1;
            _yFlip = -1;
            _swap = false;
        }
        else if (_angle == 270)
        {
            _xFlip = -1;
            _yFlip = 1;
            _swap = true;
        }
    }

    public static int GetHealth()
    {
        return Health;
    }

    public static void HealthDown()
    {
        Health -= 1;
        if (Health <= 0)
        {
            Debug.Log("Game Over");
        }
    }
}

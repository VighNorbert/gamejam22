using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public GameObject currShape;
    private GameObject[] _currTiles;
    private Color _color = Color.green;
    private int _currShapeIndex;

    private readonly List<Vector2Int> _shapeToUse = new();

    private Vector2Int _currTileCoords;
    public static Vector2Int PlayerTileCoords;
    
    public GameScript gs;

    public InventoryController ic;

    public int totalShapes;

    private int _xFlip = 1;
    private int _yFlip = 1;
    private bool _swap;
    private int _angle;

    private static int _health = 5;

    public static GameObject healthUI;
    public  GameObject _healthUI;

    private float _timeElapsed = 0f;
    private float _movementDuration = 0.2f;
    private List<Vector2Int> pathToTake = new();
    private bool _playerMoving = false;
    private int currPlayerMovementTile = 0;

    public GameObject infoText;

    private Animator _animator;

    public PlayerController()
    {
        _angle = 0;
    }

    void Start()
    {
        healthUI = _healthUI;
        totalShapes = transform.Find("Shapes").transform.childCount;
        _animator = GetComponentInChildren<Animator>();
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
        
        if (GameScript.Phase == 2)
        {
            HandlePhaseTwo();
        }   
        else if (GameScript.Phase == 5)
        {
            HandlePhaseFive();
        }

        _animator.SetFloat("runSpeed", 0f);
        if (_playerMoving)
        {
            Vector3 worldPosition = new Vector3(pathToTake[currPlayerMovementTile].x * 2f - GameScript.Width + 1, 0.5f, pathToTake[currPlayerMovementTile].y * 2f - GameScript.Height + 1);
            Vector3 nextWorldPosition =
                new Vector3(pathToTake[currPlayerMovementTile + 1].x * 2f - GameScript.Width + 1, 0.5f,
                    pathToTake[currPlayerMovementTile + 1].y * 2f - GameScript.Height + 1);
            transform.rotation = Quaternion.LookRotation(nextWorldPosition - worldPosition);
            float speed = Vector3.Distance(worldPosition, nextWorldPosition) / _movementDuration;
            Debug.Log(speed);
            
            _animator.SetFloat("runSpeed", speed / 10f);
            
            if (_timeElapsed < _movementDuration)
            {
                transform.position = Vector3.Lerp(worldPosition, nextWorldPosition, _timeElapsed / _movementDuration);
                _timeElapsed += Time.deltaTime;
            }
            else
            {
                worldPosition = nextWorldPosition;
                transform.position = worldPosition;
                _timeElapsed = 0f;
                currPlayerMovementTile++;
                if (currPlayerMovementTile == pathToTake.Count-1)
                {
                    _playerMoving = false;
                    pathToTake.Clear();
                    currPlayerMovementTile = 0;
                }
            }
        }
    }

    public void SpawnPlayer(int x, int y)
    {
        transform.position = new Vector3(x * 2f - GameScript.Width + 1, 0.5f, y * 2f - GameScript.Height + 1);
        PlayerTileCoords = new Vector2Int(x, y);
    }

    private void HandlePhaseTwo()
    {
        Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
        // Casts the ray and get the first game object hit
        Physics.Raycast(ray, out var hit);
        if (hit.transform != null)
        {
            if (hit.transform.CompareTag("Tile"))
            {
                if (currShape)
                {
                    ShapeController sc = currShape.GetComponent<ShapeController>();
                    hit.transform.GetComponent<Renderer>().material.color = Color.red;
                    _color = IsNeighbouring(sc, hit) ? Color.green : Color.red;

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

                        TileScript ts = hit.transform.GetComponent<TileScript>();
                        int y = ts.coords.y + pointsY;
                        int x = ts.coords.x + pointsX;

                        if (sc.points.Count > _shapeToUse.Count)
                        {
                            _shapeToUse.Add(new Vector2Int(pointsX, pointsY));
                        }

                        if (x < GameScript.Width && y < GameScript.Height && x >= 0 && y >= 0)
                        {
                            GameScript.Tiles[y][x].transform.GetComponent<Renderer>().material.color = _color;
                        }
                    }

                    if (Input.GetMouseButtonDown(0) && _color == Color.green)
                    {
                        _currTileCoords = new Vector2Int(hit.transform.GetComponent<TileScript>().coords.x,
                            hit.transform.GetComponent<TileScript>().coords.y);

                        SwapShape();

                        currShape = null;
                        GameScript.Phase = 3;
                        MarkInitFog();
                        infoText.GetComponent<InfotextController>().UpdateInfoText("Enemies moving");
                        gs.MoveEnemies();
                    }
                }
            }
            else if (hit.transform.CompareTag("Enemy"))
            {
                List<Vector2Int> enemyPossibleMoves = hit.transform.GetComponent<EnemyScript>().possibleMoves;
                foreach (var possibleMove in enemyPossibleMoves)
                {
                    Vector2Int enemyPosition = hit.transform.GetComponent<EnemyScript>().position;
                    if (InBounds(enemyPosition.x + possibleMove.x, enemyPosition.y + possibleMove.y))
                    {
                        GameScript.Tiles[enemyPosition.y+possibleMove.y][enemyPosition.x+possibleMove.x].transform.GetComponent<Renderer>().material.color = Color.yellow;   
                    }
                }
            }
        }
    }

    private void HandlePhaseFive()
    {
        GameScript.Tiles[PlayerTileCoords.y][PlayerTileCoords.x].hasPlayer = false;
        Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
        int x = -1, y = -1;
        // Casts the ray and get the first game object hit
        Physics.Raycast(ray, out var hit);
        if (hit.transform != null)
        {
            if (hit.transform.CompareTag("Tile"))
            {
                TileScript ts = hit.transform.GetComponent<TileScript>();

                y = ts.coords.y;
                x = ts.coords.x;
                if (ts.GetHasFog())
                {
                    if (ts.isFogConnectedToPlayer)
                    {
                        _color = Color.green;
                    }
                    else
                    {
                        _color = Color.red;
                    }

                    GameScript.Tiles[y][x].fog.transform.GetComponent<Renderer>().material.color = _color;
                }
                else
                {
                    _color = Color.red;
                    GameScript.Tiles[y][x].transform.GetComponent<Renderer>().material.color = _color;
                }
            }
            else if (hit.transform.CompareTag("Enemy"))
            {
                EnemyScript es = hit.transform.GetComponent<EnemyScript>();
                (x, y) = (es.position.x, es.position.y);
                TileScript ts = GameScript.Tiles[y][x];
                if (ts.GetHasFog())
                {
                    if (ts.isFogConnectedToPlayer)
                    {
                        _color = Color.green;
                    }
                    else
                    {
                        _color = Color.red;
                    }
                    ts.fog.transform.GetComponent<Renderer>().material.color = _color;
                }
                else
                {
                    _color = Color.red;
                    ts.transform.GetComponent<Renderer>().material.color = _color;
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && x >= 0 && y >= 0 && _color == Color.green)
        {
            FindPath(PlayerTileCoords.x, PlayerTileCoords.y, null, 0);
            TileScript tsDest = GameScript.Tiles[y][x];
            TileScript ts = tsDest;
            TileScript tsNew;
            TileScript tsPrev = null;
            while (ts != null)
            {
                tsNew = ts.cameFrom;
                ts.cameFrom = tsPrev;
                tsPrev = ts;
                ts = tsNew;
            }

            ts = tsPrev;
            while (ts != null)
            {
                pathToTake.Add(ts.coords);
                ts = ts.cameFrom;
            }

            _playerMoving = true;
            transform.position = new Vector3(x * 2f - GameScript.Width + 1, 0.5f, y * 2f - GameScript.Height + 1);
            GameScript.Tiles[y][x].hasPlayer = true;
            PlayerTileCoords = new Vector2Int(x, y);
            if (GameScript.Tiles[y][x].hasEnemy)
            {
                GameScript.Tiles[y][x].hasDeadEnemy = true;
                GameScript.Tiles[y][x].hasEnemy = false;
                GameScript.enemiesAlive.Remove(GameScript.Tiles[y][x].enemy.GetComponent<EnemyScript>());
                Destroy(GameScript.Tiles[y][x].enemy);
                GameScript.Tiles[y][x].enemy = null;
            }
            GameScript.Phase = 1;
        }
    }

    private void FindPath(int x, int y, TileScript cameFrom, int distance)
    {
        TileScript tile = GameScript.Tiles[y][x];
        if (tile.pathCost <= distance || tile.isFogConnectedToPlayer == false)
        {
            return;
        }
        tile.pathCost = distance;
        tile.cameFrom = cameFrom;
        if (x > 0) { FindPath(x - 1, y, tile, distance + 1); }
        if (x < GameScript.Width - 1) { FindPath(x + 1, y, tile, distance + 1); }
        if (y > 0) { FindPath(x, y - 1, tile, distance + 1); }
        if (y < GameScript.Height - 1) { FindPath(x, y + 1, tile, distance + 1); }
    }

    private bool IsNeighbouring(ShapeController sc, RaycastHit hit)
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

            if (GameScript.Width <= x || GameScript.Height <= y || 0 > x || 0 > y)
            {
                continue;
            }
            if ((InBounds(y + 1, x) && GameScript.Tiles[y + 1][x].GetHasFog()) ||
                (InBounds(y - 1, x) && GameScript.Tiles[y - 1][x].GetHasFog()) ||
                (InBounds(y, x + 1) && GameScript.Tiles[y][x + 1].GetHasFog()) ||
                (InBounds(y, x - 1) && GameScript.Tiles[y][x - 1].GetHasFog()))
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
        foreach (var shapePoint in _shapeToUse)
        {
            int y = _currTileCoords.y + shapePoint.y;
            int x = _currTileCoords.x + shapePoint.x;

            if (GameScript.Width <= x || GameScript.Height <= y || 0 > x || 0 > y)
            {
                continue;
            }
            GameScript.Tiles[y][x].GetComponent<TileScript>().SetFog();
        }

        _shapeToUse.Clear();
        _angle = 0;
        HandleAngles();
    }

    public void MarkInitFog()
    {
        foreach (var shapePoint in _shapeToUse)
        {
            int y = _currTileCoords.y + shapePoint.y;
            int x = _currTileCoords.x + shapePoint.x;
            if (GameScript.Width <= x || GameScript.Height <= y || 0 > x || 0 > y)
            {
                continue;
            }
            GameScript.Tiles[y][x].GetComponent<TileScript>().SetInitFog();
        }
    }

    private static bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < GameScript.Width && y < GameScript.Height;
    }

    private void SwapShape()
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


    private void HandleAngles()
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
        
        TileScript.ResetAllColors();
    }

    public static int GetHealth()
    {
        return _health;
    }

    public static void HealthDown()
    {
        _health -= 1;
        GameObject hp = healthUI.transform.GetChild(healthUI.transform.childCount - 1).gameObject;
        Destroy(hp);

        if (_health <= 0)
        {
            Debug.Log("Game Over");
        }
    }
}

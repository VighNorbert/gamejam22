using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
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
    private List<Vector2Int> _pathToTake = new();
    private bool _playerMoving = false;
    private int _currPlayerMovementTile = 0;
    
    private int _score = 0;
    private bool _hasSpecialAbility = false;

    public GameObject infoText;

    private Animator _animator;

    public static GameObject gameOverUI;
    public GameObject gameOverUINonPublic;

    public GameObject specialButton;

    public GameObject vidmo;
    public float dVidmoStarted;
    
    private bool attacking = false;
    private bool enemyNotDead = false;
    
    void Start()
    {
        _angle = 0;
        if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            _score = 23;
        }
        gameOverUI = gameOverUINonPublic;
        healthUI = _healthUI;
        totalShapes = transform.Find("Shapes").transform.childCount;
        _animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        specialButton.transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>().text = _score + "/20";

        if (vidmo.activeSelf)
        {
            dVidmoStarted += Time.deltaTime;
            if (dVidmoStarted >= 1.5f)
            {
                var e = vidmo.GetComponent<ParticleSystem>().emission;
                e.enabled = false;
            }
            if (dVidmoStarted >= 5f)
            {
                vidmo.SetActive(false);
                dVidmoStarted = 0;
            }
        }

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

/*
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
        }*/

       /* if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChooseSpecialAbility();
        }*/
        
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

            if (_pathToTake.Count == 1)
            {
                _playerMoving = false;
                _pathToTake.Clear(); 
                GameScript.Phase = 1;
            }

            else
            {
                Vector2Int c1 = _pathToTake[_pathToTake.Count - 1];
                if (_pathToTake.Count == 2 && _currPlayerMovementTile == 0 && GameScript.Tiles[c1.y][c1.x].hasEnemy && !attacking)
                {
                    attacking = true;
                    enemyNotDead = true;
                    Debug.Log("attacking2");
                    _animator.SetTrigger("attack");
                    _timeElapsed = 0f;
                }
                
                Vector3 worldPosition = new Vector3(_pathToTake[_currPlayerMovementTile].x * 2f - GameScript.Width + 1, 0f, _pathToTake[_currPlayerMovementTile].y * 2f - GameScript.Height + 1);
                Vector3 nextWorldPosition =
                    new Vector3(_pathToTake[_currPlayerMovementTile + 1].x * 2f - GameScript.Width + 1, 0f,
                        _pathToTake[_currPlayerMovementTile + 1].y * 2f - GameScript.Height + 1);
                transform.rotation = Quaternion.LookRotation(nextWorldPosition - worldPosition);
                float speed = Vector3.Distance(worldPosition, nextWorldPosition) / _movementDuration;


                if (attacking && _timeElapsed >= 1f && enemyNotDead)
                {
                    Vector2Int c = _pathToTake[_pathToTake.Count - 1];
                    TileScript tile = GameScript.Tiles[c.y][c.x];
                    if (tile.hasEnemy)
                    {
                        KillEnemy(tile);
                    }
                    enemyNotDead = false;
                }
                
                if (attacking && _timeElapsed >= 2f)
                {
                    Debug.Log("attack done");
                    _animator.SetFloat("runSpeed", 0);
                    attacking = false;
                    _timeElapsed = 0f;
                    
                } else if (attacking)
                {
                    _timeElapsed += Time.deltaTime;
                }
                
                if (_timeElapsed < _movementDuration && !attacking)
                {
                    _animator.SetFloat("runSpeed", speed / 10f);
                    transform.position = Vector3.Lerp(worldPosition, nextWorldPosition, _timeElapsed / _movementDuration);
                    _timeElapsed += Time.deltaTime;
                }
                
                else if (!attacking)
                {
                    _animator.SetFloat("runSpeed", speed / 10f);
                    Vector2Int c = _pathToTake[_pathToTake.Count - 1];
                    if (_currPlayerMovementTile == _pathToTake.Count - 3 && GameScript.Tiles[c.y][c.x].hasEnemy)
                    {
                        attacking = true;
                        enemyNotDead = true;
                        Debug.Log("attacking");
                        _animator.SetTrigger("attack");
                    }
                    worldPosition = nextWorldPosition;
                    transform.position = worldPosition;
                    _timeElapsed = 0f;
                    _currPlayerMovementTile++;
                    if (_currPlayerMovementTile == _pathToTake.Count-1)
                    {
                        _playerMoving = false;
                        _pathToTake.Clear();
                        _currPlayerMovementTile = 0;
                        GameScript.Phase = 1;
                    }
                }
            }
        }
    }

    public void SpawnPlayer(int x, int y)
    {
        GameScript.Tiles[y][x].hasPlayer = true;
        transform.position = new Vector3(x * 2f - GameScript.Width + 1, 0f, y * 2f - GameScript.Height + 1);
        PlayerTileCoords = new Vector2Int(x, y);
    }

    private void HandlePhaseTwo()
    {
        if (_score >= 20)
        {
            specialButton.GetComponent<Button>().interactable = true;
        }
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
                    if (_hasSpecialAbility)
                    {
                        _color = Color.green;
                    }
                    else
                    {
                        _color = IsNeighbouring(sc, hit) ? Color.green : Color.red;   
                    }

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

                        if (_hasSpecialAbility)
                        {
                            KillThemAll();
                            specialButton.GetComponent<Button>().interactable = false;
                            currShape = null;
                            _score -= 20;
                            gs.ToggleInventory();
                            GameScript.Phase = 1;
                        }
                        else
                        {
                            SwapShape();

                            currShape = null;
                            gs.ToggleInventory();
                            GameScript.Phase = 3;
                            MarkInitFog();
                            infoText.GetComponent<InfotextController>().UpdateInfoText("Enemies moving");
                            gs.MoveEnemies();   
                        }
                    }
                }
            }
            else if (hit.transform.CompareTag("Enemy"))
            {
                Debug.Log("here");
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
                _pathToTake.Add(ts.coords);
                ts = ts.cameFrom;
            }

            _playerMoving = true;
            var tile = GameScript.Tiles[y][x];
            tile.hasPlayer = true;
            PlayerTileCoords = new Vector2Int(x, y);
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
            if ((InBounds(PlayerTileCoords.x, PlayerTileCoords.y - 1) && y == PlayerTileCoords.y - 1 && x == PlayerTileCoords.x) ||
                (InBounds(PlayerTileCoords.x, PlayerTileCoords.y + 1) && y == PlayerTileCoords.y + 1 && x == PlayerTileCoords.x) ||
                (InBounds(PlayerTileCoords.x - 1, PlayerTileCoords.y) && y == PlayerTileCoords.y && x == PlayerTileCoords.x - 1) ||
                (InBounds(PlayerTileCoords.x + 1, PlayerTileCoords.y) && y == PlayerTileCoords.y && x == PlayerTileCoords.x + 1))
            {
                _color = Color.green;
                return true;
            }
        }

        return false;
    }

    public void MarkFog()
    {
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

    public void RefillHealth()
    {
        _health = 5;
        for (int i = 0; i < 5; i++)
        {
            healthUI.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void ResetScore()
    {
        _score = 0;
    }

    public static void HealthDown()
    {
        GameObject hp = healthUI.transform.GetChild(_health - 1).gameObject;
        _health -= 1;
        hp.gameObject.SetActive(false);

        if (_health <= 0)
        {
            gameOverUI.SetActive(true);
        }
    }

    private void IncreaseScore(EnemyScript.EnemyType type)
    {
        switch (type)
        {
            case EnemyScript.EnemyType.King: _score += 1; break;
            case EnemyScript.EnemyType.Knight: _score += 3; break;
            case EnemyScript.EnemyType.Rook: _score += 3; break;
            case EnemyScript.EnemyType.Bishop: _score += 3; break;
            case EnemyScript.EnemyType.Superman: _score += 5; break;
            // default: prefab = gs.kingPrefab; break;
        }
    }

    public void ChooseSpecialAbility()
    {
        currShape = transform.Find("Special").Find("Special").gameObject;
        _hasSpecialAbility = true;
    }

    public void SetHasSpecialAbility(bool set)
    {
        _hasSpecialAbility = set;
    }

    public void KillEnemy(TileScript tile)
    {
        tile.hasDeadEnemy = true;
        tile.hasEnemy = false;
        EnemyScript es = tile.enemy.GetComponent<EnemyScript>();
        es.Kill();
        GameScript.enemiesAlive.Remove(es);
        IncreaseScore(tile.enemy.GetComponent<EnemyScript>().type);
        // Destroy(tile.enemy);
        tile.enemy = null;
    }

    private void KillThemAll()
    {
        vidmo.SetActive(true);
        dVidmoStarted = 0;
        vidmo.transform.position = new Vector3(_currTileCoords.x * 2f - GameScript.Width + 1, 0.5f, _currTileCoords.y * 2f - GameScript.Height + 1);
        var e = vidmo.GetComponent<ParticleSystem>().emission;
        e.enabled = true;
        foreach (var shapePoint in _shapeToUse)
        {
            int y = _currTileCoords.y + shapePoint.y;
            int x = _currTileCoords.x + shapePoint.x;
            Debug.Log(x + " " + y);

            if (GameScript.Width <= x || GameScript.Height <= y || 0 > x || 0 > y)
            {
                continue;
            }

            var tile = GameScript.Tiles[y][x].GetComponent<TileScript>();
            if (tile.hasEnemy)
            {
                KillEnemy(tile);
            }
        }
    }

    public void clearShapeToUse()
    {
        _shapeToUse.Clear();
    }

    public void NewLevel()
    {
        for (int x = 0; x < GameScript.Width; x++)
        {
            for (int y = 0; y < GameScript.Height; y++)
            {
                GameScript.Tiles[y][x].SetFog(0);
                GameScript.Tiles[y][x].hasPlayer = false;
            }
        }
        SpawnPlayer(10, 0);
        GameScript.Tiles[0][10].SetFog(3);
    }
}

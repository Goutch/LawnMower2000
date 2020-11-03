using UnityEngine;
using UnityEngine.Tilemaps;


public class LawnMower : MonoBehaviour
{
    public enum Orientation
    {
        up = 0,
        right = 1,
        down = 2,
        left = 3,
    }

    public delegate void LawnMowerNextPositionEvent(Vector2Int nextPosition, Orientation orientation);

    public event LawnMowerNextPositionEvent OnReachedDestination;

    public delegate void LawnMowerHitWallEvent(Vector2Int position, Orientation orientation);

    public event LawnMowerHitWallEvent OnWallHit;

    public delegate void OnChangeNextTurnHander();
    public event OnChangeNextTurnHander OnChangeNextTurn;

    private int nextTurn = 0;
    public int NextTurn { get { return nextTurn; } set { nextTurn = value; if (value != 0) OnChangeNextTurn?.Invoke(); } }

    [SerializeField] private Transform front;
    [SerializeField] private Vector2Int nextTilePosition;
    [SerializeField] private GameManager gameManager;

    private Map map;
    private int points = 0;

    public bool Ready { get; set; } = false;

    private SpriteRenderer spriteRenderer = null;

    private Color color = Color.white;
    public Color Color { get { return color; } set { color = value; spriteRenderer.color = value; } }

    public Orientation OrientationLawnMower { get; set;}

    public int NumberOfTurn { get; set; } = 0;

    void OnEnable()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        gameManager.OnGameStart += OnGameStart;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void OnDisable()
    {
        gameManager.OnGameStart -= OnGameStart;
    }

    private void OnGameStart()
    {
        map = gameManager.GetMap();
        FindNextPosition();
        Mow();
    }

    void Update()
    {
        transform.rotation = Quaternion.AngleAxis((int)OrientationLawnMower * -90, Vector3.forward);

        if (gameManager.GameStarted)
        {
            //is in the middle of a tile
            if (map.WorldToGrid(transform.position) == nextTilePosition &&
                map.WorldToGrid(transform.position) != map.WorldToGrid(front.transform.position))
            {
                Mow();
                Turn();
                NextTurn = 0;
                OnReachedDestination?.Invoke(nextTilePosition, OrientationLawnMower);
            }

            //move if does not it wall
            Vector2Int frontPosition = map.WorldToGrid(front.transform.position);
            if (map.GetTile(frontPosition) != Map.TileType.Wall)
            {
                transform.Translate(Vector3.up * Time.deltaTime);
            }
            //turn if hit wall
            else if (NextTurn != 0)
            {
                Turn();
                NextTurn = 0;
            }
            //notify wall was hit to change tragectory
            else
            {
                OnWallHit?.Invoke(map.WorldToGrid(transform.position), OrientationLawnMower);
            }
        }
    }

    private void Mow()
    {
        Vector2Int gridPosition = map.WorldToGrid(transform.position);
        if (map.GetTile(gridPosition) == Map.TileType.Grass)
        {
            points++;
            map.SetTile(gridPosition.x, gridPosition.y, Map.TileType.Dirt);
        }
    }

    private void FindNextPosition()
    {
        nextTilePosition = map.WorldToGrid(transform.position);
        switch (OrientationLawnMower)
        {
            case Orientation.down:
                nextTilePosition.y -= 1;
                break;
            case Orientation.up:
                nextTilePosition.y += 1;
                break;
            case Orientation.right:
                nextTilePosition.x += 1;
                break;
            case Orientation.left:
                nextTilePosition.x -= 1;
                break;
        }
    }

    private void Turn()
    {
        NumberOfTurn++;
        OrientationLawnMower = (Orientation)(((int)OrientationLawnMower + NextTurn) % 4);
        FindNextPosition();
    }

    public int GetPoints()
    {
        return points;
    }

    public void SetNextTurn(int turn)
    {
        if (turn < 0)
            turn = 4 + (turn % 4);
        NextTurn = turn;
    }


}
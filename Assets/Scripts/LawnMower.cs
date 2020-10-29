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

    public delegate void LawnMowerNextPositionEvent(Vector2Int nextPosition,Orientation orientation);

    public event LawnMowerNextPositionEvent OnReachedDestination;

    public delegate void LawnMowerHitWallEvent(Vector2Int position,Orientation orientation);

    public event LawnMowerHitWallEvent OnWallHit;
    [SerializeField]private Transform front;
    [SerializeField]private Orientation orientation;
    [SerializeField]private int nextTurn = 0;
    [SerializeField]private Vector2Int nextTilePosition;
    [SerializeField]private GameManager gameManager;

    private Map map;
    private int points = 0;

    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        map = gameManager.GetMap();
        FindNextPosition();
        Mow();
    }

    public void SetMap(Map map)
    {
        this.map = map;
    }

    void Update()
    {
        //is in the middle of a tile
        if (map.WorldToGrid(transform.position) == nextTilePosition &&
            map.WorldToGrid(transform.position) != map.WorldToGrid(front.transform.position))
        {
            Mow();
            Turn();
            nextTurn = 0;
            OnReachedDestination?.Invoke(nextTilePosition,orientation);
        }

        //move if does not it wall
        Vector2Int frontPosition = map.WorldToGrid(front.transform.position);
        if (map.GetTile(frontPosition) != Map.TileType.Wall)
        {
            transform.Translate(Vector3.up * Time.deltaTime);
        }
        //turn if hit wall
        else if (nextTurn != 0)
        {
            Turn();
        }
        //notify wall was hit to change tragectory
        else
        {
            OnWallHit?.Invoke(map.WorldToGrid(transform.position),orientation);
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
        switch (orientation)
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
        orientation = (Orientation) (((int) orientation + nextTurn) % 4);
        transform.Rotate(Vector3.forward, -90 * nextTurn);
        FindNextPosition();
    }

    public int GetPoints()
    {
        return points;
    }

    public void SetNextTurn(int turn)
    {
        if (turn <0)
            turn = 4 + (turn%4);
        nextTurn = turn;
    }
}
using System;
using UnityEngine;
using UnityEngine.EventSystems;


public class LawnMower : MonoBehaviour
{
    enum Orientation
    {
        up = 0,
        right = 1,
        down = 2,
        left = 3,
    }

    [SerializeField]private Orientation orientation;
    [SerializeField]private int nextTurn = 0;
    [SerializeField]private Vector2Int nextTilePosition;
    private Map map;
    private int points=0;
    [SerializeField] private Transform front;

    private void Start()
    {
        FindNextPosition();
        Mow();
    }

    public void SetMap(Map map)
    {
        this.map = map;
    }

    void Update()
    {
        if (map.WorldToGrid(transform.position) == nextTilePosition &&
            map.WorldToGrid(transform.position) != map.WorldToGrid(front.transform.position))
        {
            Turn();
            nextTurn = 0;
            Mow();
        }
        
        Vector2Int frontPosition = map.WorldToGrid(front.transform.position);
        if (map.GetTile(frontPosition) != Map.TileType.Wall)
            transform.Translate(Vector3.up * Time.deltaTime);
        else if(nextTurn!=0)
        {
            Turn();
            nextTurn = 0;
        }
    }

    private void Mow()
    {
       // Debug.Log("MOW");
        Vector2Int gridPosition = map.WorldToGrid(transform.position);
        if (map.GetTile(gridPosition)==Map.TileType.Grass)
        {
            points++;
            map.SetTile(gridPosition.x,gridPosition.y,Map.TileType.Dirt);
        }
    }
    private void FindNextPosition()
    {
        Debug.Log("FindNext");
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

    public void SetNextTurn(int turn)
    {
        if (turn == -1)
            turn = 3;
        nextTurn = turn;
    }
}
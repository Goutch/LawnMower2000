using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Map : MonoBehaviour
{
    public enum TileType
    {
        Dirt = 0,
        Grass = 1,
        Wall = 2,
    }

    [SerializeField] private TileBase[] tiles;
    private GameManager gameManager;
    private Options options;
    private TileType[,] map;
    private List<Vector2Int> possibleSpawnPoint;
    private Tilemap tileMap;
    private Grid grid;
    private int sizeX;
    private int sizeY;
    private int seed;
    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        options = gameManager.GetComponent<Options>();

        tileMap = GetComponentInChildren<Tilemap>();
        grid = GetComponent<Grid>();

        Camera.main.transform.position = new Vector3(sizeX / 2f, sizeY / 2f, -1);
        Camera.main.orthographicSize = (tileMap.localBounds.max.y - tileMap.localBounds.min.y) / 2f;
    }

    public void Init(int seed)
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        options = gameManager.GetComponent<Options>();

        tileMap = GetComponentInChildren<Tilemap>();
        grid = GetComponent<Grid>();
        Random.InitState(seed);
        GenerateRandomMap(options.MapSize.x, options.MapSize.y);
        tileMap.CompressBounds();
    }

    public int GetSeed()
    {
        return seed;
    }
    
    private void GenerateRandomMap(int sizeX, int sizeY)
    {

        possibleSpawnPoint = new List<Vector2Int>();
        this.sizeX = sizeX;
        this.sizeY = sizeX;
        map = new TileType[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                if (x == 0 || y == 0 || x == sizeX - 1 || y == sizeY - 1)
                {
                    SetTile(x, y, TileType.Wall);
                }
                else
                {
                    if (Random.Range(0, 10) <= 0)
                    {
                        SetTile(x, y, TileType.Wall);
                    }
                    else
                    {
                        SetTile(x, y, TileType.Grass);
                        possibleSpawnPoint.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
    }

    public void SetTile(int x, int y, TileType type)
    {
        map[x, y] = type;

        tileMap.SetTile(new Vector3Int(x, y, 0), tiles[(int) type]);
    }

    public bool ContainPosition(Vector2Int gridPosition)
    {
        return tileMap.cellBounds.Contains(new Vector3Int(gridPosition.x, gridPosition.y, 0));
    }

    public TileType GetTile(Vector2Int gridPosition)
    {
        return map[gridPosition.x, gridPosition.y];
    }

    public bool ContainPosition(Vector3 worldPosition)
    {
        return tileMap.localBounds.Contains(worldPosition);
    }

    public TileType GetTile(Vector3 worldPositon)
    {
        return GetTile(WorldToGrid(worldPositon));
    }

    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        Vector3 worldPos = grid.CellToWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));
        worldPos.x += 0.5f;
        worldPos.y += 0.5f;
        return worldPos;
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        Vector3Int gridPosition = grid.WorldToCell(worldPosition);
        return new Vector2Int(gridPosition.x, gridPosition.y);
    }

    public Vector3 GetSpawnPoint()
    {
        return GridToWorld(possibleSpawnPoint[Random.Range(0, possibleSpawnPoint.Count)]);
    }
}
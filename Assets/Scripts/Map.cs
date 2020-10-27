using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Map : MonoBehaviour
{
    enum TileType
    {
        Dirt = 0,
        Grass = 1,
        Wall = 2,
    }

    [SerializeField] private TileBase[] tiles;
    private GameManager gameManager;
    private Options options;
    private TileType[,] map;
    private Tilemap tileMap;
    private Grid grid;

    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        options = gameManager.GetComponent<Options>();
        tileMap = GetComponentInChildren<Tilemap>();
        grid = GetComponent<Grid>();
        GenerateRandom(options.MapSize.x, options.MapSize.y);
        tileMap.CompressBounds();
        Camera.main.transform.position = new Vector3(options.MapSize.x/2f, options.MapSize.y/2f, -1);
        Camera.main.orthographicSize = (tileMap.localBounds.max.y - tileMap.localBounds.min.y)/2f;
    }

    private void GenerateRandom(int sizeX, int sizeY)
    {
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
                    if (Random.Range(0, 10) <= 1)
                    {
                        SetTile(x, y, TileType.Wall);
                    }
                    else
                    {
                        SetTile(x, y, TileType.Grass);
                    }
                }
            }
        }

    }

    private void SetTile(int x, int y, TileType type)
    {
        map[x, y] = type;

        tileMap.SetTile(new Vector3Int(x, y, 0), tiles[(int) type]);
    }
}
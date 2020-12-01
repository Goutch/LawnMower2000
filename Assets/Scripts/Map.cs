using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class Map : MonoBehaviour
{
    public enum TileType
    {
        Dirt = 0,
        Grass = 1,
        Rock = 2,
    }
    
    [SerializeField] private ComputeShader maskCompute;
    [SerializeField] private Color32 grassColor1 = new Color32(255, 255, 255, 255);
    [SerializeField] private Color32 grassColor2 = new Color32(255, 255, 255, 255);
    [SerializeField] private Color32 rockColor = new Color32(255, 255, 255, 255);
    [SerializeField] private Color32 dirtColor = new Color32(255, 255, 255, 255);
    [SerializeField] private FilterMode filerMode;
    [SerializeField] private int pixelsPerUnits = 16;


    public int PixelsPerUnits => pixelsPerUnits;

    private TileType[,] map;
    private GameManager gameManager;
    private Options options;
    private List<Vector2Int> possibleSpawnPoint;

    private MeshRenderer[] meshRenderers = new MeshRenderer[3];

    private MeshFilter[] meshFilters = new MeshFilter[3];

    private Texture2D dirtTexture;
    private Texture2D grassTexture;
    private Texture2D grassMaskTexture;
    private Texture2D rockTexture;
    private Mesh quad;
    private Grid grid;
    private int sizeX;
    private int sizeY;
    private int seed;
    float noiseOffset;

    public void OnEnable()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        options = gameManager.GetComponent<Options>();
        sizeX = options.MapSize.x;
        sizeY = options.MapSize.y;
        grid = GetComponent<Grid>();

        Camera.main.transform.position = new Vector3(sizeX / 2f, sizeY / 2f, -10);
        Camera.main.orthographicSize = options.MapSize.y / 2f;
        quad = GeometryUtils.CreateQuad(sizeX, sizeY, false);

        for (int i = 0; i < 3; i++)
        {
            meshFilters[i] = new GameObject().AddComponent<MeshFilter>();
            meshRenderers[i] = meshFilters[i].gameObject.AddComponent<MeshRenderer>();
            meshRenderers[i].shadowCastingMode = ShadowCastingMode.Off;
            meshRenderers[i].material = new Material(Shader.Find("Custom/MapShader"));
            meshFilters[i].mesh = quad;
            meshFilters[i].transform.parent = transform;
        }

        meshFilters[0].transform.position = new Vector3(0, 0, 2);//behind grass(z=1) and lawnmowers(z=0)
        meshFilters[1].transform.position = new Vector3(0, 0, 1);//behind lawnmowers(z=0)
        meshFilters[2].transform.position = new Vector3(0, 0, -1);//front of lawnmowers(z=0)
    }

    public void Generate(int seed)
    {
        this.seed = seed;

        Random.InitState(seed);

        dirtTexture = new Texture2D(sizeX * pixelsPerUnits, sizeY * pixelsPerUnits, TextureFormat.RGBA32, false);
        grassTexture = new Texture2D(sizeX * pixelsPerUnits, sizeY * pixelsPerUnits, TextureFormat.RGBA32, false);
        grassMaskTexture = new Texture2D(sizeX * pixelsPerUnits, sizeY * pixelsPerUnits, TextureFormat.RGBA32, false);
        rockTexture = new Texture2D(sizeX * pixelsPerUnits, sizeY * pixelsPerUnits, TextureFormat.RGBA32, false);

        dirtTexture.filterMode = filerMode;
        grassTexture.filterMode = filerMode;
        grassMaskTexture.filterMode = filerMode;
        rockTexture.filterMode = filerMode;

        meshRenderers[0].material.mainTexture = dirtTexture;
        meshRenderers[1].material.mainTexture = grassTexture;
        meshRenderers[1].material.SetTexture("_AlphaTex", grassMaskTexture);
        meshRenderers[1].material.SetFloat("_AlphaTexEnabled", 1.0f);
        meshRenderers[2].material.mainTexture = rockTexture;

        FillMap();

        dirtTexture.Apply(false, false);
        grassTexture.Apply(false, false);
        grassMaskTexture.Apply(false, false);
        rockTexture.Apply(false, false);
    }

    public Color SampleMap(Vector3 worldPosition)
    {
        if (ContainPosition(WorldToGrid(worldPosition)))
        {
            worldPosition *= PixelsPerUnits;
            return grassMaskTexture.GetPixel((int) worldPosition.x, (int) worldPosition.y).a < .5
                ? dirtTexture.GetPixel((int) worldPosition.x, (int) worldPosition.y)
                : grassTexture.GetPixel((int) worldPosition.x, (int) worldPosition.y);
        }

        return new Color(1, 1, 1, 1);
    }

    public int GetSeed()
    {
        return seed;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            MowMap(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    public void MowMap(Vector3 position)
    {
        position *= pixelsPerUnits;
        int kernelHandle = maskCompute.FindKernel("CSMain");
        RenderTexture tex = new RenderTexture(grassMaskTexture.width, grassMaskTexture.height, 24);
        tex.enableRandomWrite = true;
        tex.Create();

        maskCompute.SetTexture(kernelHandle, "Result", tex);
        maskCompute.SetTexture(kernelHandle, "ImageInput", grassMaskTexture);
        maskCompute.SetFloat("Range", (pixelsPerUnits / 2.0f) + 0.1f);
        maskCompute.SetFloats("Position", new float[2] {position.x, position.y});
        maskCompute.Dispatch(kernelHandle, grassMaskTexture.width / 8, grassMaskTexture.height / 8, 1);

        RenderTexture.active = tex;
        grassMaskTexture.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        RenderTexture.active = null;
        grassMaskTexture.Apply();
    }

    private byte ClampByte(int value)
    {
        return (byte) Math.Min(Math.Max(value, 0), 255);
    }

    private float RockNoise(float x, float y)
    {
        return Mathf.PerlinNoise(noiseOffset + 0.01f + x * 0.5f, noiseOffset + 0.5f + y * 0.5f);
    }

    private float GetDistanceToClosestRock(Vector3 worldPosition)
    {
        Vector2Int gridPosition = new Vector2Int(
            (int) Mathf.Floor(worldPosition.x),
            (int) Mathf.Floor(worldPosition.y));

        if (GetTile(worldPosition) == TileType.Rock)
        {
            return Vector2.Distance(
                new Vector2((gridPosition.x * pixelsPerUnits) + (pixelsPerUnits / 2.0f), (gridPosition.y * pixelsPerUnits) + (pixelsPerUnits / 2.0f)),
                new Vector2(worldPosition.x * pixelsPerUnits, worldPosition.y * pixelsPerUnits));
        }
        else
        {
            float[] distanceAdjacents = new float[8]
            {
                float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue,
                float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue
            };
            int count = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (!(i == 0 && j == 0))
                    {
                        Vector2Int pos = new Vector2Int(gridPosition.x + j, gridPosition.y + i);
                        if (GetTile(pos) == TileType.Rock)
                        {
                            distanceAdjacents[count] = Vector2.Distance(
                                new Vector2((pos.x * pixelsPerUnits) + (pixelsPerUnits / 2.0f), (pos.y * pixelsPerUnits) + (pixelsPerUnits / 2.0f)),
                                new Vector2(worldPosition.x * pixelsPerUnits, worldPosition.y * pixelsPerUnits));
                        }

                        count++;
                    }
                }
            }

            float min = float.MaxValue;
            for (int i = 0; i < 8; i++)
            {
                min = Mathf.Min(distanceAdjacents[i], min);
            }

            return min;
        }
    }

    public void Reset()
    {
        int count = 0;
        possibleSpawnPoint = new List<Vector2Int>();
        map = new TileType[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                if (x == 0 || y == 0 || x == sizeX - 1 || y == sizeY - 1)
                {
                    SetTile(x, y, TileType.Rock);
                }
                else
                {
                    float noise = RockNoise(0.5f + x, 0.5f + y);
                    if (noise >= 0.7f)
                    {
                        SetTile(x, y, TileType.Rock);
                    }
                    else
                    {
                        SetTile(x, y, TileType.Grass);
                        possibleSpawnPoint.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        Color32[] grassMaskPixels = new Color32[(sizeX * pixelsPerUnits) * (sizeY * pixelsPerUnits)];
        for (int y = 0; y < sizeY * pixelsPerUnits; y++)
        {
            for (int x = 0; x < sizeX * pixelsPerUnits; x++)
            {
                grassMaskPixels[count] = new Color(0, 0, 0, 255);
                count++;
            }
        }

        grassMaskTexture.SetPixels32(grassMaskPixels);
        grassMaskTexture.Apply();
    }

    private void FillMap()
    {
        noiseOffset = Random.Range(0.0f, 10000.0f);
        //***************TILE_MAP***************
        possibleSpawnPoint = new List<Vector2Int>();
        map = new TileType[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                if (x == 0 || y == 0 || x == sizeX - 1 || y == sizeY - 1)
                {
                    SetTile(x, y, TileType.Rock);
                }
                else
                {
                    float noise = RockNoise(0.5f + x, 0.5f + y);
                    if (noise >= 0.7f)
                    {
                        SetTile(x, y, TileType.Rock);
                    }
                    else
                    {
                        SetTile(x, y, TileType.Grass);
                        possibleSpawnPoint.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        Color32[] dirtPixels = new Color32[(sizeX * pixelsPerUnits) * (sizeY * pixelsPerUnits)];
        Color32[] grassPixels = new Color32[(sizeX * pixelsPerUnits) * (sizeY * pixelsPerUnits)];
        Color32[] grassMaskPixels = new Color32[(sizeX * pixelsPerUnits) * (sizeY * pixelsPerUnits)];
        Color32[] rockPixels = new Color32[(sizeX * pixelsPerUnits) * (sizeY * pixelsPerUnits)];

        int count = 0;
        for (int y = 0; y < sizeY * pixelsPerUnits; y++)
        {
            for (int x = 0; x < sizeX * pixelsPerUnits; x++)
            {
                //**************DIRT*********************
                int dirtShift = Random.Range(-10, 10);
                int grassShift = Random.Range(-5, 5);
                dirtPixels[count] = new Color32(ClampByte(dirtColor.r + dirtShift),
                    ClampByte(dirtColor.g + dirtShift),
                    ClampByte(dirtColor.b + dirtShift),
                    dirtColor.a);
                //**************GRASS*********************
                Color32 grassColor = Color32.Lerp(grassColor1, grassColor2, Mathf.PerlinNoise(noiseOffset + x * 0.01f + 0.5f, noiseOffset + y * 0.01f + 0.5f));
                grassPixels[count] = new Color32(
                    ClampByte(grassColor.r + grassShift),
                    ClampByte(grassColor.g + grassShift),
                    ClampByte(grassColor.b + grassShift),
                    grassColor.a);

                //**************ROCKS*********************
                Vector2Int gridPosition = new Vector2Int((int) Mathf.Floor(x / pixelsPerUnits), (int) Mathf.Floor(y / pixelsPerUnits));


                int rockShift = Random.Range(-10, 10);
                float distToClosestRock = GetDistanceToClosestRock(new Vector3(x / (float) pixelsPerUnits, y / (float) pixelsPerUnits, 0));
                if (gridPosition.x == 0 || gridPosition.y == 0 || gridPosition.x == sizeX - 1 || gridPosition.y == sizeY - 1)
                {
                    rockPixels[count] = new Color32(
                        ClampByte(rockColor.r + rockShift),
                        ClampByte(rockColor.b + rockShift),
                        ClampByte(rockColor.g + rockShift),
                        rockColor.a);
                }
                else if (distToClosestRock <= pixelsPerUnits)
                {
                    float noise = RockNoise(x / (float) pixelsPerUnits, y / (float) pixelsPerUnits);
                    float distRatio = distToClosestRock / pixelsPerUnits;
                    noise *= Mathf.SmoothStep(2, 0, distRatio);

                    if (noise >= 0.7)
                    {
                        float t = 1 - ((noise - 0.7f) / .3f);
                        rockShift += (int) Mathf.Lerp(-50, 30, t);
                        rockPixels[count] = new Color32(
                            ClampByte(rockColor.r - (int) rockShift),
                            ClampByte(rockColor.b - (int) rockShift),
                            ClampByte(rockColor.g - (int) rockShift),
                            rockColor.a);
                    }
                    else
                    {
                        rockPixels[count] = new Color32(0, 0, 0, 0);
                    }
                }
                else
                {
                    rockPixels[count] = new Color32(0, 0, 0, 0);
                }


                //**************MASK*********************
                grassMaskPixels[count] = new Color(0, 0, 0, 255);
                count++;
            }
        }

        //**************FLOWERS*********************
        for (int x = 0; x < 50; x++)
        {
            int pos = Random.Range(0, grassPixels.Length - 1);

            grassPixels[pos] = new Color32(100, 100, 0, 255);
            if (pos < grassPixels.Length - 1)
                grassPixels[pos + 1] = new Color32(200, 200, 0, 255);
            if (pos > 0)
                grassPixels[pos - 1] = new Color32(200, 200, 0, 255);
            if (pos < grassPixels.Length - 1 - grassTexture.width)
                grassPixels[pos + grassTexture.width] = new Color32(200, 200, 0, 255);
            if (pos > grassTexture.width)
                grassPixels[pos - grassTexture.width] = new Color32(200, 200, 0, 255);
        }

        for (int x = 0; x < 50; x++)
        {
            int pos = Random.Range(0, grassPixels.Length - 1);

            grassPixels[pos] = new Color32(200, 50, 0, 255);
            if (pos < grassPixels.Length - 1)
                grassPixels[pos + 1] = new Color32(255, 50, 0, 255);
            if (pos > 0)
                grassPixels[pos - 1] = new Color32(255, 50, 0, 255);
            if (pos < grassPixels.Length - 1 - grassTexture.width)
                grassPixels[pos + grassTexture.width] = new Color32(255, 50, 0, 255);
            if (pos > grassTexture.width)
                grassPixels[pos - grassTexture.width] = new Color32(255, 50, 0, 255);
        }


        dirtTexture.SetPixels32(dirtPixels);
        grassTexture.SetPixels32(grassPixels);
        grassMaskTexture.SetPixels32(grassMaskPixels);
        rockTexture.SetPixels32(rockPixels);
    }

    public void SetTile(int x, int y, TileType type)
    {
        map[x, y] = type;
    }

    public bool ContainPosition(Vector2Int gridPosition)
    {
        return gridPosition.x <= options.MapSize.x &&
               gridPosition.x >= 0 &&
               gridPosition.y <= options.MapSize.y &&
               gridPosition.y >= 0;
    }

    public TileType GetTile(Vector2Int gridPosition)
    {
        return map[gridPosition.x, gridPosition.y];
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
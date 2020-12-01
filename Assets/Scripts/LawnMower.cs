using System.Collections;
using UnityEngine;

public class LawnMower : MonoBehaviour
{
    public enum Orientation
    {
        up = 0,
        right = 1,
        down = 2,
        left = 3,
    }

    private Vector3[] OrientationVector = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    #region Event 
    public delegate void LawnMowerNextPositionEvent(Vector2Int nextPosition, Orientation orientation);
    public event LawnMowerNextPositionEvent OnReachedDestination;
    #endregion 

    #region SerializedField
    [SerializeField] private float Speed = 1;

    #endregion

    #region private variable
    private SpriteRenderer spriteRenderer = null;
    private GameManager gameManager = null;

    private Vector3 Destination;
    
    #endregion

    #region Attribut
    private Color color = Color.white;
    public Color Color
    {
        get { return color; }
        set
        {
            color = value;
            spriteRenderer.color = value;
        }
    }

    public int Points { get; set; }

    private int nextTurn = 0;
    public int NextTurn { get { return nextTurn; } set {if (value < 0)nextTurn = 4 + (value % 4); else nextTurn = value;}}

    public Orientation OrientationLawnMower { get; set; } = 0;

    public bool Mowing { private set;  get; } = false;
    public bool IsStuck { set; get; } = false;
    #endregion Attribut

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        gameManager.OnGameStart += GameManager_OnGameStart;
        gameManager.OnGameFinish += GameManager_OnGameFinish;
    }

    private void OnDisable()
    {
        gameManager.OnGameStart -= GameManager_OnGameStart;
        gameManager.OnGameFinish -= GameManager_OnGameFinish;
    }


    private void GameManager_OnGameFinish()
    {
        Mowing = false;
    }

    private void GameManager_OnGameStart()
    {
        GetPoint();
        Mowing = true;
        Vector2Int currentTile = gameManager.Map.WorldToGrid(transform.position);
        Destination = FindDestination();
        StartCoroutine(MowMapRoutine());
    }

    IEnumerator MowMapRoutine()
    {
        while (Mowing)
        {
            yield return new WaitForSeconds(0.1f);
            gameManager.Map.MowMap(transform.position);
        }
    }

    private void Update()
    {
        if(gameManager.GameInProgress)
        {
            transform.rotation = Quaternion.AngleAxis((int)OrientationLawnMower * -90, Vector3.forward);

            float distance = Vector3.Distance(Destination, transform.position);
            if (Time.deltaTime * Speed < distance)
            {
                transform.Translate(Vector3.up * Time.deltaTime);
            }
            else
            {
                GetPoint();
                transform.position = Destination;

                Vector3 newDestination = FindDestination();

                if(Destination == newDestination)
                {
                    IsStuck = true;
                }
                else
                {
                    IsStuck = false;
                }

                Destination = newDestination;
                OnReachedDestination?.Invoke(new Vector2Int((int)Destination.x, (int)Destination.y), OrientationLawnMower);
            }
        }
    }

    private Vector3 FindDestination()
    {
        Vector2Int currentTile = gameManager.Map.WorldToGrid(transform.position);
        Vector3 currentTileCenter = new Vector3(currentTile.x + 0.5f, currentTile.y + 0.5f, 0);

        OrientationLawnMower = (Orientation)(((int)OrientationLawnMower + NextTurn) % 4);
        Vector3 nextDestination = currentTileCenter + OrientationVector[(int)OrientationLawnMower];
        NextTurn = 0;

        if (gameManager.Map.GetTile(nextDestination) == Map.TileType.Rock)
        {
            return currentTileCenter;
        }
        
        return nextDestination;
    }

    private void GetPoint()
    {
        Vector2Int gridPosition = gameManager.Map.WorldToGrid(transform.position);
        if (gameManager.Map.GetTile(gridPosition) == Map.TileType.Grass)
        {
            Points++;
            gameManager.Map.SetTile(gridPosition.x, gridPosition.y, Map.TileType.Dirt);
        }
    }
}
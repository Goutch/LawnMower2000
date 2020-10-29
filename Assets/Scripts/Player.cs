using UnityEngine;

public class Player : MonoBehaviour
{
    private LawnMower lawnMower;
    private Options options;

    void Start()
    {
        options = GameObject.FindWithTag("GameManager").GetComponent<Options>();
        lawnMower = GetComponent<LawnMower>();
    }
    
    void Update()
    {
        int turn = 0;
        turn -= Input.GetKeyDown(options.TurnLeftKey) ? 1 : 0;
        turn += Input.GetKeyDown(options.TurnRightKey) ? 1 : 0;
        if (turn != 0)
        {
            lawnMower.SetNextTurn(turn);
        }
        if (Input.GetKeyDown(options.ContinueKey))
        {
            lawnMower.SetNextTurn(0);
        }
    }
}
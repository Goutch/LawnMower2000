using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private LawnMower lawnMower;
    private Options options;
    private GameManager gameManager;
    private bool readPlayerInputs;
    private bool hasResetStick = false;
    private bool hasResetCross = false;

    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        options = gameManager.GetComponent<Options>();
        lawnMower = GetComponent<LawnMower>();
        readPlayerInputs = !gameManager.IsMenuOpen();
        Camera.main.GetComponent<CameraMovement>().ObjectToFollow = gameObject.transform;
    }

    void Update()
    {
        if (readPlayerInputs)
        {
            int turn = 0;
            
            hasResetStick = hasResetStick || (Mathf.Abs(Input.GetAxis("MoveHorizontal")) < 0.1);
            turn -= (Input.GetAxis("MoveHorizontal") == -1 && hasResetStick) ? 1 : 0;
            turn += (Input.GetAxis("MoveHorizontal") == 1 && hasResetStick) ? 1 : 0;

            hasResetCross = hasResetCross || Math.Abs(Input.GetAxis("MoveHorizontalCross")) < 0.1;
            turn -= (Input.GetAxis("MoveHorizontalCross") == -1 && hasResetCross) ? 1 : 0;
            turn += (Input.GetAxis("MoveHorizontalCross") == 1 && hasResetCross) ? 1 : 0;

            turn -= Input.GetKeyDown(options.TurnLeftKey) ? 1 : 0;
            turn += Input.GetKeyDown(options.TurnRightKey) ? 1 : 0;
            
            if (turn != 0)
            {
                lawnMower.SetNextTurn(turn);
                hasResetStick = false;
                hasResetCross = false;
            }

            if (Input.GetKeyDown(options.ContinueKey) || Input.GetButton("Continue") || Input.GetAxis("MoveVerticalCross")>0)
            {
                lawnMower.SetNextTurn(0);
            }

            if (Input.GetKeyDown(options.ReadyKey) || Input.GetButton("Start"))
            {
                lawnMower.Ready = true;
            }
            
        }
    }

    public void TogglePlayerInputs(bool enabled)
    {
        readPlayerInputs = !enabled;
    }
}
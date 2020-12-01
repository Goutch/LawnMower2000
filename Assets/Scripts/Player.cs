using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private LawnMower lawnMower = null;
    private GameManager gameManager = null;
    private bool hasResetStick = false;
    private bool hasResetCross = false;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        lawnMower = GetComponent<LawnMower>();
    }

    private void Update()
    {
        int turn = 0;

        hasResetStick = hasResetStick || (Mathf.Abs(Input.GetAxis("MoveHorizontal")) < 0.1);
        turn -= (Input.GetAxis("MoveHorizontal") == -1 && hasResetStick) ? 1 : 0;
        turn += (Input.GetAxis("MoveHorizontal") == 1 && hasResetStick) ? 1 : 0;

        hasResetCross = hasResetCross || Math.Abs(Input.GetAxis("MoveHorizontalCross")) < 0.1;
        turn -= (Input.GetAxis("MoveHorizontalCross") == -1 && hasResetCross) ? 1 : 0;
        turn += (Input.GetAxis("MoveHorizontalCross") == 1 && hasResetCross) ? 1 : 0;

        turn -= Input.GetKeyDown(gameManager.Options.Controls.left) ? 1 : 0;
        turn += Input.GetKeyDown(gameManager.Options.Controls.right) ? 1 : 0;

        if (turn != 0)
        {
            lawnMower.NextTurn = turn;
            hasResetStick = false;
            hasResetCross = false;
        }

        if (Input.GetKeyDown(gameManager.Options.Controls.foward) || Input.GetButton("Continue") || Input.GetAxis("MoveVerticalCross") > 0)
        {
            lawnMower.NextTurn = 0;
        }
    }
}
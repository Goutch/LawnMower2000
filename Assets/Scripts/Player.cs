﻿using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private LawnMower lawnMower;
    private Options options;
    private GameManager gameManager;
    private float lastTurned = 0;
    private bool readPlayerInputs;

    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        options = gameManager.GetComponent<Options>();
        lawnMower = GetComponent<LawnMower>();
        Camera.current.GetComponent<CameraMovement>().ObjectToFollow = gameObject.transform;
        readPlayerInputs = !gameManager.IsMenuOpen();
    }

    void Update()
    {
        if (readPlayerInputs)
        {
            int turn = 0;
            turn -= Input.GetKeyDown(options.TurnLeftKey) ? 1 : 0;
            turn -= (Input.GetAxis("MoveHorizontal") == -1) ? 1 : 0;
            turn += Input.GetKeyDown(options.TurnRightKey) ? 1 : 0;
            turn += (Input.GetAxis("MoveHorizontal") == 1) ? 1 : 0;
            if (turn != 0)
            {
                lawnMower.SetNextTurn(turn);
                lastTurned = Time.time;
            }

            if (Input.GetKeyDown(options.ContinueKey) || Input.GetButton("Continue"))
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
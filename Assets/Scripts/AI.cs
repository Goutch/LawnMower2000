﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AI : MonoBehaviour
{
    private LawnMower lawnMower = null;
    private GameManager gameManager = null;

    class Node
    {
        public Vector2Int position;
        public Node parent;
        public LawnMower.Orientation orientation;

        public Node(Vector2Int position, Node parent, LawnMower.Orientation orientation)
        {
            this.parent = parent;
            this.position = position;
            this.orientation = orientation;
        }
    }

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        lawnMower = GetComponent<LawnMower>();
    }

    private void OnEnable()
    {
        lawnMower.OnReachedDestination += OnReachedDestination;
    }
    private void OnDisable()
    {
        lawnMower.OnReachedDestination -= OnReachedDestination;
    }

    private void OnReachedDestination(Vector2Int position, LawnMower.Orientation orientation)
    {
        int next = FindNextMove(position, orientation);
        lawnMower.NextTurn = next;
    }

    private int FindNextMove(Vector2Int position, LawnMower.Orientation orientation)
    {
        HashSet<Vector2Int> closed = new HashSet<Vector2Int>();
        Queue<Node> open = new Queue<Node>();
        Node current;
        open.Enqueue(new Node(position, null, orientation));
        Node destination = null;
        while (open.Count != 0)
        {
            current = open.Dequeue();
            closed.Add(current.position);
            if (current.position != position && gameManager.Map.GetTile(current.position) == Map.TileType.Grass)
            {
                destination = current;
                break;
            }

            Vector2Int front = current.position;
            Vector2Int left = current.position;
            Vector2Int right = current.position;
            switch (current.orientation)
            {
                case LawnMower.Orientation.down:
                    front.y -= 1;
                    right.x -= 1;
                    left.x += 1;
                    break;
                case LawnMower.Orientation.up:
                    front.y += 1;
                    right.x += 1;
                    left.x -= 1;
                    break;
                case LawnMower.Orientation.right:
                    front.x += 1;
                    right.y -= 1;
                    left.y += 1;
                    break;

                case LawnMower.Orientation.left:
                    front.x -= 1;
                    right.y += 1;
                    left.y -= 1;
                    break;
            }

            if (gameManager.Map.ContainPosition(front) && gameManager.Map.GetTile(front) != Map.TileType.Rock && !closed.Contains(front))
                open.Enqueue(new Node(front, current, orientation));
            if (gameManager.Map.ContainPosition(right) && gameManager.Map.GetTile(right) != Map.TileType.Rock && !closed.Contains(right))
                open.Enqueue(new Node(right, current, (LawnMower.Orientation)(((int)orientation + 1) % 4)));
            if (gameManager.Map.ContainPosition(left) && gameManager.Map.GetTile(left) != Map.TileType.Rock && !closed.Contains(left))
                open.Enqueue(new Node(left, current, (LawnMower.Orientation)(((int)orientation + 3) % 4)));
        }

        if (destination != null)
        {
            current = destination;
            while (current.parent.position != position)
            {
                current = current.parent;
            }

            return current.orientation - current.parent.orientation;
        }

        return 2;
    }
}
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public class Snake : MonoBehaviour
{
    private GridWorld world;
    
    public GridWorld SetWorld
    {
        set => world = value;
    }

    private List<GameObject> snakeParts;

    public GameObject snakePartToInstantiate;



    // private List<Func<int>> actions;

    int MoveRight() => CalculateNextMove(Vector2.right);
    int MoveUp() => CalculateNextMove(Vector2.up);
    int MoveLeft() => CalculateNextMove(Vector2.left);
    int MoveDown() => CalculateNextMove(Vector2.down);

    void Start()
    {

        snakeParts = new List<GameObject>();

        Random rg = new Random();

        int randomIndexXInWorld, randomIndexYInWorld;
        do
        {
            randomIndexXInWorld = rg.Next(world.GetGridSize);
            randomIndexYInWorld = rg.Next(world.GetGridSize);
        } while (world.GetCellState(new Vector2(randomIndexXInWorld, randomIndexYInWorld)) !=
                 WorldCell.CellState.EMPTY);


        Vector2 startCell = new Vector2(randomIndexXInWorld, randomIndexYInWorld);
        GameObject snakeHead = Instantiate(snakePartToInstantiate, startCell, Quaternion.identity, this.transform);

        //world.SnakeCell(startCell);
        snakeParts.Add(snakeHead);

        GameObject.Find("QLearningAgent").GetComponent<Agent>().SetCurrentSnake = this;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            MoveUp();
        if (Input.GetKeyDown(KeyCode.A))
            MoveLeft();
        if (Input.GetKeyDown(KeyCode.S))
            MoveDown();
        if (Input.GetKeyDown(KeyCode.D))
            MoveRight();


      
    }

   

    public  int CalculateNextMove(Vector3 directionToMove)
    {
        Vector2 newPosition = snakeParts[0].transform.position + directionToMove;
        if (!isInsideBounds(newPosition))
        {
            Death();
            return -10;
        }


        switch (world.GetCellState(newPosition))
        {
            case WorldCell.CellState.EMPTY:
                MoveSnake(newPosition);
                return -1;

            case WorldCell.CellState.APPLE:
                GrowSnake(newPosition);
                return 50;

            case WorldCell.CellState.SNAKE:
                Death();
                return -10;
        }

        return 0;
    }

    void MoveSnake(Vector2 newPosition)
    {
        int lastIndex = snakeParts.Count - 1;
        Vector2 cellToEmptyCoord = snakeParts[lastIndex].transform.position;

        world.EmptyCell(cellToEmptyCoord);

        for (int i = lastIndex; i > 0; i--)
        {
            snakeParts[i].transform.position = snakeParts[i - 1].transform.position;
        }


        snakeParts[0].transform.position = newPosition;
        world.SnakeCell(newPosition);
    }

    private bool isInsideBounds(Vector2 newPos)
    {
        return (newPos.x >= 0 && newPos.x < world.GetGridSize) &&
               (newPos.y >= 0 && newPos.y < world.GetGridSize);
    }

    private void GrowSnake(Vector2 newPosition)
    {
        Vector2 lastIndexPos = snakeParts[snakeParts.Count - 1].transform.position;
        MoveSnake(newPosition);
        GameObject newPart = Instantiate(snakePartToInstantiate, lastIndexPos, Quaternion.identity, this.transform);
        snakeParts.Add(newPart);
        world.SnakeCell(lastIndexPos);
        world.CreateNewApple();
    }


    private int Death()
    {
        world.Reset();
        return -10;
    }
}
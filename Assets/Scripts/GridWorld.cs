using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


public class GridWorld : MonoBehaviour
{
    private int gridSize =10;
    public int GetGridSize => gridSize;

    private Vector2 currentApplePosition;
    public Vector2 GetApplePosition => currentApplePosition;
    
    
    [SerializeField] private GameObject snakePrefab;
     private GameObject snake;
    
    
    [SerializeField] private GameObject cellToSpawn;
    private WorldCell[,] cellsPositions;



    public bool IsCellDanger(Vector2 cellPos)
    {
        return (IsOutsideBounds(cellPos) || cellsPositions[(int)cellPos.x,(int) cellPos.y].state == WorldCell.CellState.SNAKE);
    }

    private bool IsOutsideBounds(Vector2 cellPos) => cellPos.x < 0 || cellPos.x >= gridSize || 
                                                     cellPos.y < 0 || cellPos.y >= gridSize;
    
    public void EmptyCell(Vector2 coords)
    {
        cellsPositions[(int) coords.x, (int) coords.y].state = WorldCell.CellState.EMPTY;
        cellsPositions[(int) coords.x, (int) coords.y].ChangeColor();
    }

    public void AppleCell(Vector2 coords)
    {
        cellsPositions[(int) coords.x, (int) coords.y].state = WorldCell.CellState.APPLE;
    }

    public void SnakeCell(Vector2 coords)
    {
        cellsPositions[(int) coords.x, (int) coords.y].state = WorldCell.CellState.SNAKE;
    }

    public WorldCell.CellState GetCellState(Vector2 coords)
    {
        return cellsPositions[(int) coords.x, (int) coords.y].state;
    }
    
    void Awake()
    {
        cellsPositions = new WorldCell[gridSize, gridSize];

        for (int y = 0; y < gridSize; y++)
        for (int x = 0; x < gridSize; x++)
        {
            Vector2 temp = new Vector2(x, y);

            cellsPositions[x, y] = Instantiate(cellToSpawn, temp, Quaternion.identity).GetComponent<WorldCell>();

            //wc.WorldCoordinates = temp;
        }
        
        CreateSnake();

        CreateNewApple();
        
        
        GameObject.Find("QLearningAgent").GetComponent<Agent>().SetCurrentWorld = this;

        
    }

    private void CreateSnake()
    {
        snake = Instantiate(snakePrefab,Vector3.zero,Quaternion.identity );
        Snake snakeScript =  snake.GetComponent<Snake>();
        snakeScript.SetWorld = this;

    }

    public void CreateNewApple()
    {
        Random rg = new Random();
        int x,y;
            
        do
        {
            x = rg.Next(gridSize);
            y = rg.Next(gridSize);
            
        } while (cellsPositions[x,y].state != WorldCell.CellState.EMPTY);

        currentApplePosition = new Vector2(x, y);
        cellsPositions[x, y].state = WorldCell.CellState.APPLE;
        cellsPositions[x, y].ChangeColor();

    }


    public void Reset()
    {
        foreach (WorldCell cell in cellsPositions)
        {
            Destroy(cell.gameObject);
        }
        Array.Clear(cellsPositions,0,cellsPositions.Length);
        Destroy(snake);
        Awake();
    }
    
}
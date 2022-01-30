using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Agent : MonoBehaviour
{

    [SerializeField]private Snake currentSnake;
    [SerializeField]private GridWorld currentWorld;

    public Snake SetCurrentSnake
    {
        set => currentSnake = value;
    }
    
    public GridWorld SetCurrentWorld
    {
        set => currentWorld = value;
    }
    
    private float timeLeftToMove;
    private float timer = 0.1f;
    
    private int[,] qTable;
    private float epsilon = 0.1f;
    
    
    private List<Func<int>> actions;
    
    int MoveRight() => currentSnake.CalculateNextMove(Vector2.right);
    int MoveUp() => currentSnake.CalculateNextMove(Vector2.up);
    int MoveLeft() => currentSnake.CalculateNextMove(Vector2.left);
    int MoveDown() => currentSnake.CalculateNextMove(Vector2.down);
    
    // Start is called before the first frame update
    void Start()
    {
        timeLeftToMove = timer;
        
        actions = new List<Func<int>>();
        actions.Add(MoveRight);
        actions.Add(MoveUp);
        actions.Add(MoveLeft);
        actions.Add(MoveDown);
        
        int numberStates = currentWorld.GetGridSize * currentWorld.GetGridSize; 
        int numberActions = actions.Count;
        qTable = new int[numberStates, numberActions];
        
        
        
    }

    
    private void QLearning()
    {
        int currentState = GetState();
        
        int action = GetAction(currentState);
        
        Debug.Log("CurrentState: " + currentState);
        Debug.Log("Action: " + action);

        
        qTable[currentState, action] = actions[action].Invoke();
    }
    
    private int GetAction(int currentState)
    {
        Random rg = new Random();

        if (rg.NextDouble() < epsilon)
            return rg.Next(0, 4);

        
        int bestActionValue = 0;

        for (int i = 0; i < actions.Count; i++)
        {
            if (qTable[currentState, i] >= bestActionValue)
                bestActionValue = i;
        }

        return bestActionValue;
        
    }

    private int GetState() =>  (int) (currentSnake.transform.position.x + currentSnake.transform.position.y * currentWorld.GetGridSize);

    // Update is called once per frame
    void Update()
    {
        if (timeLeftToMove > 0)
        {
            timeLeftToMove -= Time.deltaTime;
            return;
        }

        QLearning();
        timeLeftToMove = timer;
        
    }
}

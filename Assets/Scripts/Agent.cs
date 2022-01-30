using System;
using System.Collections.Generic;
using System.Linq;
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

    struct State
    {
        public State(Vector2 snakePos, bool appleRight,bool appleTop)
        {
            snakePosition.x = (int)snakePos.x;
            snakePosition.y = (int)snakePos.y;

            appleOnRight = appleRight;
            appleOnTop = appleTop;
        }
        public Vector2 snakePosition;
        public bool appleOnRight;
        public bool appleOnTop;
    }

    private Dictionary<State, Dictionary<Func<int>, int>> qDic;
    
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
        actions = new List<Func<int>>();
        actions.Add(MoveRight);
        actions.Add(MoveUp);
        actions.Add(MoveLeft);
        actions.Add(MoveDown);
        
        
        qDic = new Dictionary<State, Dictionary<Func<int>, int>>();

        for (int y = 0; y < currentWorld.GetGridSize; y++)
        {
            for (int x = 0; x < currentWorld.GetGridSize; x++)
            {
                Dictionary<Func<int>, int> tempDict = new Dictionary<Func<int>, int>();
                foreach (Func<int> a in actions) tempDict.Add(a,0);

                //State tempState = new State(new Vector2(x, y), true, true); 
                
                qDic.Add(new State(new Vector2(x, y), true, true),tempDict);
                qDic.Add(new State(new Vector2(x, y), true, false),tempDict);
                qDic.Add(new State(new Vector2(x, y), false, true),tempDict);
                qDic.Add(new State(new Vector2(x, y), false, false),tempDict);

            }
        }


        foreach (var f in qDic)
        {
            Debug.Log(f);
        }
        
        
        timeLeftToMove = timer;
        
        
        
        int numberStates = currentWorld.GetGridSize * currentWorld.GetGridSize; 
        int numberActions = actions.Count;
        qTable = new int[numberStates, numberActions];
        
        
        
    }

    
    private void QLearning()
    {
        //int currentState = GetStateL();
        State currentState = GetStateD();
        
        
        Debug.Log(currentState.snakePosition);
        Debug.Log(currentState.appleOnRight);
        Debug.Log(currentState.appleOnTop);

        
        
        //int action = GetAction(currentState);
        Dictionary<Func<int>, int> actionsValuesForCurrentState = qDic[currentState];


        KeyValuePair<Func<int>, int> bestAction = qDic[currentState].First();
        
        int tempValue = 0;
        foreach ( KeyValuePair<Func<int>,int> k in actionsValuesForCurrentState)
        {
            if (k.Value >= tempValue)
            {
                bestAction = k;
            }
        }

        qDic[currentState][bestAction.Key]= bestAction.Key.Invoke();

        //Debug.Log("CurrentState: " + currentState);
        //Debug.Log("Action: " + action);


        //qTable[currentState, action] = actions[action].Invoke();
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

    private int GetStateL()
    {
        //first states
        return (int) (currentSnake.transform.position.x + currentSnake.transform.position.y * currentWorld.GetGridSize);
    } 

    private State GetStateD()
    {
        bool isAppleOnRight = currentSnake.transform.position.x < currentWorld.GetApplePosition.x;
        bool isAppleOnTop = currentSnake.transform.position.y < currentWorld.GetApplePosition.y;

        return new State(currentSnake.transform.position, isAppleOnRight, isAppleOnTop);

    } 
    
    
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

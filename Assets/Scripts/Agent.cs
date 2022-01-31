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
    private float timer = 0.05f;

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

    private Dictionary<State, Dictionary<Func<int>, float>> qDic;
    
    private int[,] qTable;
    private float epsilon = 0.1f;
    float alpha = 0.1f;
    float gamma = 0.6f;
    
    
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
        
        
        qDic = new Dictionary<State, Dictionary<Func<int>, float>>();

        for (int y = 0; y < currentWorld.GetGridSize; y++)
        {
            for (int x = 0; x < currentWorld.GetGridSize; x++)
            {
                Dictionary<Func<int>, float> tempDict = new Dictionary<Func<int>, float>();
                foreach (Func<int> a in actions) tempDict.Add(a,0);

                //State tempState = new State(new Vector2(x, y), true, true); 
                
                qDic.Add(new State(new Vector2(x, y), true, true),tempDict);
                qDic.Add(new State(new Vector2(x, y), true, false),tempDict);
                qDic.Add(new State(new Vector2(x, y), false, true),tempDict);
                qDic.Add(new State(new Vector2(x, y), false, false),tempDict);

            }
        }

        
        
        timeLeftToMove = timer;
        
        
        
        int numberStates = currentWorld.GetGridSize * currentWorld.GetGridSize; 
        int numberActions = actions.Count;
        qTable = new int[numberStates, numberActions];
        
        
        
    }

    
    private void QLearning()
    {
        State currentState = GetState();

        Func<int> actionToExecuteInCurrentState = GetAction(currentState);

        
      
        int reward = actionToExecuteInCurrentState.Invoke();

        State nextState = GetState();
        Func<int> bestActionInNext = BestActionInState(nextState);
        
        qDic[currentState][actionToExecuteInCurrentState] = (1-alpha)* qDic[currentState][actionToExecuteInCurrentState] + 
                                                            alpha * (reward + gamma * qDic[nextState][bestActionInNext] - qDic[currentState][actionToExecuteInCurrentState]);


        //Debug.Log("CurrentState: " + currentState);
        //Debug.Log("Action: " + action);


        //qTable[currentState, action] = actions[action].Invoke();
    }
    
    private State GetState()
    {
        bool isAppleOnRight = currentSnake.transform.position.x < currentWorld.GetApplePosition.x;
        bool isAppleOnTop = currentSnake.transform.position.y < currentWorld.GetApplePosition.y;

        return new State(currentSnake.transform.position, isAppleOnRight, isAppleOnTop);

    }

    private Func<int> GetAction(State state)
    {
        Random rg = new Random();
        float randomFloat = (float) rg.NextDouble();
        if (randomFloat < epsilon) return actions[rg.Next(4)];


        return BestActionInState(state);

    }
    private Func<int> BestActionInState(State state)
    {
        
        
        
        Dictionary<Func<int>, float> actionsValuesForCurrentState = qDic[state];
        
        KeyValuePair<Func<int>, float> bestAction = qDic[state].First();
        
        foreach ( KeyValuePair<Func<int>,float> k in actionsValuesForCurrentState)
        {
            if (k.Value >= bestAction.Value)
                bestAction = k;
            
        }

        return bestAction.Key;

    }

    private State GetNextState(State state, Func<int>)
    
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

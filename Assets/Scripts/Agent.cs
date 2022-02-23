using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Agent : MonoBehaviour
{
    private int NumGame;
    public int GetNumberGame => NumGame;
    
    [SerializeField] private Text text;
    
    
    [SerializeField] private Snake currentSnake;
    [SerializeField] private GridWorld currentWorld;

    public Snake SetCurrentSnake
    {
        set => currentSnake = value;
    }

    public GridWorld SetCurrentWorld
    {
        set => currentWorld = value;
    }

    private bool isFastMode = true;
    private float timeLeftToMove;
    private float timer = 0.05f;

    // public struct State
    // {
    //     public State(Vector2 snakePos, bool appleRight, bool appleLeft, bool appleTop, bool appleBottom, bool dangerR,
    //         bool dangerL, bool dangerB,
    //         bool dangerA)
    //     {
    //         snakePosition.x = (int) snakePos.x;
    //         snakePosition.y = (int) snakePos.y;
    //
    //         appleOnRight = appleRight;
    //         appleOnLeft = appleLeft;
    //         appleOnTop = appleTop;
    //         appleOnBottom = appleBottom;
    //
    //         dangerRight = dangerR;
    //         dangerLeft = dangerL;
    //         dangerAbove = dangerA;
    //         dangerBelow = dangerB;
    //     }
    //
    //     private Vector2 snakePosition;
    //
    //     private bool appleOnRight;
    //     private bool appleOnLeft;
    //     private bool appleOnTop;
    //     private bool appleOnBottom;
    //
    //
    //     private bool dangerRight;
    //     private bool dangerLeft;
    //     private bool dangerAbove;
    //     private bool dangerBelow;
    // }
    //
    //
    public struct State
    {
        public State( bool appleRight, bool appleLeft, bool appleTop, bool appleBottom, bool dangerR,
            bool dangerL, bool dangerB,
            bool dangerA)
        {
         

            appleOnRight = appleRight;
            appleOnLeft = appleLeft;
            appleOnTop = appleTop;
            appleOnBottom = appleBottom;

            dangerRight = dangerR;
            dangerLeft = dangerL;
            dangerAbove = dangerA;
            dangerBelow = dangerB;
        }

     

        private bool appleOnRight;
        private bool appleOnLeft;
        private bool appleOnTop;
        private bool appleOnBottom;


        private bool dangerRight;
        private bool dangerLeft;
        private bool dangerAbove;
        private bool dangerBelow;
    }
    
    
    
    public Dictionary<State, Dictionary<Func<int>, float>> qTable;


    private float epsilon = 0.25f;
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
        actions = new List<Func<int>> {MoveRight, MoveUp, MoveLeft, MoveDown};

        qTable = new Dictionary<State, Dictionary<Func<int>, float>>();
        //
        // for (int y = 0; y < currentWorld.GetGridSize; y++)
        // {
        //     for (int x = 0; x < currentWorld.GetGridSize; x++)
        //     {
        //             //Apple on Right
        //         for (int a = 0; a < 2; a++)
        //             //apple on left
        //         for (int b = 0; b < 2; b++)
        //             //apple on top
        //         for (int c = 0; c < 2; c++)
        //             //apple on bottom
        //         for (int d = 0; d < 2; d++)
        //             //danger right
        //         for (int e = 0; e < 2; e++)
        //             //danger left
        //         for (int f = 0; f < 2; f++)
        //             //danger below
        //         for (int g = 0; g < 2; g++)
        //             //danger above
        //         for (int h = 0; h < 2; h++)
        //         {
        //             Dictionary<Func<int>, float> tempDict = new Dictionary<Func<int>, float>();
        //
        //             foreach (Func<int> action in actions) tempDict.Add(action, 0);
        //
        //             // qTable.Add(new State(new Vector2(x, y), Convert.ToBoolean(a),
        //             //     Convert.ToBoolean(b), Convert.ToBoolean(c), Convert.ToBoolean(d),
        //             //     Convert.ToBoolean(e), Convert.ToBoolean(f),Convert.ToBoolean(g),
        //             //     Convert.ToBoolean(h)), tempDict);
        //
        //             // qTable.Add(new State(Convert.ToBoolean(a), 
        //             //     Convert.ToBoolean(b),Convert.ToBoolean(c),Convert.ToBoolean(d),
        //             //     Convert.ToBoolean(e),Convert.ToBoolean(f)),tempDict);
        //         }
        //     }
        // }
        //
        
        //Apple on Right
        for (int a = 0; a < 2; a++)
            //apple on left
        for (int b = 0; b < 2; b++)
            //apple on top
        for (int c = 0; c < 2; c++)
            //apple on bottom
        for (int d = 0; d < 2; d++)
            //danger right
        for (int e = 0; e < 2; e++)
            //danger left
        for (int f = 0; f < 2; f++)
            //danger below
        for (int g = 0; g < 2; g++)
            //danger above
        for (int h = 0; h < 2; h++)
        {
            Dictionary<Func<int>, float> tempDict = new Dictionary<Func<int>, float>();

            foreach (Func<int> action in actions) tempDict.Add(action, 0);

            // qTable.Add(new State(new Vector2(x, y), Convert.ToBoolean(a),
            //     Convert.ToBoolean(b), Convert.ToBoolean(c), Convert.ToBoolean(d),
            //     Convert.ToBoolean(e), Convert.ToBoolean(f),Convert.ToBoolean(g),
            //     Convert.ToBoolean(h)), tempDict);

            qTable.Add(new State( Convert.ToBoolean(a),
                Convert.ToBoolean(b), Convert.ToBoolean(c), Convert.ToBoolean(d),
                Convert.ToBoolean(e), Convert.ToBoolean(f),Convert.ToBoolean(g),
                Convert.ToBoolean(h)), tempDict);
        }

        timeLeftToMove = timer;
        NumGame = 1;
    }

    private void UPDATETEXTTEST()
    
    {   
        int currentX = (int) currentSnake.GetSnakePosition.x;
        int currentY = (int) currentSnake.GetSnakePosition.y;

        
        bool isAppleOnRight = currentX < currentWorld.GetApplePosition.x;
        bool isAppleOnLeft = currentX > currentWorld.GetApplePosition.x;
         
        bool isAppleOnTop = currentY < currentWorld.GetApplePosition.y;
        bool isAppleOnBottom = currentY > currentWorld.GetApplePosition.y;

        bool isDangerRight = currentWorld.IsCellDanger(new Vector2(currentX + 1, currentY));

        bool isDangerLeft = currentWorld.IsCellDanger(new Vector2(currentX - 1, currentY));

        bool isDangerBelow = currentWorld.IsCellDanger(new Vector2(currentX, currentY - 1));

        bool isDangerAbove = currentWorld.IsCellDanger(new Vector2(currentX, currentY + 1));
        
        
        text.text = "Game Num: " + NumGame+"\n" +
                    "ApplePos: " + currentWorld.GetApplePosition.x + " " + currentWorld.GetApplePosition.y+"\n"+
                    "Apple On Left: " + isAppleOnLeft+"\n"+
                    "Apple On Right: " + isAppleOnRight+"\n"+
                    "Apple On Top: " + isAppleOnTop+"\n"+
                    "Apple On Bot: " + isAppleOnBottom+"\n"+
                    "Danger Right: " + isDangerRight+"\n"+
                    "Danger Left: " + isDangerLeft+"\n"+
                    "Danger Above: " + isDangerAbove+"\n"+
                    "Danger Below: " + isDangerBelow+"\n";
    }

    private void QLearning()
    {
        State currentState = GetState();
        UPDATETEXTTEST();
        Func<int> actionToExecuteInCurrentState = GetAction(currentState);


        int reward = actionToExecuteInCurrentState.Invoke();

        float nextStateQValue = 0;
        
        
        if (reward != -10)
        {
            
            State nextState = GetState();
            Func<int> bestActionInNext = BestActionInState(nextState);
            nextStateQValue = qTable[nextState][bestActionInNext];
        }
        else NumGame++;
        
        UPDATETEXTTEST();

        qTable[currentState][actionToExecuteInCurrentState] =
            /*(1 - alpha)*/ qTable[currentState][actionToExecuteInCurrentState] +
                            alpha * (reward + gamma * nextStateQValue -
                                     qTable[currentState][actionToExecuteInCurrentState]);

    
        //Debug.Log("CurrentState: " + currentState);
        //Debug.Log("Action: " + action);


        //qTable[currentState, action] = actions[action].Invoke();
    }

    private State GetState()
    {
        int currentX = (int) currentSnake.GetSnakePosition.x;
        int currentY = (int) currentSnake.GetSnakePosition.y;

        bool isAppleOnRight = currentX < currentWorld.GetApplePosition.x;
        bool isAppleOnLeft = currentX > currentWorld.GetApplePosition.x;
         
        bool isAppleOnTop = currentY < currentWorld.GetApplePosition.y;
        bool isAppleOnBottom = currentY > currentWorld.GetApplePosition.y;

        bool isDangerRight = currentWorld.IsCellDanger(new Vector2(currentX + 1, currentY));

        bool isDangerLeft = currentWorld.IsCellDanger(new Vector2(currentX - 1, currentY));

        bool isDangerBelow = currentWorld.IsCellDanger(new Vector2(currentX, currentY - 1));

        bool isDangerAbove = currentWorld.IsCellDanger(new Vector2(currentX, currentY + 1));


        // return new State(currentSnake.transform.position, isAppleOnRight, isAppleOnLeft, isAppleOnTop, isAppleOnBottom,
        //     isDangerRight, isDangerLeft, isDangerBelow, isDangerAbove);

        // //TEST
        return new State( isAppleOnRight, isAppleOnLeft, isAppleOnTop, isAppleOnBottom,
            isDangerRight, isDangerLeft, isDangerBelow, isDangerAbove);
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
        Dictionary<Func<int>, float> actionsValuesForCurrentState = qTable[state];

        KeyValuePair<Func<int>, float> bestAction = qTable[state].First();

        foreach (KeyValuePair<Func<int>, float> k in actionsValuesForCurrentState)
        {
            if (k.Value >= bestAction.Value)
                bestAction = k;
        }

        return bestAction.Key;
    }

    // private State GetNextState(State state, Func<int> actionToExecute)
    // {
    //     
    // }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) isFastMode = !isFastMode;

        if (isFastMode)
        {
            QLearning();
            return;
        }
        
        
        if (timeLeftToMove > 0)
        {
            timeLeftToMove -= Time.deltaTime;
            return;
        }

        QLearning();
        timeLeftToMove = timer;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCell : MonoBehaviour
{
    //public Vector2 WorldCoordinates { get; set; }

    public enum CellState
    {
        EMPTY = 0,
        SNAKE = 1,
        APPLE = 2
    }
    
    public CellState state;

    public Material empty;
    public Material apple;
    
    private void Awake()
    {
        state = CellState.EMPTY;
    }

    public void ChangeColor()
    {
        switch (state)
        {
            case CellState.EMPTY:
                this.gameObject.GetComponent<MeshRenderer>().material = empty;
                break;
            
            case CellState.APPLE:
                this.gameObject.GetComponent<MeshRenderer>().material = apple;
                break;
        }
    }
    
}

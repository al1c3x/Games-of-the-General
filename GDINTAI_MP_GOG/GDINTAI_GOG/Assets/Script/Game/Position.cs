using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position
{
    public enum Player { PlayerOne = 0, PlayerTwo }

    public static string TAG = "Position";

   
    private int pieceID = 0;
    private int realValue = 0; 
    private int pieceValue = 0; 
    private int row = -1;
    private int column = -1;
    private bool isVisible = false;
    private int playerIndex = 0; 


    public static bool isTheSamePos(Position pos1, Position pos2)
    {
        if (pos1.Row == pos2.Row && pos1.Column == pos2.Column)
        {
            return true;
        }
       
         return false;
    }

    public static void copyPosition(Position pos, Position dup)
    {
        if (pos == null)
        {
            return;
        }
           
        dup.PieceID = pos.PieceID;
        dup.PieceValue = pos.PieceValue;
        dup.RealValue = pos.RealValue;
        dup.Row = pos.Row;
        dup.Column = pos.Column;
        dup.PlayerIndex = pos.PlayerIndex;
    }
    public bool IsVisible
    {
        get { 
            return isVisible; 
        }
        set { 
            isVisible = value; 
        }
    }

    public int PieceID
    {
        get { 
            return pieceID; 
        }
        set { 
            pieceID = value; 
        }
    }
    public int RealValue
    {
        get { 
            return realValue; 
        }
        set { 
            realValue = value; 
        }
    }
    public int PieceValue
    {
        get { 
            return pieceValue; 
        }
        set { 
            pieceValue = value; 
        }
    }
    public int Row
    {
        get { 
            return row; 
        }
        set { 
            row = value; 
       }
    }
    public int Column
    {
        get { 
            return column; 
        }
        set { 
            column = value; 
        }
    }
    public int PlayerIndex
    {
        get { 
            return playerIndex; 
        }
        set { 
            playerIndex = value; 
        }
    }
}

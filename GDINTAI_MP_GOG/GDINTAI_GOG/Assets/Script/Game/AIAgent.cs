using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgent //BoardEvaluator
{


    public static float getInitialHeuristic(int pieceType)
    {
        switch(pieceType)
        {
            case 15: return 7.50f; //Spy
            case 14: return 7.80f; //5Star
            case 13: return 6.95f; //4Star
            case 12: return 6.15f; //3Star
            case 11: return 5.40f; //2Star
            case 10: return 4.70f; //1Star
            case 9: return 4.05f; //Colonel
            case 8: return 3.45f; //Lt Colonel
            case 7: return 2.90f; //Major
            case 6: return 2.40f; //Captain
            case 5: return 1.95f; //First Lieutenant
            case 4: return 1.55f; //Second Lieutenant
            case 3: return 1.20f; //Seargent
            case 2: return 1.37f; //Private
            case 1: return 0.0f; //Flag
            default: return 0.0f;   //unknown
        }
    }
   

    public static List<BoardState> exploreNextMoves(BoardState boardState, int player)
    {
        List<BoardState> possibleStates = new List<BoardState>();

        for (int i = 0; i < boardState.getPositionSize(player); i++)
        {
            Position position = new Position();
            Position.copyPosition(boardState.getPositionAt(player, i), position);

            int row = position.Row;
            int column = position.Column;

            BoardState expandedBoardState = new BoardState();

           
            if (checkMoves(boardState, row + 1, column, player))
            {
                expandedBoardState = generateNewChildStateForMove(boardState, position, i, position.PieceID, position.PieceValue, row + 1, column, player, position.RealValue);
                expandedBoardState.moveUnit = 1;
                possibleStates.Add(expandedBoardState);
            } 
            
            if (checkMoves(boardState, row - 1, column, player))
            {
                expandedBoardState = generateNewChildStateForMove(boardState, position, i, position.PieceID, position.PieceValue, row - 1, column, player, position.RealValue);
                expandedBoardState.moveUnit = 0;
                possibleStates.Add(expandedBoardState);
            }
          
            if (checkMoves(boardState, row, column + 1, player))
            {
                expandedBoardState = generateNewChildStateForMove(boardState, position, i, position.PieceID, position.PieceValue, row, column + 1, player, position.RealValue);
                expandedBoardState.moveUnit = 3;
                possibleStates.Add(expandedBoardState);
            }
     
            if (checkMoves(boardState, row, column - 1, player))
            {
                expandedBoardState = generateNewChildStateForMove(boardState, position, i, position.PieceID, position.PieceValue, row, column - 1, player, position.RealValue);
                expandedBoardState.moveUnit = 2;
                possibleStates.Add(expandedBoardState);
            }
        }

        return possibleStates;
    }

    public static BoardState generateNewChildStateForMove(BoardState boardState, Position position, int i, int pieceId, int pieceValue, int newRow, int newCol, int player, int realV)
    {
        BoardState newBoardState = new BoardState();
        BoardState.copyBoardState(boardState, newBoardState);

        Position newPosition = new Position();
        int opposite = player == 0 ? 1 : 0;

        newBoardState.WhoseTurnToMove = opposite;

        newPosition.PieceID = pieceId;
        newPosition.PieceValue = pieceValue;
        newPosition.RealValue = realV;
        newPosition.Row = newRow;
        newPosition.Column = newCol;
        newPosition.PlayerIndex = player;
        Position.copyPosition(position, newBoardState.sourcePosition);
        newBoardState.deletePosition(position, player);
        newBoardState.addFromPosList(player, newPosition);

        Position traversePiece = new Position();
        Position.copyPosition(boardState.getPositionAtPlace(newRow, newCol), traversePiece);

        Position.copyPosition(traversePiece, newBoardState.positionEaten);
        Position.copyPosition(newPosition, newBoardState.movePosition);



        if (newPosition.RealValue > traversePiece.RealValue && traversePiece.RealValue > 0)
        {
            newBoardState.eatenState = 0;
        }
        else if (newPosition.RealValue < traversePiece.RealValue && traversePiece.RealValue > 0)
        {
            newBoardState.eatenState = 1;
        }
        else if (newPosition.RealValue == traversePiece.RealValue && traversePiece.RealValue > 0)
        {
            newBoardState.eatenState = 2;
        }

        return newBoardState;
    }

   

    public static bool checkMoves(BoardState boardState, int rowIndex, int colIndex, int player)
    {
        if(rowIndex >= BoardState.MAX_ROW || rowIndex < 0)
        {
            return false;
        }
        if (colIndex >= BoardState.MAX_COL || colIndex < 0)
        {
            return false;
        }
        if(boardState.isThereAPosition(player, rowIndex, colIndex))
        {
            return false;
        }
        return true;
    }

}

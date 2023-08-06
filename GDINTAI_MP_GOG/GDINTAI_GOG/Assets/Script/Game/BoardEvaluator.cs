using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardEvaluator
{
    public enum Player { PlayerOne = 0, PlayerTwo = 1 }
    public static string Tag = "BoardEvaluator";
    private BoardState boardState;

    public static float OFFENSE_MULTIPLIER = 50.0f; 
    public static float DEFENSE_DEDUCTION = 10.0f; 
    public static float OPENNESS_VALUE = 25.0f; 
    public static float WIN_LOSS_VALUE = 90000.0f;

 
    public BoardEvaluator(BoardState boardState) { this.boardState = boardState; }

    
    public double evaluate()
    {
        double computerScore = 0.0f;

        for (int i = 0; i < this.boardState.getPositionSize((int)Player.PlayerTwo); i++)
        {
            Position position = new Position();
            Position.copyPosition(this.boardState.getPositionAt((int)Player.PlayerTwo, i), position);
            float positionScore = this.computeOffensiveness(position, (int)Player.PlayerOne) + this.computeOpenness(position, (int)Player.PlayerOne) - this.computeDefensiveness(position, (int)Player.PlayerOne);
            computerScore += positionScore;
        }

      
        BoardState temp1 = new BoardState();
        BoardState.copyBoardState(this.boardState, temp1);
        if (this.isFlagAtRisk(temp1, (int)Player.PlayerTwo))
        {
            computerScore += -WIN_LOSS_VALUE;
            
        }
        if (playerIsWin((int)Player.PlayerTwo))
        {
            computerScore += WIN_LOSS_VALUE;
            
        }

        double humanScore = 0.0f;
        BoardState temp = new BoardState();
        BoardState.copyBoardState(this.boardState, temp);
        List<BoardState> counterBoards = AIAgent.exploreNextMoves(temp, (int)Player.PlayerOne);
        for (int x = 0; x < counterBoards.Count; x++)
        {
            for (int i = 0; i < counterBoards[x].getPositionSize((int)Player.PlayerOne); i++)
            {
                Position position = new Position();
                Position.copyPosition(this.boardState.getPositionAt((int)Player.PlayerOne, i), position);
                float positionScore = this.computeOpenness(position, (int)Player.PlayerTwo) - this.computeDefensiveness(position, (int)Player.PlayerTwo);
                humanScore += positionScore;
            }
           
            BoardState temp2 = new BoardState();
            BoardState.copyBoardState(this.boardState, temp2);
           
            if (playerIsWin((int)Player.PlayerOne))
            {
                computerScore += WIN_LOSS_VALUE;
                
            }
        }

        return computerScore - (humanScore / counterBoards.Count);
    }

    private bool playerIsWin(int player)
    {
        Position flagPos = new Position();
        foreach (var item in player == (int)Player.PlayerTwo ? boardState.getPositionList((int)Player.PlayerTwo) : boardState.getPositionList((int)Player.PlayerOne))
        {
            if (item.PieceValue == 0)
                continue;
            if (item.PieceValue == 1)
            {
                Position.copyPosition(item, flagPos);
            }
        }
        if (player == (int)Player.PlayerOne)
        {
            if (flagPos.Row == 0)
                return true;
        }
        else
        {
            if (flagPos.Row == 7)
                return true;
        }
        return false;
    }

    private bool isFlagAtRisk(BoardState boardState, int player)
    {


        Position flagPos = new Position();
        foreach (var item in player == (int)Player.PlayerTwo ? boardState.getPositionList((int)Player.PlayerTwo) : boardState.getPositionList((int)Player.PlayerOne))
        {
            if (item.PieceValue == 0)
                continue;
            if (item.PieceValue == 1)
            {
                Position.copyPosition(item, flagPos);
            }
        }
        if(flagPos.PieceValue == 0)
        {
            return false;
        }
       
        foreach (var item in player == (int)Player.PlayerTwo ? boardState.getPositionList((int)Player.PlayerOne) : boardState.getPositionList((int)Player.PlayerTwo))
        {
            if (Position.isTheSamePos(item, flagPos) || (Mathf.Abs(item.Row - flagPos.Row) <= 1 && Mathf.Abs(item.Column - flagPos.Column) <= 1))
            {
                
                return true;
            }
        }
        return false;
    }

    private bool checkIfSpyPriv(int turn, int oppo)
    {
        if(turn == 2 && oppo == 15)
        {
            return true;
        }
        else { return false; }
    }

    private float checkIfOppoNear(Position pos)
    {
        float offense = 0.0f;
        if(pos.Row < 4)
        {
            offense += 105.0f;
        }
        return offense;
    }

    private float computeOffensiveness(Position computingPos, int opposingPlayer)
    {
        int numAdjacentPieces = 0;
        float offenseScore = 0;

        Position eatenPos = new Position();
        Position.copyPosition(this.boardState.positionEaten, eatenPos);

        Position movePos = new Position();
        Position.copyPosition(this.boardState.movePosition, movePos);

        Position positionAtBottom = new Position();
        Position.copyPosition(this.boardState.getPositionAtPlace(computingPos.Row + 1, computingPos.Column), positionAtBottom);
        if (positionAtBottom.Row != -1 && positionAtBottom.PlayerIndex == opposingPlayer)
        {
            if ((computingPos.PieceValue > positionAtBottom.PieceValue || checkIfSpyPriv(computingPos.PieceValue, positionAtBottom.PieceValue)) && positionAtBottom.PieceValue > 0)
            {
                offenseScore += 50.0f;
            }
        }

        Position positionAtTop = new Position();
        Position.copyPosition(this.boardState.getPositionAtPlace(computingPos.Row - 1, computingPos.Column), positionAtTop);
        if (positionAtTop.Row != -1 && positionAtTop.PlayerIndex == opposingPlayer)
        {
            if ((computingPos.PieceValue > positionAtTop.PieceValue || checkIfSpyPriv(computingPos.PieceValue, positionAtTop.PieceValue)) && positionAtTop.PieceValue > 0)
            {
                offenseScore += 50.0f;
            }
        }

        Position positionAtRight = new Position();
        Position.copyPosition(this.boardState.getPositionAtPlace(computingPos.Row, computingPos.Column + 1), positionAtRight);
        if (positionAtRight.Row != -1 && positionAtRight.PlayerIndex == opposingPlayer)
        {
            if ((computingPos.PieceValue > positionAtRight.PieceValue || checkIfSpyPriv(computingPos.PieceValue, positionAtRight.PieceValue)) && positionAtRight.PieceValue > 0)
            {
                offenseScore += 50.0f;
            }
        }

        Position positionAtLeft = new Position();
        Position.copyPosition(this.boardState.getPositionAtPlace(computingPos.Row, computingPos.Column - 1), positionAtLeft);
        if (positionAtLeft.Row != -1 && positionAtLeft.PlayerIndex == opposingPlayer)
        {
            if ((computingPos.PieceValue > positionAtLeft.PieceValue || checkIfSpyPriv(computingPos.PieceValue, positionAtLeft.PieceValue)) && positionAtLeft.PieceValue > 0)
            {
                offenseScore += 50.0f;
            }
        }
        if (Position.isTheSamePos(movePos, computingPos) && Position.isTheSamePos(movePos, eatenPos))
        {
            
            offenseScore += checkIfOppoNear(eatenPos);
            if(computingPos.PieceValue > 9)
            {
                offenseScore += 30.0f;
            }
            if (computingPos.PieceValue <= 8 && computingPos.PieceValue == 1)
            {
                offenseScore += 15;
            }
        }
       
        if (Position.isTheSamePos(movePos, computingPos) && Position.isTheSamePos(movePos, eatenPos) && ((movePos.PieceValue > eatenPos.PieceValue) || checkIfSpyPriv(movePos.PieceValue, eatenPos.PieceValue)) && eatenPos.PieceValue > 0)
        {
            offenseScore += 250.0f;
        }
      
        if (Position.isTheSamePos(movePos, computingPos) && Position.isTheSamePos(movePos, eatenPos) && movePos.PieceValue == 1)
        {
            offenseScore += -99999.0f;
        }
    
        if (Position.isTheSamePos(movePos, computingPos) && movePos.PieceValue == 1 && movePos.Row == 7)
        {
            offenseScore += 99999.0f;
        }
      
        if (Position.isTheSamePos(movePos, computingPos) && movePos.PieceValue == 1 && flagEntry())
        {
            Position pos = new Position();
            Position.copyPosition(boardState.findPositionAt((int)Player.PlayerTwo, 1), pos);
            offenseScore += (pos.Row+1) * 50.0f;
        }

        if(Position.isTheSamePos(movePos, computingPos) && Position.isTheSamePos(movePos, eatenPos) && eatenPos.PieceValue == 1)
        {
            
            offenseScore += WIN_LOSS_VALUE * 2;
        }

        offenseScore += AIAgent.getInitialHeuristic(computingPos.PieceValue) + (OFFENSE_MULTIPLIER * numAdjacentPieces);
           
        return offenseScore;
    }

    private bool flagEntry()
    {
        Position pos = new Position();
        Position.copyPosition(boardState.findPositionAt((int)Player.PlayerTwo, 1), pos);
        int row = pos.Row;
        int col = pos.Column;

        for (int i = row + 1; i < 8; i++)
        {
            if(boardState.isThereAPosition((int)Player.PlayerOne, i, col))
            {
                return false;
            }
            if (boardState.isThereAPosition((int)Player.PlayerTwo, i, col))
            {
                return false;
            }
        }
        return true;
    }

    private float computeOpenness(Position computingPos, int opposingPlayer)
    {
        int numAdjacentPieces = 0;
        float opennnessScore = 0;

        Position eatenPos = new Position();
        Position.copyPosition(this.boardState.positionEaten, eatenPos);

        Position sourcePos = new Position();
        Position.copyPosition(this.boardState.sourcePosition, sourcePos);

        Position movePos = new Position();
        Position.copyPosition(this.boardState.movePosition, movePos);

        Position positionAtBottom = new Position();
        Position.copyPosition(this.boardState.getPositionAtPlace(computingPos.Row + 1, computingPos.Column), positionAtBottom);
        if (positionAtBottom.Row == -1)
        {
            numAdjacentPieces++;
        }

        Position positionAtTop = new Position();
        Position.copyPosition(this.boardState.getPositionAtPlace(computingPos.Row - 1, computingPos.Column), positionAtTop);
        if (positionAtTop.Row == -1)
        {
            numAdjacentPieces++;
        }


        Position positionAtRight = new Position();
        Position.copyPosition(this.boardState.getPositionAtPlace(computingPos.Row, computingPos.Column + 1), positionAtRight);
        if (positionAtRight.Row == -1)
        {
            numAdjacentPieces++;
        }

        Position positionAtLeft = new Position();
        Position.copyPosition(this.boardState.getPositionAtPlace(computingPos.Row, computingPos.Column - 1), positionAtLeft);
        if (positionAtLeft.Row == -1)
        {
            numAdjacentPieces++;
        }

        
        if (computingPos.Row == 1 && sourcePos.Row == 0 && Position.isTheSamePos(movePos, computingPos) && eatenPos.Row == -1)
        {
            opennnessScore += 70;
        }
        if (computingPos.Row == 2 && sourcePos.Row == 1 && Position.isTheSamePos(movePos, computingPos) && eatenPos.Row == -1)
        {
            opennnessScore += 60;
        }
        if (computingPos.Row == 3 && sourcePos.Row == 2 && Position.isTheSamePos(movePos, computingPos) && eatenPos.Row == -1)
        {
            opennnessScore += 50;
        }
        if (computingPos.Row == 4 && sourcePos.Row == 3 && Position.isTheSamePos(movePos, computingPos) && eatenPos.Row == -1)
        {
            opennnessScore += 40;
        }
        if (computingPos.Row == 5 && sourcePos.Row == 4 && Position.isTheSamePos(movePos, computingPos) && eatenPos.Row == -1)
        {
            opennnessScore += 30.0f;
        }
        if (computingPos.Row == 6 && sourcePos.Row == 5 && Position.isTheSamePos(movePos, computingPos) && eatenPos.Row == -1)
        {
            opennnessScore += 20.0f;
        }

        opennnessScore += (OPENNESS_VALUE * numAdjacentPieces);

        return opennnessScore;
    }

    private float computeDefensiveness(Position computingPos, int opposingPlayer)
    {
        int numAdjacentPieces = 0;
        float totalEnemyWeight = 0;
        float defensiveScore = 0;

        Position eatenPos = new Position();
        Position.copyPosition(this.boardState.positionEaten, eatenPos);

        Position movePos = new Position();
        Position.copyPosition(this.boardState.movePosition, movePos);

        Position sourcePos = new Position();
        Position.copyPosition(this.boardState.sourcePosition, sourcePos);

        Position positionAtBottom = new Position();
        Position.copyPosition(this.boardState.getPositionAtPlace(computingPos.Row + 1, computingPos.Column), positionAtBottom);
        if (positionAtBottom.Row != -1 && positionAtBottom.PlayerIndex == opposingPlayer)
        {
            numAdjacentPieces++;
        }

        Position positionAtTop = new Position();
        Position.copyPosition(this.boardState.getPositionAtPlace(computingPos.Row - 1, computingPos.Column), positionAtTop);
        if (positionAtTop.Row != -1 && positionAtTop.PlayerIndex == opposingPlayer)
        {
            numAdjacentPieces++;
        }

        Position positionAtRight = new Position();
        Position.copyPosition(this.boardState.getPositionAtPlace(computingPos.Row, computingPos.Column + 1), positionAtRight);
        if (positionAtRight.Row != -1 && positionAtRight.PlayerIndex == opposingPlayer)
        {
            numAdjacentPieces++;
        }

        Position positionAtLeft = new Position();
        Position.copyPosition(this.boardState.getPositionAtPlace(computingPos.Row, computingPos.Column - 1), positionAtLeft);
        if (positionAtLeft.Row != -1 && positionAtLeft.PlayerIndex == opposingPlayer)
        {      
            numAdjacentPieces++;
        }

        
        Position positionAtBottom1 = new Position();
        Position.copyPosition(this.boardState.getPositionAtPlace(computingPos.Row + 1, computingPos.Column), positionAtBottom1);
        if (positionAtBottom1.Row != -1 && positionAtBottom1.PlayerIndex == opposingPlayer)
        {
            if ((computingPos.PieceValue < positionAtBottom1.PieceValue && !checkIfSpyPriv(computingPos.PieceValue, positionAtBottom1.PieceValue)) && positionAtBottom1.PieceValue > 0)
            {
                
                defensiveScore += 40.0f;
            }
        }

        Position positionAtTop1 = new Position();
        Position.copyPosition(this.boardState.getPositionAtPlace(computingPos.Row - 1, computingPos.Column), positionAtTop1);
        if (positionAtTop1.Row != -1 && positionAtTop1.PlayerIndex == opposingPlayer)
        {
            if ((computingPos.PieceValue < positionAtTop1.PieceValue && !checkIfSpyPriv(computingPos.PieceValue, positionAtTop1.PieceValue)) && positionAtTop1.PieceValue > 0)
            {
                defensiveScore += 40.0f;
            }
        }

        Position positionAtRight1 = new Position();
        Position.copyPosition(this.boardState.getPositionAtPlace(computingPos.Row, computingPos.Column + 1), positionAtRight1);
        if (positionAtRight1.Row != -1 && positionAtRight1.PlayerIndex == opposingPlayer)
        {
            if ((computingPos.PieceValue < positionAtRight1.PieceValue && !checkIfSpyPriv(computingPos.PieceValue, positionAtRight1.PieceValue)) && positionAtRight1.PieceValue > 0)
            {
                defensiveScore += 40.0f;
            }
        }

        Position positionAtLeft1 = new Position();
        Position.copyPosition(this.boardState.getPositionAtPlace(computingPos.Row, computingPos.Column - 1), positionAtLeft1);
        if (positionAtLeft1.Row != -1 && positionAtLeft1.PlayerIndex == opposingPlayer)
        {
            if ((computingPos.PieceValue < positionAtLeft1.PieceValue && !checkIfSpyPriv(computingPos.PieceValue, positionAtLeft1.PieceValue)) && positionAtLeft1.PieceValue > 0)
            {
                defensiveScore += 40.0f;
            }
        }

        
        if (Position.isTheSamePos(movePos, computingPos) && Position.isTheSamePos(movePos, eatenPos) && ((movePos.PieceValue < eatenPos.PieceValue) && !checkIfSpyPriv(movePos.PieceValue, eatenPos.PieceValue)) && eatenPos.PieceValue > 0)
        {
            
            defensiveScore += 150.0f;
        }

        defensiveScore += -AIAgent.getInitialHeuristic(computingPos.PieceValue) + (DEFENSE_DEDUCTION * numAdjacentPieces);

        return defensiveScore;
    }
}

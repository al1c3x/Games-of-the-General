using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class AILogic : MonoBehaviour
{
    public static AILogic instance;


    public enum Player { PlayerOne = 0, PlayerTwo }

    private BoardScript board;
    private GameManagerScript script;

    public List<int> suspectedList;
    public List<int> usedSuspectedList;

    void Awake()
    {
      
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
           
        DontDestroyOnLoad(gameObject);

        board = FindObjectOfType<BoardScript>();
        script = FindObjectOfType<GameManagerScript>();
    }

    private void Start()
    {
        suspectedList = new List<int>() { 1, 2, 2, 2, 2, 2, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15 };
        usedSuspectedList = new List<int>();
    }
 
    public BoardState getCurrBoardState(List<GameObject> playerTwoPieceList, List<GameObject> playerOnePieceList)
    {
        BoardState currentState = new BoardState();
        int x = 1, y = 1; ;
      
        foreach (var pos2 in playerTwoPieceList)
        {
            Position newPos = new Position();
            newPos.PieceID = x++; 
            newPos.RealValue = pos2.GetComponent<GamePieceScript>().rank; 
            newPos.PieceValue = newPos.RealValue; 
            newPos.Row = pos2.GetComponent<GamePieceScript>().piecePosition[0]; 
            newPos.Column = pos2.GetComponent<GamePieceScript>().piecePosition[1];
            newPos.PlayerIndex = 1;
            newPos.IsVisible = pos2.GetComponent<GamePieceScript>().isVisible;
            currentState.addFromPosList((int)Player.PlayerTwo, newPos);
        }
        
        foreach (var pos1 in playerOnePieceList)
        {
            Position newPos = new Position();
            newPos.PieceID = y++;
            newPos.RealValue = pos1.GetComponent<GamePieceScript>().rank; 
            newPos.PieceValue = pos1.GetComponent<GamePieceScript>().suspectedRankValue; 
            newPos.Row = pos1.GetComponent<GamePieceScript>().piecePosition[0];
            newPos.Column = pos1.GetComponent<GamePieceScript>().piecePosition[1];
            newPos.PlayerIndex = 0;
            newPos.IsVisible = pos1.GetComponent<GamePieceScript>().isVisible;
            currentState.addFromPosList((int)Player.PlayerOne, newPos);
        }

        currentState.WhoseTurnToMove = (int)Player.PlayerTwo;

        return currentState;
    }

    private int findPieceIndex(Position movingPos, List<GameObject> remainingPlayerTwoPieces)
    {
        for (int i = 0; i < remainingPlayerTwoPieces.Count; i++)
        {
            if(remainingPlayerTwoPieces[i].GetComponent<GamePieceScript>().piecePosition[0] == movingPos.Row)
            {
                if(remainingPlayerTwoPieces[i].GetComponent<GamePieceScript>().piecePosition[1] == movingPos.Column)
                {
                    return i;
                }
                
            }
        }
        return 0;
    }

    public void AiMove(GameObject[] playerTwoPieceList, GameObject[] playerOnePieceList)
    {
 
        List<GameObject> remainingPlayerTwoPieces = new List<GameObject>();
        List<GameObject> remainingPlayerOnePieces = new List<GameObject>();

       
        foreach (var item in playerTwoPieceList)
        {
            if (item.GetComponent<GamePieceScript>().isPlaced != false)
            {
                remainingPlayerTwoPieces.Add(item);
            }
                
        }
        foreach (var item in playerOnePieceList)
        {
            if (item.GetComponent<GamePieceScript>().isPlaced != false)
            {
                remainingPlayerOnePieces.Add(item);
            }
               
        }

        
        BoardState currentBoardState = getCurrBoardState(remainingPlayerTwoPieces, remainingPlayerOnePieces);
        BoardState resultBoardState = currentBoardState.addEvalScoreAndGetBest();
        int pieceIndex = findPieceIndex(resultBoardState.sourcePosition, remainingPlayerTwoPieces);
        int moveIndex = resultBoardState.moveUnit;
        move(remainingPlayerTwoPieces[pieceIndex], moveIndex);  
        script.setTurn("human");
        board.turnOnInteractable();

    }

    
    private void move(GameObject aiPiece, int moveIndex)
    {
       
        int posX = aiPiece.GetComponent<GamePieceScript>().piecePosition[0];
        int posY = aiPiece.GetComponent<GamePieceScript>().piecePosition[1];
        int aiPieceRank = board.objectAndPos[posX + "" + posY].GetComponent<GamePieceScript>().rank;
        string aiPieceType = board.objectAndPos[posX + "" + posY].GetComponent<GamePieceScript>().playerType;
        int opposingPieceRank = 0;
        string opposingPieceType = "none";
        string newPos = "none";
        int newPosX = posX, newPosY = posY;
        GameObject opposingPiece = null;

        
        switch (moveIndex)
        {
            case 0:
                {
                    opposingPiece = board.objectAndPos[(posX - 1) + "" + posY];
                    newPos = (posX - 1) + "" + posY;
                    newPosX = posX - 1;
                    if (opposingPiece == null)
                        break;
                    opposingPieceRank = board.objectAndPos[(posX - 1) + "" + posY].GetComponent<GamePieceScript>().rank;
                    opposingPieceType = board.objectAndPos[(posX - 1) + "" + posY].GetComponent<GamePieceScript>().playerType;
                    break;
                }
            case 1:
                {
                    opposingPiece = board.objectAndPos[(posX + 1) + "" + posY];
                    newPos = (posX + 1) + "" + posY;
                    newPosX = posX + 1;
                    if (opposingPiece == null)
                        break;
                    opposingPieceRank = board.objectAndPos[(posX + 1) + "" + posY].GetComponent<GamePieceScript>().rank;
                    opposingPieceType = board.objectAndPos[(posX + 1) + "" + posY].GetComponent<GamePieceScript>().playerType;
                    break;
                }
            case 2:
                {
                    opposingPiece = board.objectAndPos[posX + "" + (posY - 1)];
                    newPos = posX + "" + (posY - 1);
                    newPosY = posY - 1;
                    if (opposingPiece == null)
                        break;
                    opposingPieceRank = board.objectAndPos[posX + "" + (posY - 1)].GetComponent<GamePieceScript>().rank;
                    opposingPieceType = board.objectAndPos[posX + "" + (posY - 1)].GetComponent<GamePieceScript>().playerType;
                    break;
                }
            case 3:
                {
                    opposingPiece = board.objectAndPos[posX + "" + (posY + 1)];
                    newPos = posX + "" + (posY + 1);
                    newPosY = posY + 1;
                    if (opposingPiece == null)
                        break;
                    opposingPieceRank = board.objectAndPos[posX + "" + (posY + 1)].GetComponent<GamePieceScript>().rank;
                    opposingPieceType = board.objectAndPos[posX + "" + (posY + 1)].GetComponent<GamePieceScript>().playerType;
                    break;
                }
        }


        if (opposingPiece != null)
        {
           
            if (aiPieceRank == 2 && opposingPieceRank == 15 && aiPieceType != opposingPieceType){
                

                board.objectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""+ aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
                board.objectAndPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]] = aiPiece.gameObject;
               
                opposingPiece.GetComponent<GamePieceScript>().isPlaced = false;   
                board.occupiedPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0],
                    aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                board.occupiedPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0],
                    opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]] = true;
                board.tileObjectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = false;
                board.tileObjectAndPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = true;
               
                opposingPiece.GetComponent<GamePieceScript>().isDead = true;
                
                opposingPiece.GetComponent<GamePieceScript>().suspectedRankValue = 15;
                instance.suspectedList.Remove(opposingPieceRank);
                instance.usedSuspectedList.Add(opposingPieceRank);
                aiPiece.GetComponent<GamePieceScript>().isVisible = true;
               
                aiPiece.GetComponent<RectTransform>().anchoredPosition = opposingPiece.GetComponent<RectTransform>().anchoredPosition;
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = opposingPiece.GetComponent<GamePieceScript>().piecePosition[0];
                aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = opposingPiece.GetComponent<GamePieceScript>().piecePosition[1];
                aiPiece.GetComponent<GamePieceScript>().isPlaced = true; 
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1; 
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1; 
     
               board.transferDeadPiece(opposingPiece.gameObject);
               
            }
            
            else if (aiPieceRank == 15 && opposingPieceRank == 2 &&aiPieceType != opposingPieceType) {
               
                board.objectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
                
                board.occupiedPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0], aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                board.tileObjectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = false;
                
                aiPiece.GetComponent<GamePieceScript>().isDead = true;
                
                opposingPiece.GetComponent<GamePieceScript>().suspectedRankValue = 2;
                instance.suspectedList.Remove(aiPieceRank);
                instance.usedSuspectedList.Add(aiPieceRank);
                
                aiPiece.GetComponent<GamePieceScript>().isPlaced = false;   
    
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1; 
                aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1; 
                
                board.transferDeadPiece(aiPiece.gameObject);
               
            }
          
            else if (aiPieceRank > opposingPieceRank && aiPieceType != opposingPieceType){
                
                board.objectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
                board.objectAndPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]] = aiPiece.gameObject;
               
                board.occupiedPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0], aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                board.occupiedPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0], opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]] = true;
                board.tileObjectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = false;
                board.tileObjectAndPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = true;
        
                opposingPiece.GetComponent<GamePieceScript>().isDead = true;
               
                opposingPiece.GetComponent<GamePieceScript>().suspectedRankValue = instance.suspectAgain(opposingPiece.gameObject, aiPiece.GetComponent<GamePieceScript>().rank, false);
                instance.suspectedList.Remove(opposingPiece.GetComponent<GamePieceScript>().suspectedRankValue);
                instance.usedSuspectedList.Add(opposingPiece.GetComponent<GamePieceScript>().suspectedRankValue);
                if (aiPiece.GetComponent<GamePieceScript>().rank == 15 && opposingPiece.GetComponent<GamePieceScript>().suspectedRankValue == 14)
                {
                    aiPiece.GetComponent<GamePieceScript>().isVisible = true;
                }
             
                opposingPiece.GetComponent<GamePieceScript>().isPlaced = false;   
              
                aiPiece.GetComponent<RectTransform>().anchoredPosition = opposingPiece.GetComponent<RectTransform>().anchoredPosition;
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = opposingPiece.GetComponent<GamePieceScript>().piecePosition[0];
                aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = opposingPiece.GetComponent<GamePieceScript>().piecePosition[1];
                aiPiece.GetComponent<GamePieceScript>().isPlaced = true;  
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1; 
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1;
               
                if(aiPieceRank == 15 && opposingPieceRank == 14)
                {
                    foreach (var item in board.enemyPiecesList)
                    {
                        if(item.GetComponent<GamePieceScript>().rank == 2)
                        {
                            item.GetComponent<GamePieceScript>().suspectedRankValue = 2;
                        }
                    }
                }
             
                board.transferDeadPiece(opposingPiece.gameObject);
                if (opposingPiece.GetComponent<GamePieceScript>().rankName == "Flag")
                {
                   board.GameCondition(aiPiece.GetComponent<GamePieceScript>().playerType); 
                }
              
            }
           
            else if (aiPieceRank < opposingPieceRank &&
                aiPieceType != opposingPieceType)
            {
               
                board.objectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
               
                board.occupiedPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0], aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                board.tileObjectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = false;
               
                aiPiece.GetComponent<GamePieceScript>().isDead = true;
               
                opposingPiece.GetComponent<GamePieceScript>().suspectedRankValue = instance.suspectAgain(opposingPiece.gameObject, aiPiece.GetComponent<GamePieceScript>().rank, true);
                instance.suspectedList.Remove(aiPieceRank);
                instance.usedSuspectedList.Add(aiPieceRank);
                
                aiPiece.GetComponent<GamePieceScript>().isPlaced = false;   
               
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1; 
                aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1; 
             
                board.transferDeadPiece(aiPiece.gameObject);
                if (aiPiece.GetComponent<GamePieceScript>().rankName == "Flag")
                {
                    board.GameCondition(opposingPiece.GetComponent<GamePieceScript>().playerType); 
                }
               
            }
           
            else if (aiPieceRank == opposingPieceRank &&
                aiPieceType != opposingPieceType)
            {
               
                board.objectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
                board.objectAndPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
               
                board.occupiedPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0], aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                board.occupiedPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0], opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                board.tileObjectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = false;
                board.tileObjectAndPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = false;
                
                aiPiece.GetComponent<GamePieceScript>().isDead = true;
                opposingPiece.GetComponent<GamePieceScript>().isDead = true;
         
                opposingPiece.GetComponent<GamePieceScript>().suspectedRankValue = opposingPieceRank;
                opposingPiece.GetComponent<GamePieceScript>().suspectIsSure = true;
                if(!suspectedList.Contains(opposingPieceRank))
                {
                    foreach (var item in board.playerPiecesList)
                    {
                        if(item.GetComponent<GamePieceScript>().suspectedRankValue == opposingPieceRank && !item.GetComponent<GamePieceScript>().suspectIsSure)
                        {
                            if(instance.suspectedList.Count == 2)
                            {
                                item.GetComponent<GamePieceScript>().suspectedRankValue = suspectedList[1];
                                item.GetComponent<GamePieceScript>().suspectIsSure = true;
                            }
                            else
                            {
                                item.GetComponent<GamePieceScript>().suspectedRankValue = 0;
                            }
                        }
                    }
                }
                else
                {
                    instance.suspectedList.Remove(opposingPieceRank);
                    instance.usedSuspectedList.Add(opposingPieceRank);
                }
               
                aiPiece.GetComponent<GamePieceScript>().isPlaced = false;   
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1; 
                aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1; 
          
                opposingPiece.GetComponent<GamePieceScript>().isPlaced = false;   
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1;
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1; 
              
                board.transferDeadPiece(aiPiece.gameObject);
                board.transferDeadPiece(opposingPiece.gameObject);
                if (aiPiece.GetComponent<GamePieceScript>().rankName == "Flag")
                {
                    board.GameCondition(aiPiece.GetComponent<GamePieceScript>().playerType); 
                }
              
            }
        }
        else
        {
           
           
            board.objectAndPos[posX + "" + posY] = null;
            board.objectAndPos[newPos] = aiPiece.gameObject;

            aiPiece.GetComponent<RectTransform>().anchoredPosition = board.tileObjectAndPos[newPosX + "" + newPosY].GetComponent<RectTransform>().anchoredPosition; 
            board.occupiedPos[posX, posY] = false;
            board.occupiedPos[newPosX, newPosY] = true;
            aiPiece.GetComponent<GamePieceScript>().isPlaced = true;
            aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = newPosX;
            aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = newPosY;
            board.tileObjectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = true;
            if (aiPiece.GetComponent<GamePieceScript>().rankName == "Flag" && aiPiece.GetComponent<GamePieceScript>().playerType == "ai2" && 
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] == 7)
            {
                board.GameCondition(aiPiece.GetComponent<GamePieceScript>().playerType);
            }
            
        }
        
        script.setTurn("human");

    }

    public int getSuspectedValue(int aiPieceRank, bool isEaten)
    {
        List<int> possible = new List<int>();
        int suspectedValue = 0;
        if(!isEaten)
        {
            for(int i = 2; i < aiPieceRank; i++)
            {
                if(instance.suspectedList.Contains(i))
                {
                    possible.Add(i);
                }
            }
            if (possible.Count == 0)
                return 0;
            suspectedValue = possible[possible.Count < 2 ? 0 : possible.Count / 2 -1];
        }
        else
        {
            for (int i = aiPieceRank + 1; i < (aiPieceRank == 2 ? 15 : 16); i++)
            {
                if (instance.suspectedList.Contains(i))
                {
                    possible.Add(i);
                }
            }
            if (possible.Count == 0)
                return 0;
            suspectedValue = possible[possible.Count < 2 ? 0 : possible.Count / 2 - 1];
        }
        return suspectedValue;
    }


    public int suspectAgain(GameObject piece, int aiPieceRank, bool isEaten)
    {
        int currentValue = instance.getSuspectedValue(aiPieceRank, isEaten);
        if (piece.GetComponent<GamePieceScript>().suspectedRankValue != 0 && currentValue > piece.GetComponent<GamePieceScript>().suspectedRankValue)
        {
            instance.suspectedList.Add(piece.GetComponent<GamePieceScript>().suspectedRankValue);
            instance.usedSuspectedList.Remove(piece.GetComponent<GamePieceScript>().suspectedRankValue);
        }
        else if(piece.GetComponent<GamePieceScript>().suspectedRankValue != 0 && currentValue <= piece.GetComponent<GamePieceScript>().suspectedRankValue)
        {
            currentValue = piece.GetComponent<GamePieceScript>().suspectedRankValue;
        }
        
        return currentValue;
    }

    private List<int> returnPossibleMoves(int[] piecePos, bool[,] occupiedPos)
    {
        string typeUp = "none", typeDown = "none", typeLeft = "none", typeRight = "none";
        GameObject [] sample = new GameObject[4] { null,null,null,null};
        List<int> li = new List<int>();

      
        if (piecePos[0] - 1 >= 0)
        {
            sample[0] = board.objectAndPos[(piecePos[0] - 1) + "" + piecePos[1]];
        }
        
        if (piecePos[0] + 1 < occupiedPos.GetLength(0))
        {
            sample[1] = board.objectAndPos[(piecePos[0] + 1) + "" + piecePos[1]];
        }
      
        if (piecePos[1] - 1 >= 0)
        {
            sample[2] = board.objectAndPos[piecePos[0] + "" + (piecePos[1] - 1)];
        }
      
        if (piecePos[1] + 1 < occupiedPos.GetLength(1))
        {
            sample[3] = board.objectAndPos[piecePos[0] + "" + (piecePos[1] + 1)];
        }
           
        if(sample[0] != null)
            typeUp = sample[0].GetComponent<GamePieceScript>().playerType;
        if (sample[1] != null)
            typeDown = sample[1].GetComponent<GamePieceScript>().playerType;
        if (sample[2] != null)
            typeLeft = sample[2].GetComponent<GamePieceScript>().playerType;
        if (sample[3] != null)
            typeRight = sample[3].GetComponent<GamePieceScript>().playerType;

        
       
        if ((true && piecePos[0] - 1 >= 0) && (typeUp == "human" || occupiedPos[piecePos[0] - 1, piecePos[1]] != true))
            li.Add(0);
        
        if ((true && piecePos[0] + 1 < occupiedPos.GetLength(0)) && (typeDown == "human" || occupiedPos[piecePos[0] + 1, piecePos[1]] != true))
            li.Add(1);
        
        if ((true && piecePos[1] - 1 >= 0) && (typeLeft == "human" || occupiedPos[piecePos[0], piecePos[1] - 1] != true))
            li.Add(2);
       
        if ((true && piecePos[1] + 1 < occupiedPos.GetLength(1)) && (typeRight == "human" || occupiedPos[piecePos[0], piecePos[1] + 1] != true))
            li.Add(3);

        return li;
    }

    public void randomPreConstructionAI(GameObject[] pieceList)
    {
        
        int x, y;
        Random rnd = new Random();
        for (int i = 0; i < pieceList.Length;i++)
        {
            x = rnd.Next(0, board.tileList.GetLength(0)/2-1);
            y = rnd.Next(0, board.tileList.GetLength(1));
            if (board.occupiedPos[x, y] != true)
            {
                board.tileObjectAndPos[x + "" + y].GetComponent<TileScript>().occupied = true;

                pieceList[i].GetComponent<GamePieceScript>().piecePosition[0] = x;
                pieceList[i].GetComponent<GamePieceScript>().piecePosition[1] = y;
                board.occupiedPos[x, y] = true;
                pieceList[i].GetComponent<GamePieceScript>().isPlaced = true;
                pieceList[i].GetComponent<RectTransform>().anchoredPosition = board.tileList[x, y].GetComponent<RectTransform>().anchoredPosition;
                board.objectAndPos[x + "" + y] = pieceList[i];
            }
            else
            {
                i--;
                continue;
            }
        }
    }

   

    
   



}

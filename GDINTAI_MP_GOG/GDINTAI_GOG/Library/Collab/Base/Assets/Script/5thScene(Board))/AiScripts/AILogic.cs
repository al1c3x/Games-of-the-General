using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class AILogic : MonoBehaviour
{
    private BoardScript bs;
    private GameManagerScript gms;
    void Awake()
    {
        bs = FindObjectOfType<BoardScript>();
        gms = FindObjectOfType<GameManagerScript>();
    }

    public void AiMove(GameObject[] pieceList)
    {
        int randomPieceIndex, randomMoveIndex;  //randomMove - 0-Up,1-Down,2-Left,3-Right
        Random rand = new Random();
        List<GameObject> remainingPieces = new List<GameObject>();
        foreach (var x in pieceList)
        {
            if (x.GetComponent<GamePieceScript>().isPlaced != false)
                remainingPieces.Add(x);
        }
        //checks if the randomMove is valid
        while(true)
        {
            randomPieceIndex = rand.Next(0, remainingPieces.Count);
            randomMoveIndex = rand.Next(0, 4);
            if (remainingPieces.Count == 0)
                break;
            else if (moveIsValid(randomMoveIndex, remainingPieces[randomPieceIndex].GetComponent<GamePieceScript>().piecePosition, bs.occupiedPos))
                break;
            else
                continue;
        }
        int pieceIndex = randomPieceIndex;
        int moveIndex = randomMoveIndex;
        move(remainingPieces[pieceIndex], moveIndex);
        gms.setTurn("human");
        bs.turnOnInteractable();
        Debug.Log("ARRAY OF PIECE Positions");
        foreach (var item in pieceList)
        {
            Debug.Log("Piece: " + item.GetComponent<GamePieceScript>().rankName + " Position: " + item.GetComponent<GamePieceScript>().piecePosition[0] + ":" + item.GetComponent<GamePieceScript>().piecePosition[1]);
        }
        Debug.Log("ARRAY OF Occupied Positions");
        for (int x = 0; x < bs.occupiedPos.GetLength(0); x++)
        {
            for (int y = 0; y < bs.occupiedPos.GetLength(1); y++)
            {
                Debug.Log(" Occupied Position: " + x + ":" + y + " = " + bs.occupiedPos[x, y]);
            }
        }
    }

    private void move(GameObject aiPiece, int moveIndex)
    {
        Debug.Log("Ai's Piece: " + aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + ":" + aiPiece.GetComponent<GamePieceScript>().piecePosition[1] + " Move: " + moveIndex);
        int posX = aiPiece.GetComponent<GamePieceScript>().piecePosition[0];
        int posY = aiPiece.GetComponent<GamePieceScript>().piecePosition[1];
        int aiPieceRank = bs.objectAndPos[posX + "" + posY].GetComponent<GamePieceScript>().rank;
        string aiPieceType = bs.objectAndPos[posX + "" + posY].GetComponent<GamePieceScript>().playerType;
        int opposingPieceRank = 0;
        string opposingPieceType = "none";
        string newPos = "none";
        int newPosX = posX, newPosY = posY;
        GameObject opposingPiece = null;
        switch (moveIndex)
        {
            case 0:
                {
                    opposingPiece = bs.objectAndPos[(posX - 1) + "" + posY];
                    newPos = (posX - 1) + "" + posY;
                    newPosX = posX - 1;
                    if (opposingPiece == null)
                        break;
                    opposingPieceRank = bs.objectAndPos[(posX - 1) + "" + posY].GetComponent<GamePieceScript>().rank;
                    opposingPieceType = bs.objectAndPos[(posX - 1) + "" + posY].GetComponent<GamePieceScript>().playerType;
                    break;
                }
            case 1:
                {
                    opposingPiece = bs.objectAndPos[(posX + 1) + "" + posY];
                    newPos = (posX + 1) + "" + posY;
                    newPosX = posX + 1;
                    if (opposingPiece == null)
                        break;
                    opposingPieceRank = bs.objectAndPos[(posX + 1) + "" + posY].GetComponent<GamePieceScript>().rank;
                    opposingPieceType = bs.objectAndPos[(posX + 1) + "" + posY].GetComponent<GamePieceScript>().playerType;
                    break;
                }
            case 2:
                {
                    opposingPiece = bs.objectAndPos[posX + "" + (posY - 1)];
                    newPos = posX + "" + (posY - 1);
                    newPosY = posY - 1;
                    if (opposingPiece == null)
                        break;
                    opposingPieceRank = bs.objectAndPos[posX + "" + (posY - 1)].GetComponent<GamePieceScript>().rank;
                    opposingPieceType = bs.objectAndPos[posX + "" + (posY - 1)].GetComponent<GamePieceScript>().playerType;
                    break;
                }
            case 3:
                {
                    opposingPiece = bs.objectAndPos[posX + "" + (posY + 1)];
                    newPos = posX + "" + (posY + 1);
                    newPosY = posY + 1;
                    if (opposingPiece == null)
                        break;
                    opposingPieceRank = bs.objectAndPos[posX + "" + (posY + 1)].GetComponent<GamePieceScript>().rank;
                    opposingPieceType = bs.objectAndPos[posX + "" + (posY + 1)].GetComponent<GamePieceScript>().playerType;
                    break;
                }
        }


        if (opposingPiece != null)
        {

            if (aiPieceRank == 3 && opposingPieceRank == 16 &&
                aiPieceType != opposingPieceType)
            {
                Debug.Log("Ai Private eats Spy");
                //eat this object
                opposingPiece.GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
                                                                                  //set the occupiedPosition of the board
                bs.occupiedPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0],
                    aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = true;
                bs.occupiedPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0],
                    opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                //replace the new piece to the tile
                aiPiece.GetComponent<RectTransform>().anchoredPosition = opposingPiece.GetComponent<RectTransform>().anchoredPosition;
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = opposingPiece.GetComponent<GamePieceScript>().piecePosition[0];
                aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = opposingPiece.GetComponent<GamePieceScript>().piecePosition[1];
                aiPiece.GetComponent<GamePieceScript>().isPlaced = true;  //new piece is placed
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
                opposingPiece.SetActive(false); //removes the eaten piece
                /*Debug.Log("Top: " + piece.GetComponent<GamePieceScript>().rankName);
                Debug.Log("Bottom: " + GetComponent<GamePieceScript>().rankName);*/
            }
            //when a spy attacks a private
            else if (aiPieceRank == 16 && opposingPieceRank == 3 &&
                aiPieceType != opposingPieceType)
            {
                Debug.Log("Ai spy eats private");
                bs.objectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
                //set the occupiedPosition of the board
                bs.occupiedPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0], aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                //eat this object
                aiPiece.GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
                                                                            //replace the new piece to the tile
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
                aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
                aiPiece.SetActive(false); //removes the eaten piece
                /*Debug.Log("Top: " + piece.GetComponent<GamePieceScript>().rankName);
                Debug.Log("Bottom: " + GetComponent<GamePieceScript>().rankName);*/
            }
            //when the attacking piece is stronger
            else if (aiPieceRank > opposingPieceRank &&
                aiPieceType != opposingPieceType)
            {
                Debug.Log("Ai strong eats weak");
                bs.objectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
                bs.objectAndPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]] = aiPiece.gameObject;
                //set the occupiedPosition of the board
                bs.occupiedPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0], aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = true;
                bs.occupiedPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0], opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                //eat this object
                opposingPiece.GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
                                                                    //replace the new piece to the tile
                aiPiece.GetComponent<RectTransform>().anchoredPosition = opposingPiece.GetComponent<RectTransform>().anchoredPosition;
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = opposingPiece.GetComponent<GamePieceScript>().piecePosition[0];
                aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = opposingPiece.GetComponent<GamePieceScript>().piecePosition[1];
                aiPiece.GetComponent<GamePieceScript>().isPlaced = true;  //new piece is placed
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
                opposingPiece.SetActive(false); //removes the eaten piece
                /*Debug.Log("Top: " + piece.GetComponent<GamePieceScript>().rank);
                Debug.Log("Bottom: " + GetComponent<GamePieceScript>().rank);*/
            }
            //when the attacking piece is weaker
            else if (aiPieceRank < opposingPieceRank &&
                aiPieceType != opposingPieceType)
            {
                Debug.Log("Ai weak eats strong");
                bs.objectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
                //set the occupiedPosition of the board
                bs.occupiedPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0], aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                //eat this object
                aiPiece.GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
                                                                            //replace the new piece to the tile
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
                aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
                aiPiece.SetActive(false); //removes the eaten piece
                /*Debug.Log("Top: " + piece.GetComponent<GamePieceScript>().rank);
                Debug.Log("Bottom: " + GetComponent<GamePieceScript>().rank);*/
            }
            //when both piece have the same rank
            else if (aiPieceRank == opposingPieceRank &&
                aiPieceType != opposingPieceType)
            {
                Debug.Log("Ai same eats same");
                bs.objectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
                bs.objectAndPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
                //set the occupiedPosition of the board
                bs.occupiedPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0], aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                bs.occupiedPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0], opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                //eat this object
                aiPiece.GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
                aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
                aiPiece.gameObject.SetActive(false); //removes the eaten piece
                opposingPiece.GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
                opposingPiece.SetActive(false); //removes the eaten piece
                /*Debug.Log("Top: " + piece.GetComponent<GamePieceScript>().rankName);
                Debug.Log("Bottom: " + GetComponent<GamePieceScript>().rankName);*/
            }
        }
        else
        {
            Debug.Log("Ai normal move");
            bs.objectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
            bs.objectAndPos[newPos] = aiPiece.gameObject;

            aiPiece.GetComponent<RectTransform>().anchoredPosition = bs.tileObjectAndPos[newPosX+""+newPosY].GetComponent<RectTransform>().anchoredPosition; //anchoredPosition is the pivot/origin of the object
            bs.occupiedPos[newPosX, newPosY] = true;
            aiPiece.GetComponent<GamePieceScript>().isPlaced = true;
            aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = newPosX;
            aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = newPosY;
        }
        //change side and turn off interactable for all the player's pieces
        gms.setTurn("human");
    }

    //moveIsValid(randomMoveIndex, remainingPieces[randomPieceIndex].GetComponent<GamePieceScript>().piecePosition, bs.occupiedPos)
    private bool moveIsValid(int moveIndex, int[] piecePos, bool[,] occupiedPos)
    {
        string typeUp = "none", typeDown = "none", typeLeft = "none", typeRight = "none";
        GameObject sample = null;

        Debug.Log("start suspect: " + piecePos[0] + ":" + piecePos[1]);
        if (moveIndex == 0)
        {
            if (piecePos[0] - 1 < 0 )
                return false;
            Debug.Log("RunUp: " + piecePos[0] + ":" + piecePos[1]);
            sample = bs.objectAndPos[(piecePos[0] - 1) + "" + piecePos[1]];
        }
        //going Down
        else if (moveIndex == 1)
        {
            if (piecePos[0] + 1 >= occupiedPos.GetLength(0))
                return false;
            Debug.Log("RunDown: " + piecePos[0] + ":" + piecePos[1]);
            sample = bs.objectAndPos[(piecePos[0] + 1) + "" + piecePos[1]];
        }
        //going Left
        else if (moveIndex == 2)
        {
            if (piecePos[1] - 1 < 0)
                return false;
            Debug.Log("RunLeft: " + piecePos[0] + ":" + piecePos[1]);
            sample = bs.objectAndPos[piecePos[0] + "" + (piecePos[1] - 1)];
        }
        //going Right
        else if (moveIndex == 3)
        {
            if (piecePos[1] + 1 >= occupiedPos.GetLength(1))
                return false;
            Debug.Log("RunRight: " + piecePos[0] + ":" + piecePos[1]);
            sample = bs.objectAndPos[piecePos[0] + "" + (piecePos[1] + 1)];
        }
        Debug.Log("end suspect");
        if (sample != null)
        {
            //checks if the tilePosition to traverse has a piece; determines the type of the piece
            typeUp = sample.GetComponent<GamePieceScript>().playerType;
            typeDown = sample.GetComponent<GamePieceScript>().playerType;
            typeLeft = sample.GetComponent<GamePieceScript>().playerType;
            typeRight = sample.GetComponent<GamePieceScript>().playerType;
        }

        Debug.Log("Length 0: " + occupiedPos.GetLength(0));
        Debug.Log("Length 1: " + occupiedPos.GetLength(1));
        //going Up
        if ((moveIndex == 0 && piecePos[0] - 1 >= 0) && (typeUp == "human" || occupiedPos[piecePos[0] - 1, piecePos[1]] != true))
            return true;
        //going Down
        else if ((moveIndex == 1 && piecePos[0] + 1 < occupiedPos.GetLength(0)) && (typeDown == "human" || occupiedPos[piecePos[0] + 1, piecePos[1]] != true))
            return true;
        //going Left
        else if ((moveIndex == 2 && piecePos[1] - 1 >= 0) && (typeLeft == "human" || occupiedPos[piecePos[0], piecePos[1] - 1] != true))
            return true;
        //going Right
        else if ((moveIndex == 3 && piecePos[1] + 1 < occupiedPos.GetLength(1)) && (typeRight == "human" || occupiedPos[piecePos[0], piecePos[1] + 1] != true))
            return true;

        return false;
    }

    public void preConstructionAI(int dFieldStartX, int dFieldMaxX, GameObject[] pieceList)
    {
        int x;
        int posX, posY;
        Random rnd = new Random();
        for (x = 0; x < pieceList.Length; x++)
        {
            posX = rnd.Next(dFieldStartX, dFieldMaxX);
            posY = rnd.Next(0, bs.tileList.GetLength(1));
            if (bs.occupiedPos[posX,posY] != true)
            {
                pieceList[x].GetComponent<GamePieceScript>().piecePosition[0] = posX;
                pieceList[x].GetComponent<GamePieceScript>().piecePosition[1] = posY;
                bs.occupiedPos[posX, posY] = true;
                pieceList[x].GetComponent<GamePieceScript>().isPlaced = true;
                pieceList[x].GetComponent<RectTransform>().anchoredPosition = 
                    bs.tileList[posX, posY].GetComponent<RectTransform>().anchoredPosition;
                bs.objectAndPos[posX + "" + posY] = pieceList[x];
            }
            else
            {
                --x;
                continue;
            }
        }
    }

}

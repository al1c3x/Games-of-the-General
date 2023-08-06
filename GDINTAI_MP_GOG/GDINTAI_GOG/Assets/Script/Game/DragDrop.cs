using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Math = System.Math;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerUpHandler, IDropHandler
{
    private Canvas canvas; 
    private CanvasGroup canvasGroup;
    private BoardScript boardscript;
    private GamePieceScript gamescript;
    private GameManagerScript gms;
    private RectTransform pieceTransform;
    public Vector3 previousPosition;
    public int [] previousIndexPosition = new int[2];
    public GameObject piece;
    private AILogic aiScript;

    private RectTransform rectTransform; //this is instantiated so that we can access the object’s translation

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        boardscript = FindObjectOfType<BoardScript>();
        gms = FindObjectOfType<GameManagerScript>();
        gamescript = GetComponent<GamePieceScript>();
        pieceTransform = GetComponent<RectTransform>();
        aiScript = FindObjectOfType<AILogic>();
    }
    private void Start()
    {
        canvas = Canvas.FindObjectOfType<Canvas>();
        previousIndexPosition = new int[2]{-2, -2};
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; 
    }

    public void OnDrag(PointerEventData eventData) 
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        gamescript.isPlaced = false;
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData) 
    {
        previousPosition = pieceTransform.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        if (gamescript.isPlaced == false || gms.gameState == "in-game")
        {
            
            pieceTransform.position = previousPosition;
            if ((gamescript.piecePosition[0] != -1 && gamescript.piecePosition[1] != -1) && boardscript.occupiedPos[gamescript.piecePosition[0], gamescript.piecePosition[1]] == true)
                gamescript.isPlaced = true;
        }
        boardscript.occupiedPos[GetComponent<GamePieceScript>().piecePosition[0] , GetComponent<GamePieceScript>().piecePosition[1]] = false;
        boardscript.tileObjectAndPos[GetComponent<GamePieceScript>().piecePosition[0] + "" + GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = true;
    }

    public void OnDrop(PointerEventData eventData) 
    {
       
        piece = eventData.pointerDrag.gameObject;
        int distanceX = Math.Abs(piece.GetComponent<GamePieceScript>().piecePosition[0] - gameObject.GetComponent<GamePieceScript>().piecePosition[0]);
        int distanceY = Math.Abs(piece.GetComponent<GamePieceScript>().piecePosition[1] - gameObject.GetComponent<GamePieceScript>().piecePosition[1]);
        bool valid = (distanceX <= 1 && distanceY <= 1) && !(distanceX == 1 && distanceY == 1);
        
        if (valid && piece.GetComponent<GamePieceScript>().rank == 2 && GetComponent<GamePieceScript>().rank == 15 &&
            piece.GetComponent<GamePieceScript>().playerType != GetComponent<GamePieceScript>().playerType)
        {

            boardscript.objectAndPos[piece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                + piece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
            boardscript.objectAndPos[GetComponent<GamePieceScript>().piecePosition[0] + ""
                + GetComponent<GamePieceScript>().piecePosition[1]] = piece.gameObject;
           
            GetComponent<GamePieceScript>().isPlaced = false;

            boardscript.occupiedPos[piece.GetComponent<GamePieceScript>().piecePosition[0],piece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
            boardscript.occupiedPos[GetComponent<GamePieceScript>().piecePosition[0], GetComponent<GamePieceScript>().piecePosition[1]] = true;
            boardscript.tileObjectAndPos[piece.GetComponent<GamePieceScript>().piecePosition[0] + "" + piece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = false;
            boardscript.tileObjectAndPos[GetComponent<GamePieceScript>().piecePosition[0] + "" + GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = true;
       
            GetComponent<GamePieceScript>().isDead = true;
            
            piece.GetComponent<GamePieceScript>().suspectedRankValue = piece.GetComponent<GamePieceScript>().rank;
            AILogic.instance.suspectedList.Remove(piece.GetComponent<GamePieceScript>().rank);
            AILogic.instance.usedSuspectedList.Add(piece.GetComponent<GamePieceScript>().rank);
         
            piece.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            piece.GetComponent<GamePieceScript>().piecePosition[0] = GetComponent<GamePieceScript>().piecePosition[0];
            piece.GetComponent<GamePieceScript>().piecePosition[1] = GetComponent<GamePieceScript>().piecePosition[1];
            piece.GetComponent<GamePieceScript>().isPlaced = true;  
            GetComponent<GamePieceScript>().piecePosition[0] = -1; 
            GetComponent<GamePieceScript>().piecePosition[1] = -1;

            boardscript.transferDeadPiece(gameObject);
            gms.turn = "ai2";
            
        }
       
        else if (valid && piece.GetComponent<GamePieceScript>().rank == 15 && GetComponent<GamePieceScript>().rank == 2 &&
            piece.GetComponent<GamePieceScript>().playerType != GetComponent<GamePieceScript>().playerType)
        {

            boardscript.objectAndPos[piece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                + piece.GetComponent<GamePieceScript>().piecePosition[1]] = null;

            boardscript.occupiedPos[piece.GetComponent<GamePieceScript>().piecePosition[0], piece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
            boardscript.tileObjectAndPos[piece.GetComponent<GamePieceScript>().piecePosition[0] + "" + piece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = false;
            
            piece.GetComponent<GamePieceScript>().isDead = true;
          
            piece.GetComponent<GamePieceScript>().suspectedRankValue = piece.GetComponent<GamePieceScript>().rank;
            AILogic.instance.suspectedList.Remove(piece.GetComponent<GamePieceScript>().rank);
            AILogic.instance.usedSuspectedList.Add(piece.GetComponent<GamePieceScript>().rank);
            GetComponent<GamePieceScript>().isVisible = true;
            
            piece.GetComponent<GamePieceScript>().isPlaced = false;   
            
            piece.GetComponent<GamePieceScript>().piecePosition[0] = -1; 
            piece.GetComponent<GamePieceScript>().piecePosition[1] = -1;

            boardscript.transferDeadPiece(piece.gameObject);
            gms.turn = "ai2";
            
        }
        
    
        else if (valid && piece.GetComponent<GamePieceScript>().rank < GetComponent<GamePieceScript>().rank &&
            piece.GetComponent<GamePieceScript>().playerType != GetComponent<GamePieceScript>().playerType)
        {

            boardscript.objectAndPos[piece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                + piece.GetComponent<GamePieceScript>().piecePosition[1]] = null;

            boardscript.occupiedPos[piece.GetComponent<GamePieceScript>().piecePosition[0], piece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
            boardscript.tileObjectAndPos[piece.GetComponent<GamePieceScript>().piecePosition[0] + "" + piece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = false;
            boardscript.tileObjectAndPos[GetComponent<GamePieceScript>().piecePosition[0] + "" + GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = true;
           
            piece.GetComponent<GamePieceScript>().isDead = true;
            
            piece.GetComponent<GamePieceScript>().suspectedRankValue = AILogic.instance.suspectAgain(piece.gameObject, GetComponent<GamePieceScript>().rank, false);
            AILogic.instance.suspectedList.Remove(piece.GetComponent<GamePieceScript>().suspectedRankValue);
            AILogic.instance.usedSuspectedList.Add(piece.GetComponent<GamePieceScript>().suspectedRankValue);
            if (GetComponent<GamePieceScript>().rank == 15 && piece.GetComponent<GamePieceScript>().suspectedRankValue == 14)
            {
                GetComponent<GamePieceScript>().isVisible = true;
            }
            if (GetComponent<GamePieceScript>().rank == 15 && piece.GetComponent<GamePieceScript>().rank == 14)
            {
                foreach (var item in boardscript.enemyPiecesList)
                {
                    if (item.GetComponent<GamePieceScript>().rank == 2)
                    {
                        item.GetComponent<GamePieceScript>().suspectedRankValue = 2;
                    }
                }
            }
            
            piece.GetComponent<GamePieceScript>().isPlaced = false;   
           
            piece.GetComponent<GamePieceScript>().piecePosition[0] = -1; 
            piece.GetComponent<GamePieceScript>().piecePosition[1] = -1;

            boardscript.transferDeadPiece(piece.gameObject);
            gms.turn = "ai2";
            if (piece.GetComponent<GamePieceScript>().rankName == "Flag")
            {
                boardscript.GameCondition(GetComponent<GamePieceScript>().playerType);
            }
           
        }
       
        else if (valid && piece.GetComponent<GamePieceScript>().rank == GetComponent<GamePieceScript>().rank &&
            piece.GetComponent<GamePieceScript>().playerType != GetComponent<GamePieceScript>().playerType)
        {

            boardscript.objectAndPos[piece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                + piece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
            boardscript.objectAndPos[GetComponent<GamePieceScript>().piecePosition[0] + ""
                + GetComponent<GamePieceScript>().piecePosition[1]] = null;

            boardscript.occupiedPos[piece.GetComponent<GamePieceScript>().piecePosition[0], piece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
            boardscript.occupiedPos[GetComponent<GamePieceScript>().piecePosition[0], GetComponent<GamePieceScript>().piecePosition[1]] = false;
            boardscript.tileObjectAndPos[piece.GetComponent<GamePieceScript>().piecePosition[0] + "" + piece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = false;
            boardscript.tileObjectAndPos[GetComponent<GamePieceScript>().piecePosition[0] + "" + GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = false;
           
            piece.GetComponent<GamePieceScript>().isDead = true;
            GetComponent<GamePieceScript>().isDead = true;
          
            piece.GetComponent<GamePieceScript>().suspectedRankValue = piece.GetComponent<GamePieceScript>().rank;
            piece.GetComponent<GamePieceScript>().suspectIsSure = true;
            if (!AILogic.instance.suspectedList.Contains(piece.GetComponent<GamePieceScript>().rank))
            {
                foreach (var item in boardscript.playerPiecesList)
                {
                    if (item.GetComponent<GamePieceScript>().suspectedRankValue == piece.GetComponent<GamePieceScript>().rank && !item.GetComponent<GamePieceScript>().suspectIsSure)
                    {
                        if (AILogic.instance.suspectedList.Count == 2)
                        {
                            item.GetComponent<GamePieceScript>().suspectedRankValue = AILogic.instance.suspectedList[1];
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
                AILogic.instance.suspectedList.Remove(piece.GetComponent<GamePieceScript>().rank);
                AILogic.instance.usedSuspectedList.Add(piece.GetComponent<GamePieceScript>().rank);
            }
           
            piece.GetComponent<GamePieceScript>().isPlaced = false;  
            piece.GetComponent<GamePieceScript>().piecePosition[0] = -1; 
            piece.GetComponent<GamePieceScript>().piecePosition[1] = -1; 
            
            GetComponent<GamePieceScript>().isPlaced = false;   
            GetComponent<GamePieceScript>().piecePosition[0] = -1; 
            GetComponent<GamePieceScript>().piecePosition[1] = -1;

            boardscript.transferDeadPiece(gameObject);
            boardscript.transferDeadPiece(piece.gameObject);
            gms.turn = "ai2";
            if (piece.GetComponent<GamePieceScript>().rankName == "Flag")
            {
                boardscript.GameCondition(piece.GetComponent<GamePieceScript>().playerType); 
            }
            
        }

       
        if (gms.gameState == "in-game" )
        {
          if(gms.turn != "human")
            {
                boardscript.turnOffInteractable();
                gms.setTurn("ai2");
                aiScript.AiMove(boardscript.enemyPiecesList, boardscript.playerPiecesList);

            }
           
        }
       
    }


   

}


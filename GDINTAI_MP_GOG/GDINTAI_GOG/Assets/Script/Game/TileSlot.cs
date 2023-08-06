using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 
using Math = System.Math;

public class TileSlot : MonoBehaviour, IDropHandler
{
    public GameObject piece;
    private BoardScript bs;
    private TileScript ts;
    private GameManagerScript gms;
    private AILogic aiScript;

    void Awake()
    {
        bs = FindObjectOfType<BoardScript>();
        ts = GetComponent<TileScript>();
        gms = FindObjectOfType<GameManagerScript>();
        aiScript = FindObjectOfType<AILogic>();
    }

    public void OnDrop(PointerEventData eventData)
    {
       
        if (eventData.pointerDrag != null)
        {
            piece = eventData.pointerDrag.gameObject;
            int distanceX = Math.Abs(piece.GetComponent<GamePieceScript>().piecePosition[0] - gameObject.GetComponent<TileScript>().tilePosition[0]);
            int distanceY = Math.Abs(piece.GetComponent<GamePieceScript>().piecePosition[1] - gameObject.GetComponent<TileScript>().tilePosition[1]);
          
            if ((gms.gameState == "pre-game") ||
                (distanceX <= 1 && distanceY <= 1) && !(distanceX == 1 && distanceY == 1) &&
                (piece.GetComponent<GamePieceScript>().piecePosition[0] != gameObject.GetComponent<TileScript>().tilePosition[0] ||
                piece.GetComponent<GamePieceScript>().piecePosition[1] != gameObject.GetComponent<TileScript>().tilePosition[1]))
            {
                
                bs.objectAndPos[piece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + piece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
                bs.objectAndPos[GetComponent<TileScript>().tilePosition[0] + ""
                    + GetComponent<TileScript>().tilePosition[1]] = piece.gameObject;

                piece.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition; 
               
                bs.occupiedPos[ts.tilePosition[0], ts.tilePosition[1]] = true;
                GetComponent<TileScript>().occupied = true;
                piece.GetComponent<GamePieceScript>().isPlaced = true;
                piece.GetComponent<GamePieceScript>().piecePosition[0] = gameObject.GetComponent<TileScript>().tilePosition[0];
                piece.GetComponent<GamePieceScript>().piecePosition[1] = gameObject.GetComponent<TileScript>().tilePosition[1];
                piece.GetComponent<DragDrop>().previousIndexPosition[0] = gameObject.GetComponent<TileScript>().tilePosition[0];
                piece.GetComponent<DragDrop>().previousIndexPosition[1] = gameObject.GetComponent<TileScript>().tilePosition[1];

                if (piece.GetComponent<GamePieceScript>().rankName == "Flag" && piece.GetComponent<GamePieceScript>().playerType == "human" &&
                    piece.GetComponent<GamePieceScript>().piecePosition[0] == 0)
                {
                    bs.GameCondition(piece.GetComponent<GamePieceScript>().playerType); 
                }


               
            }
            else
            {
                piece.GetComponent<RectTransform>().position = piece.GetComponent<DragDrop>().previousPosition;
            }
        }

    }

}


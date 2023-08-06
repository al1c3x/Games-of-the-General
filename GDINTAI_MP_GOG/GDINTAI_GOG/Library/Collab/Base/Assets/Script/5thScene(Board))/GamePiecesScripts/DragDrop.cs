﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Math = System.Math;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerUpHandler, IDropHandler
{
    private Canvas canvas; //gets the canvas object and access its properties
    private CanvasGroup canvasGroup;
    private BoardScript bs;
    private GamePieceScript gps;
    private GameManagerScript gms;
    private RectTransform pieceTransform;
    [HideInInspector] public Vector3 previousPosition;
    [HideInInspector] public int [] previousIndexPosition = new int[2];
    [HideInInspector] public GameObject piece;
    private AILogic aiScript;

    private RectTransform rectTransform; //this is instantiated so that we can access the object’s translation

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        bs = FindObjectOfType<BoardScript>();
        gms = FindObjectOfType<GameManagerScript>();
        gps = GetComponent<GamePieceScript>();
        pieceTransform = GetComponent<RectTransform>();
        aiScript = FindObjectOfType<AILogic>();
    }
    private void Start()
    {
        canvas = Canvas.FindObjectOfType<Canvas>();
        previousIndexPosition = new int[2]{-2, -2};
    }

    public void OnBeginDrag(PointerEventData eventData) //this is for when the object starts to be drag
    {
        canvasGroup.alpha = 0.6f; //this makes the alpha slight transparent when its hold
        canvasGroup.blocksRaycasts = false; //this makes the ondrop to activate;
    }

    public void OnDrag(PointerEventData eventData) //this is for when the object is still being drag
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; //this field will make the object move along with the mouse
        //if the piece is removed from the recent tile
        if (gps.piecePosition[0] >= 0 && bs.occupiedPos[gps.piecePosition[0],
            gps.piecePosition[1]] == true && gms.gameState == "pre-game")
        {
            //Debug.Log("turns to negative 1");
            gps.isPlaced = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData) //this is for when the object is no longer drag or the click was over on the mouse
    {
        canvasGroup.alpha = 1.0f; //this makes the alpha back to normal
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData) //this is for when the object is pressed down
    {
        previousPosition = pieceTransform.position;
    }

    public void OnPointerUp(PointerEventData eventData) //this is for when the object is pressed down
    {
        //Debug.Log("OnPointerUp");
        //when the object is placed on the wrong tile
        if (gps.isPlaced == false || gms.gameState == "in-game")
        {
            //Debug.Log("OnPointerUp: Return to recent Tile");
            pieceTransform.position = previousPosition;
            if ((gps.piecePosition[0] != -1 && gps.piecePosition[1] != -1) && bs.occupiedPos[gps.piecePosition[0], gps.piecePosition[1]] == true)
                gps.isPlaced = true;
        }

    }

    public void OnDrop(PointerEventData eventData) //this is for when an object is placed/drop on the piece(eaten state)
    {
        //Debug.Log("OnDrop");
        //this->data is the dropArea and the piece is the piece being drag
        piece = eventData.pointerDrag.gameObject;
        int distanceX = Math.Abs(piece.GetComponent<GamePieceScript>().piecePosition[0] - gameObject.GetComponent<GamePieceScript>().piecePosition[0]);
        int distanceY = Math.Abs(piece.GetComponent<GamePieceScript>().piecePosition[1] - gameObject.GetComponent<GamePieceScript>().piecePosition[1]);
        bool valid = (distanceX <= 1 && distanceY <= 1) && !(distanceX == 1 && distanceY == 1);
        //Debug.Log(piece.GetComponent<GamePieceScript>().piecePosition[0] + ":"+ piece.GetComponent<GamePieceScript>().piecePosition[1]);
        //Debug.Log(gameObject.GetComponent<GamePieceScript>().piecePosition[0] + ":" + gameObject.GetComponent<GamePieceScript>().piecePosition[1]);
        //Debug.Log(valid);

        //when a private eats a spy
        if (valid && piece.GetComponent<GamePieceScript>().rank == 3 && GetComponent<GamePieceScript>().rank == 16 &&
            piece.GetComponent<GamePieceScript>().playerType != GetComponent<GamePieceScript>().playerType)
        {
            Debug.Log("Player Private eats Spy: " + piece.GetComponent<GamePieceScript>().piecePosition[0] + ":" + piece.GetComponent<GamePieceScript>().piecePosition[1]);
            bs.objectAndPos[piece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                + piece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
            bs.objectAndPos[GetComponent<GamePieceScript>().piecePosition[0] + ""
                + GetComponent<GamePieceScript>().piecePosition[1]] = piece.gameObject;
            //eat this object
            GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
            //set the occupiedPosition of the board
            bs.occupiedPos[piece.GetComponent<GamePieceScript>().piecePosition[0],piece.GetComponent<GamePieceScript>().piecePosition[1]] = true;
            bs.occupiedPos[GetComponent<GamePieceScript>().piecePosition[0], GetComponent<GamePieceScript>().piecePosition[1]] = false;
             //replace the new piece to the tile
            piece.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            piece.GetComponent<GamePieceScript>().piecePosition[0] = GetComponent<GamePieceScript>().piecePosition[0];
            piece.GetComponent<GamePieceScript>().piecePosition[1] = GetComponent<GamePieceScript>().piecePosition[1];
            piece.GetComponent<GamePieceScript>().isPlaced = true;  //new piece is placed
            GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
            GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
            gameObject.SetActive(false); //removes the eaten piece
            gms.turn = "ai1";
            /*Debug.Log("Top: " + piece.GetComponent<GamePieceScript>().rankName);
            Debug.Log("Bottom: " + GetComponent<GamePieceScript>().rankName);*/
        }
        //when a spy attacks a private
        else if (valid && piece.GetComponent<GamePieceScript>().rank == 16 && GetComponent<GamePieceScript>().rank == 3 &&
            piece.GetComponent<GamePieceScript>().playerType != GetComponent<GamePieceScript>().playerType)
        {
            Debug.Log("Player spy eats private: " + piece.GetComponent<GamePieceScript>().piecePosition[0] + ":" + piece.GetComponent<GamePieceScript>().piecePosition[1]);
            bs.objectAndPos[piece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                + piece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
            //set the occupiedPosition of the board
            bs.occupiedPos[piece.GetComponent<GamePieceScript>().piecePosition[0], piece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
            //eat this object
            piece.GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
            //replace the new piece to the tile
            piece.GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
            piece.GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
            piece.gameObject.SetActive(false); //removes the eaten piece
            gms.turn = "ai1";
            /*Debug.Log("Top: " + piece.GetComponent<GamePieceScript>().rankName);
            Debug.Log("Bottom: " + GetComponent<GamePieceScript>().rankName);*/
        }
        //when the attacking piece is stronger
        else if (valid && piece.GetComponent<GamePieceScript>().rank > GetComponent<GamePieceScript>().rank &&
            piece.GetComponent<GamePieceScript>().playerType != GetComponent<GamePieceScript>().playerType)
        {
            Debug.Log("Player strong eats weak: " + piece.GetComponent<GamePieceScript>().piecePosition[0] + ":" + piece.GetComponent<GamePieceScript>().piecePosition[1]);
            bs.objectAndPos[piece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                + piece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
            bs.objectAndPos[GetComponent<GamePieceScript>().piecePosition[0] + ""
                + GetComponent<GamePieceScript>().piecePosition[1]] = piece.gameObject;
            //set the occupiedPosition of the board
            bs.occupiedPos[piece.GetComponent<GamePieceScript>().piecePosition[0], piece.GetComponent<GamePieceScript>().piecePosition[1]] = true;
            bs.occupiedPos[GetComponent<GamePieceScript>().piecePosition[0], GetComponent<GamePieceScript>().piecePosition[1]] = false;
            //eat this object
            GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
            //replace the new piece to the tile
            piece.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            piece.GetComponent<GamePieceScript>().piecePosition[0] = GetComponent<GamePieceScript>().piecePosition[0];
            piece.GetComponent<GamePieceScript>().piecePosition[1] = GetComponent<GamePieceScript>().piecePosition[1];
            piece.GetComponent<GamePieceScript>().isPlaced = true;  //new piece is placed
            GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
            GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
            gameObject.SetActive(false); //removes the eaten piece
            gms.turn = "ai1";
            /*Debug.Log("Top: " + piece.GetComponent<GamePieceScript>().rank);
            Debug.Log("Bottom: " + GetComponent<GamePieceScript>().rank);*/
        }
        //when the attacking piece is weaker
        else if (valid && piece.GetComponent<GamePieceScript>().rank < GetComponent<GamePieceScript>().rank &&
            piece.GetComponent<GamePieceScript>().playerType != GetComponent<GamePieceScript>().playerType)
        {
            Debug.Log("Player weak eats strong: " + piece.GetComponent<GamePieceScript>().piecePosition[0] + ":" + piece.GetComponent<GamePieceScript>().piecePosition[1]);
            bs.objectAndPos[piece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                + piece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
            //set the occupiedPosition of the board
            bs.occupiedPos[piece.GetComponent<GamePieceScript>().piecePosition[0], piece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
            //eat this object
            piece.GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
            //replace the new piece to the tile
            piece.GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
            piece.GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
            piece.gameObject.SetActive(false); //removes the eaten piece
            gms.turn = "ai1";
            /*Debug.Log("Top: " + piece.GetComponent<GamePieceScript>().rank);
            Debug.Log("Bottom: " + GetComponent<GamePieceScript>().rank);*/
        }
        //when both piece have the same rank
        else if (valid && piece.GetComponent<GamePieceScript>().rank == GetComponent<GamePieceScript>().rank &&
            piece.GetComponent<GamePieceScript>().playerType != GetComponent<GamePieceScript>().playerType)
        {
            Debug.Log("Player same eats same");
            bs.objectAndPos[piece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                + piece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
            bs.objectAndPos[GetComponent<GamePieceScript>().piecePosition[0] + ""
                + GetComponent<GamePieceScript>().piecePosition[1]] = null;
            //set the occupiedPosition of the board
            bs.occupiedPos[piece.GetComponent<GamePieceScript>().piecePosition[0], piece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
            bs.occupiedPos[GetComponent<GamePieceScript>().piecePosition[0], GetComponent<GamePieceScript>().piecePosition[1]] = false;
            //eat this object
            piece.GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
            piece.GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
            piece.GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
            piece.gameObject.SetActive(false); //removes the eaten piece
            GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
            GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
            GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
            gameObject.SetActive(false); //removes the eaten piece
            gms.turn = "ai1";
            /*Debug.Log("Top: " + piece.GetComponent<GamePieceScript>().rankName);
            Debug.Log("Bottom: " + GetComponent<GamePieceScript>().rankName);*/
        }
        //change side and turn off interactable for all the player's pieces
        if (gms.gameState == "in-game" && gms.turn != "human")
        {
            Debug.Log("On drop AiMove trigger");
            bs.turnOffInteractable();
            gms.setTurn("ai1");
            aiScript.AiMove(bs.enemyPiecesList);
        }
    }

    /*gamePiecesNames = new string[15]{ "Flag", "Spy", "Private", "Sergeant", "2nd Lieutenant", "1st Lieutenant", 
        "Captain", "Major", "Lieutenant Colonel", "Colonel", "One-Star General", 
        "Two-Star General", "Three-Star General", "Four-Star General", "Five-Star General"};*/

}


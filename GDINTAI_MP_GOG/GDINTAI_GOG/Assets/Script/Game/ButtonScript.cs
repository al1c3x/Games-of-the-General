using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ButtonScript : MonoBehaviour
{
    private BoardScript script;
    private GameManagerScript gms;
    public GameObject refreshButton;
    void Awake()
    {
        script = FindObjectOfType<BoardScript>();
        gms = FindObjectOfType<GameManagerScript>();
    }

    public void checkIfAllPlaced()
    {
        int x;
        if(script.checkIfAllPlaced())
        {
            gms.gameState = "in-game";
           
            gms.turn = "human";
            gameObject.SetActive(false);
            refreshButton.SetActive(false);
            for (x = 0; x < script.enemyPiecesList.Length; x++)
            {
                script.enemyPiecesList[x].SetActive(true);
            }
            for (x = 0; x < 21; x++)
            {
                script.blackGraveyard[x].deadTileObject.SetActive(true);
                script.whiteGraveyard[x].deadTileObject.SetActive(true);
            }
        }
    }

    public void preConstructionHuman()
    {
        int x, y;
        int posX, posY;
        Random rnd = new Random();

        for (x = 0; x < script.playerPiecesList.Length; x++)
        {
            script.playerPiecesList[x].GetComponent<GamePieceScript>().piecePosition[0] = -1;
            script.playerPiecesList[x].GetComponent<GamePieceScript>().piecePosition[1] = -1;
            script.playerPiecesList[x].GetComponent<GamePieceScript>().isPlaced = false;
        }

        for (x = 4; x < 8; x++)
        {
            for (y = 0; y < 9; y++)
            {
                script.occupiedPos[x, y] = false;
                script.tileObjectAndPos[x + "" + y].GetComponent<TileScript>().occupied = false;
            }
        }

        for (x = 0; x < script.playerPiecesList.Length; x++)
        {
            posX = rnd.Next(5, 8);
            posY = rnd.Next(0, 9);
            if (script.occupiedPos[posX, posY] != true)
            {
                script.tileObjectAndPos[posX + "" + posY].GetComponent<TileScript>().occupied = true;

               script.playerPiecesList[x].GetComponent<GamePieceScript>().piecePosition[0] = posX;
                script.playerPiecesList[x].GetComponent<GamePieceScript>().piecePosition[1] = posY;

                script.occupiedPos[posX, posY] = true;
                script.playerPiecesList[x].GetComponent<GamePieceScript>().isPlaced = true;
                script.playerPiecesList[x].GetComponent<RectTransform>().anchoredPosition 
                    = script.tileList[posX, posY].GetComponent<RectTransform>().anchoredPosition;
                script.objectAndPos[posX + "" + posY] = script.playerPiecesList[x];
            }
            else
            {
                --x;
                continue;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    private AILogic aiLog;

    public GameObject tileObject;   //a prefab of a tile
    public GameObject pieceObject;   //a prefab of a piece
    [HideInInspector] public GameObject[,] tileList = new GameObject[8,9]; //the tiles represented in an array
    [HideInInspector] public GameObject[] playerPiecesList = new GameObject[21]; //player's pieces gameobject
    [HideInInspector] public GameObject[] enemyPiecesList = new GameObject[21]; //player's pieces gameobject
    [HideInInspector] public int[] gamePiecesIndex = new int[21]; /*the number of pieces in each side; 
    sorted in increasing order(weakest(flag) to strongest rank(5 star))*/
    [HideInInspector] public bool[,] occupiedPos = new bool[8, 9]; //the state of the tiles if its occupied

    float posX = -414.0f + 960.0f;  //position x of the tiles;2nd param is the offset of the default pos
    float posY = 408.0f + 540.5f;  //position y of the tiles;2nd param is the offset of the default pos
    float gapLength = 105.0f; //for tiles

    float posXPieces = -864.0f + 960.0f;  //position x of the pieces;2nd param is the offset of the default pos
    float posYPieces = 200.0f + 540.5f;  //position y of the pieces;2nd param is the offset of the default pos

    Vector3 pos; //pos for tiles
    Vector3 posPieces; //pos for pieces

    // Start is called before the first frame update

    void Awake()
    {
        aiLog = FindObjectOfType<AILogic>();
    }

    private void Start()
    {
        pos = new Vector3(posX, posY, 0);
        instantiateTiles();
        gamePiecesIndex = new int[21] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 3, 3, 3, 3, 3, 2, 2, 1 };
        posPieces = new Vector3(posXPieces, posYPieces, 0);
        instantiatePieces();
    }
    //instantiate the tiles in their corresponding positions
    public void instantiateTiles()
    {
        int x, y;
        for (x = 0; x < tileList.GetLength(0) / 2; x++)
        {
            for (y = 0; y < tileList.GetLength(1); y++)
            {
                tileList[x, y] = Instantiate(tileObject, pos, Quaternion.identity);
                tileList[x, y].GetComponent<TileScript>().tilePosition[0] = x;
                tileList[x, y].GetComponent<TileScript>().tilePosition[1] = y;
                tileList[x, y].transform.position = pos;
                if (y >= 2)
                    pos.x += gapLength - 1.0f;
                else
                    pos.x += gapLength;
            }
            pos.y -= gapLength;
            pos.x = posX;
        }

        posX = -415.0f + 960.0f;  //2nd param is the offset of the default pos
        posY = -66.0f + 540.0f;  //2nd param is the offset of the default pos
        pos = new Vector3(posX, posY, 0);

        for (; x < tileList.GetLength(0); x++)
        {
            for (y = 0; y < tileList.GetLength(1); y++)
            {
                tileList[x, y] = Instantiate(tileObject, pos, Quaternion.identity);
                tileList[x, y].GetComponent<TileScript>().tilePosition[0] = x;
                tileList[x, y].GetComponent<TileScript>().tilePosition[1] = y;
                tileList[x, y].transform.position = pos;
                if (y >= 2)
                    pos.x += gapLength - 1.0f;
                else
                    pos.x += gapLength;
            }
            pos.y -= gapLength;
            pos.x = posX;
        }
    }

    public void instantiatePieces()
    {
        int x;
        for (x = 0; x < playerPiecesList.Length; x++)
        {
            playerPiecesList[x] = Instantiate(pieceObject, posPieces, Quaternion.identity);
            playerPiecesList[x].transform.position = posPieces;
            playerPiecesList[x].GetComponent<GamePieceScript>().rankImageLoad =
                GameObject.FindObjectOfType<GameManagerScript>().gamePiecesImagesW[x];
            playerPiecesList[x].GetComponent<GamePieceScript>().playerType = "human";
            playerPiecesList[x].GetComponent<GamePieceScript>().pieceIndex = gamePiecesIndex[x];

            posPieces.x += 80.0f;
            if (x % 4 == 0)
            {
                posPieces.x = posXPieces;
                posPieces.y -= 100.0f;
            }
        }

        posXPieces = 614.0f + 960.0f;
        posPieces = new Vector3(posXPieces, posYPieces, 0);

        for (x = 0; x < enemyPiecesList.Length; x++)
        {
            enemyPiecesList[x] = Instantiate(pieceObject, posPieces, Quaternion.identity);
            enemyPiecesList[x].transform.position = posPieces;
            enemyPiecesList[x].GetComponent<GamePieceScript>().rankImageLoad =
              GameObject.FindObjectOfType<GameManagerScript>().gamePiecesImagesB[x];
            enemyPiecesList[x].GetComponent<GamePieceScript>().playerType = "ai";
            enemyPiecesList[x].GetComponent<GamePieceScript>().pieceIndex = gamePiecesIndex[x];

            posPieces.x += 80.0f;
            if (x % 4 == 0)
            {
                posPieces.x = posXPieces;
                posPieces.y -= 100.0f;
            }
        }
        for (x = 0; x < enemyPiecesList.Length; x++)
        {
            enemyPiecesList[x].SetActive(false);
        }
        aiLog.preConstructionAI(0, 3, enemyPiecesList);
    }

    //check if all the pieces of the plaer is placed on the right tiles; all pieces are placed on the board
    public bool checkIfAllPlaced()
    {
        int x, y;
        for (x = 0; x < playerPiecesList.Length; x++)
        {
            if (playerPiecesList[x].GetComponent<GamePieceScript>().isPlaced == false)
                return false;
        }
        for (x = 0; x < occupiedPos.GetLength(1); x++)
        {
            if (occupiedPos[4,x] == true)
                return false;
        }
        return true;
    }

}

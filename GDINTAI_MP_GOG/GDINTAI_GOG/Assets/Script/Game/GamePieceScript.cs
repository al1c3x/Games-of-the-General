using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePieceScript : MonoBehaviour
{
    public int[]  piecePosition = new int[2] { -1, -1 };
    public string rankName;
    public Sprite rankImageLoad;
    public string playerType;  
    public bool isPlaced = false;
    private Image rankImage;
    public int pieceIndex;  
    public int rank;  
    public bool isDead = false;   
    private BoardScript board;

    public string suspectedName = "none";
    public int suspectedRankValue = 0;
    public bool suspectIsSure = false;
    public bool isVisible = false;
   
    void Awake()
    {
        rankImage = GetComponent<Image>();
        board = FindObjectOfType<BoardScript>();
    }

   
    void Start()
    {
        rankName = FindObjectOfType<GameManagerScript>().gamePiecesNames[pieceIndex-1];
        rankImage.sprite = rankImageLoad;
        if(playerType == "human"){
            gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("PlayerPieceSetTag").transform);
        }
            
        else{
            gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("EnemyPieceSetTag").transform);
        }
           
        if (pieceIndex == 2){
            rank = 15;
        }

        else{
            rank = pieceIndex == 1 ? 1 : pieceIndex - 1;
        }
          
       
    }

}

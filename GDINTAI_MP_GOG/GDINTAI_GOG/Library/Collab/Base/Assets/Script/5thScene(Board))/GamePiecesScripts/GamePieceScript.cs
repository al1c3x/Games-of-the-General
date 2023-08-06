using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePieceScript : MonoBehaviour
{
    [HideInInspector] public int[]  piecePosition = new int[2] { -1, -1 };
    private string rankName;
    [HideInInspector]public Sprite rankImageLoad;
    [HideInInspector]public string playerType;  //human | ai
    [HideInInspector] public bool isPlaced = false;
    private Image rankImage;
    [HideInInspector] public int pieceIndex;  //index use for setting the string names(source from GameManager)
    [HideInInspector] public int rank;   //ranks of the pieces
    // Start is called before the first frame update
    void Awake()
    {
        rankImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Start()
    {
        rankName = FindObjectOfType<GameManagerScript>().gamePiecesNames[pieceIndex-1];
        rankImage.sprite = rankImageLoad;
        gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("PieceSetTag").transform);
        rank = pieceIndex;
    }

}

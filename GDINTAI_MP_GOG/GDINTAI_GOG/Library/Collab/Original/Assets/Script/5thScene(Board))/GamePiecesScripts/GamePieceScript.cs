using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePieceScript : MonoBehaviour
{
    private string rankName;
    [HideInInspector]public Sprite rankImageLoad;
    [HideInInspector]public string playerType;
    [HideInInspector] public bool placed;
    private Image rankImage;
    private int pieceIndex;
    private int rank;
    // Start is called before the first frame update
    void Awake()
    {
        rankImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Start()
    {
        rankName = FindObjectOfType<GameManagerScript>().gamePiecesNames[pieceIndex];
        rankImage.sprite = rankImageLoad;
        gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("PieceSetTag").transform);
        rank = pieceIndex;
    }
}

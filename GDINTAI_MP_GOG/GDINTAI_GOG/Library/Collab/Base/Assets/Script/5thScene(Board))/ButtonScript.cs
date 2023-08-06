using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    private BoardScript bs;
    public GameObject constructText;
    void Awake()
    {
        bs = FindObjectOfType<BoardScript>();
    }

    public void checkIfAllPlaced()
    {
        int x;
        Debug.Log(bs.checkIfAllPlaced());
        if(bs.checkIfAllPlaced())
        {
            gameObject.SetActive(false);
            constructText.SetActive(false);
            for (x = 0; x < bs.enemyPiecesList.Length; x++)
            {
                bs.enemyPiecesList[x].SetActive(true);
            }
        }
    }
}

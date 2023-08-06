using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; //this is needed for the event functions

public class TileSlot : MonoBehaviour, IDropHandler
{
    [HideInInspector] public GameObject piece;
    private BoardScript bs;
    private TileScript ts;

    void Awake()
    {
        bs = FindObjectOfType<BoardScript>();
        ts = GetComponent<TileScript>();
    }

    public void OnDrop(PointerEventData eventData) //this is for when the object is placed/drop on the slot
    {
        if (eventData.pointerDrag != null)//pointerDrag is the object that is being drag; checks if the object is passing through the slot
        {
            piece = eventData.pointerDrag.gameObject;
            piece.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition; //anchoredPosition is the pivot/origin of the object
            bs.occupiedPos[ts.tilePosition[0], ts.tilePosition[1]] = true;
            piece.GetComponent<GamePieceScript>().isPlaced = true;
            piece.GetComponent<GamePieceScript>().piecePosition[0] = gameObject.GetComponent<TileScript>().tilePosition[0];
            piece.GetComponent<GamePieceScript>().piecePosition[1] = gameObject.GetComponent<TileScript>().tilePosition[1];
        }
    }

}


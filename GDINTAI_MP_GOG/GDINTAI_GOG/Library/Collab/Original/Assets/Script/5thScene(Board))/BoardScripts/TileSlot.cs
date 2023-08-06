using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; //this is needed for the event functions

public class TileSlot : MonoBehaviour, IDropHandler
{
    [HideInInspector] public bool occupied;
    [HideInInspector] public GameObject piece;

    void Awake()
    {
        occupied = false;
    }

    public void OnDrop(PointerEventData eventData) //this is for when the object is placed/drop on the slot
    {
        if (eventData.pointerDrag != null)//pointerDrag is the object that is being drag; checks if the object is passing through the slot
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition; //anchoredPosition is the pivot/origin of the object
            piece = eventData.pointerDrag.gameObject;
            piece.GetComponent<GamePieceScript>().placed = true;
        }
    }

}


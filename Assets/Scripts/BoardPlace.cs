using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardPlace : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        print("Sürükledi");
    }

    public void OnDrop(PointerEventData eventData)
    {
        print("düþtü");
        MoveManager.Instance.OnDrop(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        print("Býraktý");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var go = transform.GetChild(0);
        if (go == null)
            return;

        print("Bastý");
        go.transform.parent = null;
        MoveManager.Instance.SetCurrentPiece(go.GetComponent<Piece>());
    }

    public void AddPiece(Piece piece)
    {
        piece.transform.parent = transform;

    }
}

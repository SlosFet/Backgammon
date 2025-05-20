using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardPlace : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IDropHandler
{
    public int Id;
    [SerializeField] private bool _canAvailable = false;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private bool hasSelected = false;

    public void OnDrag(PointerEventData eventData)
    {
        if (!_canAvailable)
            return;

        print("Sürükledi");
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!_canAvailable)
            return;

        print("düþtü");
        MoveManager.Instance.OnDrop(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!hasSelected)
            return;

        hasSelected = false;
        MoveManager.Instance.OnEndDrag(this);
        print("Býraktý");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (MoveManager.Instance.HasPiece)
            return;

        var go = transform.GetChild(0);
        if (go == null)
            return;

        print("Bastý");
        hasSelected = true;
        MoveManager.Instance.SetCurrentPiece(go.GetComponent<Piece>());
        MoveManager.Instance.CalculateAvailablePosses(Id);
    }

    public void AddPiece(Piece piece)
    {
        piece.transform.parent = transform;
        piece.transform.localPosition = Vector3.zero;
    }

    public void RemovePiece(Piece piece)
    {

    }

    public bool CheckAvailable()
    {
        return true;
    }

    public void SetAvailable(bool state)
    {
        _canAvailable = state;
        _spriteRenderer.color = state ? Color.green : Color.gray;
    }


}

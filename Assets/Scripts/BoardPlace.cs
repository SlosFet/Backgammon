using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardPlace : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDropHandler, IDragHandler
{
    public int Id;
    [field : SerializeField] protected bool _canAvailable = false;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [field : SerializeField] protected List<Piece> _pieces;

    private bool hasSelected = false;
    public int GetPieceCount => _pieces.Count;
    public PieceType GetLastPieceType => _pieces[0].PieceType;

    private void Start()
    {
        var pieces = GetComponentsInChildren<Piece>().ToList();
        foreach (var piece in pieces)
        {
            AddPiece(piece);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!_canAvailable)
            return;

        print("düþtü");
        MoveManager.Instance.OnDrop(this,TryGetComponent(out CollectPlace place));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!hasSelected)
            return;

        print("Býraktý : " + hasSelected);

        hasSelected = false;
        MoveManager.Instance.OnEndDrag(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (MoveManager.Instance.HasPiece || !_canAvailable)
            return;

        if (_pieces.Count <= 0)
            return;

        print("Bastý");
        hasSelected = true;
        MoveManager.Instance.SetCurrentPiece(_pieces[^1]);
        MoveManager.Instance.CloseAllPlaces();
        MoveManager.Instance.CalculateAvailablePosses(Id);
    }

    public virtual void AddPiece(Piece piece)
    {
        piece.transform.parent = transform;
        piece.transform.localEulerAngles = Vector3.right * -90;

        if (_pieces.Count == 1 && _pieces[0].PieceType != piece.PieceType)
        {
            MoveManager.Instance.AddBrokenPiece(_pieces[0]);
            _pieces.RemoveAt(0);
        }

        if (!_pieces.Contains(piece))
            _pieces.Add(piece);

        piece.transform.localPosition = Vector3.zero + Vector3.up * 0.1f * (_pieces.Count - 1);

    }

    public virtual void RemovePiece(Piece piece)
    {
        if (_pieces.Contains(piece))
            _pieces.Remove(piece);
    }

    public bool CheckAvailable()
    {
        if (_pieces.Count <= 1 || _pieces[^1].PieceType == GameManager.CurrentPieceType)
            return true;

        return false;
    }

    public virtual void SetAvailable(bool state)
    {
        _canAvailable = state;

        _spriteRenderer.color = state ? Color.green : Color.gray;
    }

    public bool CheckAvailableForChoose()
    {
        if (_pieces.Count <= 0 || _pieces[^1].PieceType != GameManager.CurrentPieceType)
            return false;

        else
            return true;
    }

    public void SetAvailableForChoose()
    {
        _canAvailable = true;
        _spriteRenderer.color = Color.blue;
    }


    public void OnDrag(PointerEventData eventData)
    {

    }
}

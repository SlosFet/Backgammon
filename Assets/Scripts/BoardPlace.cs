using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardPlace : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDropHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int Id;
    [field: SerializeField] protected bool _canAvailable = false;

    [SerializeField] private GameObject _selectedImage;
    [SerializeField] private GameObject _hoverObject;

    [field: SerializeField] protected List<Piece> _pieces;

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
        MoveManager.Instance.OnDrop(this, TryGetComponent(out CollectPlace place));
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

        if (_pieces.Count == 1 && _pieces[0].PieceType != piece.PieceType)
        {
            Broke();
        }

        if (!_pieces.Contains(piece))
            _pieces.Add(piece);

        piece.MovePos(Vector3.zero + Vector3.up * 0.1f * (_pieces.Count - 1), Vector3.one);

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
        PointerExit();
        _canAvailable = state;
        _selectedImage.SetActive(state);
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
    }

    public void Broke()
    {
        MoveManager.Instance.AddBrokenPiece(_pieces[0]);
        _pieces.RemoveAt(0);
    }

    public void OnDrag(PointerEventData eventData)
    {

    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_canAvailable)
            return;

        PointerEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_canAvailable)
            return;

        PointerExit();
    }

    public virtual void PointerEnter()
    {
        if (_hoverObject != null)
            _hoverObject.SetActive(true);
        _pieces.ForEach(x => x.ToggleHover(true));
    }

    public virtual void PointerExit()
    {
        if (_hoverObject != null)
            _hoverObject.SetActive(false);
        _pieces.ForEach(x => x.ToggleHover(false));
    }
}

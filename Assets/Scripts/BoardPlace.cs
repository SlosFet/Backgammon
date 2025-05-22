using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardPlace : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDropHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int Id;
    public MoveManager MoveManager;
    [field: SerializeField] protected bool _canAvailable = false;

    [SerializeField] private GameObject _selectedImage;
    [SerializeField] private GameObject _hoverObject;

    [field: SerializeField] protected List<Piece> _pieces;

    [field: SerializeField] protected List<Piece> _startPieces;

    private bool hasSelected = false;
    public int GetPieceCount => _pieces.Count;
    public PieceType GetLastPieceType => _pieces[0].PieceType;
    public List<Piece> GetPieces => _pieces;

    private void Start()
    {
        var pieces = GetComponentsInChildren<Piece>().ToList();
        foreach (var piece in pieces)
        {
            AddPiece(piece);
        }

        _startPieces.AddRange(pieces);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!_canAvailable)
            return;

        MoveManager.OnDrop(this, TryGetComponent(out CollectPlace place));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!hasSelected)
            return;

        hasSelected = false;
        MoveManager.OnEndDrag(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (MoveManager.HasPiece || !_canAvailable)
            return;

        if (_pieces.Count <= 0)
            return;

        hasSelected = true;
        MoveManager.SetCurrentPiece(_pieces[^1]);
        MoveManager.CloseAllPlaces();
        MoveManager.CalculateAvailablePosses(Id);
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

        GetComponent<PiecePlacer>().PlacePieces(_pieces);

    }

    public virtual void RemovePiece(Piece piece)
    {
        if (_pieces.Contains(piece))
            _pieces.Remove(piece);

        GetComponent<PiecePlacer>().PlacePieces(_pieces);
    }

    public bool CheckAvailable()
    {
        if (_pieces.Count <= 1 || _pieces[^1].PieceType == MoveManager.CurrentPieceType)
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
        if (_pieces.Count <= 0 || _pieces[^1].PieceType != MoveManager.CurrentPieceType)
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
        MoveManager.AddBrokenPiece(_pieces[0]);
        _pieces.RemoveAt(0);
    }

    public void RemoveAllPieces()
    {
        _pieces.Clear();
    }
    public void AddStartPieces()
    {
        foreach (var piece in _startPieces)
            AddPiece(piece);
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

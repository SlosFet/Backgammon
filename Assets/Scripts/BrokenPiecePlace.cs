using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BrokenPiecePlace : BoardPlace
{
    [SerializeField] private TextMeshProUGUI _text;
    public override void AddPiece(Piece piece)
    {
        piece.transform.parent = transform;


        if (!_pieces.Contains(piece))
            _pieces.Add(piece);

        piece.MovePos(Vector3.zero * (_pieces.Count - 1), piece.transform.localEulerAngles);

        _text.text = GetPieceCount.ToString();
        _text.gameObject.SetActive(GetPieceCount > 1);
    }

    public override void RemovePiece(Piece piece)
    {
        if (_pieces.Contains(piece))
            _pieces.Remove(piece);

        _text.text = GetPieceCount.ToString();
        _text.gameObject.SetActive(GetPieceCount > 1);
    }

    public override void SetAvailable(bool state)
    {
        _canAvailable = state;
        base.PointerExit();
    }

    public override void MyOnPointerDown(int pointerId)
    {
        if (MoveManager.HasPiece || !_canAvailable)
            return;

        if (_pieces.Count <= 0)
            return;

        hasSelected = true;
        MoveManager.SetCurrentPiece(_pieces[^1],pointerId);
        MoveManager.CheckPlaces();
    }
}

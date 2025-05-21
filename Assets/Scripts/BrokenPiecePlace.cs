using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenPiecePlace : BoardPlace
{
    public override void AddPiece(Piece piece)
    {
        piece.transform.parent = transform;

        if (_pieces.Count == 1 && _pieces[0].PieceType != piece.PieceType)
        {
            MoveManager.Instance.AddBrokenPiece(_pieces[0]);
            _pieces.RemoveAt(0);
        }

        if (!_pieces.Contains(piece))
            _pieces.Add(piece);

        piece.transform.localPosition = Vector3.zero + Vector3.forward * 0.1f * (_pieces.Count - 1);
    }

    public override void RemovePiece(Piece piece)
    {
        base.RemovePiece(piece);
    }

    public override void SetAvailable(bool state)
    {
        _canAvailable = state;
    }
}

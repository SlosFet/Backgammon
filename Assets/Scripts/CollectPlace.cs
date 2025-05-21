using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectPlace : BoardPlace
{
    [SerializeField] private TextMeshProUGUI _text;

    public override void AddPiece(Piece piece)
    {
        piece.transform.parent = transform;

        if (!_pieces.Contains(piece))
            _pieces.Add(piece);

        piece.MovePos(Vector3.zero + Vector3.up * 0.1f * (_pieces.Count - 1), Vector3.right * 90);

        _text.text = GetPieceCount.ToString();
    }
}

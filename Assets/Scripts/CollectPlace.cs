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

        GetComponent<PiecePlacer>().PlacePieces(_pieces);

        _text.text = GetPieceCount.ToString();
    }

}

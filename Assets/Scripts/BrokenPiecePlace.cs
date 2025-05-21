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
        base.RemovePiece(piece);
        _text.text = GetPieceCount.ToString();
        _text.gameObject.SetActive(GetPieceCount > 1);
    }

    public override void SetAvailable(bool state)
    {
        _canAvailable = state;
        base.PointerExit();
    }

}

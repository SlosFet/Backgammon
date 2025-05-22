using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CollectPlace : BoardPlace
{
    [SerializeField] private TextMeshProUGUI _text;
    public UnityEvent<PieceType> OnPlayerCollectedAll;

    public override async void AddPiece(Piece piece)
    {
        piece.transform.parent = transform;

        if (!_pieces.Contains(piece))
            _pieces.Add(piece);

        GetComponent<PiecePlacer>().PlacePieces(_pieces);

        _text.text = GetPieceCount.ToString();
        CheckItIsDone();
        await Task.Delay(200);
        ParticleManager.Instance.PlayCollectParticle(piece.transform.position);
    }

    private void CheckItIsDone()
    {
        if (GetPieceCount >= 15)
        {
            print("Bitti");
            OnPlayerCollectedAll.Invoke(GetLastPieceType);
        }
    }

    private void PlayParticle()
    {
    }

}

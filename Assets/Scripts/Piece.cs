using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public PieceType PieceType;
    public void SetPos(Vector3 pos)
    {
        transform.position = pos;
    }

    public void MovePos(Vector3 pos, Vector3 rot)
    {
        transform.DOKill();
        transform.DOLocalMove(pos, 0.2f).SetEase(Ease.Linear);
        transform.DOLocalRotate(rot, 0.2f).SetEase(Ease.Linear);
    }
}

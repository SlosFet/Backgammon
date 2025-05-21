using DG.Tweening;
using EPOOutline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public PieceType PieceType;
    private Outlinable _outline;

    private void Awake()
    {
        _outline = GetComponent<Outlinable>();
        ToggleHover(false);
    }
    public void SetPos(Vector3 pos)
    {
        transform.position = pos;
    }

    public void MovePos(Vector3 pos, Vector3 rot)
    {
        transform.DOKill();
        transform.DOLocalMove(pos, 0.2f).SetEase(Ease.Linear);
        transform.DORotate(rot, 0.2f).SetEase(Ease.Linear);
    }

    public void ToggleHover(bool toggle)
    {
        _outline.enabled = toggle;
    }
}

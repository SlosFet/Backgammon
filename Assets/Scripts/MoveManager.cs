using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : Singleton<MoveManager>
{
    [SerializeField] private float distance;
    [SerializeField] private Piece currentPiece;

    public LayerMask layer;
    public Vector3 GetMousePos()
    {
        Vector3 pos = Vector3.forward * -0.2f;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, layer))
        {
            return hitInfo.point - (ray.direction * distance);
        }

        return pos;
    }

    public void SetCurrentPiece(Piece piece)
    {
        currentPiece = piece;
    }

    private void Update()
    {
        if (currentPiece == null)
            return;

        currentPiece.SetPos(GetMousePos());
    }

    public void OnDrop(BoardPlace place)
    {
        place.AddPiece(currentPiece);
        currentPiece = null;
    }
}

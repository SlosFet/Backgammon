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
}

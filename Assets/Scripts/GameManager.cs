using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Player player1;
    public Player player2;

    public static PieceType CurrentPieceType;

    private void Start()
    {
        Application.targetFrameRate = 60;
        CurrentPieceType = PieceType.Black;
    }

    public void TourDone()
    {
        CurrentPieceType = CurrentPieceType == PieceType.White ? PieceType.Black : PieceType.White;
        DiceManager.Instance.OnTourDone();
    }
}

[System.Serializable]
public enum PieceType
{
    White,
    Black
}

[System.Serializable]
public class Player
{
    public PieceType Type;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Player player1;
    public Player player2;

    public static PieceType CurrentPieceType;
    public PieceType StartPieceType;

    private void Start()
    {
        Application.targetFrameRate = 240;
        QualitySettings.vSyncCount = 1;
        CurrentPieceType = StartPieceType;
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

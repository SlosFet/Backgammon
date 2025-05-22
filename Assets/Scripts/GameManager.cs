using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Player player1;
    public Player player2;

    private void Start()
    {
        Application.targetFrameRate = 240;
        QualitySettings.vSyncCount = 1;
    }
}

[System.Serializable]
public enum PieceType
{
    White,
    Black
}

[System.Serializable]
public enum TableType
{
    Left,
    Right
}

[System.Serializable]
public class Player
{
    public PieceType Type;
}

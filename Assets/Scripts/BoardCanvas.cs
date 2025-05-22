using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardCanvas : MonoBehaviour
{

    [SerializeField] private List<CanvasItems> _canvasItems;

    public void OpenWinPanel(PieceType pieceType)
    {
        _canvasItems.First(x=>x.PieceType == pieceType).WinPanel.SetActive(true);
    }
}

[System.Serializable]
public struct CanvasItems
{
    public PieceType PieceType;
    public GameObject WinPanel;
}

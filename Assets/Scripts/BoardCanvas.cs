using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoardCanvas : MonoBehaviour
{

    [SerializeField] private List<CanvasItems> _canvasItems;

    public void OpenWinPanel(PieceType pieceType)
    {
        GameManager.CurrentPieceType = pieceType;
        _canvasItems.First(x=>x.PieceType == pieceType).WinPanel.SetActive(true);
    }

    public void CloseWinPanel(PieceType pieceType)
    {
        _canvasItems.First(x => x.PieceType == pieceType).WinPanel.SetActive(false);
    }

    public void SubscribeToRestart(UnityAction action)
    {
        _canvasItems.ForEach(x=>x.RestartButtons.ForEach(b=>b.onClick.AddListener(action)));
    }
}

[System.Serializable]
public struct CanvasItems
{
    public PieceType PieceType;
    public GameObject WinPanel;
    public List<Button> RestartButtons;
}

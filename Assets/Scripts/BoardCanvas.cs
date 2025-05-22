using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoardCanvas : MonoBehaviour
{

    [SerializeField] private List<BoardCanvasItems> _canvasItems;
    public MoveManager MoveManager;

    public void OpenWinPanel(PieceType pieceType)
    {
        MoveManager.CurrentPieceType = pieceType;
        _canvasItems.First(x=>x.PieceType == pieceType).TogglewWinPanel(true);
    }

    public void CloseWinPanel(PieceType pieceType)
    {
        _canvasItems.First(x => x.PieceType == pieceType).TogglewWinPanel(false);
    }

    public void SubscribeToRestart(UnityAction action)
    {
        _canvasItems.ForEach(x=>x.AddFunctionToRestartButtons(action));
    }

    public void SubscribeToRoll(UnityAction action) => _canvasItems.ForEach(x => x.SetRollFunction(action));
    public void SubscribeToDoneReturn(UnityAction Return, UnityAction Done) => _canvasItems.ForEach(x => x.SetFunctions(Return,Done));

    public void ToggleRollButton(bool state) => _canvasItems.First(x => x.PieceType == MoveManager.CurrentPieceType).ToggleRollButton(state);
    public void CloseRollButton() => _canvasItems.ForEach(x => x.ToggleRollButton(false));
    public void ToggleReturnButton(bool state) => _canvasItems.First(x => x.PieceType == MoveManager.CurrentPieceType).ToggleReturnButton(state);
    public void ToggleDoneButton(bool state) => _canvasItems.First(x => x.PieceType == MoveManager.CurrentPieceType).ToggleDoneButton(state);
    public void ToggleGameButtons(bool state) => _canvasItems.First(x => x.PieceType == MoveManager.CurrentPieceType).ToggleGameButtons(state);

}

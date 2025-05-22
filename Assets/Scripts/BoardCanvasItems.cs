using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoardCanvasItems : MonoBehaviour
{
    public PieceType PieceType;
    public GameObject WinPanel;
    public List<Button> RestartButtons;

    public Button RollButton;
    public Button DoneButton;
    public Button ReturnButton;

    public void AddFunctionToRestartButtons(UnityAction action) => RestartButtons.ForEach(x=>x.onClick.AddListener(action));
    public void TogglewWinPanel(bool state) => WinPanel.SetActive(state);
    public void ToggleRollButton(bool state) => RollButton.gameObject.SetActive(state);
    public void ToggleReturnButton(bool state) => ReturnButton.gameObject.SetActive(state);
    public void ToggleDoneButton(bool state) => DoneButton.gameObject.SetActive(state);

    public void SetFunctions(UnityAction Return, UnityAction Done)
    {
        ReturnButton.onClick.AddListener(Return);
        DoneButton.onClick.AddListener(Done);
    }

    public void SetRollFunction(UnityAction Roll)
    {
        RollButton.onClick.AddListener(Roll);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    public int diceVal1;
    public int diceVal2;

    [SerializeField] private bool isCheatActive = false;
    [SerializeField] private BoardCanvas _boardCanvas;

    [SerializeField] private Dice _whiteDice1;
    [SerializeField] private Dice _whiteDice2;

    [SerializeField] private Dice _blackDice1;
    [SerializeField] private Dice _blackDice2;

    [SerializeField] private Dice _startLeftDice;
    [SerializeField] private Dice _startRightDice;

    [SerializeField] private MoveManager MoveManager;

    public bool isEqual => diceVal1 == diceVal2;
    public int GetRealTotalValue => diceVal1 + diceVal2;

    public List<int> Values;
    public int TotalValue;


    private void Start()
    {
        _boardCanvas.SubscribeToRoll(RollDices);
    }

    public async void RollFirstDices(int dice1, int dice2, int waitTime)
    {
        _startRightDice.RollFirst(dice1);
        _startLeftDice.RollFirst(dice2);

        await Task.Delay(waitTime);

        _startRightDice.gameObject.SetActive(false);
        _startLeftDice.gameObject.SetActive(false);
    }

    public void SetPlaces()
    {
        Values.Clear();
        TotalValue = 0;
        if (diceVal1 == diceVal2)
        {
            for (int i = 0; i < 4; i++)
                Values.Add(diceVal1 * (i + 1));
        }
        else
        {
            Values.Add(diceVal1);
            Values.Add(diceVal2);
            TotalValue = diceVal1 + diceVal2;
        }
    }

    public async void RollDices()
    {
        diceVal1 = isCheatActive ? diceVal1 : Random.Range(1, 7);
        diceVal2 = isCheatActive ? diceVal2 : Random.Range(1, 7);
        _boardCanvas.CloseRollButton();
        SoundManager.Instance.PlaySound(SoundTypes.RollSound, 3);

        if (MoveManager.CurrentPieceType == PieceType.White)
        {
            _whiteDice1.Roll(diceVal1);
            await _whiteDice2.Roll(diceVal2);
        }
        else
        {
            _blackDice1.Roll(diceVal1);
            await _blackDice2.Roll(diceVal2);
        }

        SetPlaces();

        MoveManager.CheckPlaces();
    }

    public void OnPiecePlaced(int val)
    {
        if (!isEqual)
        {
            if (Values.Contains(val))
                Values.Remove(val);
            if (val == TotalValue)
                Values.Clear();
            TotalValue -= val;
        }
        else
        {
            List<int> newList = new List<int>();
            foreach (var value in Values)
            {
                if (val >= value)
                    newList.Add(value);
            }

            foreach (var value in newList)
            {
                Values.Remove(value);
            }

            for (int i = 0; i < Values.Count; i++)
            {
                Values[i] = Values[i] - val;
            }
        }
    }

    public void OnMoveReturn(int val)
    {
        if (!isEqual)
        {
            if (val == diceVal1 + diceVal2)
                SetPlaces();
            else
            {
                Values.Add(val);
                TotalValue += val;
            }
        }

        else
        {
            for (int i = 0; i < Values.Count; i++)
            {
                Values[i] = Values[i] + val;
            }
            int count = val / diceVal1;
            for (int i = 0; i < count; i++)
            {
                Values.Add(diceVal1 * (i + 1));
            }
        }

        Values = Values.OrderBy(x => x).ToList();
    }

    public void OnTourDone()
    {
        Values.Clear();
        _whiteDice1.gameObject.SetActive(false);
        _whiteDice2.gameObject.SetActive(false);
        _blackDice1.gameObject.SetActive(false);
        _blackDice2.gameObject.SetActive(false);

        _boardCanvas.CloseRollButton();
        _boardCanvas.ToggleRollButton(true);
    }

    public void SetFill(int val)
    {
        Dice dice1 = MoveManager.CurrentPieceType == PieceType.White ? _whiteDice1 : _blackDice1;
        Dice dice2 = MoveManager.CurrentPieceType == PieceType.White ? _whiteDice2 : _blackDice2;

        if(!isEqual)
        {
            if(dice1.value == val)
                dice1.SetImageFill(val);
            else
                dice2.SetImageFill(val);
        }

        else
        {
            for (int i = 0; i < Values.Count; i++)
            {
                if (i == 0)
                    dice1.SetImageFill(0.5f);
                if (i == 1)
                    dice1.SetImageFill(1f);
                if (i == 2)
                    dice2.SetImageFill(0.5f);
                if (i == 3)
                    dice2.SetImageFill(1f);
            }
        }
    }

    public void ResetFill()
    {
        Dice dice1 = MoveManager.CurrentPieceType == PieceType.White ? _whiteDice1 : _blackDice1;
        Dice dice2 = MoveManager.CurrentPieceType == PieceType.White ? _whiteDice2 : _blackDice2;

        dice1.SetImageFill(0);
        dice2.SetImageFill(0);
    }
}

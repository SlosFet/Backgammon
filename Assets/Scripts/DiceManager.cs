using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : Singleton<DiceManager>
{
    public int diceVal1;
    public int diceVal2;

    [SerializeField] private TextMeshProUGUI dice1Text;
    [SerializeField] private TextMeshProUGUI dice2Text;

    [SerializeField] private bool isCheatActive = false;
    [SerializeField] private Button _rollButton;

    private bool isEqual => diceVal1 == diceVal2;

    public List<int> Values;
    public int TotalValue;


    private void Awake()
    {
        _rollButton.onClick.AddListener(RollDices);
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

    public void RollDices()
    {
        diceVal1 = isCheatActive ? diceVal1 : Random.Range(1, 7);
        diceVal2 = isCheatActive ? diceVal2 : Random.Range(1, 7);

        dice1Text.text = diceVal1.ToString();
        dice2Text.text = diceVal2.ToString();

        SetPlaces();
    }

    public void OnPiecePlaced(int val)
    {
        if(!isEqual)
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
                if(val >= value)
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
            if(val == diceVal1 + diceVal2)
                SetPlaces();
            else
            {
                Values.Add(val);
                TotalValue += val;
            }
        }

        else
        {

        }
    }
}

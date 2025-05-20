using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartButtonHandler : MonoBehaviour
{
    private int minPlayerCount;
    private int maxPlayerCount;
    [SerializeField] private List<int> _supportedPlayerCounts;
    private List<int> unsupportedPlayerCount = new List<int>();

    [SerializeField] private Button _startButton;
    [SerializeField] private GameObject _startButtonBlocker;
    [SerializeField] private TextMeshProUGUI _startButtonBlockerText;

    [SerializeField] private string _unsupportedText;
    [SerializeField] private string _maxPlayerText;
    [SerializeField] private string _minPlayerText;

    [SerializeField] private List<Placing> Placing;

    public static UnityEvent<List<int>> SetAvailableIndexImages = new UnityEvent<List<int>>();

    private void Start()
    {
        minPlayerCount = _supportedPlayerCounts.OrderBy(x => x).ToList().First();
        maxPlayerCount = _supportedPlayerCounts.OrderByDescending(x => x).ToList().First();

        SetPanelState(false);
        _startButtonBlockerText.text = string.Format(_minPlayerText, minPlayerCount);
        SetAvailableIndexImages.Invoke(_supportedPlayerCounts);

        //6 kiþiye kadar olan sistemlerde eklenmemiþ sayýlarý desteklenmeyen oyuncu sayýsý listesine ekler
        for (int i = 1; i < 7; i++)
        {
            if(!_supportedPlayerCounts.Contains(i))
                unsupportedPlayerCount.Add(i);
        }
    }

    public void UpdateState(int playerCount)
    {
        if (_supportedPlayerCounts.Contains(playerCount))
        {
            Placing placing = Placing.Find(x => x.AvailablePlacesForMode == playerCount);
            List<int> indexes = AvatarSelectPanel.playerIndexes;

            foreach (var place in placing.Exceptions)
            {
                bool hasPlacing = indexes.OrderBy(x => x).SequenceEqual(place.indexes.OrderBy(x => x));
                if (hasPlacing)
                {
                    SetPanelState(false);
                    _startButtonBlockerText.text = placing.WarningText;
                    return;
                }
            }

            SetPanelState(true);
        }

        else
            SetPanelState(false);


        if (unsupportedPlayerCount.Contains(playerCount))
        {
            _startButtonBlockerText.text = string.Format(_unsupportedText, playerCount);
        }

        else if (playerCount > maxPlayerCount)
        {
            _startButtonBlockerText.text = string.Format(_maxPlayerText, maxPlayerCount);
        }

        else if (playerCount < minPlayerCount)
        {
            _startButtonBlockerText.text = string.Format(_minPlayerText, minPlayerCount);
        }
    }

    private void SetPanelState(bool state)
    {
        _startButtonBlocker.SetActive(!state);
        _startButton.interactable = state;
    }
}

[System.Serializable]
public struct Placing
{
    public int AvailablePlacesForMode;
    public string WarningText;
    public List<Places> Exceptions;
}

[System.Serializable]
public struct Places
{
    public List<int> indexes;
}

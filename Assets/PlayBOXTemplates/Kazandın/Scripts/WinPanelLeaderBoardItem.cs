using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPanelLeaderBoardItem : MonoBehaviour
{
    [SerializeField] private Image _avatarImage;
    [SerializeField] private TextMeshProUGUI _avatarName;
    [SerializeField] private TextMeshProUGUI _scoreText;

    public void SetItem(Sprite sprite,string name, int score)
    {
        gameObject.SetActive(true);
        _avatarImage.sprite = sprite;
        _avatarName.text = name;
        _scoreText.text = score.ToString();
    }
}

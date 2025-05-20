using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> _players;
    private void OnEnable()
    {
        AvatarSelectPanel.OnStart.AddListener(SetPlayers);
    }

    private void OnDisable()
    {
        AvatarSelectPanel.OnStart.RemoveListener(SetPlayers);
    }

    private void SetPlayers()
    {
        List<int> indexes = AvatarSelectPanel.playerIndexes;

        foreach (int index in indexes)
            _players[index - 1].SetActive(true);
    }
}

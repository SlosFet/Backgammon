using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCountController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _images;

    private void OnEnable()
    {
        StartButtonHandler.SetAvailableIndexImages.AddListener(SetAvailableIndexImages);
    }

    private void OnDisable()
    {
        StartButtonHandler.SetAvailableIndexImages.RemoveListener(SetAvailableIndexImages);
    }

    private void SetAvailableIndexImages(List<int> playerCount)
    {
        for (int i = 0; i < _images.Count; i++)
        {
            if (!playerCount.Contains(i + 1))
                _images[i].SetActive(false);
        }
    }
}

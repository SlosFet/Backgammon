using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSection : MonoBehaviour
{
    public int PlayerID;
    [SerializeField] private Image AvatarImage;
    [SerializeField] private TextMeshProUGUI _rank;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private Image _crown;
    [SerializeField] private HorizontalLayoutGroup _horizontal;

    public void SetSection(Sprite sprite,int rank,string name, int score)
    {
        AvatarImage.sprite = sprite;
        _rank.text = rank + ".Oldun";
        _nameText.text = name;
        _scoreText.text = score.ToString();
        gameObject.SetActive(true);

        StartCoroutine(UpdateHorizontal(_horizontal.spacing));
    }

    public void ShowCrown(Sprite sprite)
    {
        _crown.gameObject.SetActive(true);
        _crown.sprite = sprite;
    }

    IEnumerator UpdateHorizontal(float space)
    {
        yield return new WaitForSeconds(0.35f);
        space += 0.1f;
        while(true)
        {
            _horizontal.spacing = space + .01f;
            yield return null;
        }
    }
}

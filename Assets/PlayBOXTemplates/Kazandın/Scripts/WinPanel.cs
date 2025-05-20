using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    public static WinPanel Instance;
    [SerializeField] private List<WinPanelLeaderBoardItem> _winPanelLeaderBoardItems;
    [SerializeField] private List<PlayerSection> _playerSections;
    [SerializeField] private List<Sprite> _crowns;
    [SerializeField] private VerticalLayoutGroup _vertical;
    [SerializeField] private Transform _rotate;

    private void Awake()
    {
        Instance = this;
    }

    public async void ShowLeaderBoard()
    {
        _winPanelLeaderBoardItems.ForEach(x => x.gameObject.SetActive(false));
        _playerSections.ForEach(x => x.gameObject.SetActive(false));

        var scores = ScoreManager.GetOrderedDictionary();    //Burada playerID ve score tutan bir liste veya dictionary tarzý bir deðiþken alýyoruz. Bu liste skoru en büyükten en küçüðe olarak sýralý olmalý

        for (int i = 0; i < scores.Count; i++)
        {
            var item = _winPanelLeaderBoardItems[i];
            var score = scores[i];  //Burada gönderdiðin player ý veya ne gönderiyorsan onu alýyorsun

            var data = AvatarSelectPanel.playerIndexesToAvatarDatas[score.playerId];

            item.SetItem(data.AvatarSprite, data.AvatarName, score.score);
            _playerSections.Find(x => x.PlayerID == score.playerId).SetSection(data.AvatarSprite, i + 1, data.AvatarName, score.score);

            if (i <= 0)
                SetRotation(score.playerId);

            if (i < 3)
                _playerSections.Find(x => x.PlayerID == score.playerId).ShowCrown(_crowns[i]);
        }

        await Task.Delay(50);
        _vertical.spacing = -1;
    }

    private void SetRotation(int id)
    {
        int rotZ = 0;
        switch (id)
        {
            case 1:
                rotZ = -90;
                break;
            case 2:
                rotZ = 90;
                break;
            case 3:
                rotZ = 180;
                break;
            case 4:
                break;
            case 5:
                rotZ = 180;
                break;
            case 6:
                break;
        }

        _rotate.transform.eulerAngles = Vector3.forward * rotZ;
    }

    public void Restart()
    {
        //Statik bir deðiþken veya liste varsa resetlemeliyiz

        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

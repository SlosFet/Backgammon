using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AvatarSelectPanel : MonoBehaviour
{
    public static AvatarSelectPanel Instance;
    [SerializeField] private StartButtonHandler _startButtonHandler;
    [SerializeField] private Transform _tutorialHand;
    [SerializeField] private int _tutorialTargetIndex;

    [SerializeField] private List<DropZone> _dropZones;

    private Vector3 handPos;
    private Vector3 ZonePos;

    public static List<int> playerIndexes = new List<int>(); //Oyun sahnelerinde sadece index gerekliyse
    public static Dictionary<int, AvatarData> playerIndexesToAvatarDatas = new Dictionary<int, AvatarData>(); //Oyun sahnelerinde oyuncunun indexine g�re laz�m olan datalar� �ekebilirsiniz. Class�n en alt�ndaki structu iste�inize g�re �zelle�tirebilirsiniz.
    //Struct� de�i�tirirseniz AvatarSelectPanel objesinin i�indeki OriginalSprite isimli objeleri edit�rden d�zenlemeniz gerek.


    //ExampleScript in �al��mas� i�in yap�ld�
    public static UnityEvent OnStart = new UnityEvent();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerIndexes.Clear();
        playerIndexesToAvatarDatas.Clear();
        SetDropZones();
        Invoke(nameof(StartTutorial), 0.1f);
    }

    private void SetDropZones()
    {
        for (int i = 0; i < _dropZones.Count; i++)
        {
            _dropZones[i].Initialize(i + 1);
        }
    }

    public void StartGame()
    {
        //Oyun yap�n�za g�re buray� �zelle�tirebilirsiniz
        OnStart.Invoke();
        gameObject.SetActive(false);
        //Bu eventi �ok basit tutarak koydum detayl� �rnek kullan�m olarak PanelManager yap�s�n� kullabilirsiniz.
        //PanelManager da Panel abstract� i�eren classlar� tutarak a� kapa y�ntemiyle oyunu ayn� sahnede ba�latabilirsiniz�
        //PanelManager olmadan da farkl� sahneler y�kleyerek oyunun yap�s�na g�re ba�latabilirsiniz
    }

    public void AddPlayer(int i, AvatarData data)
    {
        if (!playerIndexes.Contains(i))
            playerIndexes.Add(i);

        if (playerIndexesToAvatarDatas.ContainsKey(i))
            playerIndexesToAvatarDatas[i] = data;

        else
            playerIndexesToAvatarDatas.TryAdd(i, data);
        CheckPlayersToButton();
    }

    public void RemovePlayer(int i)
    {
        if (playerIndexes.Contains(i))
            playerIndexes.Remove(i);

        if (playerIndexesToAvatarDatas.ContainsKey(i))
            playerIndexesToAvatarDatas.Remove(i);

        CheckPlayersToButton();
    }

    private void CheckPlayersToButton()
    {
        _startButtonHandler.UpdateState(playerIndexes.Count);
    }

    #region TutorialHand

    public void StartTutorial()
    {
        ZonePos = _dropZones[_tutorialTargetIndex - 1].transform.position;
        handPos = _tutorialHand.position;
        GoToDropZone();
    }

    private void GoToDropZone()
    {
        _tutorialHand.DOMove(ZonePos, 1).OnComplete(GoToBase);
    }

    private void GoToBase()
    {
        _tutorialHand.DOMove(handPos, 0.33f).OnComplete(GoToDropZone);
    }

    public void CloseTutorial()
    {
        _tutorialHand.DOKill();
        _tutorialHand.gameObject.SetActive(false);
    }

    #endregion
}

[System.Serializable]
public struct AvatarData
{
    public Sprite AvatarSprite;
    public string AvatarName;
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    [SerializeField] private float distance;
    [SerializeField] private Piece currentPiece;
    [SerializeField] private List<BoardPlace> places;

    [SerializeField] private List<Move> moves;

    [SerializeField] private List<BrokenVariables> _brokenVariables;

    [SerializeField] private List<int> _whiteCollectIds;
    [SerializeField] private List<int> _blackCollectIds;


    [SerializeField] private CollectPlace _whiteCollectPlace;
    [SerializeField] private CollectPlace _blackCollectPlace;

    [SerializeField] private BoardCanvas _boardCanvas;
    [SerializeField] private DiceManager DiceManager;


    public LayerMask layer;
    public PieceType CurrentPieceType;
    public bool HasPiece => currentPiece != null;
    private bool isFirstCheck = true;
    private int fingerId;

    private void Start()
    {
        for (int i = 0; i < places.Count; i++)
        {
            places[i].Id = i;
            places[i].MoveManager = this;
        }

        _brokenVariables.ForEach(x => x.parent.MoveManager = this);
        _whiteCollectPlace.MoveManager = this;
        _blackCollectPlace.MoveManager = this;

        SoundManager.Instance.PlaySound(SoundTypes.NewTourSound);
        _boardCanvas.MoveManager = this;

        _whiteCollectPlace.OnPlayerCollectedAll.AddListener(_boardCanvas.OpenWinPanel);
        _blackCollectPlace.OnPlayerCollectedAll.AddListener(_boardCanvas.OpenWinPanel);

        _boardCanvas.SubscribeToRestart(Restart);
        _boardCanvas.SubscribeToDoneReturn(MoveBack, MoveDone);

        SetType();
    }

    private async void SetType()
    {
        int dice1 = Random.Range(1, 7);
        int dice2 = Random.Range(1, 7);
        if (dice1 == dice2)
        {
            if (dice2 != 1)
                dice2 -= 1;
            else
                dice2 += 1;
        }

        DiceManager.RollFirstDices(dice1, dice2, 2000);
        CurrentPieceType = dice1 > dice2 ? PieceType.White : PieceType.Black;

        await Task.Delay(2000);

        DiceManager.OnTourDone();
    }


    public Vector3 GetMousePos()
    {
        Vector3 rayPos = Vector3.zero;

        if (Input.touchCount == 0)
        {
            rayPos = Input.mousePosition;

        }

        else
        {
            foreach (var touch in Input.touches)
            {
                if (touch.fingerId == fingerId)
                {
                    rayPos = touch.position;
                    break;
                }
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(rayPos);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, layer))
        {
            return hitInfo.point - (ray.direction * distance);
        }

        return Vector3.zero;
    }

    public void SetCurrentPiece(Piece piece, int pointerId)
    {
        fingerId = pointerId;
        SoundManager.Instance.PlaySound(SoundTypes.TakeSound);
        currentPiece = piece;
        _boardCanvas.ToggleReturnButton(false);
    }

    public void CheckPlaces()
    {
        _boardCanvas.ToggleGameButtons(true);
        CloseAllPlaces();
        DiceManager.ResetFill();

        var broken = _brokenVariables.First(x => x.pieceType == CurrentPieceType);

        bool canPlayerPlay = false;


        if (broken.parent.GetPieceCount > 0)
            canPlayerPlay = CheckForBroken(broken);

        else if (CheckPlayerCanCollectPieces(CurrentPieceType).Item1)
            canPlayerPlay = CheckForCollect();

        else
            canPlayerPlay = CheckForNormal();


        if (!canPlayerPlay)
        {
            //E�er zar ilk at�ld���nda check ediliyor ve oynanacak yer yoksa s�ra otomatik olarak ge�er
            if (isFirstCheck)
                Invoke(nameof(MoveDone), 1);

            //De�ilse oyuncuya ta��n� geri almas� veya tamamlamas� i�in hak sunulur
            else
                _boardCanvas.ToggleDoneButton(true);
        }

        isFirstCheck = false;
    }

    private bool CheckForNormal()
    {
        List<BoardPlace> selectedPlaces = new List<BoardPlace>();

        //�nce masadaki b�t�n yerleri kontrol eder
        foreach (var place in places)
        {
            //E�er yer s�radaki oyuncu i�in uygun ta� bar�nd�rm�yor veya bo�sa d�ng�ye devam eder
            if (!place.CheckAvailableForChoose())
                continue;

            //Yerdeki ta� gelen zara g�re hareket edebilir mi kontrol eder
            if (CalculateAvailablePosses(place.Id))
            {
                selectedPlaces.Add(place);
            }
        }

        //Hareket ettirilebilen ta� olan yerler mavi ���kla oyuncuya g�sterilir
        foreach (var place in selectedPlaces)
        {
            place.SetAvailableForChoose();
        }

        //Oyuncunun hareketinin olup olmad��� kontrol edilir
        return selectedPlaces.Count > 0;
    }

    private bool CheckForBroken(BrokenVariables broken)
    {
        bool hasPlace = false;


        if (broken.parent.GetPieceCount > 1)
        {
            for (int i = 0; i < 2; i++)
            {
                print("CHECK");
                var total = (i == 0 ? DiceManager.diceVal1 : DiceManager.diceVal2) - 1;

                if (i == 0 && !DiceManager.Values.Contains(DiceManager.diceVal1))
                    continue;

                if (i == 1 && !DiceManager.Values.Contains(DiceManager.diceVal2))
                    continue;

                if (CheckPlace(broken.places[total].Id))
                {
                    broken.parent.SetAvailable(true);
                    hasPlace = true;
                }
            }
        }

        else
        {
            //Oyuncunun k�r�k ta��n�n bulundu�u id den yerle�im yerlerine konulabilir mi bakar
            if (CalculateAvailablePosses(broken.parent.Id))
            {
                //E�er konulabiliyorsa set eder
                broken.parent.SetAvailable(true);

                //Konulabilecek yerleri ye�il yapar e�er oyuncu ta�a dokunduysa ye�il yapar yoksa yanmaz
                foreach (int val in DiceManager.Values)
                {
                    var total = CurrentPieceType == PieceType.White ? broken.parent.Id - val : broken.parent.Id + val;
                    CheckPlace(total);
                }

                return true;
            }
        }


        return hasPlace;
    }

    private bool CheckForCollect()
    {
        List<BoardPlace> selectedPlaces = new List<BoardPlace>();

        //�nce masadaki b�t�n yerleri kontrol eder
        foreach (var place in places)
        {
            //E�er yer s�radaki oyuncu i�in uygun ta� bar�nd�rm�yor veya bo�sa d�ng�ye devam eder
            if (!place.CheckAvailableForChoose())
                continue;

            //Yerdeki ta� gelen zara g�re hareket edebilir mi kontrol eder
            if (CalculateAvailablePosses(place.Id))
            {
                selectedPlaces.Add(place);
            }
        }

        //Hareket ettirilebilen ta� olan yerler mavi ���kla oyuncuya g�sterilir
        foreach (var place in selectedPlaces)
        {
            place.SetAvailableForChoose();
        }

        //Oyuncunun hareketinin olup olmad��� kontrol edilir
        return selectedPlaces.Count > 0;
    }

    public bool CalculateAvailablePosses(int id)
    {
        bool hasAnyPlace = false;
        foreach (int val in DiceManager.Values)
        {
            bool checkForEqual = false;

            var total = CurrentPieceType == PieceType.White ? id - val : id + val;
            if (CheckPlace(total) || CheckPlaceForCollect(total, id))
            {
                checkForEqual = true;
                hasAnyPlace = true;
                DiceManager.SetFill(val);
            }

            if (DiceManager.isEqual && !checkForEqual)
                break;
        }

        if (!hasAnyPlace)
            return false;

        var totalValue = DiceManager.TotalValue;

        if (totalValue > 0)
        {
            var total = CurrentPieceType == PieceType.White ? id - totalValue : id + totalValue;
            CheckPlace(total);
            CheckPlaceForCollect(total, id);
        }

        return true;
    }

    private bool CheckPlace(int val)
    {
        if (val < places.Count && val >= 0)
        {
            if (places[val].CheckAvailable())
            {
                if (currentPiece != null)
                    places[val].SetAvailable(true);
                return true;
            }
        }

        return false;
    }

    private bool CheckPlaceForCollect(int val, int id)
    {
        var check = CheckPlayerCanCollectPieces(CurrentPieceType);


        if (check.Item1)
            return CheckPlaceForCollectAlternate(val, id);

        var _places = CurrentPieceType == PieceType.White ? _whiteCollectIds : _blackCollectIds;

        if (!check.Item1 && check.Item2 > 1)
            return false;

        else if (check.Item2 == 1 && _places.Contains(id))
            return false;



        if (val == places.Count || val == -1)
        {
            print(val);

            var place = CurrentPieceType == PieceType.White ? _whiteCollectPlace : _blackCollectPlace;
            if (currentPiece != null)
                place.SetAvailable(true);
            return true;
        }
        return false;
    }

    private bool CheckPlaceForCollectAlternate(int val, int id)
    {
        var place = CurrentPieceType == PieceType.White ? _whiteCollectPlace : _blackCollectPlace;

        if (val == places.Count || val == -1)
        {
            if (currentPiece != null)
                place.SetAvailable(true);
            return true;
        }

        if (val > places.Count || val < -1)
        {
            var indexes = CurrentPieceType == PieceType.White ? _whiteCollectIds : _blackCollectIds;
            int startIndex = indexes.IndexOf(id);
            for (int i = startIndex + 1; i < 6; i++)
            {
                if (places[indexes[i]].GetPieceCount > 0 && places[indexes[i]].GetLastPieceType == CurrentPieceType)
                    return false;
            }

            if (currentPiece != null)
                place.SetAvailable(true);
            return true;
        }

        return false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            var pieces = GameObject.FindObjectsOfType<Piece>();
            foreach (var piece in pieces)
            {
                if (piece.PieceType == PieceType.Black)
                {
                    _blackCollectPlace.AddPiece(piece);
                }
            }
        }

        if (currentPiece == null)
            return;

        currentPiece.SetPos(GetMousePos());

    }

    public void OnDrop(BoardPlace place, bool isCollectedPiece)
    {
        SoundManager.Instance.PlaySound(SoundTypes.PlaceSound);
        BoardPlace oldPlace = currentPiece.transform.parent.GetComponent<BoardPlace>();
        int moveVal = Mathf.Abs(oldPlace.Id - place.Id);

        if (isCollectedPiece && !DiceManager.Values.Contains(moveVal))
        {
            bool hasChanged = false;
            var list = DiceManager.Values.OrderBy(x => x).ToList();
            foreach (var item in list)
            {
                if (item > moveVal)
                {
                    moveVal = item;
                    hasChanged = true;
                    break;
                }
            }

            if (!hasChanged)
                moveVal = DiceManager.TotalValue;
        }

        Move move = new Move(currentPiece, oldPlace, place, moveVal);

        moves.Add(move);

        oldPlace.RemovePiece(currentPiece);
        place.AddPiece(currentPiece);

        if (moveVal == DiceManager.GetRealTotalValue || (DiceManager.isEqual && moveVal > DiceManager.diceVal1))
        {
            BrokePiecesOnFastMove(oldPlace.Id, moveVal);
        }

        DiceManager.OnPiecePlaced(moveVal);

        CloseAllPlaces();
        currentPiece = null;
    }

    public async void OnEndDrag(BoardPlace place)
    {
        await Task.Delay(10);
        if (currentPiece != null && currentPiece.transform.parent == place.transform)
        {
            CloseAllPlaces();
            var list = DiceManager.Values.OrderByDescending(x => x).ToList();
            if (DiceManager.isEqual)
                list = list.OrderBy(x => x).ToList();

            foreach (var val in list)
            {
                var total = CurrentPieceType == PieceType.White ? place.Id - val : place.Id + val;

                if (total <= -1 && CheckPlayerCanCollectPieces(CurrentPieceType).Item1)
                    OnDrop(_whiteCollectPlace, true);

                else if (total >= places.Count && CheckPlayerCanCollectPieces(CurrentPieceType).Item1)
                    OnDrop(_blackCollectPlace, true);

                else if (total >= 0 && total < places.Count && places[total].CheckAvailable())
                    OnDrop(places[total], false);

                else
                    continue;

                break;
            }

            currentPiece = null;
        }

        _boardCanvas.ToggleReturnButton(moves.Count > 0);
        _boardCanvas.ToggleDoneButton(DiceManager.Values.Count <= 0);

        if (DiceManager.Values.Count > 0)
            CheckPlaces();
        else
            DiceManager.ResetFill();
    }

    public void AddBrokenPiece(Piece piece)
    {
        SoundManager.Instance.PlaySound(SoundTypes.BrokeSound);
        ParticleManager.Instance.PlayBrokeParticle(piece.transform.position);
        var broken = _brokenVariables.First(x => x.pieceType == piece.PieceType);
        moves[^1].AddBrokenPiece(piece, piece.transform.parent.GetComponent<BoardPlace>());
        broken.parent.AddPiece(piece);
    }

    private void MoveDone()
    {
        _boardCanvas.ToggleGameButtons(false);
        moves.Clear();
        _boardCanvas.ToggleDoneButton(false);
        _boardCanvas.ToggleReturnButton(false);
        SoundManager.Instance.PlaySound(SoundTypes.NewTourSound);
        CurrentPieceType = CurrentPieceType == PieceType.White ? PieceType.Black : PieceType.White;
        DiceManager.OnTourDone();
        isFirstCheck = true;
    }

    private void MoveBack()
    {
        if (moves.Count <= 0)
            return;

        SoundManager.Instance.PlaySound(SoundTypes.PlaceSound);
        Move move = moves[^1];
        DiceManager.OnMoveReturn(move.moveVal);
        move.newPlace.RemovePiece(move.piece);
        move.oldPlace.AddPiece(move.piece);

        if (move.brokenPiece != null)
        {
            _brokenVariables.First(x => x.pieceType == move.brokenPiece.PieceType).parent.RemovePiece(move.brokenPiece);
            move.brokenPieceOldPlace.AddPiece(move.brokenPiece);
        }

        moves.Remove(move);

        _boardCanvas.ToggleReturnButton(moves.Count > 0);
        _boardCanvas.ToggleDoneButton(DiceManager.Values.Count <= 0);

        CheckPlaces();
    }

    //�lk parametre toplayabilir mi �kinci parametre ka� tane kald�
    private (bool, int) CheckPlayerCanCollectPieces(PieceType type)
    {
        bool canCollect = true;
        int count = 0;
        var list = type == PieceType.White ? _whiteCollectIds : _blackCollectIds;

        if (_brokenVariables.First(x => x.pieceType == type).parent.GetPieceCount > 0)
            return (false, 0);

        foreach (var place in places)
        {
            if (place.GetPieceCount > 0 && place.GetLastPieceType == type)
            {
                if (!list.Contains(place.Id))
                {
                    canCollect = false;
                    count += place.GetPieceCount;
                }
            }
        }
        return (canCollect, count);
    }

    public void CloseAllPlaces()
    {
        places.ForEach(x => x.SetAvailable(false));
        _brokenVariables.ForEach(x => x.parent.SetAvailable(false));
        _whiteCollectPlace.SetAvailable(false);
        _blackCollectPlace.SetAvailable(false);
    }

    private void BrokePiecesOnFastMove(int startId, int moveVal)
    {
        print("Check pi�");
        var list = new List<int>();
        list.AddRange(DiceManager.Values);

        if (DiceManager.isEqual)
        {
            List<int> removeIds = new List<int>();
            foreach (var index in list)
            {
                if (index >= moveVal)
                    removeIds.Add(index);
            }
            foreach (var id in removeIds)
            {
                list.Remove(id);
            }
        }

        foreach (var val in list)
        {
            var total = CurrentPieceType == PieceType.White ? startId - val : startId + val;
            if (total < places.Count && total >= 0)
            {
                if (places[total].GetPieceCount == 1 && places[total].GetLastPieceType != CurrentPieceType)
                {
                    print("Checking");

                    places[total].Broke();
                }
            }
        }
    }

    private void Restart()
    {
        places.ForEach(x => x.RemoveAllPieces());
        places.ForEach(x => x.AddStartPieces());
        _brokenVariables.ForEach(x => x.parent.RemoveAllPieces());
        _whiteCollectPlace.RemoveAllPieces();
        _blackCollectPlace.RemoveAllPieces();
        moves.Clear();
        _boardCanvas.ToggleDoneButton(false);
        _boardCanvas.ToggleReturnButton(false);
        isFirstCheck = true;
        CloseAllPlaces();
        SoundManager.Instance.PlaySound(SoundTypes.NewTourSound);

        _boardCanvas.CloseWinPanel(CurrentPieceType);
        DiceManager.OnTourDone();
    }
}

[System.Serializable]
public class BrokenVariables
{
    public List<BoardPlace> places;
    public BoardPlace parent;
    public PieceType pieceType;
}

[System.Serializable]
public class Move
{
    public Piece piece;
    public BoardPlace oldPlace;
    public BoardPlace newPlace;
    public int moveVal;
    public Piece brokenPiece;
    public BoardPlace brokenPieceOldPlace;

    public Move(Piece piece, BoardPlace oldPlace, BoardPlace newPlace, int moveVal)
    {
        this.piece = piece;
        this.oldPlace = oldPlace;
        this.newPlace = newPlace;
        this.moveVal = moveVal;
    }

    public void AddBrokenPiece(Piece brokenPiece, BoardPlace brokenPieceOldPlace)
    {
        this.brokenPiece = brokenPiece;
        this.brokenPieceOldPlace = brokenPieceOldPlace;
    }
}

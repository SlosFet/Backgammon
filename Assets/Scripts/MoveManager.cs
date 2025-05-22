using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MoveManager : Singleton<MoveManager>
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


    public LayerMask layer;
    public bool HasPiece => currentPiece != null;
    private bool isFirstCheck = true;

    private void Start()
    {
        for (int i = 0; i < places.Count; i++)
        {
            places[i].Id = i;
        }

        SoundManager.Instance.PlaySound(SoundTypes.NewTourSound);

        _whiteCollectPlace.OnPlayerCollectedAll.AddListener(_boardCanvas.OpenWinPanel);
        _blackCollectPlace.OnPlayerCollectedAll.AddListener(_boardCanvas.OpenWinPanel);

        _boardCanvas.SubscribeToRestart(Restart);
        _boardCanvas.SubscribeToDoneReturn(MoveBack,MoveDone);

    }

    public Vector3 GetMousePos()
    {
        Vector3 pos = Vector3.forward * -0.2f;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, layer))
        {
            return hitInfo.point - (ray.direction * distance);
        }

        return pos;
    }

    public void SetCurrentPiece(Piece piece)
    {
        SoundManager.Instance.PlaySound(SoundTypes.TakeSound);
        currentPiece = piece;
        _boardCanvas.ToggleReturnButton(false);
    }

    public void CheckPlaces()
    {
        CloseAllPlaces();
        DiceManager.Instance.ResetFill();

        var broken = _brokenVariables.First(x => x.pieceType == GameManager.CurrentPieceType);

        bool canPlayerPlay = false;


        if (broken.parent.GetPieceCount > 0)
            canPlayerPlay = CheckForBroken(broken);

        else if (CheckPlayerCanCollectPieces(GameManager.CurrentPieceType).Item1)
            canPlayerPlay = CheckForCollect();

        else
            canPlayerPlay = CheckForNormal();


        if (!canPlayerPlay)
        {
            //Eðer zar ilk atýldýðýnda check ediliyor ve oynanacak yer yoksa sýra otomatik olarak geçer
            if (isFirstCheck)
                Invoke(nameof(MoveDone), 1);

            //Deðilse oyuncuya taþýný geri almasý veya tamamlamasý için hak sunulur
            else
                 _boardCanvas.ToggleDoneButton(true);
        }

        isFirstCheck = false;
    }

    private bool CheckForNormal()
    {
        List<BoardPlace> selectedPlaces = new List<BoardPlace>();

        //Önce masadaki bütün yerleri kontrol eder
        foreach (var place in places)
        {
            //Eðer yer sýradaki oyuncu için uygun taþ barýndýrmýyor veya boþsa döngüye devam eder
            if (!place.CheckAvailableForChoose())
                continue;

            //Yerdeki taþ gelen zara göre hareket edebilir mi kontrol eder
            if (CalculateAvailablePosses(place.Id))
            {
                selectedPlaces.Add(place);
            }
        }

        //Hareket ettirilebilen taþ olan yerler mavi ýþýkla oyuncuya gösterilir
        foreach (var place in selectedPlaces)
        {
            place.SetAvailableForChoose();
        }

        //Oyuncunun hareketinin olup olmadýðý kontrol edilir
        return selectedPlaces.Count > 0;
    }

    private bool CheckForBroken(BrokenVariables broken)
    {
        //Oyuncunun kýrýk taþýnýn bulunduðu id den yerleþim yerlerine konulabilir mi bakar
        if (CalculateAvailablePosses(broken.parent.Id))
        {
            //Eðer konulabiliyorsa set eder
            broken.parent.SetAvailable(true);

            //Konulabilecek yerleri yeþil yapar eðer oyuncu taþa dokunduysa yeþil yapar yoksa yanmaz
            foreach (int val in DiceManager.Instance.Values)
            {
                var total = GameManager.CurrentPieceType == PieceType.White ? broken.parent.Id - val : broken.parent.Id + val;
                CheckPlace(total);
            }

            return true;
        }

        return false;
    }

    private bool CheckForCollect()
    {
        List<BoardPlace> selectedPlaces = new List<BoardPlace>();

        //Önce masadaki bütün yerleri kontrol eder
        foreach (var place in places)
        {
            //Eðer yer sýradaki oyuncu için uygun taþ barýndýrmýyor veya boþsa döngüye devam eder
            if (!place.CheckAvailableForChoose())
                continue;

            //Yerdeki taþ gelen zara göre hareket edebilir mi kontrol eder
            if (CalculateAvailablePosses(place.Id))
            {
                selectedPlaces.Add(place);
            }
        }

        //Hareket ettirilebilen taþ olan yerler mavi ýþýkla oyuncuya gösterilir
        foreach (var place in selectedPlaces)
        {
            place.SetAvailableForChoose();
        }

        //Oyuncunun hareketinin olup olmadýðý kontrol edilir
        return selectedPlaces.Count > 0;
    }

    public bool CalculateAvailablePosses(int id)
    {
        bool hasAnyPlace = false;
        foreach (int val in DiceManager.Instance.Values)
        {
            bool checkForEqual = false;

            var total = GameManager.CurrentPieceType == PieceType.White ? id - val : id + val;
            if (CheckPlace(total) || CheckPlaceForCollect(total, id))
            {
                checkForEqual = true;
                hasAnyPlace = true;
                DiceManager.Instance.SetFill(val);
            }

            if (DiceManager.Instance.isEqual && !checkForEqual)
                break;
        }

        if (!hasAnyPlace)
            return false;

        var totalValue = DiceManager.Instance.TotalValue;

        if (totalValue > 0)
        {
            var total = GameManager.CurrentPieceType == PieceType.White ? id - totalValue : id + totalValue;
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
        var check = CheckPlayerCanCollectPieces(GameManager.CurrentPieceType);


        if (check.Item1)
            return CheckPlaceForCollectAlternate(val, id);

        var _places = GameManager.CurrentPieceType == PieceType.White ? _whiteCollectIds : _blackCollectIds;

        if (!check.Item1 && check.Item2 > 1)
            return false;

        else if (check.Item2 == 1 && _places.Contains(id))
            return false;



        if (val == places.Count || val == -1)
        {
            print(val);

            var place = GameManager.CurrentPieceType == PieceType.White ? _whiteCollectPlace : _blackCollectPlace;
            if (currentPiece != null)
                place.SetAvailable(true);
            return true;
        }
        return false;
    }

    private bool CheckPlaceForCollectAlternate(int val, int id)
    {
        var place = GameManager.CurrentPieceType == PieceType.White ? _whiteCollectPlace : _blackCollectPlace;

        if (val == places.Count || val == -1)
        {
            if (currentPiece != null)
                place.SetAvailable(true);
            return true;
        }

        if (val > places.Count || val < -1)
        {
            var indexes = GameManager.CurrentPieceType == PieceType.White ? _whiteCollectIds : _blackCollectIds;
            int startIndex = indexes.IndexOf(id);
            for (int i = startIndex + 1; i < 6; i++)
            {
                if (places[indexes[i]].GetPieceCount > 0 && places[indexes[i]].GetLastPieceType == GameManager.CurrentPieceType)
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
        if(Input.GetKeyDown(KeyCode.U))
        {
            var pieces = GameObject.FindObjectsOfType<Piece>();
            foreach (var piece in pieces)
            {
                if(piece.PieceType == PieceType.Black)
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

        if (isCollectedPiece && !DiceManager.Instance.Values.Contains(moveVal))
        {
            bool hasChanged = false;
            var list = DiceManager.Instance.Values.OrderBy(x => x).ToList();
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
                moveVal = DiceManager.Instance.TotalValue;
        }

        Move move = new Move(currentPiece, oldPlace, place, moveVal);

        moves.Add(move);

        oldPlace.RemovePiece(currentPiece);
        place.AddPiece(currentPiece);

        if (moveVal == DiceManager.Instance.GetRealTotalValue || (DiceManager.Instance.isEqual && moveVal > DiceManager.Instance.diceVal1))
        {
            BrokePiecesOnFastMove(oldPlace.Id, moveVal);
        }

        DiceManager.Instance.OnPiecePlaced(moveVal);

        CloseAllPlaces();
        currentPiece = null;
    }

    public async void OnEndDrag(BoardPlace place)
    {
        await Task.Delay(10);
        if (currentPiece != null && currentPiece.transform.parent == place.transform)
        {
            CloseAllPlaces();
            var list = DiceManager.Instance.Values.OrderBy(x => x).ToList();

            foreach (var val in list)
            {
                var total = GameManager.CurrentPieceType == PieceType.White ? place.Id - val : place.Id + val;

                if (total <= -1 && CheckPlayerCanCollectPieces(GameManager.CurrentPieceType).Item1)
                    OnDrop(_whiteCollectPlace, true);

                else if (total >= places.Count && CheckPlayerCanCollectPieces(GameManager.CurrentPieceType).Item1)
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
        _boardCanvas.ToggleDoneButton(DiceManager.Instance.Values.Count <= 0);

        if (DiceManager.Instance.Values.Count > 0)
            CheckPlaces();
        else
            DiceManager.Instance.ResetFill();
    }

    public void AddBrokenPiece(Piece piece)
    {
        SoundManager.Instance.PlaySound(SoundTypes.BrokeSound);
        var broken = _brokenVariables.First(x => x.pieceType == piece.PieceType);
        moves[^1].AddBrokenPiece(piece, piece.transform.parent.GetComponent<BoardPlace>());
        broken.parent.AddPiece(piece);
    }

    private void MoveDone()
    {
        moves.Clear();
        _boardCanvas.ToggleDoneButton(false);
        _boardCanvas.ToggleReturnButton(false);
        SoundManager.Instance.PlaySound(SoundTypes.NewTourSound);
        GameManager.Instance.TourDone();
        isFirstCheck = true;
    }

    private void MoveBack()
    {
        if (moves.Count <= 0)
            return;

        SoundManager.Instance.PlaySound(SoundTypes.PlaceSound);
        Move move = moves[^1];
        DiceManager.Instance.OnMoveReturn(move.moveVal);
        move.newPlace.RemovePiece(move.piece);
        move.oldPlace.AddPiece(move.piece);

        if (move.brokenPiece != null)
        {
            _brokenVariables.First(x => x.pieceType == move.brokenPiece.PieceType).parent.RemovePiece(move.brokenPiece);
            move.brokenPieceOldPlace.AddPiece(move.brokenPiece);
        }

        moves.Remove(move);

        _boardCanvas.ToggleReturnButton(moves.Count > 0);
        _boardCanvas.ToggleDoneButton(DiceManager.Instance.Values.Count <= 0);

        CheckPlaces();
    }

    //Ýlk parametre toplayabilir mi Ýkinci parametre kaç tane kaldý
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

    private void BrokePiecesOnFastMove(int startId,int moveVal)
    {
        print("Check piç");
        var list = new List<int>();
        list.AddRange(DiceManager.Instance.Values);

        if (DiceManager.Instance.isEqual)
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
            var total = GameManager.CurrentPieceType == PieceType.White ? startId - val : startId + val;
            if (total < places.Count && total >= 0)
            {
                if (places[total].GetPieceCount == 1 && places[total].GetLastPieceType != GameManager.CurrentPieceType)
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
        _brokenVariables.ForEach(x=>x.parent.RemoveAllPieces());
        _whiteCollectPlace.RemoveAllPieces();
        _blackCollectPlace.RemoveAllPieces();
        moves.Clear();
        _boardCanvas.ToggleDoneButton(false);
        _boardCanvas.ToggleReturnButton(false);
        isFirstCheck = true;
        CloseAllPlaces();
        SoundManager.Instance.PlaySound(SoundTypes.NewTourSound);

        _boardCanvas.CloseWinPanel(GameManager.CurrentPieceType);
        DiceManager.Instance.OnTourDone();
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

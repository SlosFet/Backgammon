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


    [SerializeField] private Button _doneButton;
    [SerializeField] private Button _returnButton;


    [SerializeField] private List<Move> moves;


    [SerializeField] private List<BrokenVariables> _brokenVariables;


    [SerializeField] private List<int> _whiteCollectIds;
    [SerializeField] private List<int> _blackCollectIds;


    [SerializeField] private BoardPlace _whiteCollectPlace;
    [SerializeField] private BoardPlace _blackCollectPlace;


    public LayerMask layer;
    public bool HasPiece => currentPiece != null;
    private bool isFirstCheck = true;

    private void Start()
    {
        _doneButton.onClick.AddListener(MoveDone);
        _returnButton.onClick.AddListener(MoveBack);
        for (int i = 0; i < places.Count; i++)
        {
            places[i].Id = i;
        }
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
        currentPiece = piece;
        _returnButton.interactable = false;
    }

    public void CheckPlaces()
    {
        CloseAllPlaces();

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
                _doneButton.interactable = true;
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
            var total = GameManager.CurrentPieceType == PieceType.White ? id - val : id + val;
            if (CheckPlace(total))
                hasAnyPlace = true;

            if (CheckPlaceForCollect(total, id))
                hasAnyPlace = true;
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
        if (currentPiece == null)
            return;

        currentPiece.SetPos(GetMousePos());
    }

    public void OnDrop(BoardPlace place, bool isCollectedPiece)
    {
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

        if (moveVal == DiceManager.Instance.GetRealTotalValue)
        {
            BrokePiecesOnFastMove(oldPlace.Id);
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
            place.AddPiece(currentPiece);
            currentPiece = null;
        }

        _returnButton.interactable = moves.Count > 0;
        _doneButton.interactable = DiceManager.Instance.Values.Count <= 0;

        if (DiceManager.Instance.Values.Count > 0)
            CheckPlaces();
    }

    public void AddBrokenPiece(Piece piece)
    {
        var broken = _brokenVariables.First(x => x.pieceType == piece.PieceType);
        moves[^1].AddBrokenPiece(piece, piece.transform.parent.GetComponent<BoardPlace>());
        broken.parent.AddPiece(piece);
    }

    private void MoveDone()
    {
        moves.Clear();
        _returnButton.interactable = false;
        _doneButton.interactable = false;
        GameManager.Instance.TourDone();
        isFirstCheck = true;
    }

    private void MoveBack()
    {
        if (moves.Count <= 0)
            return;

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

        _returnButton.interactable = moves.Count > 0;
        _doneButton.interactable = DiceManager.Instance.Values.Count <= 0;


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

    private void BrokePiecesOnFastMove(int startId)
    {
        foreach (var val in DiceManager.Instance.Values)
        {
            var total = GameManager.CurrentPieceType == PieceType.White ? startId - val : startId + val;
            if (val < places.Count && val >= 0)
            {
                if (places[val].GetPieceCount == 1 && places[val].GetLastPieceType != GameManager.CurrentPieceType)
                {
                    print("Checking");

                    places[val].Broke();
                }
            }
        }
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

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

    public async void CheckPlaces()
    {
        List<BoardPlace> selectedPlaces = new List<BoardPlace>();
        _brokenVariables.ForEach(x=>x.parent.SetAvailable(false));

        var broken = _brokenVariables.First(x => x.pieceType == GameManager.CurrentPieceType);
        Debug.LogError(broken.parent.GetPieceCount > 0);
        broken.parent.SetAvailable(broken.parent.GetPieceCount > 0);


        var selectedList = broken.parent.GetPieceCount <= 0 ?
            places :
            broken.places;

        foreach (var place in places)
        {
            if (!place.CheckAvailableForChoose())
                continue;

            if (CalculateAvailablePosses(place.Id))
            {
                selectedPlaces.Add(place);
            }
        }

        if (broken.parent.GetPieceCount <= 0)
        {
            foreach (var place in selectedPlaces)
            {
                place.SetAvailableForChoose();
            }
        }


        if (selectedPlaces.Count <= 0)
        {
            if (isFirstCheck)
            {
                await Task.Delay(1000);
                MoveDone();
            }
            else
            {
                _doneButton.interactable = true;
            }
        }

        isFirstCheck = false;
    }

    public bool CalculateAvailablePosses(int id)
    {
        places.ForEach(x => x.SetAvailable(false));
        bool hasAnyPlace = false;
        foreach (int val in DiceManager.Instance.Values)
        {
            var total = GameManager.CurrentPieceType == PieceType.White ? id - val : id + val;
            if (CheckPlace(total))
                hasAnyPlace = true;
        }

        if (!hasAnyPlace)
            return false;

        var totalValue = DiceManager.Instance.TotalValue;

        if (totalValue > 0)
        {
            var total = GameManager.CurrentPieceType == PieceType.White ? id - totalValue : id + totalValue;
            CheckPlace(total);
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

    private void Update()
    {
        if (currentPiece == null)
            return;

        currentPiece.SetPos(GetMousePos());
    }

    public void OnDrop(BoardPlace place)
    {
        BoardPlace oldPlace = currentPiece.transform.parent.GetComponent<BoardPlace>();
        int moveVal = Mathf.Abs(oldPlace.Id - place.Id);

        DiceManager.Instance.OnPiecePlaced(moveVal);
        Move move = new Move(currentPiece, oldPlace, place, moveVal);
        moves.Add(move);

        oldPlace.RemovePiece(currentPiece);
        place.AddPiece(currentPiece);

        places.ForEach(x => x.SetAvailable(false));
        currentPiece = null;
    }

    public async void OnEndDrag(BoardPlace place)
    {
        await Task.Delay(10);
        if (currentPiece != null && currentPiece.transform.parent == place.transform)
        {
            places.ForEach(x => x.SetAvailable(false));
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
        moves.Remove(move);

        _returnButton.interactable = moves.Count > 0;
        _doneButton.interactable = DiceManager.Instance.Values.Count <= 0;


        CheckPlaces();
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

    public Move(Piece piece, BoardPlace oldPlace, BoardPlace newPlace, int moveVal)
    {
        this.piece = piece;
        this.oldPlace = oldPlace;
        this.newPlace = newPlace;
        this.moveVal = moveVal;
    }
}

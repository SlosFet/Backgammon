using System.Collections;
using System.Collections.Generic;
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

    public LayerMask layer;

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
    }

    public void CalculateAvailablePosses(int id)
    {
        places.ForEach(x => x.SetAvailable(false));
        foreach (int val in DiceManager.Instance.Values)
        {
            CheckPlace(id + val);
        }

        var totalValue = DiceManager.Instance.TotalValue;
        if (totalValue > 0)
            CheckPlace(id + totalValue);
    }

    private void CheckPlace(int val)
    {
        if (val < places.Count)
        {
            if (places[val].CheckAvailable())
            {
                places[val].SetAvailable(true);
            }
        }
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
    }

    private void MoveDone()
    {
        moves.Clear();
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
    }
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

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private int ID;
    [SerializeField] private Transform _targetPlace;
    [SerializeField] private TextMeshProUGUI _playerText;

    public bool hasAvatar { get; private set; }

    private AvatarObject avatar;

    public void Initialize(int index)
    {
        gameObject.SetActive(true);
        _playerText.text = "OYUNCU " + index;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && !hasAvatar)
        {
            hasAvatar = true;
            AvatarSelectPanel.Instance.CloseTutorial();

            avatar = eventData.pointerDrag.GetComponent<AvatarObject>();
            avatar.SetDropZone(this);
            avatar.transform.SetParent(_targetPlace);
            avatar.transform.localPosition = Vector3.zero;

            AvatarSelectPanel.Instance.AddPlayer(ID, avatar.AvatarData);
        }
    }

    public void OnRemove()
    {
        hasAvatar = false;
        avatar = null;

        AvatarSelectPanel.Instance.RemovePlayer(ID);
    }

    public void OnPlacementChange()
    {
        avatar?.ReturnToOriginPos();
        OnRemove();
        gameObject.SetActive(false);
    }
}

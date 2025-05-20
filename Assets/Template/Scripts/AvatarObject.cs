using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private DropZone dropZone;
    private Transform orijinParent;

    [SerializeField] private List<GameObject> _normalImages;
    [SerializeField] private GameObject _transparentImages;
    [field:SerializeField] public AvatarData AvatarData { get; private set; }
    private Vector3 localPos;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        localPos = transform.localPosition;
        orijinParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (dropZone)
        {
            dropZone.OnRemove();
            SetDropZone(null);
        }

        _normalImages.ForEach(x => x.SetActive(false));
        _transparentImages.SetActive(true);

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(orijinParent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        transform.localEulerAngles = Vector3.zero;

        if (transform.parent == orijinParent)
        {
            transform.localPosition = localPos;
            transform.SetSiblingIndex(1);
            _normalImages.ForEach(x => x.SetActive(true));
            _transparentImages.SetActive(false);
        }
    }

    public void ReturnToOriginPos()
    {
        transform.SetParent(orijinParent);
        transform.localEulerAngles = Vector3.zero;
        transform.localPosition = localPos;
        transform.SetSiblingIndex(1);
        _normalImages.ForEach(x => x.SetActive(true));
        _transparentImages.SetActive(false);
    }

    public void SetDropZone(DropZone dropZone)
    {
        this.dropZone = dropZone;
    }
}

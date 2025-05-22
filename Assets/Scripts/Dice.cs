using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    [Header("Roll Ayarlarý")]
    public Vector3 firstPlace;     // Tavla duvarý gibi araya çarpacaðý yer
    public Transform firstTarget;     // Tavla duvarý gibi araya çarpacaðý yer
    public Transform finalTarget;     // Zarýn duracaðý yer
    public float jumpPower = 1f;
    public float jumpPower2 = 0.3f;
    public float jumpDuration = 0.5f;
    public float moveDuration = 0.6f;
    public float spinSpeed = 360f; // derece/saniye

    public Vector3 rot; // derece/saniye

    private Dictionary<int, Quaternion> faceRotations; // 1–6 için rotasyonlar
    private bool isRolling = false;
    public int value;
    public Transform canvas;
    public Image image;

    [SerializeField] private float _canvasYOffset;

    private void Awake()
    {
        // Zar deðerlerine göre üstte hangi yüzey olmalý, o rotasyonlar
        faceRotations = new Dictionary<int, Quaternion>
        {
            { 1, Quaternion.Euler(0, 0, 90) },
            { 2, Quaternion.Euler(90, 0, 0) },
            { 3, Quaternion.Euler(0, 0, 0) },
            { 4, Quaternion.Euler(180, 0, 0) },
            { 5, Quaternion.Euler(-90, 0, 0) },
            { 6, Quaternion.Euler(0, -90, -90) }
        };
        firstPlace = transform.position;

    }

    public async Task Roll(int value)
    {

        gameObject.SetActive(true);
        canvas.gameObject.SetActive(false);
        this.value = value;
        transform.position = firstPlace;

        if (isRolling || !faceRotations.ContainsKey(value)) return;
        isRolling = true;

        Quaternion targetRotation = faceRotations[value];


        Sequence spinSeq = DOTween.Sequence();




        Tween spin = transform.DORotate(Vector3.one * spinSpeed, jumpDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear);

        // Ýlk zýplama (duvara çarpma simülasyonu)
        await transform.DOJump(firstTarget.position, jumpPower, 1, jumpDuration).SetEase(Ease.Linear).AsyncWaitForCompletion();
        spin.Kill();

        transform.DOJump(finalTarget.position, jumpPower2, 1, moveDuration);
        await transform.DORotate(targetRotation.eulerAngles, moveDuration, RotateMode.FastBeyond360).SetEase(Ease.OutCubic).AsyncWaitForCompletion();


        canvas.gameObject.SetActive(true);
        isRolling = false;
        canvas.transform.localPosition = Vector3.zero;
        canvas.transform.position += Vector3.up * _canvasYOffset;
        canvas.transform.eulerAngles = Vector3.zero;
        SetImageFill(0);
        await Task.CompletedTask;
    }

    public void SetImageFill(float amount) => image.fillAmount = amount;
    public float GetImageFill() => image.fillAmount;

    public void RollFirst(int value)
    {
        canvas.gameObject.SetActive(false);
        gameObject.SetActive(true);
        var targetRotation = faceRotations[value];
        transform.eulerAngles = new Vector3(targetRotation.eulerAngles.x + Random.Range(90f, 270f), targetRotation.eulerAngles.y + Random.Range(90f, 270f), targetRotation.eulerAngles.z + Random.Range(90f, 270f));

        transform.DOJump(transform.position, 0.5f, 1, 0.5f);
        transform.DORotate(targetRotation.eulerAngles * 360 * spinSpeed, 0.4f, RotateMode.FastBeyond360).SetEase(Ease.Linear).OnComplete(() =>
        transform.DORotate(targetRotation.eulerAngles, 0.1f, RotateMode.FastBeyond360).SetEase(Ease.OutCubic));
    }
}

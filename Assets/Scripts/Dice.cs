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
    public float spinSpeed2 = 360f; // derece/saniye
    public Vector3 rot; // derece/saniye

    private Dictionary<int, Quaternion> faceRotations; // 1–6 için rotasyonlar
    private bool isRolling = false;
    public int value;
    public Transform canvas;
    public Image image;

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
        print("Value : " + value + " Rotation : " + targetRotation.eulerAngles);
        // Animasyon sýrasýnda sürekli dönsün
        Tween spin = transform.DORotate(new Vector3(Random.Range(0.6f, 1f), 0, Random.Range(0.6f, 1f)) * spinSpeed, .3f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1);

        // Ýlk zýplama (duvara çarpma simülasyonu)
        await transform.DOJump(firstTarget.position, jumpPower, 1, jumpDuration).SetEase(Ease.Linear).AsyncWaitForCompletion();

        spin.Kill();
        //Tween spin2 = transform.DORotate(new Vector3(Random.Range(0.6f, 1f), 0, Random.Range(0.6f, 1f)) * spinSpeed2, .3f, RotateMode.FastBeyond360)
        // .SetEase(Ease.Linear)
        //  .SetLoops(-1);

        transform.DOJump(finalTarget.position, jumpPower2, 1, moveDuration);
        //await Task.Delay((int)(moveDuration * 1000) / 2);
        //spin2.Kill();

        canvas.gameObject.SetActive(true);
        transform.rotation = targetRotation;
        isRolling = false;
        canvas.transform.eulerAngles = Vector3.zero;
        SetImageFill(0);
        await Task.CompletedTask;
    }

    public void SetImageFill(float amount) => image.fillAmount = amount;
    public float GetImageFill() => image.fillAmount ;
}

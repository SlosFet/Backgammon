using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Dice : MonoBehaviour
{
    [Header("Roll Ayarlar�")]
    public Vector3 firstPlace;     // Tavla duvar� gibi araya �arpaca�� yer
    public Transform firstTarget;     // Tavla duvar� gibi araya �arpaca�� yer
    public Transform finalTarget;     // Zar�n duraca�� yer
    public float jumpPower = 1f;
    public float jumpPower2 = 0.3f;
    public float jumpDuration = 0.5f;
    public float moveDuration = 0.6f;
    public float spinSpeed = 360f; // derece/saniye
    public float spinSpeed2 = 360f; // derece/saniye
    public Vector3 rot; // derece/saniye

    private Dictionary<int, Quaternion> faceRotations; // 1�6 i�in rotasyonlar
    private bool isRolling = false;

    private void Awake()
    {
        // Zar de�erlerine g�re �stte hangi y�zey olmal�, o rotasyonlar
        faceRotations = new Dictionary<int, Quaternion>
        {
            { 1, Quaternion.Euler(0, 0, 90) },
            { 2, Quaternion.Euler(90, 0, 0) },
            { 3, Quaternion.Euler(0, 0, 0) },
            { 4, Quaternion.Euler(180, 0, 0) },
            { 5, Quaternion.Euler(-90, 0, 0) },
            { 6, Quaternion.Euler(0, -90, -90) },
        };
        firstPlace = transform.position;

    }

    public async void Roll(int value)
    {
        transform.position = firstPlace;
        gameObject.SetActive(true);
        if (isRolling || !faceRotations.ContainsKey(value)) return;
        isRolling = true;

        Quaternion targetRotation = faceRotations[value];

        // Animasyon s�ras�nda s�rekli d�ns�n
        Tween spin = transform.DORotate(new Vector3(Random.Range(0.6f, 1f), 0, Random.Range(0.6f, 1f)) * spinSpeed, .3f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1);

        // �lk z�plama (duvara �arpma sim�lasyonu)
        transform.DOJump(firstTarget.position, jumpPower, 1, jumpDuration).SetEase(Ease.Linear).OnComplete(async () =>
        {
            spin.Kill();
            Tween spin2 = transform.DORotate(new Vector3(Random.Range(0.6f, 1f), 0, Random.Range(0.6f, 1f)) * spinSpeed2, .3f, RotateMode.FastBeyond360)
             .SetEase(Ease.Linear)
              .SetLoops(-1);

            transform.DOJump(finalTarget.position, jumpPower2, 1, moveDuration);
            await Task.Delay((int)(moveDuration * 1000) / 2);
            spin2.Kill();

            transform.DORotate(targetRotation.eulerAngles, moveDuration / 2f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .OnComplete(() => isRolling = false);

        });
    }
}

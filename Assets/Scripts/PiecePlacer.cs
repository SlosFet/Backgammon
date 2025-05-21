using System.Collections.Generic;
using UnityEngine;

public class PiecePlacer : MonoBehaviour
{
    [Header("3D Yerle�im Ayarlar�")]          // Y�kseklik (Y ekseni)
    public float spacing = 0.05f;                // Normal aral�k
    public float startY = 0f;                    // Ba�lang�� Y pozisyonu
    public int maxVisiblePieces = 5;             // Bu kadar ta�a kadar normal dizilim
    public float zOffsetPerPiece = -0.01f;       // S�k���k modda her ta��n Z'ye kayd�r�lmas� (negatif geri �eker)
    public Vector3 targetRot;       // S�k���k modda her ta��n Z'ye kayd�r�lmas� (negatif geri �eker)

    public void PlacePieces(List<Piece> pieces)
    {
        if (pieces == null || pieces.Count == 0) return;

        int count = pieces.Count;

        for (int i = 0; i < count; i++)
        {
            Transform piece = pieces[i].transform;
            if (piece == null) continue;

            Vector3 localPos = Vector3.zero;

            if (count <= maxVisiblePieces)
            {
                // Normal dizilim
                localPos.y = startY + i * spacing;
                localPos.z = 0f;
            }
            else
            {
                // S�k���k dizilim
                float compressedHeight = spacing * maxVisiblePieces / count;
                localPos.y = startY + i * compressedHeight;
                localPos.z = i * zOffsetPerPiece;
            }

            pieces[i].MovePos(localPos, targetRot);
        }
    }
}

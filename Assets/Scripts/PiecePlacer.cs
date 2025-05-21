using System.Collections.Generic;
using UnityEngine;

public class PiecePlacer : MonoBehaviour
{
    [Header("3D Yerleþim Ayarlarý")]          // Yükseklik (Y ekseni)
    public float spacing = 0.05f;                // Normal aralýk
    public float startY = 0f;                    // Baþlangýç Y pozisyonu
    public int maxVisiblePieces = 5;             // Bu kadar taþa kadar normal dizilim
    public float zOffsetPerPiece = -0.01f;       // Sýkýþýk modda her taþýn Z'ye kaydýrýlmasý (negatif geri çeker)
    public Vector3 targetRot;       // Sýkýþýk modda her taþýn Z'ye kaydýrýlmasý (negatif geri çeker)

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
                // Sýkýþýk dizilim
                float compressedHeight = spacing * maxVisiblePieces / count;
                localPos.y = startY + i * compressedHeight;
                localPos.z = i * zOffsetPerPiece;
            }

            pieces[i].MovePos(localPos, targetRot);
        }
    }
}

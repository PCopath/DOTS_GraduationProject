// ChunkController.cs
using UnityEngine;

public class ChunkController : MonoBehaviour
{
    public Vector2Int ChunkCoordinates { get; private set; } // Bu chunk'�n �zgaradaki koordinatlar�

    public void Initialize(Vector2Int coordinates)
    {
        ChunkCoordinates = coordinates;
        // Chunk'a �zg� ek ba�lang�� i�lemleri burada yap�labilir
    }

    // Chunk'� devre d��� b�rak�rken �a�r�lacak metod
    public void Deactivate()
    {
        // �rne�in, t�m child objelerini kapatabilir veya render'lar�n� devre d��� b�rakabilirsiniz
        gameObject.SetActive(false);
    }

    // Chunk'� aktifle�tirirken �a�r�lacak metod
    public void Activate()
    {
        gameObject.SetActive(true);
        // Chunk'a �zg� ek aktivasyon i�lemleri burada yap�labilir
    }
}
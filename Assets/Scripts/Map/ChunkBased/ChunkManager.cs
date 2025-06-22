// ChunkManager.cs
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public Transform playerTransform; // Karakterinizin Transform'u
    public float chunkSize = 10f; // Chunk'lar�n bir kenar uzunlu�u (�rn: 10 birim)
    public int renderDistance = 3; // Oyuncunun etraf�nda ka� chunk g�r�necek (�rn: 3x3 bir alan i�in 1)
                                   // renderDistance = 1 ise 3x3 alan (merkez + 8 kom�u)
                                   // renderDistance = 2 ise 5x5 alan (merkez + 24 kom�u)

    private Vector2Int lastPlayerChunkCoords;
    private Dictionary<Vector2Int, GameObject> activeChunks; // Aktif chunk'lar� tutar

    private void Start()
    {
        activeChunks = new Dictionary<Vector2Int, GameObject>();
        lastPlayerChunkCoords = GetChunkCoordinates(playerTransform.position);
        GenerateInitialChunks();
    }

    private void Update()
    {
        Vector2Int currentPlayerChunkCoords = GetChunkCoordinates(playerTransform.position);

        if (currentPlayerChunkCoords != lastPlayerChunkCoords)
        {
            UpdateChunks(currentPlayerChunkCoords);
            lastPlayerChunkCoords = currentPlayerChunkCoords;
        }
    }

    // D�nya pozisyonundan chunk koordinatlar�n� al�r
    Vector2Int GetChunkCoordinates(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / chunkSize);
        int y = Mathf.FloorToInt(worldPosition.y / chunkSize); // 2D top-down oldu�u i�in y eksenini kullan�yoruz
        return new Vector2Int(x, y);
    }

    // Ba�lang��ta chunk'lar� olu�tur
    void GenerateInitialChunks()
    {
        UpdateChunks(lastPlayerChunkCoords);
    }

    // Chunk'lar� g�ncelleme ve y�netim ana fonksiyonu
    void UpdateChunks(Vector2Int currentPlayerChunkCoords)
    {
        HashSet<Vector2Int> chunksToKeep = new HashSet<Vector2Int>();

        // Yeni g�r�n�r alandaki chunk'lar� belirle ve olu�tur/aktifle�tir
        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int y = -renderDistance; y <= renderDistance; y++)
            {
                Vector2Int targetChunkCoords = new Vector2Int(currentPlayerChunkCoords.x + x, currentPlayerChunkCoords.y + y);
                chunksToKeep.Add(targetChunkCoords);

                if (!activeChunks.ContainsKey(targetChunkCoords))
                {
                    // Yeni bir chunk gerekiyor, havuzdan al veya olu�tur
                    GameObject chunkObj = ChunkPoolManager.Instance.GetRandomChunk(); // Rastgele chunk prefab� se�ebilirsiniz
                    if (chunkObj != null)
                    {
                        chunkObj.name = "Chunk_" + targetChunkCoords.x + "_" + targetChunkCoords.y; // Kolay takip i�in isim verelim
                        // Chunk'�n sol alt k��esini d�nya pozisyonuna ayarlayal�m
                        chunkObj.transform.position = new Vector3(targetChunkCoords.x * chunkSize, targetChunkCoords.y * chunkSize, 0);
                        ChunkController chunkController = chunkObj.GetComponent<ChunkController>();
                        if (chunkController != null)
                        {
                            chunkController.Initialize(targetChunkCoords);
                            chunkController.Activate();
                        }
                        activeChunks.Add(targetChunkCoords, chunkObj);
                    }
                }
            }
        }

        // Art�k oyuncu etraf�nda olmayan chunk'lar� devre d��� b�rak
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (var entry in activeChunks)
        {
            if (!chunksToKeep.Contains(entry.Key))
            {
                chunksToRemove.Add(entry.Key);
            }
        }

        foreach (Vector2Int coords in chunksToRemove)
        {
            GameObject chunkObj = activeChunks[coords];
            ChunkController chunkController = chunkObj.GetComponent<ChunkController>();
            if (chunkController != null)
            {
                chunkController.Deactivate(); // Chunk'� devre d��� b�rak
            }
            ChunkPoolManager.Instance.ReturnChunk(chunkObj); // Havuza geri d�nd�r
            activeChunks.Remove(coords);
        }
    }
}
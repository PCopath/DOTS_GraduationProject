// ChunkPoolManager.cs
using System.Collections.Generic;
using UnityEngine;

public class ChunkPoolManager : MonoBehaviour
{
    public static ChunkPoolManager Instance { get; private set; } // Singleton Pattern
    public GameObject[] chunkPrefabs; // Farkl� chunk prefab'lar�n�z� buraya s�r�kleyin
    public int initialPoolSize = 10; // Ba�lang��ta havuzda ka� chunk olacak

    private Dictionary<string, Queue<GameObject>> chunkPools; // Her prefab tipi i�in ayr� bir kuyruk

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        InitializePool();
    }

    private void InitializePool()
    {
        chunkPools = new Dictionary<string, Queue<GameObject>>();
        foreach (GameObject prefab in chunkPrefabs)
        {
            string prefabName = prefab.name;
            chunkPools.Add(prefabName, new Queue<GameObject>());

            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject chunk = Instantiate(prefab, transform);
                chunk.SetActive(false); // Ba�lang��ta inaktif
                chunkPools[prefabName].Enqueue(chunk);
            }
        }
    }

    public GameObject GetChunk(string prefabName)
    {
        if (chunkPools.ContainsKey(prefabName) && chunkPools[prefabName].Count > 0)
        {
            GameObject chunk = chunkPools[prefabName].Dequeue();
            chunk.SetActive(true);
            return chunk;
        }
        else
        {
            // Havuzda yoksa veya yeterli de�ilse yeni bir tane olu�tur
            foreach (GameObject prefab in chunkPrefabs)
            {
                if (prefab.name == prefabName)
                {
                    GameObject newChunk = Instantiate(prefab, transform);
                    return newChunk;
                }
            }
            Debug.LogError($"Chunk prefab not found: {prefabName}");
            return null;
        }
    }

    public void ReturnChunk(GameObject chunkToReturn)
    {
        chunkToReturn.SetActive(false);
        string prefabName = chunkToReturn.name.Replace("(Clone)", ""); // Prefab ad�n� bul
        if (chunkPools.ContainsKey(prefabName))
        {
            chunkPools[prefabName].Enqueue(chunkToReturn);
        }
        else
        {
            // Bu durum genellikle olmaz, ancak hata ay�klama i�in b�rak�labilir
            Debug.LogWarning($"Attempted to return unknown chunk: {chunkToReturn.name}");
            Destroy(chunkToReturn); // G�venlik i�in yok et
        }
    }

    // Belirli bir prefab ad�yla rastgele bir chunk alma fonksiyonu
    public GameObject GetRandomChunk()
    {
        if (chunkPrefabs.Length == 0) return null;
        GameObject randomPrefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Length)];
        return GetChunk(randomPrefab.name);
    }
}
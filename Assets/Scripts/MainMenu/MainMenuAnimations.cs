using UnityEngine;

public class MainMenuAnimations : MonoBehaviour
{
    public float moveSpeed = 5f; // Hareket h�z�
    public float rangeX = 15f;   // X eksenindeki maksimum menzil
    public float rangeY = 15f;   // Y eksenindeki maksimum menzil
    public float targetReachThreshold = 0.1f; // Hedefe ne kadar yakla�t���m�zda yeni hedef belirleyece�imiz e�ik

    private Vector3 initialPosition; // Objelerin ba�lang�� konumu
    private Vector3 targetPosition;  // Objenin gidece�i hedef konum

    void Start()
    {
        // Objelerin ba�lang�� konumunu kaydet
        initialPosition = transform.position;
        // �lk hedefi belirle
        SetNewRandomTarget();
    }

    void Update()
    {
        // Objeyi hedefe do�ru hareket ettir
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Hedefe yakla�t�ysak yeni bir hedef belirle
        if (Vector3.Distance(transform.position, targetPosition) < targetReachThreshold)
        {
            SetNewRandomTarget();
        }
    }

    void SetNewRandomTarget()
    {
        // Ba�lang�� konumuna g�re rastgele bir X ve Y ofseti olu�tur
        float randomOffsetX = Random.Range(-rangeX, rangeX);
        float randomOffsetY = Random.Range(-rangeY, rangeY);

        // Yeni hedef konumu hesapla (ba�lang�� konumu + rastgele ofset)
        targetPosition = initialPosition + new Vector3(randomOffsetX, randomOffsetY, 0f);
    }

    // Objeyi belirli bir s�re bekletmek isterseniz bu metodu kullanabilirsiniz.
    // Ancak bu senaryoda s�rekli hareket istendi�i i�in Update i�inde daha uygun.
    // Yine de bilgi ama�l� b�rak�lm��t�r.
    /*
    IEnumerator MoveToTarget(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > targetReachThreshold)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null; // Bir sonraki frame'e kadar bekle
        }
        SetNewRandomTarget(); // Hedefe ula�t���nda yeni hedef belirle
    }
    */
}

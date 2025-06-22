using UnityEngine;
using System.Collections; // IEnumerator i�in gerekli
using UnityEngine.UI;     // UI elemanlar� i�in gerekli

public class UIScaleAnimator : MonoBehaviour
{
    public float scaleSpeed = 1f;    // B�y�me/K���lme h�z�
    public float maxScaleFactor = 1.2f; // Orijinal boyutun ka� kat�na kadar b�y�yecek (�rn: 1.2 = %120)
    public float minScaleFactor = 1.0f; // Orijinal boyutun ka� kat�na kadar k���lecek (�rn: 1.0 = %100, yani orijinal boyut)

    private Vector3 originalScale; // UI objesinin orijinal boyutu
    private bool scalingUp = true; // B�y�yor muyuz, k���l�yor muyuz?

    void Start()
    {
        // UI objesinin ba�lang�� boyutunu kaydet
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (scalingUp)
        {
            // Hedeflenen boyuta do�ru b�y�t
            transform.localScale = Vector3.MoveTowards(transform.localScale, originalScale * maxScaleFactor, scaleSpeed * Time.deltaTime);

            // Hedeflenen boyuta ula�t�ysak y�n� de�i�tir
            if (transform.localScale.x >= originalScale.x * maxScaleFactor - 0.01f) // K���k bir e�ik ekledik
            {
                scalingUp = false;
            }
        }
        else
        {
            // Orijinal boyuta do�ru k���lt
            transform.localScale = Vector3.MoveTowards(transform.localScale, originalScale * minScaleFactor, scaleSpeed * Time.deltaTime);

            // Orijinal boyuta ula�t�ysak y�n� de�i�tir
            if (transform.localScale.x <= originalScale.x * minScaleFactor + 0.01f) // K���k bir e�ik ekledik
            {
                scalingUp = true;
            }
        }
    }
}
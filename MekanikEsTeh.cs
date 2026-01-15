using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MekanikEsTeh : MonoBehaviour
{
    public List<GameObject> esTehObjects; // 3 es teh
    public GameObject tekoObject;         // Teko isi ulang

    private int currentEsTeh = 3;
    private Vector3 originalScaleTeko;
    private bool isAnimating = false;

    public AudioClip refilSFX;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ResetEsTeh();
        if (tekoObject != null)
            originalScaleTeko = tekoObject.transform.localScale;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(clickPos);

            if (hit != null)
            {
                // Jika klik Es Teh
                if (hit.CompareTag("EsTeh") && currentEsTeh > 0)
                {
                    hit.gameObject.SetActive(false);
                    currentEsTeh--;

                    Debug.Log("Es Teh diambil. Sisa: " + currentEsTeh);
                }
                // Jika klik Teko dan stok habis
                else if (hit.CompareTag("Teko") && currentEsTeh == 0)
                {
                    if (refilSFX != null && audioSource != null)
                        audioSource.PlayOneShot(refilSFX);
                                          
                    ResetEsTeh();
                    Debug.Log("Es Teh diisi ulang!");

                    if (!isAnimating)
                        StartCoroutine(PlayTekoPopAnimation());
                }
            }
        }
    }

    void ResetEsTeh()
    {
        foreach (GameObject teh in esTehObjects)
        {
            teh.SetActive(true);
        }
        currentEsTeh = esTehObjects.Count;
    }

    IEnumerator PlayTekoPopAnimation()
    {
        isAnimating = true;

        float duration = 0.1f;
        float timer = 0f;
        Vector3 targetScale = originalScaleTeko * 1.2f;

        // Membesar
        while (timer < duration)
        {
            tekoObject.transform.localScale = Vector3.Lerp(originalScaleTeko, targetScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        tekoObject.transform.localScale = targetScale;

        // Mengecil kembali
        timer = 0f;
        while (timer < duration)
        {
            tekoObject.transform.localScale = Vector3.Lerp(targetScale, originalScaleTeko, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        tekoObject.transform.localScale = originalScaleTeko;
        isAnimating = false;
    }
}

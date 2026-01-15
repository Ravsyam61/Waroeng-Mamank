using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickPlace : MonoBehaviour
{
    public string foodName;
    public int foodValue;

    private GameFlow gameFlow;

    private Vector3 originalScale;
    private bool isAnimating = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        gameFlow = GameObject.FindObjectOfType<GameFlow>();
        originalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

    }

    private void OnMouseDown()
    {
        bool success = gameFlow.AddFood(foodName, foodValue);

        if (!success)
        {
            StartCoroutine(FlashRedColor());

            if (!isAnimating)
                StartCoroutine(PlayPopAnimation());
            return;
        }

        if (!isAnimating)
            StartCoroutine(PlayPopAnimation());
    }

    IEnumerator FlashRedColor()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = originalColor;
    }



    IEnumerator PlayPopAnimation()
    {
        isAnimating = true;

        // Membesar
        float duration = 0.1f;
        float timer = 0f;
        Vector3 targetScale = originalScale * 1.2f;

        while (timer < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;

        // Mengecil kembali
        timer = 0f;
        while (timer < duration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        isAnimating = false;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InteractCooking : MonoBehaviour
{
    public TextMeshProUGUI interactionText;
    public Animator fadeAnimator;
    public float fadeDuration = 1f;

    private bool isPlayerInRange = false;
    private bool isFading = false;

    void Start()
    {
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isFading)
        {
            StartCoroutine(FadeAndLoadCookingScene());
        }
    }

    IEnumerator FadeAndLoadCookingScene()
    {
        isFading = true;
        fadeAnimator.SetTrigger("StartFade");
        yield return new WaitForSeconds(fadeDuration);

        //Pilih scene sesuai stage aktif
        string targetScene = "Masak"; // Default

        if (LevelProgress.GameProgress.currentStage == 2)
            targetScene = "Masak2";
        else if (LevelProgress.GameProgress.currentStage == 3)
            targetScene = "Masak3";

        SceneManager.LoadScene(targetScene);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactionText != null)
                interactionText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactionText != null)
                interactionText.gameObject.SetActive(false);
        }
    }
}

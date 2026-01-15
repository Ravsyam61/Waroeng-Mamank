using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StageLevel : MonoBehaviour
{
    public TextMeshProUGUI interactionText;
    public Animator fadeAnimator;
    public float fadeDuration = 1f;

    public int sceneToLoad = 0;

    private bool isPlayerInRange = false;
    private bool isFading = false;
    public GameObject stageSelectionPanel;
    public GameObject peringatan;
    private Vector3 panelOriginalScale;
    private bool isAnimatingPanel = false;


    void Start()
    {
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);

        if (stageSelectionPanel != null)
        {
            panelOriginalScale = stageSelectionPanel.transform.localScale;
            stageSelectionPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isFading)
        {
            if (LevelProgress.GameProgress.cookingMinigameFinished)
            {
                if (stageSelectionPanel != null && !isAnimatingPanel)
                    StartCoroutine(PlayStagePanelPop());
            }
            else
            {
                peringatan.SetActive(true);
                Debug.Log("Selesaikan memasak dulu!");
            }
        }
    }

    IEnumerator FadeAndLoadScene()
    {
        isFading = true;
        fadeAnimator.SetTrigger("StartFade");
        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(sceneToLoad);
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

            if (peringatan != null)
                peringatan.gameObject.SetActive(false);
        }
    }

    public void LoadJualanScene()
    {
        if (!isFading)
            StartCoroutine(FadeAndLoadScene());
    }
    
    IEnumerator PlayStagePanelPop()
    {
        isAnimatingPanel = true;

        stageSelectionPanel.SetActive(true); // Tampilkan panel

        float duration = 0.1f;
        float timer = 0f;
        Vector3 targetScale = panelOriginalScale * 1.2f;

        // Scale up
        while (timer < duration)
        {
            stageSelectionPanel.transform.localScale = Vector3.Lerp(panelOriginalScale, targetScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        stageSelectionPanel.transform.localScale = targetScale;

        // Scale back
        timer = 0f;
        while (timer < duration)
        {
            stageSelectionPanel.transform.localScale = Vector3.Lerp(targetScale, panelOriginalScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        stageSelectionPanel.transform.localScale = panelOriginalScale;
        isAnimatingPanel = false;
    }

}

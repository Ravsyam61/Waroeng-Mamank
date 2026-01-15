using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InteractObject : MonoBehaviour
{
    public TextMeshProUGUI interactionText;
    public Animator fadeAnimator;
    public float fadeDuration = 1f;

    public int sceneToLoad = 0;

    private bool isPlayerInRange = false;
    private bool isFading = false;

    public AudioClip clickSFX;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isFading)
        {
            if (clickSFX != null && audioSource != null)
            audioSource.PlayOneShot(clickSFX);

            StartCoroutine(FadeAndLoadScene());
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
        }
    }
}
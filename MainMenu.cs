using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator fadeAnimator;
    public GameObject fadePanel;
    public float fadeDuration = 1f;
    private bool isFading = false;
    public AudioClip clickSFX;
    private AudioSource audioSource;

    public GameObject settingsPanel;

    public GameObject IndexPanel;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && settingsPanel.activeSelf)
        {
            CloseSetting();
            CloseIndex();
        }
    }

    public void StartGame()
    {
        if (!isFading)
            StartCoroutine(FadeAndLoadScene());

        if (clickSFX != null && audioSource != null)
            audioSource.PlayOneShot(clickSFX);
    }

    public void Setting()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
        if (clickSFX != null && audioSource != null)
            audioSource.PlayOneShot(clickSFX);
    }

    public void CloseSetting()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void QuitGame()
    {
        if (clickSFX != null && audioSource != null)
            audioSource.PlayOneShot(clickSFX);

        Application.Quit();
    }

    IEnumerator FadeAndLoadScene()
    {
        isFading = true;
        fadeAnimator.SetTrigger("StartFade");
        fadePanel.SetActive(true);
        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(1);
    }

    public void Restart()
    {
        if (clickSFX != null && audioSource != null)
            audioSource.PlayOneShot(clickSFX);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Next()
    {
        SceneManager.LoadScene(1);
    }

    public void Menu()
    {
        if (clickSFX != null && audioSource != null)
            audioSource.PlayOneShot(clickSFX);

        SceneManager.LoadScene("MainMenu");
    }
    
    public void Index()
    {
        if (IndexPanel != null)
        {
            IndexPanel.SetActive(true);
        }
        if (clickSFX != null && audioSource != null)
                audioSource.PlayOneShot(clickSFX);
    }

    public void CloseIndex()
    {
        if (IndexPanel != null)
        {
            IndexPanel.SetActive(false);
        }
    }
}

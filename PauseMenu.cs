using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button indexButton;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSFX;
    [SerializeField] private IndexManager indexManager;

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClicked);

        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuClicked);

        if (indexButton != null)
            indexButton.onClick.AddListener(OnIndexClicked);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void PauseGame()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
    }

    private void ResumeGame()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    private void OnResumeClicked()
    {
        PlayClickSFX();
        ResumeGame();
    }

    private void OnMenuClicked()
    {
        PlayClickSFX();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnIndexClicked()
    {
        PlayClickSFX();
        if (indexManager != null)
        {
            indexManager.OpenIndex();
        }
    }

    private void PlayClickSFX()
    {
        if (audioSource != null && clickSFX != null)
        {
            audioSource.PlayOneShot(clickSFX);
        }
    }
}

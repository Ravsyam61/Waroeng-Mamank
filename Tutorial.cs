using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPanel;               
    public List<GameObject> tutorialPages;         
    public Button nextButton;
    public Button previousButton;
    public AudioClip clickSFX;
    private AudioSource audioSource;            

    private int currentPageIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (tutorialPages.Count == 0) return;

        tutorialPanel.SetActive(true);             // Muncul di awal
        ShowPage(0);

        if (nextButton != null)
            nextButton.onClick.AddListener(NextPage);
        
        if (previousButton != null)
            previousButton.onClick.AddListener(PreviousPage);

        Time.timeScale = 0;
    }

    void ShowPage(int index)
    {
        for (int i = 0; i < tutorialPages.Count; i++)
        {
            tutorialPages[i].SetActive(i == index); // Hanya aktifkan page yang sedang digunakan
        }

        if (previousButton != null)
            previousButton.gameObject.SetActive(index > 0);

    }

    void NextPage()
    {
        currentPageIndex++;

        if (clickSFX != null && audioSource != null)
                audioSource.PlayOneShot(clickSFX);

        if (currentPageIndex < tutorialPages.Count)
        {
            ShowPage(currentPageIndex);
        }
        else
        {
            tutorialPanel.SetActive(false); // Sembunyikan panel saat selesai
            Time.timeScale = 1;
        }
    }

    void PreviousPage()
    {
        currentPageIndex--;

        if (clickSFX != null && audioSource != null)
                audioSource.PlayOneShot(clickSFX);

        if (currentPageIndex >= 0)
        {
            ShowPage(currentPageIndex);
        }
    }
}

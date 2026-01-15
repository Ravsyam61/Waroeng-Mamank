using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndexManager : MonoBehaviour
{
    public GameObject indexPanel;                  // Panel Index Resep
    public List<GameObject> indexPages;            // Halaman-halaman resep
    public Button nextButton;
    public Button previousButton;
    public Button closeButton;
    public AudioClip clickSFX;

    private AudioSource audioSource;
    private int currentPageIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Panel index tidak muncul di awal
        if (indexPanel != null)
            indexPanel.SetActive(false);
        
        foreach (GameObject page in indexPages)
            ShowPage(0);


        if (nextButton != null)
            nextButton.onClick.AddListener(NextPage);

        if (previousButton != null)
            previousButton.onClick.AddListener(PreviousPage);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseIndex);

        // Tombol close disembunyikan di awal
        if (closeButton != null)
            closeButton.gameObject.SetActive(false);
    }

    public void OpenIndex()
    {
        if (indexPanel == null || indexPages.Count == 0) return;

        indexPanel.SetActive(true);
        currentPageIndex = 0;
        ShowPage(0);

        PlayClickSFX();
    }

    void ShowPage(int index)
    {
        for (int i = 0; i < indexPages.Count; i++)
            indexPages[i].SetActive(i == index);

        if (previousButton != null)
            previousButton.gameObject.SetActive(index > 0);

        if (nextButton != null)
            nextButton.gameObject.SetActive(index < indexPages.Count - 1);

        if (closeButton != null)
            closeButton.gameObject.SetActive(index == indexPages.Count - 1); // Tampilkan close di akhir
    }

    void NextPage()
    {
        if (currentPageIndex < indexPages.Count - 1)
        {
            currentPageIndex++;
            ShowPage(currentPageIndex);
            PlayClickSFX();
        }
    }

    void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            ShowPage(currentPageIndex);
            PlayClickSFX();
        }
    }

    void CloseIndex()
    {
        if (indexPanel != null)
            indexPanel.SetActive(false);
        
        currentPageIndex = 0;
        ShowPage(0); // Reset tampilan ke halaman pertama

        PlayClickSFX();
    }

    void PlayClickSFX()
    {
        if (clickSFX != null && audioSource != null)
            audioSource.PlayOneShot(clickSFX);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (indexPanel != null && indexPanel.activeSelf)
            {
                CloseIndex();
            }
        }
    }
}

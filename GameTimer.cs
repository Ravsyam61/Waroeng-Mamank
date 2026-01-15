using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    public float gameDuration = 300f;
    private float timer;

    public int targetPenjualan = 1000000;

    public GameObject panelGameOver;
    public GameObject nextButton;
    public TextMeshProUGUI textUang;
    public TextMeshProUGUI textStatus;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI targetText;

    public Image characterImage; 
    public Sprite successSprite; 
    public Sprite failureSprite; 

    public AudioClip successSFX; 
    public AudioClip failureSFX; 
    private AudioSource audioSource;

    private bool gameEnded = false;

    void Start()
    {
        timer = gameDuration;

        if (panelGameOver != null)
            panelGameOver.SetActive(false);

        if (nextButton != null)
            nextButton.SetActive(false);
        
        if (targetText != null)
            targetText.text = "Target: " + targetPenjualan.ToString("N0");

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (gameEnded) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            EndGame();
        }

        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    void EndGame()
    {
        gameEnded = true;

        if (panelGameOver != null)
            panelGameOver.SetActive(true);

        if (nextButton != null)
        {
            if (GameManager.Instance.money >= targetPenjualan)
                nextButton.SetActive(true);
            else
                nextButton.SetActive(false);
        }

        if (textUang != null)
            textUang.text = $"{GameManager.Instance.money}/{targetPenjualan}";

        if (textStatus != null)
        {
            if (GameManager.Instance.money >= targetPenjualan)
            {
                textStatus.text = "Mencapai Target!";
                textStatus.color = Color.green;

                if (characterImage != null && successSprite != null)
                {
                    characterImage.sprite = successSprite;
                }

                if (audioSource != null && successSFX != null)
                {
                    audioSource.PlayOneShot(successSFX);
                }

                if (SceneManager.GetActiveScene().name == "Jualan")
                {
                    LevelProgress.GameProgress.stage1Completed = true;
                    LevelProgress.GameProgress.stage2Unlocked = true;
                    LevelProgress.GameProgress.currentStage = 2;
                }
                else if (SceneManager.GetActiveScene().name == "Jualan2")
                {
                    LevelProgress.GameProgress.stage2Completed = true;
                    LevelProgress.GameProgress.stage3Unlocked = true;
                    LevelProgress.GameProgress.currentStage = 3;
                }
            }
            else
            {
                textStatus.text = "Tidak Mencapai Target!";
                textStatus.color = Color.red;

                if (characterImage != null && failureSprite != null)
                {
                    characterImage.sprite = failureSprite;
                }

                if (audioSource != null && failureSFX != null)
                {
                    audioSource.PlayOneShot(failureSFX);
                }
            }
        }
        LevelProgress.GameProgress.cookingMinigameFinished = false;
    }
}

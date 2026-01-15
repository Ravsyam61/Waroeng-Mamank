using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CookingMechanics : MonoBehaviour
{
    public Slider cuttingSlider;
    public Slider fryingSlider;
    public Slider stirringSlider;
    public GameObject knife;
    public GameObject pan;
    public GameObject stirObject; // Objek Sendok (2D)
    public Transform panCenter; // Titik pusat panci
    public TMP_Text fryingTempText;
    public TMP_Text balanceText;
    public GameObject nextStepButton;

    public GameObject cutResultObject; //Objek hasil potongan
    public GameObject blendResultObject; //Objek hasil blender

    private Animator knifeAnimator; //Animator untuk animasi memotong

    private bool isCutting = false;
    private bool isFrying = false;
    private bool isStirring = false;
    private bool isHoldingObject = false;
    private Vector2 lastMousePosition;
    private float fryingTemperature = 70f;
    private float totalRotation = 0f;
    private const float requiredRotation = 360f * 3;
    private float rotationSpeed = 0f;
    private float maxRotationSpeed = 200f;
    private float stableTime = 0f;
    private float requiredStableTime = 5f;

    private Vector3 nextStepOriginalScale;
    private bool isAnimatingNextButton = false;

    public AudioClip clickSFX;
    public AudioClip cuttingSFX;
    public AudioClip fryingSFX;
    public AudioClip stirringSFX;
    public AudioClip successSFX;
    private AudioSource audioSource;
    private AudioSource fryingAudioSource;
    private AudioSource stirringAudioSource;

    public Slider blendingSlider;
    public GameObject blender;
    public AudioClip blendingSFX;

    private Animator blenderAnimator;
    private bool isBlending = false;
    private AudioSource blendingAudioSource;


    private enum CookingStep { Cutting, Blending, Frying, Stirring, Done }
    private CookingStep currentStep = CookingStep.Cutting;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        fryingAudioSource = gameObject.AddComponent<AudioSource>();
        fryingAudioSource.loop = true;
        fryingAudioSource.volume = 0.5f;
        fryingAudioSource.playOnAwake = false;
        fryingAudioSource.clip = fryingSFX;

        stirringAudioSource = gameObject.AddComponent<AudioSource>();
        stirringAudioSource.loop = true;
        stirringAudioSource.playOnAwake = false;
        stirringAudioSource.clip = stirringSFX;

        blendingAudioSource = gameObject.AddComponent<AudioSource>();
        blendingAudioSource.loop = false;
        blendingAudioSource.playOnAwake = false;
        blendingAudioSource.clip = blendingSFX;

        if (blender != null)
            blenderAnimator = blender.GetComponent<Animator>();

        nextStepButton.SetActive(false);
        knifeAnimator = knife.GetComponent<Animator>(); //Ambil animator
        StartCutting(); // Mulai dari langkah pertama: memotong

        if (nextStepButton != null)
            nextStepOriginalScale = nextStepButton.transform.localScale;

    }

    void Update()
    {
        if (isCutting)
        {
            // Tekan Space = tambah slider + lanjut animasi
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (cuttingSFX != null && audioSource != null)
                    audioSource.PlayOneShot(cuttingSFX);

                cuttingSlider.value += 0.03f;

                if (knifeAnimator != null)
                    knifeAnimator.speed = 3.5f;

                if (cuttingSlider.value >= 1f)
                    FinishCutting();
            }

            // Tidak tekan apa-apa = hentikan animasi
            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (knifeAnimator != null)
                    knifeAnimator.speed = 0f;
            }
        }


        if (isFrying)
        {
            fryingTemperature -= Time.deltaTime * 6;
            if (Input.GetKey(KeyCode.W)) fryingTemperature += 10 * Time.deltaTime;
            if (Input.GetKey(KeyCode.S)) fryingTemperature -= 10 * Time.deltaTime;
            fryingTemperature = Mathf.Clamp(fryingTemperature, 0, 100);
            fryingSlider.value = fryingTemperature / 100f;
            fryingTempText.text = "Suhu: " + Mathf.RoundToInt(fryingTemperature) + "°C";

            if (fryingTemperature >= 70 && fryingTemperature <= 85)
            {
                stableTime += Time.deltaTime;
                if (stableTime >= requiredStableTime)
                    FinishFrying(true);
            }
            else
            {
                stableTime = 0f;
            }

            if (fryingTemperature >= 90)
                FinishFrying(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);
            if (hit != null && hit.gameObject == stirObject)
            {
                isHoldingObject = true;
                isStirring = true;
                lastMousePosition = Input.mousePosition;
            }
        }

        if (isHoldingObject && Input.GetMouseButton(0))
        {
            Vector2 currentMousePosition = Input.mousePosition;
            float mouseDelta = (currentMousePosition - lastMousePosition).magnitude;
            rotationSpeed = Mathf.Clamp(mouseDelta * 7f, 0, maxRotationSpeed);
            lastMousePosition = currentMousePosition;
        }
        else
        {
            rotationSpeed = 0f;
        }

        if (isStirring && rotationSpeed > 0)
        {
            stirObject.transform.RotateAround(panCenter.position, Vector3.forward, rotationSpeed * Time.deltaTime);
            totalRotation += rotationSpeed * Time.deltaTime;
            float progress = Mathf.Clamp(totalRotation / requiredRotation, 0f, 1f);
            stirringSlider.value = progress;
            balanceText.text = "Progress: " + Mathf.RoundToInt(progress * 100) + "%";

            if (progress >= 1f)
                FinishStirring();
        }
        
        if (isBlending)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (blendingSFX != null && blendingAudioSource != null)
                blendingAudioSource.PlayOneShot(blendingSFX);

                blendingSlider.value += 0.25f;

                if (blenderAnimator != null)
                    blenderAnimator.speed = 3.5f;

                if (blendingSlider.value >= 1f)
                    FinishBlending();
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (blenderAnimator != null)
                    blenderAnimator.speed = 0f;
            }
        }

    }

    public void NextStep()
    {
        nextStepButton.SetActive(false);

        if (clickSFX != null && audioSource != null)
            audioSource.PlayOneShot(clickSFX);

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentStep == CookingStep.Cutting)
        {
            if (currentScene == "Masak3")
            {
                currentStep = CookingStep.Blending;
                StartBlending();
            }
            else
            {
                currentStep = CookingStep.Frying;
                StartFrying();
            }
        }
        else if (currentStep == CookingStep.Blending)
        {
            currentStep = CookingStep.Frying;
            StartFrying();
        }
        else if (currentStep == CookingStep.Frying)
        {
            currentStep = CookingStep.Stirring;
            StartStirring();
        }
        else if (currentStep == CookingStep.Stirring)
        {
            currentStep = CookingStep.Done;
            Debug.Log("Semua langkah memasak selesai!");
            StartCoroutine(BackToScene());
        }
    }

    public void StartCutting()
    {
        isCutting = true;
        knife.SetActive(true);
        cuttingSlider.gameObject.SetActive(true);
        cuttingSlider.value = 0f;

        if (knifeAnimator != null)
        {
            knifeAnimator.Play("Motong", 0, 0); // Restart dari awal
            knifeAnimator.speed = 0f; // Awalnya pause
        }

        if (cutResultObject != null)
            cutResultObject.SetActive(false);
    }
    
    public void StartBlending()
    {
        isBlending = true;

        if (blendResultObject != null)
        blendResultObject.SetActive(false);

        if (blender != null) blender.SetActive(true);
        if (blendingSlider != null)
        {
            blendingSlider.gameObject.SetActive(true);
            blendingSlider.value = 0f;
        }

        if (blenderAnimator != null)
        {
            blenderAnimator.Play("Blender", 0, 0); // Pastikan animator punya clip
            blenderAnimator.speed = 0f;
        }
    }


    public void StartFrying()
    {
        isFrying = true;
        pan.SetActive(true);
        fryingSlider.gameObject.SetActive(true);
        fryingTempText.gameObject.SetActive(true);
        fryingTemperature = 70f;
        stableTime = 0f;

        if (cutResultObject != null)
            cutResultObject.SetActive(false); //Sembunyikan hasil potongan saat masuk tahap menggoreng

        if (fryingAudioSource != null && fryingSFX != null)
            fryingAudioSource.Play();
        
        if (blendResultObject != null)
            blendResultObject.SetActive(false);
    }

    public void StartStirring()
    {
        isStirring = true;
        stirObject.SetActive(true);
        stirringSlider.gameObject.SetActive(true);
        balanceText.gameObject.SetActive(true);
        totalRotation = 0f;
        stirringSlider.value = 0f;

        if (fryingAudioSource != null && fryingSFX != null)
            fryingAudioSource.Play();
        
        if (stirringAudioSource != null && stirringSFX != null)
            stirringAudioSource.Play();
    }

    private void FinishCutting()
    {
        isCutting = false;
        knife.SetActive(false);
        cuttingSlider.gameObject.SetActive(false);
        if (nextStepButton != null && !isAnimatingNextButton)
            StartCoroutine(PlayNextButtonPop());

        if (knifeAnimator != null)
            knifeAnimator.SetBool("isCutting", false); // Matikan animasi

        if (successSFX != null && audioSource != null)
            audioSource.PlayOneShot(successSFX);

        if (cutResultObject != null)
            cutResultObject.SetActive(true); //Tampilkan hasil potongan

        Debug.Log("Bahan berhasil dipotong.");
    }

    private void FinishFrying(bool success)
    {
        isFrying = false;

        if (fryingAudioSource != null && fryingSFX != null)
            fryingAudioSource.Stop();
        
        if (successSFX != null && audioSource != null)
            audioSource.PlayOneShot(successSFX);

        fryingSlider.gameObject.SetActive(false);
        fryingTempText.gameObject.SetActive(false);
        if (nextStepButton != null && !isAnimatingNextButton)
            StartCoroutine(PlayNextButtonPop());
        Debug.Log(success ? "Menggoreng berhasil." : "Menggoreng gagal.");
    }

    private void FinishStirring()
    {
        isStirring = false;
        isHoldingObject = false;
        stirObject.SetActive(false);
        stirringSlider.gameObject.SetActive(false);
        balanceText.gameObject.SetActive(false);
        if (stirringAudioSource != null && stirringSFX != null)
            stirringAudioSource.Stop();
        if (fryingAudioSource != null && fryingSFX != null)
            fryingAudioSource.Stop();
        
        if (successSFX != null && audioSource != null)
            audioSource.PlayOneShot(successSFX);

        if (nextStepButton != null && !isAnimatingNextButton)
            StartCoroutine(PlayNextButtonPop());
        Debug.Log("Mengaduk berhasil.");
        LevelProgress.GameProgress.cookingMinigameFinished = true; //Tandai bahwa minigame selesai
    }

    private void FinishBlending()
    {
        isBlending = false;
        if (blender != null) blender.SetActive(false);
        if (blendingSlider != null) blendingSlider.gameObject.SetActive(false);

        if (blendResultObject != null)
            blendResultObject.SetActive(true);

        if (nextStepButton != null && !isAnimatingNextButton)
            StartCoroutine(PlayNextButtonPop());
        
        if (successSFX != null && audioSource != null)
            audioSource.PlayOneShot(successSFX);

        Debug.Log("Blender selesai.");
    }


    IEnumerator BackToScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Area 4");
    }
    
    IEnumerator PlayNextButtonPop()
    {
        isAnimatingNextButton = true;

        nextStepButton.SetActive(true); // Pastikan aktif

        float duration = 0.1f;
        float timer = 0f;
        Vector3 targetScale = nextStepOriginalScale * 1.2f;

        // Besarkan
        while (timer < duration)
        {
            nextStepButton.transform.localScale = Vector3.Lerp(nextStepOriginalScale, targetScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        nextStepButton.transform.localScale = targetScale;

        // Kecilkan lagi
        timer = 0f;
        while (timer < duration)
        {
            nextStepButton.transform.localScale = Vector3.Lerp(targetScale, nextStepOriginalScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        nextStepButton.transform.localScale = nextStepOriginalScale;
        isAnimatingNextButton = false;
    }

}
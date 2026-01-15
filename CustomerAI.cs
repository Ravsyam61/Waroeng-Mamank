using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CustomerAI : MonoBehaviour
{
    public enum CustomerState { Arriving, Waiting, Leaving } // FSM state untuk pelanggan
    public CustomerState currentState;

    public float patienceTime = 10f;
    private float timer;

    private SpriteRenderer spriteRenderer;
    private Canvas orderCanvas;

    private List<string> orderList = new List<string>();

    public List<FoodComboSprite> foodComboSprites;
    public Image orderImage;
    public Slider patienceBar;

    private CanvasGroup orderCanvasGroup;

    [Header("Audio")]
    public AudioClip successSFX;
    public AudioClip failSFX;
    public AudioClip disappointedSFX;
    private AudioSource audioSource;


    [System.Serializable]
    public class FoodComboSprite
    {
        public string comboKey;
        public Sprite comboSprite;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        orderCanvas = GetComponentInChildren<Canvas>();
        orderCanvasGroup = orderCanvas.GetComponent<CanvasGroup>();
        audioSource = GetComponent<AudioSource>();
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Inisialisasi FSM ke state awal: Arriving
        timer = patienceTime;
        currentState = CustomerState.Arriving;

        // Set pelanggan transparan
        Color c = spriteRenderer.color;
        c.a = 0;
        spriteRenderer.color = c;

        // Set canvas order transparan
        if (orderCanvasGroup != null)
            orderCanvasGroup.alpha = 0;

        // Inisialisasi bar waktu
        if (patienceBar != null)
        {
            patienceBar.maxValue = patienceTime;
            patienceBar.value = patienceTime;
        }

        StartCoroutine(FadeIn()); // Transisi ke Waiting akan dilakukan di sini
    }

    void Update()
    {
        if (currentState == CustomerState.Waiting)
        {
            timer -= Time.deltaTime;

            if (patienceBar != null)
                patienceBar.value = timer;

            if (timer <= 0)
            {
                Leave(); // Waktu habis, pelanggan pergi
            }
        }
    }

    IEnumerator FadeIn()
    {
        float duration = 1.5f;
        float elapsed = 0f;

        Color c = spriteRenderer.color;
        while (elapsed < duration)
        {
            c.a = Mathf.Lerp(0, 1, elapsed / duration);
            spriteRenderer.color = c;

            if (orderCanvasGroup != null)
                orderCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        c.a = 1;
        spriteRenderer.color = c;
        if (orderCanvasGroup != null)
            orderCanvasGroup.alpha = 1;

        // Setelah fade in selesai, pelanggan masuk ke state Waiting
        currentState = CustomerState.Waiting;
        GenerateRandomOrder();

        if (orderImage != null && orderImage.sprite != null)
        {
            orderImage.canvasRenderer.SetAlpha(0f);
            orderImage.gameObject.SetActive(true);
            orderImage.CrossFadeAlpha(1f, 1f, false);
        }
    }

    void GenerateRandomOrder()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Jualan")
        {
            GenerateStage1Order();
        }
        else if (currentScene == "Jualan2")
        {
            GenerateStage2Order();
        }
        else if (currentScene == "Jualan3")
        {
            GenerateStage3Order();
        }
    }

    void GenerateStage1Order()
    {
        string[] mainFoods = { "Nasi", "Rendang", "Dendeng" };
        string[] possibleFoods = { "Nasi", "Rendang", "Dendeng", "Sayur", "Pempek" };

        orderList.Clear();

        int choice = Random.Range(0, possibleFoods.Length);

        if (possibleFoods[choice] == "Pempek")
        {
            orderList.Add("Pempek");
        }
        else
        {
            orderList.Add("Nasi");
            string mainDish = mainFoods[Random.Range(1, mainFoods.Length)];
            orderList.Add(mainDish);
            if (Random.value < 0.5)
            {
                orderList.Add("Sayur");
            }
        }
        FinalizeOrder();
    }

    void GenerateStage2Order()
    {
        string[] laukTambahan = { "Nasi", "Gudeg", "Telor", "Tempe" };
        orderList.Clear();

        // Selalu ada nasi
        orderList.Add("Nasi");

        // Tambah 1 sampai 2 lauk (random)
        int jumlahLauk = Random.Range(1, 3); // bisa 1 atau 2 lauk

        // Ambil lauk secara acak tanpa duplikat
        List<string> laukList = new List<string>(laukTambahan);
        for (int i = 0; i < jumlahLauk; i++)
        {
            if (laukList.Count == 0) break;
            int index = Random.Range(1, laukList.Count);
            orderList.Add(laukList[index]);
            laukList.RemoveAt(index);
        }

        // 50% kemungkinan tambah es teh
        if (Random.value < 0.5f)
        {
            orderList.Add("EsTeh");
        }

        FinalizeOrder();
    }

    void GenerateStage3Order()
    {
        string[] laukTambahan = { "Nasi", "Soto", "Lontong", "Kue" };
        orderList.Clear();

        int jumlahLauk = Random.Range(1, 3);

        List<string> laukList = new List<string>(laukTambahan);
        List<string> selectedLauk;

        bool isValidCombination = false;

        while (!isValidCombination)
        {
            selectedLauk = new List<string>(laukTambahan);
            orderList.Clear();

            for (int i = 0; i < jumlahLauk; i++)
            {
                if (selectedLauk.Count == 0) break;
                int index = Random.Range(0, selectedLauk.Count);
                orderList.Add(selectedLauk[index]);
                selectedLauk.RemoveAt(index);
            }

            // Check for disallowed combinations
            if (orderList.Contains("Soto") && orderList.Contains("Lontong"))
                continue;
            if (orderList.Contains("Nasi") && orderList.Contains("Lontong"))
                continue;
            if (orderList.Contains("Nasi") && orderList.Contains("Kue"))
                continue;

            isValidCombination = true;
        }

        // 50% kemungkinan tambah es teh
        if (Random.value < 0.5f)
        {
            orderList.Add("EsTeh");
        }

        FinalizeOrder();
    }




    void FinalizeOrder()
    {
        orderList.Sort();
        string comboKey = string.Join("+", orderList);

        Sprite comboSprite = GetComboSprite(comboKey);

        if (comboSprite != null && orderImage != null)
        {
            orderImage.sprite = comboSprite;
            orderImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Tidak menemukan sprite untuk combo '{comboKey}'!");
        }
    }



    Sprite GetComboSprite(string comboKey)
    {
        foreach (var combo in foodComboSprites)
        {
            if (combo.comboKey == comboKey)
                return combo.comboSprite;
        }
        return null;
    }

    public void GiveFood(List<string> foodGiven)
    {
        bool isOrderCorrect = true;

        if (foodGiven.Count != orderList.Count)
        {
            isOrderCorrect = false;
        }
        else
        {
            List<string> sortedOrderList = new List<string>(orderList);
            List<string> sortedFoodGiven = new List<string>(foodGiven);

            sortedOrderList.Sort();
            sortedFoodGiven.Sort();

            for (int i = 0; i < sortedOrderList.Count; i++)
            {
                if (sortedOrderList[i] != sortedFoodGiven[i])
                {
                    isOrderCorrect = false;
                    break;
                }
            }
        }

        if (isOrderCorrect)
        {
            Debug.Log("Pesanan Sesuai! Pelanggan senang!");
            if (successSFX != null && audioSource != null)
                audioSource.PlayOneShot(successSFX);

            GameManager.Instance.AddMoney(GameFlow.plateValue);
            StartCoroutine(DestroyAfterDelay(0.2f)); // Pesanan benar, pelanggan pergi
        }
        else
        {
            Debug.Log("Pesanan tidak cocok! Pelanggan marah!");
            if (failSFX != null && audioSource != null)
                audioSource.PlayOneShot(failSFX); 

            timer -= 3f; // Kurangi waktu tunggu jika salah
            if (patienceBar != null)
                patienceBar.value = timer;
        }
    }

    void Leave()
    {
        currentState = CustomerState.Leaving; // Ubah state ke Leaving

        if (disappointedSFX != null && audioSource != null)
            audioSource.PlayOneShot(disappointedSFX);
    
        StartCoroutine(FadeOutAndDestroy()); // transisi saat keluar dari scene
    }

    IEnumerator FadeOutAndDestroy()
    {
        float duration = 1f;
        float elapsed = 0f;

        Color c = spriteRenderer.color;
        if (orderCanvasGroup != null)
            orderCanvasGroup.alpha = 1;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1, 0, elapsed / duration);
            c.a = alpha;
            spriteRenderer.color = c;

            if (orderCanvasGroup != null)
                orderCanvasGroup.alpha = alpha;

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}

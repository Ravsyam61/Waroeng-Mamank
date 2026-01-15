using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public static int plateValue = 0;
    public static int orderValue = 0;

    public SpriteRenderer plateRenderer;

    private List<string> foodList = new List<string>();

    public List<FoodCombo> foodCombos;
    private Dictionary<string, Sprite> comboDict = new Dictionary<string, Sprite>();

    public Sprite emptyPlateSprite;

    public Transform TongPopUp;

    private Vector3 originalScale;
    private bool isClearingAnimating = false;


    [System.Serializable]
    public class FoodCombo
    {
        public string comboKey;
        public Sprite comboSprite;
    }

    void Start()
    {
        foreach (var combo in foodCombos)
        {
            comboDict[combo.comboKey] = combo.comboSprite;
        }
        if (TongPopUp != null)
        originalScale = TongPopUp.localScale;
    }

    public bool AddFood(string foodName, int value)
    {
        bool pempekSudahAda = foodList.Contains("Pempek");
        bool mauTambahPempek = foodName == "Pempek" && foodList.Count > 0;
        bool pempekDanMauTambahLain = pempekSudahAda && foodName != "Pempek";

        if (mauTambahPempek || pempekDanMauTambahLain)
        {
            Debug.LogWarning("Pempek tidak bisa dikombinasikan dengan makanan lain!");
            return false;
        }

        // Cek duplikat
        if (foodList.Contains(foodName))
        {
            Debug.LogWarning($"Makanan '{foodName}' sudah ada di piring!");
            return true;
        }

        //Dendeng + Rendang tidak boleh bersamaan
        if ((foodList.Contains("Dendeng") && foodName == "Rendang") ||
            (foodList.Contains("Rendang") && foodName == "Dendeng"))
        {
            Debug.LogWarning("Dendeng dan Rendang tidak boleh dikombinasikan!");
            return false;
        }

        //Sayur tidak boleh jadi makanan pertama
        if (foodList.Count == 0 && foodName == "Sayur")
        {
            Debug.LogWarning("Sayur tidak boleh jadi makanan pertama!");
            return false;
        }

        //Sayur hanya boleh jika ada nasi + rendang/dendeng
        if (foodName == "Sayur")
        {
            bool adaNasi = foodList.Contains("Nasi");
            bool adaLauk = foodList.Contains("Dendeng") || foodList.Contains("Rendang");

            if (!(adaNasi && adaLauk))
            {
                Debug.LogWarning("Sayur hanya boleh jika sudah ada Nasi dan Dendeng atau Rendang!");
                return false;
            }
        }

        if ((foodName == "Lontong" || foodName == "Kue") && foodList.Contains("Nasi"))
        {
            Debug.LogWarning($"{foodName} tidak bisa dikombinasikan dengan Nasi!");
            return false;
        }
        if (foodName == "Nasi" && (foodList.Contains("Lontong") || foodList.Contains("Kue")))
        {
            Debug.LogWarning("Nasi tidak bisa dikombinasikan dengan Lontong atau Kue!");
            return false;
        }


        //Lontong tidak bisa dikombinasikan dengan Soto
        if ((foodName == "Lontong" && foodList.Contains("Soto")) ||
            (foodName == "Soto" && foodList.Contains("Lontong")))
        {
            Debug.LogWarning("Lontong tidak bisa dikombinasikan dengan Soto!");
            return false;
        }

        //Tambahkan makanan
        foodList.Add(foodName);
        List<string> sortedFood = new List<string>(foodList);
        sortedFood.Sort();
        string comboKey = string.Join("+", sortedFood);

        // Ubah sprite jika tersedia
        if (comboDict.TryGetValue(comboKey, out Sprite newSprite))
        {
            plateRenderer.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning($"Kombinasi '{comboKey}' belum ada sprite-nya!");
        }

        plateValue += value;
        Debug.Log($"Plate Value: {plateValue}, Order Value: {orderValue}");

        return true;
    }


    public void ClearFood()
    {
        foodList.Clear();
        plateValue = 0;
        plateRenderer.sprite = emptyPlateSprite;

        Debug.Log("Piring dikosongkan!");
    }

    public void ThrowFood()
    {
        foodList.Clear();
        plateValue = 0;
        plateRenderer.sprite = emptyPlateSprite;

        if (TongPopUp != null && !isClearingAnimating)
            StartCoroutine(PlayPopAnimation());

        Debug.Log("Piring dikosongkan!");
    }

    IEnumerator PlayPopAnimation()
    {
        isClearingAnimating = true;

        float duration = 0.1f;
        float timer = 0f;
        Vector3 targetScale = originalScale * 1.2f;

        // Membesar
        while (timer < duration)
        {
            TongPopUp.localScale = Vector3.Lerp(originalScale, targetScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        TongPopUp.localScale = targetScale;

        // Mengecil
        timer = 0f;
        while (timer < duration)
        {
            TongPopUp.localScale = Vector3.Lerp(targetScale, originalScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        TongPopUp.localScale = originalScale;

        isClearingAnimating = false;
    }


    public List<string> GetCurrentFoodList()
    {
        return new List<string>(foodList);
    }
}

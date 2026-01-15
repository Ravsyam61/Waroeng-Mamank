using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platting : MonoBehaviour
{
    void Start()
    {
        // Inisialisasi jika diperlukan
    }

    void Update()
    {
        // Update setiap frame jika diperlukan
    }

    private void OnMouseDown()
    {
        // Cek apakah pesanan sesuai dengan nilai yang ada di piring
        if (GameFlow.orderValue == GameFlow.plateValue)
        {
            Debug.Log("Correct");
        }
        else
        {
            Debug.Log("Incorrect");
        }
    }
}

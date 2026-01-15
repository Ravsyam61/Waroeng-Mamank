using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    private Vector3 offset;
    private bool dragging = false;
    private Vector3 initialPosition; // Simpan posisi awal

    private GameFlow gameFlow;

    private void Start()
    {
        gameFlow = FindObjectOfType<GameFlow>();
        initialPosition = transform.position;
    }

    private void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPosition();
        dragging = true;
    }

    private void OnMouseUp()
    {
        dragging = false;


        // Cek tabrakan
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);

        foreach (var hit in hits)
        {
            CustomerAI customer = hit.GetComponent<CustomerAI>();
            if (customer != null && customer.currentState == CustomerAI.CustomerState.Waiting)
            {
                // Kasih makanan
                customer.GiveFood(gameFlow.GetCurrentFoodList());

                // Reset piring
                gameFlow.ClearFood();
                break; // Hanya layani satu pelanggan
            }
        }

        ReturnToInitialPosition();
    }

    private void Update()
    {
        if (dragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void ReturnToInitialPosition()
    {
        transform.position = initialPosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Customer Settings")]
    public GameObject[] customerPrefabs;
    public Transform[] spawnPoints;     
    public float spawnInterval = 5f;    

    private bool spawning = true;

    [Header("Audio")]
    public AudioClip customerSpawnSFX;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(SpawnCustomers());
    }

    IEnumerator SpawnCustomers()
    {
        while (spawning)
        {
            SpawnCustomer();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnCustomer()
    {
        if (spawnPoints.Length == 0 || customerPrefabs.Length == 0)
        {
            Debug.LogWarning("Spawn points atau customer prefabs belum diisi!");
            return;
        }

        List<Transform> availableSpawns = new List<Transform>();

        foreach (Transform spawn in spawnPoints)
        {
            bool occupied = false;

            Collider2D[] hits = Physics2D.OverlapCircleAll(spawn.position, 0.1f); // radius kecil
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Customer")) // tag pelanggan
                {
                    occupied = true;
                    break;
                }
            }

            if (!occupied)
                availableSpawns.Add(spawn);
        }

        if (availableSpawns.Count == 0)
        {
            Debug.Log("Semua spawn point sedang terisi.");
            return;
        }

        // Pilih spawn point kosong
        Transform chosenSpawn = availableSpawns[Random.Range(0, availableSpawns.Count)];

        // Pilih prefab pelanggan secara acak
        int prefabIndex = Random.Range(0, customerPrefabs.Length);
        GameObject chosenPrefab = customerPrefabs[prefabIndex];

        // Spawn pelanggan
        Instantiate(chosenPrefab, chosenSpawn.position, Quaternion.identity);

        if (customerSpawnSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(customerSpawnSFX);
        }
    }


    public void StopSpawning()
    {
        spawning = false;
    }
}

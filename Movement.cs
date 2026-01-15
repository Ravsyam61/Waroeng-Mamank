using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private float moveX = 0f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public AudioClip walkSFX;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveX = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveX = 1f;
        }

        // Gerakan karakter
        transform.position += new Vector3(moveX * moveSpeed * Time.deltaTime, 0f, 0f);

        // Atur animasi
        animator.SetFloat("Speed", Mathf.Abs(moveX));

        // Flip sprite otomatis
        if (moveX > 0)
        {
            spriteRenderer.flipX = false; // menghadap kanan
        }
        else if (moveX < 0)
        {
            spriteRenderer.flipX = true; // menghadap kiri
        }

        if (Mathf.Abs(moveX) > 0f)
        {
            if (!audioSource.isPlaying)
                audioSource.PlayOneShot(walkSFX);
        }
    }
}

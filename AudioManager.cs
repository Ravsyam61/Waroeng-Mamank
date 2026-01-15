using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource bgmSource; // drag AudioSource (misalnya musik BGM)

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadVolume();
    }

    public void SetVolume(float volume)
    {
        if (bgmSource != null)
            bgmSource.volume = volume;

        PlayerPrefs.SetFloat("volume", volume); // Simpan ke storage
    }

    public void LoadVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat("volume", 1f); // default 100%
        if (bgmSource != null)
            bgmSource.volume = savedVolume;
    }

    public float GetVolume()
    {
        return bgmSource != null ? bgmSource.volume : 1f;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (bgmSource != null && clip != null && bgmSource.clip != clip)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }
}


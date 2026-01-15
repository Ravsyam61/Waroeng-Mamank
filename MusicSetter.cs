using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSetter : MonoBehaviour
{
    public AudioClip musicForThisScene;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(musicForThisScene);
        }
    }
}


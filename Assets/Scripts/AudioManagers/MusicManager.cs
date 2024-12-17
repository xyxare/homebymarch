using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioClip[] musicSounds;
    public AudioSource musicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMusic("PlaceHolder Music");
    }

    public void PlayMusic(string name)
    {
        AudioClip s = Array.Find(musicSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Music: " + name + " not found!");
        }
        else{
            musicSource.clip = s;
            musicSource.Play();
        }
        
    }

    public void MuteMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void  MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

}

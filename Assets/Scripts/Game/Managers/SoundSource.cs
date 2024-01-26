using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : SingletonMonoBehavior<SoundSource>
{
    private AudioSource audioSource;
    private static SoundSource m_Instance;
    public AudioSource audioSourceClip;
    public static SoundSource Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = GameObject.FindObjectOfType<SoundSource>();

            return m_Instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        audioSource = this.GetComponent<AudioSource>();
    }
    public void PlayUI()
    {
        audioSource.Play();
    }
    public void PlayClip(AudioClip audio)
    {
        audioSourceClip.clip = audio;
        audioSourceClip.Play();
    }
}

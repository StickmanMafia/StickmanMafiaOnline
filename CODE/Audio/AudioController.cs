using System;
using UnityEngine;

public abstract class AudioController : MonoBehaviour, IAudioController
{
    protected AudioSource AudioSource;

    protected virtual void Awake() => AudioSource = GetComponent<AudioSource>();

    protected void PlayClip(AudioClip clip)
    {
        if (AudioSource != null && AudioSource.isPlaying)
            AudioSource.Stop();
        
        AudioSource.clip = clip;
        AudioSource.loop = false;
        AudioSource.Play();
    }

    public void Stop() => AudioSource.Stop();
}

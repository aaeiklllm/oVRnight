using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingAudioManager : MonoBehaviour
{
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource longSfxSource;
    [SerializeField] AudioSource longSfxSource2;
    [SerializeField] AudioSource musicSource;

    public AudioClip baby;
    public AudioClip dog;
    public AudioClip fairy;
    public AudioClip goofy;

    public AudioClip walking;
    public AudioClip walkingScary;
    public AudioClip chasing;
    public AudioClip walkingSlow;
    public AudioClip chasingSlow;
    public AudioClip walkingFast;
    public AudioClip chasingFast;

    public AudioClip hide;
    public AudioClip transformEffect;
    public AudioClip ambience;

    void Start()
    {
        playMusic(ambience);
    }

    public void playSFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void playLongSFX(AudioClip clip, bool loop = true) //Walking
    {
        if (longSfxSource.isPlaying && longSfxSource.clip == clip)
            return;

        longSfxSource.clip = clip;
        longSfxSource.loop = loop;
        longSfxSource.Play();
    }

    public void playLongSFX2(AudioClip clip, bool loop = true) //Other SFX
    {
        if (longSfxSource2.isPlaying && longSfxSource2.clip == clip)
            return;

        longSfxSource2.clip = clip;
        longSfxSource2.loop = loop;
        longSfxSource2.Play();
    }

    public void stopLongSFX()
    {
        longSfxSource.Stop();
    }

    public void stopLongSFX2()
    {
        longSfxSource2.Stop();
    }

    public void playMusic(AudioClip clip, bool loop = true)
    {
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingAudioManager : MonoBehaviour
{
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource longSfxSource;
    [SerializeField] AudioSource longSfxSource2;
    [SerializeField] AudioSource musicSource;

    public AudioClip banana;
    public AudioClip lollipop;
    public AudioClip rubberChicken;
    public AudioClip block;
    public AudioClip punch;
    public AudioClip waterGun;
    public AudioClip waterBalloon;
    public AudioClip sword;
    public AudioClip teddy;
    public AudioClip transformEffect;
    public AudioClip dance;
    public AudioClip walk;
    public AudioClip walkSlow;
    public AudioClip walkFast;
    public AudioClip ambience;


    void Start()
    {
        playMusic(ambience);
    }

    public void playSFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void playLongSFX(AudioClip clip, bool loop = true)
    {
        if (longSfxSource.isPlaying && longSfxSource.clip == clip)
            return;

        longSfxSource.clip = clip;
        longSfxSource.loop = loop;
        longSfxSource.Play();
    }

    public void playLongSFX2(AudioClip clip, bool loop = true)
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

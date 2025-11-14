using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource longSfxSource;
    [SerializeField] AudioSource musicSource;

    public AudioClip mainMenu;

    void Start()
    {
        playMusic(mainMenu);
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

    public void stopLongSFX()
    {
        longSfxSource.Stop();
    }

    public void playMusic(AudioClip clip, bool loop = true)
    {
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

}

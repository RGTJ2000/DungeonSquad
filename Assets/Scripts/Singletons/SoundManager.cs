using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;


public class SoundManager : MonoBehaviour
{
    [Serializable]
    public struct CharacterVoices
    {
        public int characterID; // First dimension: Character ID
        public AudioClip[] voiceClips; // Second dimension: Voice clips
    }

    // Singleton instance
    public static SoundManager Instance { get; private set; }

    // Audio sources for music and sound effects
    public AudioSource characterVoiceSource;
    public AudioSource sfxSource;
    public AudioSource magicSfxSource;
    public AudioSource uiSource;

    // Audio clips

    public CharacterVoices[] characterVoices;
    public AudioClip arrowLaunch;
    public AudioClip[] mmClips;
    public AudioClip click;
    public AudioClip fireBurning;
    public AudioClip fireballBoom;
  

    private void Awake()
    {
        // Ensure only one instance of SoundManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayFireLoop(System.Func<bool> condition)
    {
        magicSfxSource.loop = true;
        magicSfxSource.clip = fireBurning;
        magicSfxSource.Play();
        StartCoroutine(MonitorSound(magicSfxSource, condition));
    }
 
    public void PlayFireballBoom()
    {
        magicSfxSource.PlayOneShot(fireballBoom);
    }

    // Play a sound effect
    public void PlayCharacterSelectAffirm(int ch_ID)
    {

        AudioClip clip = characterVoices[ch_ID].voiceClips[ UnityEngine.Random.Range(0,2) ];

        characterVoiceSource.PlayOneShot(clip, 1.0f);
    }


    public void PlayArrowLaunch()
    {
        sfxSource.PlayOneShot(arrowLaunch, .6f);
    }

    public void PlayMMLaunch()
    {
        magicSfxSource.PlayOneShot(mmClips[0]);
    }

    public void PlayMMBoom()
    {
        magicSfxSource.PlayOneShot(mmClips[1]);
    }
    
    public void PlayClick()
    {
        uiSource.PlayOneShot(click);
    }

    private IEnumerator MonitorSound(AudioSource source, System.Func<bool> condition)
    {
        while (source != null && condition())
        {
            yield return null; // Wait one frame
        }
        if (source != null)
        {
            source.Stop();
            source.loop = false;
        }
    }
}
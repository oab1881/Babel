    /*
 ==== Created by Jake Wardell 11/11/24 ====

Plays audio clips both sound effects and music

Attached: Will be attached to the AudioPlayer Gameobject

Changelog:
    - Created script : 11/11/24 : Jake
    - Total revamp of the system : 12/03/24 : Jake
    - Made it so audioPlayer persists between scenes; Made music player version : 12/04/24 : Jake
    - Added singleton and static access : 05/29/25 : Jake
    - Added fade and crossfade support : 05/29/25 : Jake

*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Static reference to the instance
    public static AudioManager Instance { get; private set; }

    // For storing all sound effects
    [SerializeField]
    List<AudioClip> soundEffectClip = new List<AudioClip>();

    // Number of audio sources
    [SerializeField]
    int audioSourceCount = 1;

    // Stores reference to audio sources
    [SerializeField]
    List<AudioSource> audioSources;

    private void Awake()
    {
        // Singleton pattern, destroy duplicates
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        // Creates the list then adds all the audio source components
        audioSources = new List<AudioSource>();
        for (int i = 0; i < audioSourceCount; i++)
        {
            AudioSource temp = gameObject.AddComponent<AudioSource>();
            audioSources.Add(temp);
        }
    }

    /// <summary>
    /// Plays a Sound effect by passing the name in
    /// </summary>
    /// <param name="name">name of clip to play</param>
    /// <param name="index">index in the audio source list</param>
    public static void PlaySoundEffect(string name, int index)
    {
        if (!IsValid(index)) return;

        // We create a temp audioClip
        AudioClip temp = Instance.GetAudioClip(name);

        // Plays the sound once
        Instance.audioSources[index].PlayOneShot(temp);
    }

    /// <summary>
    /// Plays a long sound only if nothing is currently playing
    /// </summary>
    /// <param name="name">name of the clip to play</param>
    /// <param name="index">audioSource index to use</param>
    public static void PlayLongSound(string name, int index)
    {
        if (!IsValid(index)) return;

        // Tests if the specified audioSource is already playing.
        if (Instance.audioSources[index].isPlaying)
        {
            return;
        }

        // Otherwise we create a temp audioClip
        AudioClip temp = Instance.GetAudioClip(name);

        // Sets the clip to temp then plays that clip
        Instance.audioSources[index].clip = temp;
        Instance.audioSources[index].Play();
    }

    /// <summary>
    /// Plays a looping music track
    /// </summary>
    /// <param name="name">name of the clip to loop</param>
    /// <param name="index">audioSource index to use</param>
    public static void PlayMusic(string name, int index)
    {
        if (!IsValid(index)) return;

        // Set loop to true then call the long sound player
        Instance.audioSources[index].loop = true;
        PlayLongSound(name, index);
    }

    /// <summary>
    /// Stops a sound from playing
    /// </summary>
    /// <param name="index">index in the list of audioSources</param>
    public static void StopSound(int index)
    {
        if (!IsValid(index)) return;

        Instance.audioSources[index].Stop();
    }

    /// <summary>
    /// Sets volume for a specific audio source
    /// </summary>
    /// <param name="index">index in the list</param>
    /// <param name="volume">volume to set (0 to 1)</param>
    public static void SetVolume(int index, float volume)
    {
        if (!IsValid(index)) return;

        Instance.audioSources[index].volume = volume;
    }

    /// <summary>
    /// Fades in audio over time
    /// </summary>
    /// <param name="index">Audio source index</param>
    /// <param name="duration">Fade duration in seconds</param>
    /// <param name="targetVolume">Final volume (default = 1)</param>
    public static void FadeIn(int index, float duration, float targetVolume = 1f)
    {
        if (!IsValid(index)) return;
        Instance.StartCoroutine(Instance.FadeAudioIn(index, duration, targetVolume));
    }

    /// <summary>
    /// Fades out audio over time
    /// </summary>
    /// <param name="index">Audio source index</param>
    /// <param name="duration">Fade duration in seconds</param>
    public static void FadeOut(int index, float duration)
    {
        if (!IsValid(index)) return;
        Instance.StartCoroutine(Instance.FadeAudioOut(index, duration));
    }

    /// <summary>
    /// Crossfades from one audio source to another
    /// </summary>
    /// <param name="fromIndex">Source currently playing</param>
    /// <param name="toName">Name of new audio clip</param>
    /// <param name="toIndex">New audio source to fade into</param>
    /// <param name="duration">Fade duration</param>
    public static void Crossfade(int fromIndex, string toName, int toIndex, float duration)
    {
        if (!IsValid(fromIndex) || !IsValid(toIndex)) return;

        AudioClip newClip = Instance.GetAudioClip(toName);
        Instance.StartCoroutine(Instance.CrossfadeAudio(fromIndex, newClip, toIndex, duration));
    }

    /// <summary>
    /// Avoids redundant code that finds the audio clip
    /// </summary>
    /// <param name="name">name of the audio clip</param>
    /// <returns>AudioClip</returns>
    /// <exception cref="System.Exception">If not found</exception>
    private AudioClip GetAudioClip(string name)
    {
        // Goes through our list and tries finding our audio clip
        for (int i = 0; i < soundEffectClip.Count; i++)
        {
            if (name == soundEffectClip[i].name)
            {
                return soundEffectClip[i];
            }
        }

        // If temp doesn't get set to anything, i.e. name doesn't exist
        throw new System.Exception("No variable found " + name);
    }

    /// <summary>
    /// Checks if audio source index is valid
    /// </summary>
    private static bool IsValid(int index)
    {
        return Instance != null && index >= 0 && index < Instance.audioSources.Count;
    }

    /// <summary>
    /// Coroutine that fades audio in
    /// </summary>
    private IEnumerator FadeAudioIn(int index, float duration, float targetVolume)
    {
        AudioSource source = audioSources[index];
        source.volume = 0f;
        source.Play();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    /// <summary>
    /// Coroutine that fades audio out
    /// </summary>
    private IEnumerator FadeAudioOut(int index, float duration)
    {
        AudioSource source = audioSources[index];
        float startVolume = source.volume;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }

    /// <summary>
    /// Coroutine that fades from one source to another
    /// </summary>
    private IEnumerator CrossfadeAudio(int fromIndex, AudioClip newClip, int toIndex, float duration)
    {
        AudioSource from = audioSources[fromIndex];
        AudioSource to = audioSources[toIndex];

        float fromStartVolume = from.volume;

        to.clip = newClip;
        to.volume = 0f;
        to.loop = true;
        to.Play();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            from.volume = Mathf.Lerp(fromStartVolume, 0f, t);
            to.volume = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        from.Stop();
        to.volume = 1f;
    }
}


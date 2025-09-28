using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum Sound
{
    MainTheme,
    UIButtonClick,
    PlayerFootstep,
    PistolEmpty,
    PistolShot,
    PistolReload
}

[Serializable]
public class SoundAudioClip
{
    public Sound sound;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxOneShotSource;
    [SerializeField] private AudioSource sfxLoopSource;
    [SerializeField] private AudioSource uiSource;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Controls")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float uiVolume = 1f;

    [Header("Audio Clips")]
    [SerializeField] private List<SoundAudioClip> soundClips;

    private Dictionary<Sound, AudioClip> clipDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Build dictionary for quick lookup
        clipDict = new Dictionary<Sound, AudioClip>();
        foreach (var entry in soundClips)
        {
            if (!clipDict.ContainsKey(entry.sound))
                clipDict.Add(entry.sound, entry.clip);
        }
    }

    private void Update()
    {
        ApplyVolumes();
    }

    public void PlayMusic(Sound sound, bool loop = true)
    {
        if (!clipDict.TryGetValue(sound, out AudioClip clip)) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFXOneShot(Sound sound)
    {
        if (!clipDict.TryGetValue(sound, out AudioClip clip)) return;
        sfxOneShotSource.PlayOneShot(clip);
    }

    public void PlaySFXLoop(Sound sound, bool loop = false)
    {
        if (!clipDict.TryGetValue(sound, out AudioClip clip)) return;
        sfxLoopSource.clip = clip;
        sfxLoopSource.loop = loop;
        sfxLoopSource.Play();
    }

    public void PlayUI(Sound sound)
    {
        if (!clipDict.TryGetValue(sound, out AudioClip clip)) return;
        uiSource.PlayOneShot(clip);
    }

    private void ApplyVolumes()
    {
        SetVolume("MasterVolume", masterVolume);
        SetVolume("MusicVolume", musicVolume);
        SetVolume("SFXVolume", sfxVolume);
        SetVolume("UIVolume", uiVolume);
    }

    private void SetVolume(string parameter, float value)
    {
        // Convert linear slider (0–1) to decibels
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameter, dB);
    }
}
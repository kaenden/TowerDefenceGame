using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;
    public bool loop = false;
    
    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Sounds")]
    public Sound[] sounds;
    
    [Header("Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 1f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    
    private Dictionary<string, Sound> soundDictionary;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeAudio()
    {
        soundDictionary = new Dictionary<string, Sound>();
        
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            
            soundDictionary.Add(sound.name, sound);
        }
        
        LoadAudioSettings();
    }
    
    public void PlaySound(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            Sound sound = soundDictionary[soundName];
            sound.source.volume = sound.volume * sfxVolume * masterVolume;
            sound.source.Play();
        }
        else
        {
            Debug.LogWarning($"Sound {soundName} not found!");
        }
    }
    
    public void PlaySoundAtPosition(string soundName, Vector3 position)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            Sound sound = soundDictionary[soundName];
            AudioSource.PlayClipAtPoint(sound.clip, position, sound.volume * sfxVolume * masterVolume);
        }
        else
        {
            Debug.LogWarning($"Sound {soundName} not found!");
        }
    }
    
    public void StopSound(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            soundDictionary[soundName].source.Stop();
        }
    }
    
    public void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        if (musicSource != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = loop;
            musicSource.volume = musicVolume * masterVolume;
            musicSource.Play();
        }
    }
    
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
    
    public void PauseMusic()
    {
        if (musicSource != null)
        {
            musicSource.Pause();
        }
    }
    
    public void ResumeMusic()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
        SaveAudioSettings();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
        SaveAudioSettings();
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        SaveAudioSettings();
    }
    
    private void UpdateAllVolumes()
    {
        foreach (Sound sound in sounds)
        {
            if (sound.source != null)
            {
                sound.source.volume = sound.volume * sfxVolume * masterVolume;
            }
        }
        
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
    }
    
    private void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }
    
    private void LoadAudioSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        
        UpdateAllVolumes();
    }
    
    public bool IsSoundPlaying(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            return soundDictionary[soundName].source.isPlaying;
        }
        return false;
    }
    
    public void FadeInMusic(AudioClip musicClip, float fadeTime = 1f)
    {
        if (musicSource != null)
        {
            StartCoroutine(FadeInCoroutine(musicClip, fadeTime));
        }
    }
    
    public void FadeOutMusic(float fadeTime = 1f)
    {
        if (musicSource != null)
        {
            StartCoroutine(FadeOutCoroutine(fadeTime));
        }
    }
    
    private System.Collections.IEnumerator FadeInCoroutine(AudioClip clip, float fadeTime)
    {
        musicSource.clip = clip;
        musicSource.volume = 0f;
        musicSource.Play();
        
        float targetVolume = musicVolume * masterVolume;
        float currentTime = 0f;
        
        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, currentTime / fadeTime);
            yield return null;
        }
        
        musicSource.volume = targetVolume;
    }
    
    private System.Collections.IEnumerator FadeOutCoroutine(float fadeTime)
    {
        float startVolume = musicSource.volume;
        float currentTime = 0f;
        
        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, currentTime / fadeTime);
            yield return null;
        }
        
        musicSource.volume = 0f;
        musicSource.Stop();
    }
}
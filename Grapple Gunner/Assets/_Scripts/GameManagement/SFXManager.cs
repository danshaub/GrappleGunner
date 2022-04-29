using System;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine;

public class SFXManager : SingletonPersistent<SFXManager>
{
    public Sound[] sfxs;
    public Sound[] voiceClips;
    public Sound[] ambientBackgroundClips;
    public AudioMixer mixer;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup voiceGroup;
    public AudioMixerGroup ambientGroup;

    private AudioSource musicSource;

    protected override void Awake()
    {
        base.Awake();

        InitSoundArray(sfxs, sfxGroup);
        InitSoundArray(voiceClips, voiceGroup);
        InitSoundArray(ambientBackgroundClips, ambientGroup);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.loop = true;
    }

    private void InitSoundArray(Sound[] sounds, AudioMixerGroup group)
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.outputAudioMixerGroup = group;

            s.source.playOnAwake = s.playOnAwake;
            s.source.loop = s.loop;


            if (s.playOnAwake) s.source.Play();
        }
    }

    public void PlaySFXOneShot(string name)
    {
        Sound s = Array.Find(sfxs, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound Effect: " + name + " was not found.");
            return;
        }
        s.source.PlayOneShot(s.clip);
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxs, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound Effect: " + name + " was not found.");
            return;
        }
        s.source.Play();
    }

    public void SetSFXVolume(string name, float volume, bool playSound)
    {
        Sound s = Array.Find(sfxs, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound Effect: " + name + " was not found.");
            return;
        }
        s.source.volume = volume;

        if (playSound && !s.source.isPlaying)
        {
            s.source.Play();
        }
    }

    public float GetSFXVolume(string name)
    {
        Sound s = Array.Find(sfxs, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound Effect: " + name + " was not found.");
            return 0;
        }
        return s.source.volume;
    }

    public void SetSFXPitch(string name, float pitch)
    {
        Sound s = Array.Find(sfxs, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound Effect: " + name + " was not found.");
            return;
        }
        s.source.pitch = pitch;
    }

    public float GetSFXPitch(string name)
    {
        Sound s = Array.Find(sfxs, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound Effect: " + name + " was not found.");
            return 0;
        }
        return s.source.pitch;
    }

    public void PlayVoiceClip(string name)
    {
        Sound s = Array.Find(voiceClips, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Voice Clip: " + name + " was not found.");
            return;
        }
        s.source.Play();
    }

    public void StopSFX(string name)
    {
        Sound s = Array.Find(sfxs, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound Effect: " + name + " was not found.");
            return;
        }
        s.source.Stop();
    }

    public void StopVoiceClip(string name)
    {
        Sound s = Array.Find(voiceClips, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Voice Clip: " + name + " was not found.");
            return;
        }
        s.source.Stop();
    }

    public void FadeInSFX(string name, float decayTime)
    {
        Sound s = Array.Find(sfxs, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound Effect: " + name + " was not found.");
            return;
        }

        if (!s.source.isPlaying)
        {
            s.source.Play();
        }
        StartCoroutine(FadeInSFXCoroutine(decayTime, s));
    }

    private IEnumerator FadeInSFXCoroutine(float decayTime, Sound s)
    {
        float decayRate = s.volume / decayTime;

        while (s.source.volume < s.volume)
        {
            s.source.volume += (decayRate * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public void FadeOutSFX(string name, float decayTime, bool stopClip)
    {
        Sound s = Array.Find(sfxs, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound Effect: " + name + " was not found.");
            return;
        }

        StartCoroutine(FadeOutSFXCoroutine(decayTime, s, stopClip));
    }

    private IEnumerator FadeOutSFXCoroutine(float decayTime, Sound s, bool stopClip)
    {
        float decayRate = s.volume / decayTime;

        while (s.source.volume > 0f)
        {
            s.source.volume -= (decayRate * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        if (stopClip)
        {
            s.source.Stop();
        }
    }

    public void StopAllVoiceClips()
    {
        foreach (Sound s in voiceClips)
        {
            s.source.Stop();
        }
    }

    public void StopAllLoopingSounds()
    {
        foreach (Sound s in voiceClips)
        {
            if (s.loop)
            {
                s.source.Stop();
            }
        }
        foreach (Sound s in ambientBackgroundClips)
        {
            if (s.loop)
            {
                s.source.Stop();
            }
        }
    }

    public void StartAmbientSounds()
    {
        foreach (Sound s in ambientBackgroundClips)
        {
            s.source.Play();
        }
    }

    public void PlayMusic(Sound music)
    {
        musicSource.Stop();

        musicSource.clip = music.clip;
        musicSource.volume = music.volume;
        musicSource.pitch = music.pitch;

        musicSource.PlayDelayed(1f);
    }

    public void FadeOutMusic(float decayTime)
    {
        StartCoroutine(FadeOutMusicCoroutine(decayTime));
    }

    private IEnumerator FadeOutMusicCoroutine(float decayTime)
    {
        float decayRate = musicSource.volume / decayTime;

        while (musicSource.volume > 0f)
        {
            musicSource.volume -= (decayRate * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        musicSource.Stop();
    }

    public void VolumeIncrease(string mixerVolume)
    {
        float vol;
        mixer.GetFloat(mixerVolume, out vol);

        vol = Mathf.Clamp(vol + 5, -80, 20);

        mixer.SetFloat(mixerVolume, vol);
    }

    public void VolumeDecrease(string mixerVolume)
    {
        float vol;
        mixer.GetFloat(mixerVolume, out vol);

        vol = Mathf.Clamp(vol - 5, -80, 20);

        mixer.SetFloat(mixerVolume, vol);
    }

    public void SetVolume(string mixerVolume, float vol)
    {
        SetVolume(mixerVolume, vol, true);
    }

    public void SetVolume(string mixerVolume, float vol, bool saveGame)
    {
        switch (mixerVolume)
        {
            case "MusicVolume":
                GameManager.Instance.options.ambientVolume = Mathf.Clamp(vol, -80, 20);
                break;
            case "SFXVolume":
                GameManager.Instance.options.ambientVolume = Mathf.Clamp(vol, -80, 20);
                break;
            case "VoiceVolume":
                GameManager.Instance.options.ambientVolume = Mathf.Clamp(vol, -80, 20);
                break;
            case "AmbientVolume":
                GameManager.Instance.options.ambientVolume = Mathf.Clamp(vol, -80, 20);
                break;
        }

        mixer.SetFloat(mixerVolume, Mathf.Clamp(vol, -80, 20));

        if(saveGame) GameSaveManager.Instance.SaveGame();
    }

    public float GetVolume(string mixerVolume)
    {
        float vol;
        mixer.GetFloat(mixerVolume, out vol);
        return vol;
    }
}

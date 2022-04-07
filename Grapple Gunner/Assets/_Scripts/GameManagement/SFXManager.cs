using System;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine;

public class SFXManager : SingletonPersistent<SFXManager>
{
    public Sound[] sfxs;
    public Sound[] voiceClips;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup voiceGroup;

    private AudioSource musicSource;

    protected override void Awake()
    {
        base.Awake();

        InitSoundArray(sfxs, musicGroup);
        InitSoundArray(voiceClips, voiceGroup);

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
        }
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


}

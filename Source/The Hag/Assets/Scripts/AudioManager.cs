using UnityEngine.Audio;
using System.Collections;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Audio[] musics;
    public Audio[] sounds;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Audio m in musics)
        {
            m.source = gameObject.AddComponent<AudioSource>();
            m.source.clip = m.audioClip;

            m.source.volume = m.volume;
            m.source.pitch = m.pitch;
            m.source.loop = m.loop;
            m.source.playOnAwake = m.playOnAwake;
        }

        foreach (Audio s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
        }
    }

    //Play all onAwake audio
    void Start()
    {
        foreach (Audio m in musics)
        {
            if (m.source.playOnAwake)
            {
                playMusic(m.name);
            }
        }

        foreach (Audio s in sounds)
        {
            if (s.source.playOnAwake)
            {
                playSound(s.name);
            }
        }
    }

    public void playSound(string soundName)
    {
        Audio s = Array.Find(sounds, sound => sound.name == soundName);

        if(s != null)
        {
            if (!s.source.isPlaying && !s.source.loop)
            {
                s.source.Play();
            }
        }
        else
        {
            Debug.Log("Could not find sound to play: " + soundName);
        }
    }
    public void stopSound(string soundName)
    {
        Audio s = Array.Find(sounds, sound => sound.name == soundName);

        if (s != null)
        {
            if (s.source.isPlaying) { 
                s.source.Stop();
            }
        }
        else
        {
            Debug.Log("Could not find sound to stop: " + soundName);
        }
    }

    public void playMusic(string musicName)
    {
        Audio m = Array.Find(musics, sound => sound.name == musicName);

        if (m != null)
        {
            if (!m.source.isPlaying) {
                if (!isMusicPlaying())
                {
                    m.source.Play();
                }
                else
                {
                    StartCoroutine(swapMusic(m.source, 2.5f));
                }
            }
        }
        else
        {
            Debug.Log("Could not find music to play: " + musicName);
        }
    }
    public void stopMusic(string musicName)
    {
        Audio m = Array.Find(musics, sound => sound.name == musicName);

        if (m != null)
        {
            if (m.source.isPlaying)
            {
                m.source.Stop();
            }
        }
        else
        {
            Debug.Log("Could not find music to stop: " + musicName);
        }
    }

    IEnumerator swapMusic(AudioSource musicSource, float swapSpeed)
    {
        StartCoroutine(fadeOutCurrentMusic(swapSpeed));

        yield return new WaitForSeconds(0.4f);

        musicSource.Play();
    }

    public bool isMusicPlaying()
    {
        foreach (Audio m in musics)
        {
            if (m.source.isPlaying)
            {
                return true;
            }
        }

        return false;
    }

    public IEnumerator fadeOutAudio(string audioName, float fadeTime)
    {
        Audio m = Array.Find(musics, sound => sound.name == audioName);
        Audio s = Array.Find(sounds, sound => sound.name == audioName);

        if (m != null && m.source.isPlaying)
        {
            float startVolume = m.source.volume;

            while (m.source.volume > 0)
            {
                m.source.volume -= startVolume * Time.deltaTime / fadeTime;

                yield return null;
            }

            m.source.Stop();
            m.source.volume = startVolume;
        }
        else if (s != null && s.source.isPlaying)
        {
            float startVolume = s.source.volume;

            while (s.source.volume > 0)
            {
                s.source.volume -= startVolume * Time.deltaTime / fadeTime;

                yield return null;
            }

            s.source.Stop();
            s.source.volume = startVolume;
        }
        else
        {
            Debug.Log("Could not find audio to fade out: " + audioName);
        }
    }

    public IEnumerator fadeOutCurrentMusic(float fadeTime)
    {
        foreach (Audio m in musics)
        {
            if (m.source.isPlaying)
            {
                float startVolume = m.source.volume;

                while (m.source.volume > 0)
                {
                    m.source.volume -= startVolume * Time.deltaTime / fadeTime;

                    yield return null;
                }

                m.source.Stop();
                m.source.volume = startVolume;

                break;
            }
        }
    }

    public IEnumerator fadeOutAllSound(float fadeTime)
    {
        foreach (Audio s in sounds)
        {
            if (s.source.isPlaying)
            {
                float startVolume = s.source.volume;

                while (s.source.volume > 0)
                {
                    s.source.volume -= startVolume * Time.deltaTime / fadeTime;

                    yield return null;
                }

                s.source.Stop();
                s.source.volume = startVolume;
            }
        }
    }

    public void fadeOutAllAudio(float fadeTime)
    {
        StartCoroutine(fadeOutCurrentMusic(fadeTime));
        StartCoroutine(fadeOutAllSound(fadeTime));
    }
}

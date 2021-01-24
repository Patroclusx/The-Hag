using System.Collections;
using System;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public Audio[] musics;
    public Audio[] sounds;
    public AudioCollection[] soundCollections;

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

        foreach (AudioCollection sc in soundCollections)
        {
            for(int i = 0; i < sc.audioClips.Length; i++)
            {
                sc.collectionSource.Add(gameObject.AddComponent<AudioSource>());
                sc.collectionSource[i].clip = sc.audioClips[i];

                sc.collectionSource[i].volume = sc.volume;
                sc.collectionSource[i].pitch = sc.pitch;
                sc.collectionSource[i].playOnAwake = false;
            }
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
                playSound(s.name, false);
            }
        }
    }

    public void playSound(string soundName, bool canOverlap)
    {
        Audio s = Array.Find(sounds, sound => sound.name == soundName);

        if(s != null)
        {
            if(s.source.loop && !s.source.isPlaying)
            {
                s.source.Play();
            }
            else if (!s.source.loop && (!s.source.isPlaying || canOverlap))
            {
                s.source.PlayOneShot(s.audioClip);
            }
        }
        else
        {
            Debug.Log("Could not find sound to play: " + soundName);
        }
    }
    public void playSound(string soundName, float frequencyInSeconds)
    {
        if (!AudioFrequency.getInstance(soundName, gameObject).isWaiting(frequencyInSeconds))
        {
            Audio s = Array.Find(sounds, sound => sound.name == soundName);

            if (s != null)
            {
                if (!s.source.loop)
                {
                    s.source.PlayOneShot(s.audioClip);
                }
            }
            else
            {
                Debug.Log("Could not find sound to play: " + soundName);
            }
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

    public void playCollectionSound(string soundCollectionName, float frequencyInSeconds, GameObject gameObjectAttach)
    {
        if (!AudioFrequency.getInstance(gameObjectAttach.name + "_" + soundCollectionName, gameObjectAttach).isWaiting(frequencyInSeconds))
        {
            AudioCollection sc = Array.Find(soundCollections, sound => sound.collectionName == soundCollectionName);

            if (sc != null)
            {
                int collectionLenght = sc.audioClips.Length;
                int randomClip = UnityEngine.Random.Range(0, collectionLenght);

                sc.collectionSource[randomClip].PlayOneShot(sc.collectionSource[randomClip].clip);
            }
            else
            {
                Debug.Log("Could not find sound collection to play: " + soundCollectionName);
            }
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

    public IEnumerator fadeOutSound(string audioName, float fadeTime)
    {
        Audio s = Array.Find(sounds, sound => sound.name == audioName);

        if (s != null && s.source.isPlaying)
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

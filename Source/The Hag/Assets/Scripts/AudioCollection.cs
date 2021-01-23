using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioCollection
{
    public string collectionName;

    public AudioClip[] audioClips;

    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    [HideInInspector]
    public List<AudioSource> collectionSource;
}

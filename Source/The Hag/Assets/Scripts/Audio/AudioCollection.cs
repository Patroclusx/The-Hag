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

    [Header("3D settings (Ignore if 2D only sound)")]
    [Range(0f, 1f)]
    public float spatialBlend;
    [Range(1f, 30f)]
    public float maxDistance;

    [HideInInspector]
    public List<AudioSource> collectionSources;
    [HideInInspector]
    public AudioSource lastSourcePlayed;
}

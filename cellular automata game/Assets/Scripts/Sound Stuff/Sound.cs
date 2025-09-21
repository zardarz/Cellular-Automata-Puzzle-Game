using System;
using UnityEngine;

[Serializable]
public class Sound
{
    public AudioClip clip;

    [Range(0f,1f)]
    public float volume = .5f;

    [Range(.1f, 3f)]
    public float pitch = 1;

    public bool looping;
}
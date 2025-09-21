using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private List<SoundEntry> soundEntries = new List<SoundEntry>();

    private static Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();

    void Awake()
    {
        foreach (SoundEntry entry in soundEntries)
        {
            if (!sounds.ContainsKey(entry.key))
            {
                sounds.Add(entry.key, entry.value);
            }
            else
            {
                Debug.LogWarning($"Duplicate key '{entry.key}' detected!");
            }
        }
    }

    public static void Play(string name)
    {
        if (!sounds.ContainsKey(name))
        {
            Debug.LogError($"Sound '{name}' not found!");
            return;
        }

        Sound s = sounds[name];
        GameObject soundObject = new GameObject("TempAudioSource");
        AudioSource source = soundObject.AddComponent<AudioSource>();

        source.clip = s.clip;
        source.volume = s.volume;
        source.pitch = s.pitch + Random.Range(-0.05f, 0.05f);
        source.loop = s.looping;

        source.Play();

        if (!s.looping)
        {
            Destroy(soundObject, source.clip.length);
        }
    }
}
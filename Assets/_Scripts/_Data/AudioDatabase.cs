using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDatabase", menuName = "Scriptable Objects/AudioDatabase")]
public class AudioDatabase : ScriptableObject
{
    public List<AudioClip> bgmClips;
    public List<AudioClip> sfxClips;

    public AudioClip GetBGM(string clipName)
    {
        return bgmClips.Find(clip => clip.name == clipName);
    }
    public AudioClip GetSFX(string clipName)
    {
        return sfxClips.Find(clip => clip.name == clipName);
    }
}

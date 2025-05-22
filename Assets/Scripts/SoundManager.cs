using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private List<Sounds> _sounds;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void PlaySound(SoundTypes type,float destroyTime = 1)
    {
        var sound = Instantiate(_sounds.Find(x => x.Type == type).Audio);
        sound.Play();
        Destroy(sound.gameObject, destroyTime);
    }

    public void StopSound(SoundTypes type)
    {
        _sounds.Find(x => x.Type == type).Audio.Stop();
    }
}

[System.Serializable]
public struct Sounds
{
    public AudioSource Audio;
    public SoundTypes Type;
}

[System.Serializable]
public enum SoundTypes
{
    Background,
    RollSound,
    TakeSound,
    PlaceSound,
    BrokeSound,
    NewTourSound
}

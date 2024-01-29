using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectPlayer : MonoBehaviour
{
    public static SoundEffectPlayer Instance { get; private set; }

    [SerializeField] private AudioClip[] _audioClips;

    private AudioSource _audioSource;

    private Dictionary<string, AudioClip> _audioClipsByName;

    private void Awake()
    {
        if (Instance == null)
        {
            Debug.Log($"Setting {GetInstanceID()} as singleton");
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log($"Destroying {GetInstanceID()}");
            Destroy(this);
            return;
        }

        _audioSource = GetComponent<AudioSource>();
        _audioClipsByName = new Dictionary<string, AudioClip>();
        foreach (var audioClip in _audioClips)
        {
            _audioClipsByName[audioClip.name] = audioClip;
        }
    }

    public void PlayClip(string clipName)
    {
        _audioSource.PlayOneShot(_audioClipsByName[clipName]);
    }
}
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectPlayer : MonoBehaviour
{
    private static SoundEffectPlayer _instance;

    [SerializeField] private AudioClip[] _audioClips;

    private AudioSource _audioSource;

    private Dictionary<string, AudioClip> _audioClipsByName;

    private void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
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
        _audioSource.clip = _audioClipsByName[clipName];
        _audioSource.Play();
    }
}
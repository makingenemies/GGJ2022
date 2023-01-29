using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioVolumeController : MonoBehaviour
{
    [SerializeField] private int _audioVolume;

    private AudioSource[] _audioSources;

    public static AudioVolumeController Instance { get; private set; }

    public float AudioVolume => _audioVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyVolumeToAudioSources();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void ApplyVolumeToAudioSources()
    {
        _audioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in _audioSources)
        {
            audioSource.volume = (float)_audioVolume / 100;
        }
    }

    public void SetAudioVolume(int audioVolume)
    {
        _audioVolume = audioVolume;
        ApplyVolumeToAudioSources();
    }

    public void UpdateAudioVolume(int delta)
    {
        _audioVolume = Math.Max(0, Math.Min(100, _audioVolume + delta));
        ApplyVolumeToAudioSources();
    }
}
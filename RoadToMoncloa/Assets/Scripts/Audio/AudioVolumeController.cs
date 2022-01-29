using UnityEngine;

public class AudioVolumeController : MonoBehaviour
{
    private static AudioVolumeController _instance;

    [SerializeField] private float _audioVolume;

    private AudioSource[] _audioSources;

    public float AudioVolume => _audioVolume;

    private void Start()
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

        ApplyVolumeToAudioSources();
    }

    private void ApplyVolumeToAudioSources()
    {
        _audioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in _audioSources)
        {
            audioSource.volume = _audioVolume;
        }
    }

    public void UpdateAudioVolume(float audioVolume)
    {
        _audioVolume = audioVolume;
    }
}
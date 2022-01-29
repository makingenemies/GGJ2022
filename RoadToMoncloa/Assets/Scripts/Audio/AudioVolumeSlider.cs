using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeSlider : Slider
{
    private AudioVolumeController _audioVolumeController;

    protected override void Start()
    {
        base.Start();
        _audioVolumeController = FindObjectOfType<AudioVolumeController>();
        value = _audioVolumeController.AudioVolume;
    }

    public void UpdateAudioVolume()
    {
        _audioVolumeController.UpdateAudioVolume(this.value);
    }
}
using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeSlider : Slider
{
    private AudioVolumeController _audioVolumeController;

    protected override void Start()
    {
        base.Start();
        _audioVolumeController = AudioVolumeController.Instance;
        value = _audioVolumeController.AudioVolume;
    }

    public void UpdateAudioVolume()
    {
        _audioVolumeController.SetAudioVolume((int)(this.value * 100));
    }
}
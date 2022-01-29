using TMPro;
using UnityEngine;

public class AudioVolumeTextController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _valueText;

    private AudioVolumeController _audioVolumeController;

    protected void Start()
    {
        _audioVolumeController = FindObjectOfType<AudioVolumeController>();
        _valueText.text = ((int)_audioVolumeController.AudioVolume).ToString();
    }

    public void UpdateAudioVolume(int delta)
    {
        _audioVolumeController.UpdateAudioVolume(delta);
        _valueText.text = ((int)_audioVolumeController.AudioVolume).ToString();
    }
}
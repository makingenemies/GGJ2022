using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSoundEffectPlayer : MonoBehaviour
{
    [SerializeField] private string _clipName;

    private SoundEffectPlayer _soundEffectPlayer;

    private void Start()
    {
        _soundEffectPlayer = FindObjectOfType<SoundEffectPlayer>();
    }

    public void PlaySound()
    {
        _soundEffectPlayer.PlayClip(_clipName);
    }
}

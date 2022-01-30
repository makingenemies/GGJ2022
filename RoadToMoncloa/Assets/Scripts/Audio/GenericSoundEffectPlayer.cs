using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSoundEffectPlayer : MonoBehaviour
{
    private SoundEffectPlayer _soundEffectPlayer;

    private void Start()
    {
        _soundEffectPlayer = FindObjectOfType<SoundEffectPlayer>();
    }

    public void PlaySound(string clipName)
    {
        _soundEffectPlayer.PlayClip(clipName);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSoundEffectPlayer : MonoBehaviour
{
    private SoundEffectPlayer _soundEffectPlayer;

    private void Start()
    {
        _soundEffectPlayer = SoundEffectPlayer.Instance;
    }

    public void PlaySound(string clipName)
    {
        _soundEffectPlayer.PlayClip(clipName);
    }
}

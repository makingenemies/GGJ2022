using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonSoundPlayer : MonoBehaviour
{
    private SoundEffectPlayer _soundEffectPlayer;

    private void Start()
    {
        _soundEffectPlayer = SoundEffectPlayer.Instance;
    }

    public void PlayMouseHoverSound()
    {
        _soundEffectPlayer.PlayClip(SoundNames.Menu.MouseHover);
    }

    public void PlayClickButton()
    {
        _soundEffectPlayer.PlayClip(SoundNames.Menu.ClickButton);
    }
}

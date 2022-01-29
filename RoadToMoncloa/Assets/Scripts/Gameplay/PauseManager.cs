using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private bool _isPaused;

    public bool IsPaused => _isPaused;

    public void Pause()
    {
        _isPaused = true;
    }

    public void Unpause()
    {
        _isPaused = false;
    }
}
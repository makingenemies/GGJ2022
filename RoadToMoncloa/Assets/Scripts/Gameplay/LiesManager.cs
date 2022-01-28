using UnityEngine;

public class LiesManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] _lieMarkers;

    private int _playedLiesCount;

    public bool PlayLie()
    {
        if (_playedLiesCount >= _lieMarkers.Length)
        {
            return false;
        }

        _lieMarkers[_playedLiesCount].color = Color.red;
        _playedLiesCount++;
        return true;
    }
}
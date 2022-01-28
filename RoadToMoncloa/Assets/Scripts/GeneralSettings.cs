using UnityEngine;

public class GeneralSettings : MonoBehaviour
{
    [SerializeField] private Language _language;

    public Language Language => _language;
}

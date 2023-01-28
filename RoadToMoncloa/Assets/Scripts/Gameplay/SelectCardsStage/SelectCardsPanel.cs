using System.Collections.Generic;
using UnityEngine;

public class SelectCardsPanel : MonoBehaviour
{
    [SerializeField] private List<Transform> _cardPlaceHolders;

    public List<Transform> CardPlaceHolders => _cardPlaceHolders;

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
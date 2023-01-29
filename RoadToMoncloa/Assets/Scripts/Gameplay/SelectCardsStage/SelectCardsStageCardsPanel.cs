using System.Collections.Generic;
using UnityEngine;

public class SelectCardsStageCardsPanel : MonoBehaviour
{
    [SerializeField] private List<Transform> _cardPlaceHolders;

    public List<Transform> CardPlaceHolders => _cardPlaceHolders;
}
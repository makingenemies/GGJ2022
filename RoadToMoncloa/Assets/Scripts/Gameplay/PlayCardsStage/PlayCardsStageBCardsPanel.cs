using UnityEngine;

public class PlayCardsStageBCardsPanel : MonoBehaviour
{
    [SerializeField] private Transform[] _cardPlaceholders;
    [SerializeField] PlayStageCard _cardPrefab;

    private int _cardsCounter;

    public PlayStageCard InstantiateCard()
    {
        var card = Instantiate(_cardPrefab, _cardPlaceholders[_cardsCounter]).GetComponent<PlayStageCard>();
        card.GetComponentInChildren<CardUI>().IncreaseSortingLayer(_cardsCounter);
        _cardsCounter++;
        return card;
    }
}
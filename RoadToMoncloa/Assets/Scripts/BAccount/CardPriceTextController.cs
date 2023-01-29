using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardPriceTextController : MonoBehaviour
{

    [SerializeField] private TextMeshPro _cardPriceText;

    public void UpdateCardPriceText(int amount)
    {
        _cardPriceText.text = $"{amount.ToString()} €";
    }
}

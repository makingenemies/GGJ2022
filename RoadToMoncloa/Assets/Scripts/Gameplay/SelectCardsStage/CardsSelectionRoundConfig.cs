using System;

[Serializable]
public class CardsSelectionRoundConfig
{
    public int NumberOfOfferedCards;
    public int NumberOfCardsToSelect;

    public static CardsSelectionRoundConfig GetDefaultConfig()
    {
        return new CardsSelectionRoundConfig
        {
            NumberOfOfferedCards = 5,
            NumberOfCardsToSelect = 3,
        };
    }
}
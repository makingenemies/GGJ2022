using System;

[Serializable]
public class RoundConfig
{
    public int NumberOfOfferedCards;
    public int NumberOfCardsToSelect;
    public int NumberOfCardsToPlay;

    public int NumberOfRandomCards => NumberOfCardsToPlay - NumberOfCardsToSelect;

    public static RoundConfig GetDefaultConfig()
    {
        return new RoundConfig
        {
            NumberOfOfferedCards = 5,
            NumberOfCardsToSelect = 3,
            NumberOfCardsToPlay = 5,
        };
    }
}
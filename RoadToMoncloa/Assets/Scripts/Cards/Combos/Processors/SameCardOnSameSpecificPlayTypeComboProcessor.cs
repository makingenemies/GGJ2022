/// <summary>
/// After a card is played, if there was one more similar card (and only one more)
/// played in the side set up in the combo, we update the stats 
/// of both the card that was just played and the similar one that had been played earlier.
/// </summary>
public class SameCardOnSameSpecificPlayTypeComboProcessor : BaseComboProcessor<SameCardOnSameSpecificPlayTypeComboSO>
{
    private PlayCardsStageGameplayManager _gameplayManager;

    public SameCardOnSameSpecificPlayTypeComboProcessor(PlayCardsStageGameplayManager gameplayManager)
    {
        _gameplayManager = gameplayManager;
    }

    protected override void ProcessComboAfterPlayImpl(SameCardOnSameSpecificPlayTypeComboSO combo, PlayStageCard playedCard, CardPlayType playType)
    {
        if (playType != combo.PlayType)
        {
            return;
        }

        var similarCardsCount = 0;
        PlayStageCard similarCard = null;
        foreach (var card in _gameplayManager.CardsPlayedByPlayType[playType])
        {
            // TODO: Compare Id instead of TitleId
            if (card.CardData.Category == card.CardData.Category)
            {
                similarCardsCount++;
                similarCard = card;
            }
        }
        if (similarCardsCount == 1)
        {
            ApplyComboModifiersToCard(similarCard, combo);
            ApplyComboModifiersToCard(playedCard, combo);
        }
    }

    private void ApplyComboModifiersToCard(PlayStageCard card, SameCardOnSameSpecificPlayTypeComboSO combo)
    {
        card.MoneyWonModifier += combo.MoneyWonModifier;
        card.MoneyLostModifier += combo.MoneyLostModifier;
        card.VotersWonModifier += combo.VotersWonModifier;
        card.VotersLostModifier += combo.VotersLostModifier;
        card.UpdateVotersWonText();
        card.UpdateVotersLostText();
        card.UpdateMoneyWonText();
        card.UpdateMoneyLostText();
        _gameplayManager.UpdateVotersCounter(combo.VotersWonModifier - combo.VotersLostModifier);
        _gameplayManager.UpdateMoneyCounter(combo.MoneyWonModifier - combo.MoneyLostModifier);
    }
}
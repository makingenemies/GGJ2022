using System;

[Serializable]
public class CardSlotRowConfig {
    private const int MaxCardSlotsCount = 3;
    
    public CardSlotConfig[] CardSlotsLeftToRight;

    public void FixData() {
        if (CardSlotsLeftToRight.Length > MaxCardSlotsCount) {
            var tempSlots = new CardSlotConfig[MaxCardSlotsCount];
            for (var i = 0; i < MaxCardSlotsCount; i++) {
                tempSlots[i] = CardSlotsLeftToRight[i];
            }

            CardSlotsLeftToRight = tempSlots;
        }
    }
}
using System;

[Serializable]
public class BoardSideConfig {
    
    private const int MaxCardSlotRowsCount = 2;
    
    public CardSlotRowConfig[] CardSlotsRowsUpToDown;

    public void FixData() {
        if (CardSlotsRowsUpToDown.Length > MaxCardSlotRowsCount) {
            var tempRows = new CardSlotRowConfig[MaxCardSlotRowsCount];
            for (var i = 0; i < MaxCardSlotRowsCount; i++) {
                tempRows[i] = CardSlotsRowsUpToDown[i];
            }

            CardSlotsRowsUpToDown = tempRows;
        }

        foreach (var row in CardSlotsRowsUpToDown) {
            row.FixData();
        }
    }
}
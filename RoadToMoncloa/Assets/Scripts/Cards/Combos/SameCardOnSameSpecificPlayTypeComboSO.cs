using UnityEngine;

[CreateAssetMenu(fileName = "SameCardOnSameSpecificPlayTypeComboSO", menuName = "RoadToMoncloa/Combos/SameCardOnSameSpecificPlayType")]
public class SameCardOnSameSpecificPlayTypeComboSO : CardComboSO
{
    public CardPlayType PlayType;
    public int MoneyWonModifier;
    public int MoneyLostModifier;
    public int VotersWonModifier;
    public int VotersLostModifier;
}
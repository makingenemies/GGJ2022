using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "RoadToMoncloa/CardData")]
public class CardData : ScriptableObject
{
    public string TitleId;
    public int VotersWon;
    public int MoneyWon;
}

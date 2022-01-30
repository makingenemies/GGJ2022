using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "RoadToMoncloa/CardData")]
public class CardData : ScriptableObject
{
    public string TitleId;
    public string LeftAttributeId;
    public string RightAttributeId;
    public int VotersWon;
    public int MoneyWon;
    public int VotersLost;
    public int MoneyLost;
    public CardCategory Category;
}

using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "RoadToMoncloa/LevelData")]
public class LevelData : ScriptableObject
{
    public CardData[] Cards;
    public int VotersGoal;
    public int DonationCost;
    public RoundConfig[] CardSelectionRoundConfigs;
}
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster", menuName = "Monster Selection/Beast")]
public class MonsterSelection : ScriptableObject
{
    public string MonsterName;
    public Sprite MonsterImage;
    public string MonsterReward;
}
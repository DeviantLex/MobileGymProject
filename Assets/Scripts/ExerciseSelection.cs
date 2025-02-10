using UnityEngine;

[CreateAssetMenu(fileName = "New Exercise", menuName = "Exercise Selection/Exercise")]
public class ExerciseSelection : ScriptableObject
{
    public string exerciseName;
    public Sprite exerciseImage;
    public string exerciseDescription;
    public string exerciseReward;
}
using UnityEngine;


[CreateAssetMenu(fileName = "ID_", menuName = "Game/Gameplay ID")]
public class GameplayObjectID : ScriptableObject
{
    [SerializeField] public string displayName;
    [SerializeField] public Sprite icon;
}

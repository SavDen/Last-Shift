using UnityEngine;

[CreateAssetMenu(menuName = "EnemyData/Enemy")]
public class EnemyData : ScriptableObject
{
    public EnemyBase prefab;

    public int Health;
    public float Speed;
    public float StopDis;
    public float Damage;
    public int MinPriority;
    public int MaxPriority;
}

using UnityEngine;

[CreateAssetMenu(menuName = "PlayerProfile/Player")]
public class PlayerData : ScriptableObject
{
    [Header("Class")]
    public PlayerClass PlayerClass;

    [Header("GameObject")]
    public GameObject prefab;

    [Header("DefaultWeapon")]
    public ShotableData RangedWeapon1;
    public ShotableData RangedWeapon2;
    public MeleeWeaponData MeleeWeaponData;
    public ThrowableWeaponData[] ThrowableWeaponDatas;
    public int CountExplode, CountSmok, CountFlash;

    [Header("Property")]
    public int Health;
    public int Armor;
    public int SpeedMove;
    public int BufferWeapon;
    public float ReloadTime;

}

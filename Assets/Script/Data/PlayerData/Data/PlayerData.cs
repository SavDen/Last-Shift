using FishNet.Object;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerProfile/Player")]
public class PlayerData : ScriptableObject
{
    public int ID;
    
    [Header("Class")]
    public PlayerClass PlayerClass;

    [Header("Prefab")]
    public NetworkObject PlayerPrefab;
    public NetworkObject LobbyPlayerPrefab;

    [Header("View")] 
    public Mesh BodySkin;
    public Mesh HairSkin;
    public Mesh FaceSkin;
    public Material MaterialSkin;
    

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

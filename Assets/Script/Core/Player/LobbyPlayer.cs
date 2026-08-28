using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Zenject;

public class LobbyPlayer : NetworkBehaviour
{
    [Header("View")] [SerializeField] private SkinnedMeshRenderer _bodySkin;
    [SerializeField] private MeshFilter _hairSkin, _faceSkin;
    [SerializeField] private Animator _animator;
    [SerializeField] private TwoBoneIKConstraint[] handsPos;//0 -left, 1- right
    [SerializeField] private RigBuilder rig;


    [Header("WeaponPos")] 
    [SerializeField] private Transform mainWPos;

    private readonly SyncVar<int> _classId = new(-1);
    private PlayerData _playerData;
    private PlayerClassCatalog _playerClassCatalog;
    
    public void SetClass(PlayerClassCatalog playerClassCatalog)
    {
        _playerClassCatalog = playerClassCatalog;
    }
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        _classId.OnChange += OnClassChanged;

        if (IsOwner)
        {
            LoadDataLobbyPlayer();
        }
    }

    private void OnClassChanged(int prev, int next, bool asServer)
    {
        InitPlayerLobby();
    }
    
    
    private void LoadDataLobbyPlayer()
    {
        int id = 0;
        if (PlayerPrefs.HasKey("PlayerClass"))
        {
            id = PlayerPrefs.GetInt("PlayerClass");
            _playerData = _playerClassCatalog.GetPlayerData(id);
        }
        else
        {
            _playerData = _playerClassCatalog.GetPlayerData(0);
            PlayerPrefs.SetInt("PlayerClass", id);
        }
        
        SetClassIDRPC(id);
    }

    public void SetClassID(int id)
    {
        _classId.Value = id;
        SetClassPlayerDataRPC(id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetClassPlayerDataRPC(int id)
    {
        GameSessionState.instance.SetClass(conn:ClientManager.Connection, _playerClassCatalog.GetPlayerData(id));
    }

    [ServerRpc]
    public void SetClassIDRPC(int id)
    {
        _classId.Value = id;
    }
    
    private void InitPlayerLobby()
    {
        _playerData = _playerClassCatalog.GetPlayerData(_classId.Value);

        
        _bodySkin.sharedMesh = _playerData.BodySkin;
        _bodySkin.sharedMaterial = _playerData.MaterialSkin;

        _hairSkin.sharedMesh = _playerData.HairSkin;
        _faceSkin.sharedMesh = _playerData.FaceSkin;


        foreach (Transform weaponOld in mainWPos)
        {
            Destroy(weaponOld.gameObject);
        }
        
        var weapon = Instantiate(_playerData.RangedWeapon1.prefab, mainWPos);
        
        UpdPosRigHandWeapon(weapon.GetComponent<IShootableWeapon>());

    }
    
    
    private void UpdPosRigHandWeapon(IShootableWeapon weapon)
    {
        handsPos[0].data.target = weapon.HandsPos[0];
        handsPos[1].data.target = weapon.HandsPos[1];

        UpdateRig();
    }

    public void UpdateRig()
    {
        rig.enabled = false;

        foreach (var rig in rig.layers)
        {
            rig.Update();
        }

        rig.enabled = true;
    }
    
}
using System;
using System.Collections;
using Cinemachine;
using FishNet;
using FishNet.Component.Transforming;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : EntityBase
{
    public PlayerData PlayerData;

    [Header ("MVC")]
    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private PlayerView playerView;
    [SerializeField] private WeaponController weaponController;

    [Header("Render")]
    [SerializeField] private LineRenderer lineRenderer;

    // [Header("Camera")]
    // [SerializeField] private Camera _playerMainCamera;
    // [SerializeField] private CinemachineVirtualCamera  _playerCinemachineCamera;
    
    private Coroutine _reloadCorutine;

    private string _inputDevice;
    private Vector2 _moveInput;
    private Vector2 _lookInput;

    private float _nextMeleeAttack;

    private bool _isShooting;
    private bool _isReloading;
    private bool _isChange;
    private bool _isMeleeAttack;

    public bool IsDead() => playerModel.IsDead();

    #region InputSystem
    // Input System events
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        _isShooting = context.performed;
    }

    
    public void OnMeleeAttack(InputAction.CallbackContext context)
    {
        if(context.started && Time.time >= _nextMeleeAttack + PlayerData.MeleeWeaponData.coolDown && !_isChange)
        {
            MeleeAttackObserverRpc();
        }

    }
    
    [ObserversRpc]
    private void MeleeAttackObserverRpc()
    {
        float timeAnim = PlayerData.MeleeWeaponData.reloadTime / PlayerData.MeleeWeaponData.SpeedAnim;
        MaleeAttack(PlayerData.MeleeWeaponData.SpeedAnim ,timeAnim);
        _nextMeleeAttack = Time.time + timeAnim;
    }

    public void OnGrenade(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            weaponController.ActiveRender(true);              
        }

        if(context.canceled)
        {
            weaponController.Throwable(playerView.GrenagePos, playerView.MinePos);
            weaponController.ActiveRender(false);
        }
    }

    public void OnChangeGrenade(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            weaponController.ChageGrenade();
        }
    }

    public void OnChangeWeapon(InputAction.CallbackContext context)
    {
        if (context.started && !_isMeleeAttack && !_isChange)
        {
            ChangeWeaponServerRpc();
        }
    }
    
    [ServerRpc]
    private void ChangeWeaponServerRpc()
    {
        _isChange = true;
        if(_isReloading)
        {
            StopReload();
        }

        weaponController.ChangeWeapon();
        ChangeWeaponObserverRpc();
    }

    [ObserversRpc]
    private void ChangeWeaponObserverRpc()
    {
        StartCoroutine(playerView.ChangeWeaponView((IsChanged) =>
        {
            _isChange = IsChanged;
        }));
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.started && !_isReloading && !_isChange && !_isMeleeAttack)
        {
            ReloadServerRpc();
        }
    }

    [ServerRpc]
    private void ReloadServerRpc()
    {
        _reloadCorutine = StartCoroutine(Reload());
    }

    public void OnChangeScheme(PlayerInput playerInput)
    {
        _inputDevice = playerInput.currentControlScheme;
        Debug.Log($"{_inputDevice}");
        
    }
    #endregion

    //public void InitPlayer(PlayerData playerData)
    //{

    //}
    //Init
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if (IsOwner)
        {
            var camera= FindFirstObjectByType<CinemachineVirtualCamera>();

            if (camera != null)
            {
                camera.Follow = transform;
                camera.LookAt = transform;   
            }
        }
        else
        {
            GetComponent<PlayerInput>().enabled = false;
            GetComponent<FlashVolumeEffect>().enabled = false;
            GetComponent<PlayerController>().enabled = false;   
        }
    }

    private void Awake()
    {
        //Cursor.visible = false;
        playerModel.InitModel(PlayerData);

       playerView.SpawnWeapon(PlayerData.RangedWeapon1.prefab, PlayerData.RangedWeapon2.prefab, PlayerData.MeleeWeaponData.prefab);

        weaponController.InitWeapon(playerView.MainSlotView, PlayerData.RangedWeapon1,
            playerView.AdditionalSlotView, PlayerData.RangedWeapon2,            
            playerView.MeleeWeaponView,
            PlayerData.MeleeWeaponData,
            PlayerData.ThrowableWeaponDatas,
            PlayerData.CountExplode,
            PlayerData.CountSmok,
            PlayerData.CountFlash,
            lineRenderer,
            Owner);
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        
        Move();

        Turn();
        
        ShootWeapon();

        weaponController.ShowTrRender(transform);
    }

   
    [ServerRpc]
    private void ShootWeapon()
    {
        if (_isShooting && !_isMeleeAttack && !_isChange && !_isReloading)
        {
            ShootServerRpc();
        }

        else
        {
            StopShootServerRpc();
        }
    }

    [ObserversRpc]
    private void ShootServerRpc()
    {
        
        print("получен на сервере Shoot");
        weaponController.Shoot();
    }

    [ObserversRpc]
    private void StopShootServerRpc()
    {
        print("получен на сервере Stop");
        weaponController.StopShoot();
    }

    private void Turn()
    {
        if (_lookInput != Vector2.zero)
        {
            playerModel.Rotate(_lookInput, _inputDevice);
        }
    }

    private void Move()
    {
        playerModel.Move(_moveInput);
        playerView.AnimMove(playerModel.VelocityX, playerModel.VelocityY);
    }

    private void MaleeAttack(float speedAnim,float timeAnim)
    {
        _isMeleeAttack = true;

        if(_isReloading)
        {
            StopReload();
        }

        StartCoroutine(playerView.MeleeAttackCorutine((IsMelleAttackState)=>
        {
             _isMeleeAttack = IsMelleAttackState;
        },
        speedAnim, timeAnim));
        weaponController.MelleAttack();
    }
    
    private IEnumerator Reload()
    {

        _isReloading = true;

        AnimReloadObserverRpc(true);
        yield return new WaitForSeconds(weaponController.ReloadTimeWeapon - playerModel.ReloadTime);
        AnimReloadObserverRpc(false);
        weaponController.Reload();

        _isReloading = false;

    }

    [ObserversRpc]
    private void AnimReloadObserverRpc(bool isReload)
    {
        playerView.AnimReload(isReload);
    }

    [ObserversRpc]
    private void StopReload()
    {
        StopCoroutine(_reloadCorutine);
        playerView.AnimReload(false);
        _isReloading = false;
        _reloadCorutine = null;
    }

    public override void TakeDamage(float damage, TypeDamage typeDamage)
    {
        playerModel.TakeDamage(damage);

        EffectDamage(typeDamage);
    }

    [ObserversRpc]
    private void EffectDamage(TypeDamage typeDamage)
    {
        switch (typeDamage)
        {
            case TypeDamage.Fire:
                playerView.FireDamage();
                break;

            case TypeDamage.Blood:
                playerView.BloodDamage();
                break;

        }
    }

    public override void FlashEffect(float duration)
    {
        playerView.FlashEffect(duration);
    }
}

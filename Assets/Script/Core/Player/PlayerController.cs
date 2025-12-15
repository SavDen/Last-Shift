using System;
using System.Collections;
using Cinemachine;
using FishNet;
using FishNet.Component.Transforming;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
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
    private bool _localShoot = false;

    private float _nextMeleeAttack;


    private readonly SyncVar<bool> _isShooting = new();
    private readonly SyncVar<bool> _isReloading = new();
    private readonly SyncVar<bool> _isChange = new();
    private readonly SyncVar<bool> _isMeleeAttack = new();

    private readonly SyncVar<bool> _isStopedShoot = new();

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
        if (context.performed)
        {
            SetShootServerRpc(context.performed);
        }
        
    }

    [ServerRpc]
    private void SetShootServerRpc(bool contextPerformed)
    {
        _isShooting.Value = contextPerformed;
    }


    public void OnMeleeAttack(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            MeleeAttackServerRPC();
        }

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
        if (context.started)
        {
            ChangeWeaponServerRpc();
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ReloadServerRpc();
        }
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
        //print($"{_isShooting} , {_isChange} , {_isMeleeAttack} , {_isReloading}");
        
        if (!IsOwner)
        {
            return;
        }
        
        Move();

        Turn();
        
        ShootWeapon();

        weaponController.ShowTrRender(transform);
    }
    
    //Network Transform Sync
    private void Move()
    {
        playerModel.Move(_moveInput);
        playerView.AnimMove(playerModel.VelocityX, playerModel.VelocityY);
    }
    
    //Network Transform Sync
    private void Turn()
    {
        if (_lookInput != Vector2.zero)
        {
            playerModel.Rotate(_lookInput, _inputDevice);
        }
    }

    
    private void ShootWeapon()
    {
        if (_isShooting.Value && !_isMeleeAttack.Value && !_isChange.Value && !_isReloading.Value)
        {
            ShootServerRpc(true);
            _isStopedShoot.Value = false;
        }

        else
        {
            if (!_isStopedShoot.Value)
            {
                ShootServerRpc(false);
                _isStopedShoot.Value = true;
            }
        }
    }
    
    [ServerRpc]
    private void ShootServerRpc(bool state)
    {
        if (state)
        {
            weaponController.Shoot();
        
            ShootEffectViewObserverRpc(true);
        }

        else
        {
            ShootEffectViewObserverRpc(false);
        }
    }
    
    [ObserversRpc]
    private void ShootEffectViewObserverRpc(bool state)
    {
        if (state)
        {
            weaponController.StartShootParticle();    
        }

        else
        {
            //print("Stop Shoot Particle");
            weaponController.StopShootParticle();
        }
        
    }
    
    
    [ServerRpc]
    private void MeleeAttackServerRPC()
    {
        MelleAttackInternal();
    }

    private void MelleAttackInternal()
    {
        if (Time.time >= _nextMeleeAttack + PlayerData.MeleeWeaponData.coolDown && !_isChange.Value)
        {
            float timeAnim = PlayerData.MeleeWeaponData.reloadTime / PlayerData.MeleeWeaponData.SpeedAnim;
            MeleeAttack(PlayerData.MeleeWeaponData.SpeedAnim ,timeAnim);
            StartCoroutine(TimerMeleeAttack(timeAnim));
            _nextMeleeAttack = Time.time + timeAnim;
        }
    }

    private IEnumerator TimerMeleeAttack(float timeAnim)
    {
        yield return new WaitForSeconds(timeAnim);
        _isMeleeAttack.Value = false;

    }
    private void MeleeAttack(float speedAnim,float timeAnim)
    {
        _isMeleeAttack.Value = true;

        if(_isReloading.Value)
        {
            StopReload();
        }
        
        weaponController.MelleAttack();
        MeleeAttackObserverRpc(speedAnim, timeAnim);
    }


    [ObserversRpc]
    private void MeleeAttackObserverRpc(float speedAnim, float timeAnim)
    {
        StartCoroutine(playerView.MeleeAttackCorutine(speedAnim, timeAnim));
    }
    
    [ServerRpc]
    private void ChangeWeaponServerRpc()
    {
        print("Change enter");
        ChangeWeaponInternal();
    }

    private void ChangeWeaponInternal()
    {
        if (!_isMeleeAttack.Value && !_isChange.Value)
        {
            _isChange.Value = true;
        
            ShootEffectViewObserverRpc(false);
        
            if(_isReloading.Value)
            {
                StopReload();
            }
        
            weaponController.ChangeWeapon();
            ChangeWeaponObserverRpc();   
        }
    }

    [ObserversRpc]
    private void ChangeWeaponObserverRpc()
    {
        StartCoroutine(playerView.ChangeWeaponView((IsChanged) =>
        {
            _isChange.Value = IsChanged;
            // тут да, пока не решил как отлседить окончание перезарядки, скорее всего так же через корутину с фиксировнным временям
        }));
    }
    
    [ServerRpc]
    private void ReloadServerRpc()
    {
        ReloadInternal(); 
    }

    private void ReloadInternal()
    {
        if (!_isReloading.Value && !_isChange.Value && !_isMeleeAttack.Value)
        {
            _reloadCorutine = StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        _isReloading.Value = true;

        AnimReloadObserverRpc(true);
        yield return new WaitForSeconds(weaponController.ReloadTimeWeapon - playerModel.ReloadTime);
        AnimReloadObserverRpc(false);
        weaponController.Reload();

        _isReloading.Value = false;

    }
    
    private void StopReload()
    {
        if (_isReloading.Value)
        {
            //print("Enter Reload?");
            AnimReloadObserverRpc(false);
            StopCoroutine(_reloadCorutine);
             _isReloading.Value = false;
        }
    }
    
    [ObserversRpc]
    private void AnimReloadObserverRpc(bool isReload)
    {
        playerView.AnimReload(isReload);
    }

    [ServerRpc]
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

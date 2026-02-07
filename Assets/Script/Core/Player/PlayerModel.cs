using System;
using Unity.Cinemachine;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerModel
{
    [SerializeField] private CharacterController characterController;

    [SerializeField] private PlayerUIHub _playerUIHub;

    [SerializeField] private LayerMask layserMask;
    private PlayerData _playerData;
    
    private Camera _camera;

    private float _health;
    private float _armor;
    private float _speedMove;
    private float _reloadTime;

    private float velocityX;
    private float velocityY;

    public float Health => _health;
    public float Armor => _armor;
    public float SpeedMove => _speedMove;
    public float ReloadTime => _reloadTime;

    public float VelocityX => velocityX;
    public float VelocityY => velocityY;
    
    public bool IsDead() => _health <= 0;



    public void InitModel(PlayerData data)
    {
        _playerData = data;
        _health = data.Health;
        _armor = data.Armor;
        _speedMove = data.SpeedMove;
        _reloadTime = data.ReloadTime;
        //_playerUIHub.Init(_health, _armor);
        
    }

    #region Motor
    public void Move(Vector2 direction)
    {
        if(characterController)
        {
            characterController.SimpleMove(new Vector3(direction.x, 0, direction.y) * _speedMove);
            CalculateVelocity();
        }
    }

    private void CalculateVelocity()
    {
        Vector3 globalVelocity = new Vector3(
            characterController.velocity.x,
            0,
            characterController.velocity.z
        );

        Vector3 localVelocity = characterController.transform.InverseTransformDirection(globalVelocity);

        velocityY = localVelocity.z;
        velocityX = localVelocity.x;

    }

    public void Rotate(Vector2 inputRotation, string device)
    {
        if (!characterController) return;

        
        switch(device)
        {
            case "Gamepad":
                RotateTowardStick(inputRotation);
                break;

            case "Keyboard":
                //Debug.Log("Keyboard");
                RotateTowardMouse(inputRotation);
                break;
            
            default:
                RotateTowardMouse(inputRotation);
                break;
        }
    }

    private void RotateTowardStick(Vector2 inputRotation)
    {
        Vector3 lookDirection = new Vector3(inputRotation.x, 0, inputRotation.y);
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            characterController.transform.rotation = Quaternion.Slerp(
                characterController.transform.rotation,
                targetRotation,
                5 * Time.deltaTime
            );
        }
    }

    private void RotateTowardMouse(Vector2 inputRotation)
    {
        Ray ray = Camera.main.ScreenPointToRay(inputRotation);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layserMask))
        {
            Vector3 direction = hit.point - characterController.transform.position;
            direction.y = 0;
            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                characterController.transform.rotation = Quaternion.Slerp(
                    characterController.transform.rotation,
                    targetRotation,
                    Time.deltaTime * 15f
                );
            }
        }
    }
    #endregion

    public void TakeDamage(float damage)
    {
        if(_armor !=0)
        {
            _armor -= damage;
        }

        else if (_armor - damage <= 0)
        {
            var armor = _armor;
            _armor -= damage;
            _health -= damage - armor;
        }

        else
        {
            _health -= damage;
        }

        //_playerUIHub.UpdateSliders(_health, _armor);

        // Debug.Log($"{_health}");
    }
}

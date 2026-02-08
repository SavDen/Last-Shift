using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Component.Animating;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

[System.Serializable]
public class PlayerView: ICombinedDamageable
{
    [SerializeField] private FlashVolumeEffect flashEffect;
    [SerializeField] private Animator animator;
    [SerializeField] private RigBuilder rig;
    [SerializeField] private TwoBoneIKConstraint[] handsPos; //0 -left, 1- right
    [SerializeField] private Transform mainWPos;
    [SerializeField] private Transform additionalWPos;
    [SerializeField] private Transform grenadePos;
    [SerializeField] private Transform meleePos;
    [SerializeField] private Transform minePos;
    [SerializeField] private Transform reloadPos;

    [SerializeField] private List<ParticleSystem> bloodEffect;
    [SerializeField] private ParticleSystem fireEffect;

    private MeleeWeapon _meleeWeapon;
    private IShootableWeapon _mainSlot, _additionalSlot;

    public IShootableWeapon MainSlotView => _mainSlot;
    public IShootableWeapon AdditionalSlotView => _additionalSlot;
    public MeleeWeapon MeleeWeaponView => _meleeWeapon;

    public Transform GrenagePos => grenadePos;
    public Transform MinePos => minePos;


    public void SpawnWeapon(GameObject mainSlot, GameObject additional, GameObject meleeWeaponData)
    {
        _mainSlot = Object.Instantiate(mainSlot, mainWPos).GetComponent<IShootableWeapon>();

        _additionalSlot = Object.Instantiate(additional, additionalWPos).GetComponent<IShootableWeapon>();

        _meleeWeapon = Object.Instantiate(meleeWeaponData, meleePos).GetComponent<MeleeWeapon>();
        _meleeWeapon.gameObject.SetActive(false);

        UpdPosRigHandWeapon();
    }

    private void UpdPosRigHandWeapon()
    {
        handsPos[0].data.target = mainWPos.GetComponentInChildren<IShootableWeapon>().HandsPos[0];
        handsPos[1].data.target = mainWPos.GetComponentInChildren<IShootableWeapon>().HandsPos[1];

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
    
    public IEnumerator ChangeWeaponView(Action<bool> IsChanged)
    {
        RigEnable(false);
        HardViewWeaponHand(true);
        animator.SetTrigger("ChangeWeapon");
        yield return new WaitForSeconds(0.5f);
        NewParentTransform();
        yield return new WaitForSeconds(0.5f);
        HardViewWeaponHand(false);
        UpdPosRigHandWeapon();
        IsChanged?.Invoke(false);
    }

    private void NewParentTransform()
    {
        var transformObj = reloadPos.GetChild(0);
        transformObj.SetParent(additionalWPos);
        transformObj.localPosition = Vector3.zero;
        transformObj.localRotation = Quaternion.identity;

        var transformObj2 = additionalWPos.GetChild(0);
        transformObj2.SetParent(reloadPos);
        transformObj2.localPosition = Vector3.zero;
        transformObj2.localRotation = Quaternion.identity;
    }

    public void AnimMove(float x, float y)
    {
        animator.SetFloat("x", x);
        animator.SetFloat("y", y);
    }

    private void AnimDamage()
    {
        animator.SetTrigger("Hit");
    }

    public void FireDamage()
    {
        AnimDamage();
        if(!fireEffect.isPlaying)
        {
            fireEffect.Play();
        }
    }

    public void BloodDamage()
    {
        AnimDamage();
        bloodEffect[Random.Range(0, bloodEffect.Count)].Play();
    }

    public IEnumerator MeleeAttackCorutine(float speedAnim, float timeAnim)
    {
        _meleeWeapon.gameObject.SetActive(true);
        animator.SetFloat("MeleeSpeed", speedAnim);
        animator.SetTrigger("MeleeAttack");
        RigEnable(false);
        mainWPos.gameObject.SetActive(false);
        yield return null;
        yield return new WaitForSeconds(timeAnim + 0.2f);
        RigEnable(true);
        mainWPos.gameObject.SetActive(true);
        _meleeWeapon.gameObject.SetActive(false);
    }

    public void AnimReload(bool state)
    {
        if(state)
        {
            RigEnable(false);
            HardViewWeaponHand(true);
            animator.SetBool("Reload", true);
        }
        else
        {
            animator.SetBool("Reload", false);
            HardViewWeaponHand(false);
            RigEnable(true);
        }

    }

    private void HardViewWeaponHand(bool state)
    {
        if (state)
        {
            var transformObj = mainWPos.GetChild(0);
            transformObj.SetParent(reloadPos);
            transformObj.localPosition = Vector3.zero;
            transformObj.localRotation = Quaternion.identity;
        }

        else
        {
            var transformObj = reloadPos.GetChild(0);
            transformObj.SetParent(mainWPos);
            transformObj.localPosition = Vector3.zero;
            transformObj.localRotation = Quaternion.identity;
        }
    }

    public void RigEnable(bool state)
    {
        rig.enabled = state;
    }

    public void FlashEffect(float duration)
    {
        flashEffect.StartEffect(duration);
    }
}

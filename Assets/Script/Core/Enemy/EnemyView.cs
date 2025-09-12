using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class EnemyView : ICombinedDamageable
{
    [SerializeField] private List<ParticleSystem> bloodEffect;
    [SerializeField] private ParticleSystem fireEffect;
    [SerializeField] private Transform _rHand;

    private Animator _animator;
    private CapsuleCollider _capsuleCollider;
    private List<Rigidbody> _ragdollRig = new List<Rigidbody>();
    private List<Collider> _ragdollColl = new List<Collider>();


    public float AnimAttackLenght => _animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
    public Transform HandPos => _rHand;

    public void Init(Animator animator, List<Rigidbody> ragdollRig, List<Collider> ragdollColl, CapsuleCollider capsuleCollider)
    {
        _animator = animator;

        _ragdollRig = ragdollRig;
        _ragdollColl = ragdollColl;

        _capsuleCollider = capsuleCollider;

        EnableRagdoll(false);
    }

    public void EnableRagdoll(bool state)
    {
        for (int i = 0; i < _ragdollRig.Count; i++)
        {
            _ragdollRig[i].isKinematic = !state;
            _ragdollRig[i].useGravity = state;
        }

        for (int i = 0; i < _ragdollColl.Count; i++)
        {
            _ragdollColl[i].enabled = state;
        }

        _animator.enabled = !state;
        _capsuleCollider.enabled = !state;
    }

    internal void StateRun(bool state)
    {
        _animator.SetBool("Run", state);
    }

    public void Dead()
    {
        EnableRagdoll(true);
        AddForceRagdollDead();
    }

    public void FireDamage()
    {
        if (!fireEffect.isPlaying)
        {
            fireEffect.Play();
        }
    }

    public void AnimAttack()
    {
        _animator.SetInteger("RandomAttack", Random.Range(1, 2));
        _animator.SetTrigger("Attack");
    }

    public void BloodDamage()
    {
        bloodEffect[Random.Range(0, bloodEffect.Count)].Play();
    }

    public void AddForceRagdollDead()
    {
        _ragdollRig[Random.Range(0, _ragdollRig.Count)].AddForce(Vector3.one * 50, ForceMode.Impulse);
    }

    public void DisableRagdool()
    {
        for (int i = 0; i < _ragdollRig.Count; i++)
        {
            _ragdollRig[i].isKinematic = true;
            _ragdollRig[i].useGravity = false;
        }

        for (int i = 0; i < _ragdollColl.Count; i++)
        {
            _ragdollColl[i].enabled = false;
        }

        _capsuleCollider.enabled = false;
    }

    public IEnumerator FlashEffect(Action<bool> complete,  float duration)
    {
        _animator.SetBool("Flash", true);
        yield return new WaitForSeconds(duration);
        _animator.SetBool("Flash", false);
        complete?.Invoke(true);

    }
}

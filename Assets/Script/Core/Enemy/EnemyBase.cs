using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyBase : EntityBase, ISmokeDamageable
{
    [SerializeField] private EnemyView _enemyView;
    [SerializeField] private LayerMask layerMask;

    private EnemyData _enemyData;
    private NavMeshAgent _navMeshAgent;

    private Transform _target;

    private Coroutine _followCorutine;
    private Coroutine _smokeCorutine;
    private Coroutine _flashCorutine;

    private int _selectTarget;

    private readonly SyncVar<float> _health = new ();

    private bool _follow;
    private readonly SyncVar<bool> _isDead = new();
    
    public bool IsDead() => _isDead.Value;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        
        _enemyView.Init(GetComponent<Animator>(),
            GetComponentsInChildren<Rigidbody>().ToList(),
            GetComponentsInChildren<Collider>().ToList(),
            GetComponent<CapsuleCollider>());
    }

    public void Initialized(EnemyData enemyData)
    {
        _enemyData = enemyData;
        
        _health.Value = _enemyData.Health;
        
        _navMeshAgent.speed = Random.Range(_enemyData.Speed - 0.5f, _enemyData.Speed + 1);
        _navMeshAgent.stoppingDistance = _enemyData.StopDis;
        _navMeshAgent.radius = 0.3f;
        _navMeshAgent.avoidancePriority = Random.Range(_enemyData.MinPriority, _enemyData.MaxPriority);

        _follow = true;
    }
    
    public void GetTarget(Transform target)
    {
        if (_smokeCorutine == null)
        {
            _target = target;    
        }
        
        if (!_navMeshAgent.hasPath)
        {
            StartCoroutine(Follow());
        }
    }

    private IEnumerator Follow()
    {
        while(_follow && !_isDead.Value && _target != null)
        {
            
            _navMeshAgent.SetDestination(_target.position);

            _enemyView.StateRun(_navMeshAgent.velocity.magnitude > 0.5f);

            if (Vector3.Distance(_target.position, transform.position) <= _navMeshAgent.stoppingDistance)
            {
                _navMeshAgent.velocity = Vector3.zero;
                _follow = false;
                print("StartAttack");
                StartCoroutine(AttackCorutine());
            }

            yield return new WaitForSeconds(0.5f);
        }

    }


    private IEnumerator AttackCorutine()
    {
        while(!_follow && !_isDead.Value)
        {
            _enemyView.AnimAttack();
            _navMeshAgent.updateRotation = true;
            yield return new WaitForSeconds(_enemyView.AnimAttackLenght + 1f);

            if(Vector3.Distance(_target.position, transform.position) > _navMeshAgent.stoppingDistance)
            {
                _follow = true;
                print("EndAttack");
                StartCoroutine(Follow());
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public override void TakeDamage(float damage, TypeDamage typeDamage)
    {
        _health.Value -= damage;

        EffectDamage(typeDamage);

        if (_health.Value <= 0)
        {
            _follow = false;
            _isDead.Value = true;
            StopAllCoroutines();
            Dead();
        }

    }

    public virtual void Attack()
    {
        var collisionHit = Physics.OverlapSphere(transform.position, 1f, layerMask);

        for (int i = 0; i < collisionHit.Length; i++)
        {
            if(collisionHit[i].TryGetComponent(out IDamage damage))
            {
                damage.TakeDamage(_enemyData.Damage, TypeDamage.Blood);
            }
        }
    }


    [ObserversRpc]
    public virtual void EffectDamage(TypeDamage typeDamage)
    {
        switch (typeDamage)
        {
            case TypeDamage.Fire:
                _enemyView.FireDamage();
                break;

            case TypeDamage.Blood:
                _enemyView.BloodDamage();
                break;

        }
    }

    [ObserversRpc]
    private void Dead()
    {
        _navMeshAgent.enabled = false;
        _enemyView.Dead();

        StartCoroutine(DisableRagdool());
    }

    private IEnumerator DisableRagdool()
    {
        yield return new WaitForSeconds(5f);
        _enemyView.DisableRagdool();
    }

    public void SmokeEffect()
    {
        if(_smokeCorutine == null)
        _smokeCorutine = StartCoroutine(SmokeEffectCorutine());
    }

    private IEnumerator SmokeEffectCorutine()
    {
        _follow = false;
        var originTarget = _target;

        var tempTarget = new GameObject("TempTarget");
        tempTarget.transform.position = GetRandomTarget();
        _target = tempTarget.transform;
        _follow = true;

        yield return new WaitForSeconds(Random.Range(10, 15));

        _follow = false;
        _target = originTarget;
        _follow = true;
        Destroy(tempTarget, 1);
        _smokeCorutine = null;
    }

    private Vector3 GetRandomTarget()
    {
        for(int i =0; i<10; i++)
        {
            var randomPointXZ = Random.insideUnitCircle * 8;
            var randomPoint = transform.position + new Vector3(randomPointXZ.x, 0, randomPointXZ.y);

            NavMeshHit hit;

            if(NavMesh.SamplePosition(randomPoint, out hit, 100, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        print("Non pos");
        return transform.position;
    }

    public override void FlashEffect(float duration)
    {
        if(_flashCorutine == null)
        {
            bool _complte = false;

            StopCoroutine(Follow());

            _navMeshAgent.isStopped = true;

            StartCoroutine(_enemyView.FlashEffect((complete) =>
            {
                _complte = complete;
                if(_complte)
                {
                    _navMeshAgent.isStopped = false;
                    StartCoroutine(Follow());
                    _flashCorutine = null;
                }

            }, duration));
        }

    }
}

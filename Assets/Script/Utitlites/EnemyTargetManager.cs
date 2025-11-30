using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public class EnemyTargetManager : NetworkBehaviour
{
   [SerializeField] private List<PlayerController> _targets = new();
   [SerializeField] private  List<EnemyBase> _enemys = new ();

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(UpdateTargetsCorutine());

        InstanceFinder.ServerManager.OnRemoteConnectionState += RemovePlayer;
    }

    private void RemovePlayer(NetworkConnection arg1, RemoteConnectionStateArgs arg2)
    {
        if (arg2.ConnectionState == RemoteConnectionState.Stopped)
        {
            StopAllCoroutines();
            _targets.RemoveAt(_targets.IndexOf(arg1.FirstObject.GetComponent<PlayerController>()));
            StartCoroutine(UpdateTargetsCorutine());
        }
    }

    [Server] 
    public void RegisterTarget(PlayerController target)
    {
        if (!_targets.Contains(target))
            _targets.Add(target);
    }
    
    [Server]
    public void RegisterEnemy(EnemyBase enemy)
    {
        if (!_enemys.Contains(enemy))
            _enemys.Add(enemy);
    }

    [Server]
    private IEnumerator UpdateTargetsCorutine()
    {
        yield return new WaitForSeconds(0.5f); //delay for init first enemy
        
        while (true)
        {
            UpdateTargets();
            yield return new WaitForSeconds(0.3f);
            ClearDataManager();
        }
    }

    private void ClearDataManager()
    {
        for (int i = _enemys.Count - 1; i >= 0; i--)
        {
            if(_enemys[i].IsDead())
                _enemys.RemoveAt(i);
        }
        
        for (int i = _targets.Count - 1; i >= 0; i--)
        {
            if(_targets[i].IsDead())
                _targets.RemoveAt(i);
        }
    }

    [Server]
    private void UpdateTargets()
    {
        if (_targets.Count != 0 && _enemys.Count != 0)
        {
            for (int i = 0; i < _enemys.Count; i++)
            {
                if (!_enemys[i].IsDead())
                {
                    float minDis = Single.MaxValue;
                    int targetIndex = 0;
                    for (int j = 0; j < _targets.Count; j++)
                    {
                        if (!_targets[j].IsDead())
                        {
                            float currentDis = (_enemys[i].transform.position -
                                                _targets[j].transform.position).sqrMagnitude;

                            if (currentDis < minDis)
                            {
                                minDis = currentDis;
                                targetIndex = j;
                            }

                        }
                    }

                    if (!_targets[targetIndex].IsDead())
                    {
                        _enemys[i].GetTarget(_targets[targetIndex].transform);   
                    }
                }
            }
                
        }
    }
}

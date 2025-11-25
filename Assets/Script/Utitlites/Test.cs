using System;
using UnityEngine;
using Zenject;

    public class Test: MonoBehaviour
    {
        [Inject] EnemyTargetManager  _targetManager;

        private void Start()
        {
            _targetManager.RegisterEnemy(null);
            print("start test");
        }
    }

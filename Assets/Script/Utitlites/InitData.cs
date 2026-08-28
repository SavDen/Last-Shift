using UnityEngine;
using Zenject;

public class InitData: MonoInstaller
{
        [SerializeField] private PlayerClassCatalog _playerDataCatalog;
        [SerializeField] private WeaponCatalog _weaponDataCatalog;
        public override void InstallBindings()
        {
                BindPlayerDataContainer();
                BindWeaponData();
        }

        private void BindPlayerDataContainer()
        {
                Container.BindInstance(_playerDataCatalog).AsSingle();
        }

        private void BindWeaponData()
        {
                Container.BindInstance(_weaponDataCatalog).AsSingle();
        }
        
}
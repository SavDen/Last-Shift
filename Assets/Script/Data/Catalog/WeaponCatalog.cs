using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Catalogs/Weapons")]
public class WeaponCatalog : ScriptableObject
{
        public List<WeaponData> Catalog = new();

        public WeaponData GetWeaponData(int id)
        {
                return Catalog.Find(weapon => weapon.ID == id);
        }
        
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Catalogs/PlayerClass")]
public class PlayerClassCatalog: ScriptableObject
{
        public List<PlayerData> Catalog = new();

        public PlayerData GetPlayerData(int id = 1)
        {
                return Catalog.Find(classPlayer => classPlayer.ID == id);
        }
        
}
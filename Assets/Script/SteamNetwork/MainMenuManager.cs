using System;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
   // public static  MainMenuManager instance;
   //
   // private void Awake()
   // {
   //    instance = this;
   // }

   public void CreateLobby()
   {
      SteamLobbyManager.CreateLobby();
   }
}

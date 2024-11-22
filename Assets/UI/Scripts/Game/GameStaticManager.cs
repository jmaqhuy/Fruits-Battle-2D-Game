using System.Collections.Generic;
using UI.Scripts.Game.Multiplayer;
using UnityEngine;

namespace UI.Scripts.Game
{
    public class GameStaticManager
    {
        public static string LocalPlayerID { get; set; }
        public static Client NetworkManager { get; set; }
        public static Dictionary<string, GameObject> Players { get; set; }
        
        public static void InitializeGameManager(int port, string game, LoginManage loginManage, SignUpManage signUpManage)
        {
            Debug.Log("Starting game manager.");

            LocalPlayerID = "";
            NetworkManager = new Client(port, game, loginManage, signUpManage);
            Players = new Dictionary<string, GameObject>();
        }
    }
    
}
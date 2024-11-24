using System;
using System.Collections.Generic;
using NetworkThread.Multiplayer;
using UnityEngine;

namespace NetworkThread
{
    public static class NetworkStaticManager
    {
        public static Client ClientHandle { get; set; }
        
        public static void InitializeGameManager()
        {
            
            try
            {
                Debug.Log("Starting ClientHandle.");
                ClientHandle = new Client();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in Create new Client(): {e.Message}");
            }
        }
    }
}
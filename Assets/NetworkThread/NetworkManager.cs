using UnityEngine;
using System.Collections;
using EasyUI.Progress;

namespace NetworkThread
{
    public class NetworkManager : MonoBehaviour
    {
        private static NetworkManager _instance;

        private bool IsConnecting = false;
        
        private void Awake()
        {
            if (_instance != null)
            {   
                Debug.Log("NetworkManager instance already exists!");
                Destroy(gameObject);
                return;
            }
            
            Debug.Log("Instantiating NetworkManager!");
            _instance = this;
            DontDestroyOnLoad(gameObject);
            NetworkStaticManager.InitializeGameManager();
        }

        void Update()
        {
            if (!NetworkStaticManager.ClientHandle.IsConnected() && !IsConnecting)
            {
                IsConnecting = true;
                StartCoroutine(DiscoveryServer());
            }
            
            else if (NetworkStaticManager.ClientHandle.IsConnected() && IsConnecting)
            {
                IsConnecting = false;
                Progress.Hide();
            }
        }

        private IEnumerator DiscoveryServer()
        {
            int time = 1;
            while (!NetworkStaticManager.ClientHandle.IsConnected())
            {
                Progress.Show("Connecting to server ...", ProgressColor.Red);
                Debug.Log($"Starting discovery: {time}");
                NetworkStaticManager.ClientHandle.DiscoveryServer();
                yield return new WaitForSeconds(3f);
                time++;
            }
        }

        void OnApplicationQuit()
        {
            Debug.Log("Application is quitting. Sending disconnect to server.");
    
            if (NetworkStaticManager.ClientHandle != null)
            {
                NetworkStaticManager.ClientHandle.SendDisconnect();
            }
            Debug.Log("Game manager quit successfully.");
        }
    }
}


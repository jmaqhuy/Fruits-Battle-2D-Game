using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace NetworkThread
{
    public class NetworkManager : MonoBehaviour
    {
        private static NetworkManager _instance;
        public GameObject connectingAnimationBg;

        private bool IsConnecting = false;
        
        private GameObject _mainCamera;


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
                connectingAnimationBg.gameObject.SetActive(true);
                IsConnecting = true;
                StartCoroutine(DiscoveryServer());
            }
            
            else if (NetworkStaticManager.ClientHandle.IsConnected() && IsConnecting)
            {
                connectingAnimationBg.gameObject.SetActive(false);
                IsConnecting = false;
            }
        }

        private IEnumerator DiscoveryServer()
        {
            int time = 1;
            while (!NetworkStaticManager.ClientHandle.IsConnected())
            {
                
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


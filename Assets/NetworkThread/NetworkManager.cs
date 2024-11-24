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
        //public LoginScenesScript loginScenesScript;
        //public MainMenu mainMenuScript;
        private bool IsConnecting = false;
        //private List<MonoBehaviour> uiScripts = new List<MonoBehaviour>();
        private string _currentScene;
        private MonoBehaviour _currentUIScript;
        private GameObject _mainCamera;


        private void Awake()
        {
            _currentScene = SceneManager.GetActiveScene().name;
            _mainCamera = GameObject.Find("Main Camera");
            if (_currentScene == "Login")
            {
                _currentUIScript = _mainCamera.GetComponent<LoginScenesScript>();
            } else if (_currentScene == "Main Menu")
            {
                _currentUIScript = _mainCamera.GetComponent<MainMenu>();
            }
            Debug.Log($"Scene {_currentScene}");
            //getScriptNow();
            
            if (_instance != null)
            {   
                Debug.Log("NetworkManager instance already exists!");
                NetworkStaticManager.ClientHandle.SetUiScripts(_currentUIScript);
                NetworkStaticManager.ClientHandle.GetScriptNameNow();
                Destroy(gameObject);
                return;
            }
            
            //uiScripts.Add(loginScenesScript);
            //uiScripts.Add(mainMenuScript);
            Debug.Log("Instantiating NetworkManager!");
            _instance = this;
            
            DontDestroyOnLoad(gameObject);
            NetworkStaticManager.InitializeGameManager();
            NetworkStaticManager.ClientHandle.SetUiScripts(_currentUIScript);
            NetworkStaticManager.ClientHandle.GetScriptNameNow();
            
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
                yield return new WaitForSeconds(5f);
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


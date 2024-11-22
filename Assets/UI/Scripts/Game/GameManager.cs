using UnityEngine;

namespace UI.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        [Header("Network Info")]
        public int port = 14242;
        public string gameName = "game";
        public LoginManage loginManage;
        public SignUpManage signUpManage;

        void Awake()
        {
            Debug.Log("Starting Game Manager");
            GameStaticManager.InitializeGameManager(port, gameName, loginManage, signUpManage);
            
        }
    
        void OnApplicationQuit()
        {
            GameStaticManager.NetworkManager.SendDisconnect();
        }
    }
}
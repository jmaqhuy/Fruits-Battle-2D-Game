using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NetworkThread
{
    public class LoginManage : MonoBehaviour
    {
        public TMP_InputField usernameText; 
        public TMP_InputField passwordText;
        public TextMeshProUGUI errorText;
        public Button loginButton;
    
        // Start is called before the first frame update
        void Start()
        {
            loginButton.onClick.AddListener(OnLoginButtonClicked);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    
        void OnLoginButtonClicked()
        {
            errorText.text = "";
            string username = usernameText.text;
            string password = passwordText.text;
            //string username = "testUser";
            //string password = "password123";

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                NetworkStaticManager.ClientHandle.SendLoginPackage(username, password);
                Debug.Log($"Login request sent for user: {username}");
            }
            else
            {
                Debug.Log("Username or password is empty.");
            }
        }

        public void LoginSuccess()
        {
            SceneManager.LoadScene("Main Menu");
        }

        public void LoginFail()
        {
            errorText.text = "Login failed!";
        }
    }
}
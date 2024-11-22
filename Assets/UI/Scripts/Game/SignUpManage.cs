using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Scripts.Game
{
    public class SignUpManage : MonoBehaviour
    {
        public TMP_InputField usernameText; 
        public TMP_InputField passwordText;
        public TMP_InputField retypePasswordText;
        public TextMeshProUGUI errorText;
        public Button signUpButton;
        public GameObject signUpPanel;
        public GameObject loginPanel;
        // Start is called before the first frame update
        void Start()
        {
            signUpButton.onClick.AddListener(OnClickSignUpButton);
        }

        private void OnClickSignUpButton()
        {
            errorText.text = "";
            string username = usernameText.text;
            string password = passwordText.text;
            string retype = retypePasswordText.text;

            if (string.IsNullOrEmpty(username))
            {
                errorText.text = "Username is required";
            } 
            else if (string.IsNullOrEmpty(password))
            {
                errorText.text = "Password is required";
            } 
            else if (string.IsNullOrEmpty(retype))
            {
                errorText.text = "Retype is required";
            } 
            else if (password != retype)
            {
                errorText.text = "Passwords do not match";
            }
            else
            {
                GameStaticManager.NetworkManager.SendSignUpPackage(username,password);
                Debug.Log($"Sign Up request sent for user: {username}");
            }
        }

        public void SignUpFail(string error)
        {
            errorText.text = error;
        }

        public void SignUpSuccess()
        {
            StartCoroutine(CountdownToLogin(3));
        }
    
        private IEnumerator CountdownToLogin(int seconds)
        {
            // Thực hiện đếm ngược
            while (seconds > 0)
            {
                errorText.text = $"Sign Up Success! Back to Login in {seconds} seconds.";
                yield return new WaitForSeconds(1);
                seconds--;
            }

            // Sau khi đếm ngược xong, chuyển về màn hình đăng nhập
            signUpPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
    }
}

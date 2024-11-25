using System.Collections;
using NetworkThread;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginScenesScript : MonoBehaviour
{
    [Header("Login Parameters")]
    public TMP_InputField usernameText_login; 
    public TMP_InputField passwordText_login;
    public TextMeshProUGUI errorText_login;
    public Button loginButton;
    public Button DontHaveAccountButton;
    
    [Header("Sign Up Parameters")]
    public TMP_InputField usernameText_signUp; 
    public TMP_InputField passwordText_signUp;
    public TMP_InputField retypePasswordText_signUp;
    public TextMeshProUGUI errorText_signUp;
    public Button signUpButton;
    public Button HaveAlreadyAccountButton;
    public GameObject signUpPanel;
    public GameObject loginPanel;
    
    [Header("Processing ...")]
    public GameObject loadingPanel;
    public TextMeshProUGUI loadingText;

    private void Awake()
    {
        Debug.Log($"Scene {SceneManager.GetActiveScene().name}");
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        signUpButton.onClick.AddListener(OnClickSignUpButton);
        DontHaveAccountButton.onClick.AddListener(OnDontHaveAccountButtonClicked);
        HaveAlreadyAccountButton.onClick.AddListener(OnClickHaveAlreadyAccountButton);
    }

    void Start()
    {
        NetworkStaticManager.ClientHandle.SetUiScripts(this);
        NetworkStaticManager.ClientHandle.GetScriptNameNow();
    }
    
    public void OnLoginButtonClicked()
    {
        ShowProcessPanel("Login");
        
        var username = usernameText_login.text;
        var password = passwordText_login.text;
        errorText_login.text = "";
        //string username = "testUser";
        //string password = "password123";

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            NetworkStaticManager.ClientHandle.SendLoginPackage(username, password);
            errorText_login.text = $"Login request sent for user: {username}";
        }
        else
        {
            errorText_login.text = "Username or password is empty.";
            HideProcessPanel();
        }
    }

    private void OnClickHaveAlreadyAccountButton()
    {
        usernameText_login.text = usernameText_signUp.text;
        usernameText_signUp.text = string.Empty;
        passwordText_signUp.text = string.Empty;
        retypePasswordText_signUp.text = string.Empty;
        errorText_signUp.text = string.Empty;
    }

    private void OnDontHaveAccountButtonClicked()
    {
        usernameText_signUp.text = usernameText_login.text;
        usernameText_login.text = string.Empty;
        passwordText_login.text = string.Empty;
        errorText_login.text = string.Empty;
    }

    

    private void ShowProcessPanel(string content)
    {
        loadingText.text = content;
        loadingPanel.SetActive(true);
    }

    private void HideProcessPanel()
    {
        loadingPanel.SetActive(false);
    }
    
    public void OnClickSignUpButton()
    {
        ShowProcessPanel("Sign Up");
        errorText_signUp.text = "";
        string username = usernameText_signUp.text;
        string password = passwordText_signUp.text;
        string retype = retypePasswordText_signUp.text;

        if (string.IsNullOrEmpty(username))
        {
            errorText_signUp.text = "Username is required";
            HideProcessPanel();
        } 
        else if (string.IsNullOrEmpty(password))
        {
            errorText_signUp.text = "Password is required";
            HideProcessPanel();
        } 
        else if (string.IsNullOrEmpty(retype))
        {
            errorText_signUp.text = "Retype is required";
            HideProcessPanel();
        } 
        else if (password != retype)
        {
            errorText_signUp.text = "Passwords do not match";
            HideProcessPanel();
        }
        else
        {
            NetworkStaticManager.ClientHandle.SendSignUpPackage(username,password);
            Debug.Log($"Sign Up request sent for user: {username}");
        }
    }
    
    public void LoginSuccess()
    {
        HideProcessPanel();
        SceneManager.LoadScene("Main Menu");
    }

    public void LoginFail()
    {
        HideProcessPanel();
        errorText_login.text = "Login failed!";
    }
    
    public void SignUpFail(string error)
    {
        HideProcessPanel();
        errorText_signUp.text = error;
    }

    public void SignUpSuccess()
    {
        HideProcessPanel();
        StartCoroutine(CountdownToLogin(3));
    }
    
    
    private IEnumerator CountdownToLogin(int seconds)
    {
        // Thực hiện đếm ngược
        while (seconds > 0)
        {
            errorText_signUp.text = $"Sign Up Success! Back to Login in {seconds} seconds.";
            yield return new WaitForSeconds(1);
            seconds--;
        }
        usernameText_login.text = usernameText_signUp.text;
        usernameText_signUp.text = string.Empty;
        passwordText_signUp.text = string.Empty;
        retypePasswordText_signUp.text = string.Empty;
        // Sau khi đếm ngược xong, chuyển về màn hình đăng nhập
        signUpPanel.SetActive(false);
        loginPanel.SetActive(true);
    }
    
}

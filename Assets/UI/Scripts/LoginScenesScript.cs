using System.Collections;
using NetworkThread;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginScenesScript : MonoBehaviour
{
    [Header("Panels")]
    public GameObject signUpPanel;
    public GameObject loginPanel;
    public GameObject loadingPanel;
    public GameObject successPanel;
    
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
    
    
    [Header("LoginSuccess Parameters")]
    public TextMeshProUGUI welcomeUserText;
    public Button PlayButton;
    public Button LogoutButton;
    
    
    [Header("Processing Parameters")]
    public GameObject processingAnimation;
    public TextMeshProUGUI loadingText;

    private void Awake()
    {
        Debug.Log($"Scene {SceneManager.GetActiveScene().name}");
        RegisterButtonClicked();
    }

    private void OnClickLogoutButton()
    {
        ShowLoginPanel();
        NetworkStaticManager.ClientHandle.SendLogoutPacket();
        usernameText_login.text = "";
        passwordText_login.text = "";
        errorText_login.text = "";
        usernameText_signUp.text = "";
        passwordText_signUp.text = "";
        errorText_signUp.text = "";
        
    }

    void Start()
    {
        NetworkStaticManager.ClientHandle.SetUiScripts(this);
        NetworkStaticManager.ClientHandle.GetScriptNameNow();
        loadingText.text = "";
        ShowLoadingPanel();
    }
    
    public void OnLoginButtonClicked()
    {
        var username = usernameText_login.text;
        var password = passwordText_login.text;
        errorText_login.text = "";
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            ShowLoadingPanel("Login");
            NetworkStaticManager.ClientHandle.SendLoginPackage(username, password);
        }
        else
        {
            ShowLoginPanel("Username or password is empty.");
        }
    }

    private void OnClickHaveAlreadyAccountButton()
    {
        usernameText_login.text = usernameText_signUp.text;
        usernameText_signUp.text = string.Empty;
        passwordText_signUp.text = string.Empty;
        retypePasswordText_signUp.text = string.Empty;
        errorText_signUp.text = string.Empty;
        ShowLoginPanel();
    }

    private void OnDontHaveAccountButtonClicked()
    {
        usernameText_signUp.text = usernameText_login.text;
        usernameText_login.text = string.Empty;
        passwordText_login.text = string.Empty;
        errorText_login.text = string.Empty;
        ShowSignUpPanel();
    }
    
    public void OnClickSignUpButton()
    {
        string username = usernameText_signUp.text;
        string password = passwordText_signUp.text;
        string retype = retypePasswordText_signUp.text;

        if (string.IsNullOrEmpty(username))
        {
            ShowSignUpPanel("Username is required");
        } 
        else if (string.IsNullOrEmpty(password))
        {
            ShowSignUpPanel("Password is required");
        } 
        else if (string.IsNullOrEmpty(retype))
        {
            ShowSignUpPanel("Retype Password is required");
        } 
        else if (password != retype)
        {
            ShowSignUpPanel("Passwords do not match");
        }
        else
        {
            NetworkStaticManager.ClientHandle.SendSignUpPackage(username,password);
            ShowLoadingPanel("Sign Up");
            Debug.Log($"Sign Up request sent for user: {username}");
        }
    }
    
    public void LoginSuccess()
    {
        ShowSuccessPanel(NetworkStaticManager.ClientHandle.GetUsername());
    }

    public void LoginFail()
    {
        ShowLoginPanel("Login failed");
    }
    
    public void SignUpFail(string error)
    {
        ShowSignUpPanel(error);
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
            loadingText.text = $"Sign Up Success! Back to Login in {seconds} seconds.";
            yield return new WaitForSeconds(1);
            seconds--;
        }
        errorText_signUp.text = string.Empty;
        usernameText_login.text = usernameText_signUp.text;
        usernameText_signUp.text = string.Empty;
        passwordText_signUp.text = string.Empty;
        retypePasswordText_signUp.text = string.Empty;
        
        ShowLoginPanel();
    }

    public void ShowLoadingPanel(string content = "")
    {
        loadingText.text = content;
        errorText_login.text = "";
        errorText_signUp.text = "";
        welcomeUserText.text = "";
        processingAnimation.SetActive(true);
        loadingPanel.SetActive(true);
        signUpPanel.SetActive(false);
        loginPanel.SetActive(false);
        successPanel.SetActive(false);
    }

    public void ShowLoginPanel(string loginError = "")
    {
        loadingText.text = "";
        errorText_login.text = loginError;
        errorText_signUp.text = "";
        welcomeUserText.text = "";
        loadingPanel.SetActive(false);
        signUpPanel.SetActive(false);
        loginPanel.SetActive(true);
        successPanel.SetActive(false);
    }

    public void ShowSignUpPanel(string signupError = "")
    {
        loadingText.text = "";
        errorText_login.text = "";
        errorText_signUp.text = signupError;
        welcomeUserText.text = "";
        loadingPanel.SetActive(false);
        signUpPanel.SetActive(true);
        loginPanel.SetActive(false);
        successPanel.SetActive(false);
    }

    public void ShowSuccessPanel(string username)
    {
        loadingText.text = "";
        errorText_login.text = "";
        errorText_signUp.text = "";
        welcomeUserText.text = $"Welcome back, {username}!";
        loadingPanel.SetActive(false);
        signUpPanel.SetActive(false);
        loginPanel.SetActive(false);
        successPanel.SetActive(true);
    }

    private void RegisterButtonClicked()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        signUpButton.onClick.AddListener(OnClickSignUpButton);
        DontHaveAccountButton.onClick.AddListener(OnDontHaveAccountButtonClicked);
        HaveAlreadyAccountButton.onClick.AddListener(OnClickHaveAlreadyAccountButton);
        LogoutButton.onClick.AddListener(OnClickLogoutButton);
        PlayButton.onClick.AddListener(OnClickPlayButton);
    }

    private void OnClickPlayButton()
    {
        SceneManager.LoadScene("Main Menu");
    }
}

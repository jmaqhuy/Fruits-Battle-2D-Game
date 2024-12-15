using System;
using System.Collections;
using System.Text.RegularExpressions;
using NetworkThread;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginScenesScript : MonoBehaviour
{
    [Header("Panels")]
    public GameObject signUpPanel;
    public GameObject forgotPasswordPanel;
    public GameObject loginPanel;
    public GameObject loadingPanel;
    public GameObject successPanel;
    public GameObject verificationPanel;
    
    [Header("Login Parameters")]
    public TMP_InputField usernameText_login; 
    public TMP_InputField passwordText_login;
    public TextMeshProUGUI errorText_login;
    public Button loginButton;
    public Button DontHaveAccountButton;
    public Button ForgotPasswordButton;
    
    [Header("Forgot Password")]
    public TMP_InputField usernameText_forgotPassword;
    public TMP_InputField emailText_forgotPassword;
    public Button ResetPasswordButton;
    public Button HaveAlreadyAccountFwpButton;
    public TextMeshProUGUI errorText_forgotPassword;
    
    
    [Header("Sign Up Parameters")]
    public TMP_InputField usernameText_signUp; 
    public TMP_InputField emailText_signUp;
    public TMP_InputField passwordText_signUp;
    public TMP_InputField retypePasswordText_signUp;
    public TextMeshProUGUI errorText_signUp;
    public Button signUpButton;
    public Button HaveAlreadyAccountButton;

    [Header("Verify Registration")] 
    public TMP_InputField[] digits;
    public Button verificationButton;
    public TextMeshProUGUI errorText_verification;
    
    [Header("LoginSuccess Parameters")]
    public TextMeshProUGUI welcomeUserText;
    public Button PlayButton;
    public Button LogoutButton;
    
    
    [Header("Processing Parameters")]
    public GameObject processingAnimation;
    public TextMeshProUGUI loadingText;

    
    private string _tmpUsername;
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
        emailText_signUp.text = "";
        
    }

    void Start()
    {
        for (int i = 0; i < digits.Length; i++)
        {
            int index = i; // Lưu chỉ số hiện tại (để tránh lỗi closure)
            digits[i].onValueChanged.AddListener((text) => OnInputChanged(index, text));
        }
        NetworkStaticManager.ClientHandle.SetUiScripts(this);
        NetworkStaticManager.ClientHandle.GetScriptNameNow();
        loadingText.text = "";
        for (int i = 0; i < digits.Length; i++)
        {
            int index = i; // Lưu chỉ số hiện tại (để tránh lỗi closure)
            digits[i].onValueChanged.AddListener((text) => OnInputChanged(index, text));
        }
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
        emailText_signUp.text = string.Empty;
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
        _tmpUsername = username;
        string email = emailText_signUp.text;
        string password = passwordText_signUp.text;
        string retype = retypePasswordText_signUp.text;

        if (string.IsNullOrEmpty(username))
        {
            ShowSignUpPanel("Username is required");
        } 
        else if (string.IsNullOrEmpty(email))
        {
            ShowSignUpPanel("Email is required");
        }
        else if (string.IsNullOrEmpty(password))
        {
            ShowSignUpPanel("Password is required");
        } 
        else if (string.IsNullOrEmpty(retype))
        {
            ShowSignUpPanel("Retype Password is required");
        } 
        else if(!IsValidEmail(email))
        {
            ShowSignUpPanel("Email is invalid");
        }
        else if (password != retype)
        {
            ShowSignUpPanel("Passwords do not match");
        }
        else
        {
            NetworkStaticManager.ClientHandle.SendSignUpPackage(username,password, email);
            ShowLoadingPanel("Sign Up");
            Debug.Log($"Sign Up request sent for user: {username}");
        }
    }
    
    private void OnClickResetPasswordButton()
    {
        string username = usernameText_forgotPassword.text;
        string email = emailText_forgotPassword.text;
        if (string.IsNullOrEmpty(username))
        {
            ShowForgotPasswordPanel("Username is required");
        } 
        else if (string.IsNullOrEmpty(email))
        {
            ShowForgotPasswordPanel("Email is required");
        } else if (!IsValidEmail(email))
        {
            ShowForgotPasswordPanel("Email is invalid");
        }
        else
        {
            NetworkStaticManager.ClientHandle.SendResetPasswordPacket(username, email);
            ShowLoadingPanel("Reset Password");
        }
    }

    public void ResetPasswordFail(string reason)
    {
        errorText_forgotPassword.text = reason;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            var regex = new Regex(emailPattern);
            return regex.IsMatch(email);
        }
        catch (Exception)
        {
            return false;
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
        _tmpUsername = string.Empty;
    }

    public void SignUpSuccess()
    {
        ShowVerificationPanel();
        
    }

    public void VerificationSuccess()
    {
        StartCoroutine(CountdownToLogin(3));
    }

    public void VerificationFail(string error)
    {
        ShowVerificationPanel(error);
    }
    
    
    private IEnumerator CountdownToLogin(int seconds)
    {
        // Thực hiện đếm ngược
        while (seconds > 0)
        {
            loadingText.text = $"Verify Success! Back to Login in {seconds} seconds.";
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
        forgotPasswordPanel.SetActive(false);
        verificationPanel.SetActive(false);
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
        forgotPasswordPanel.SetActive(false);
        verificationPanel.SetActive(false);
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
        forgotPasswordPanel.SetActive(false);
        verificationPanel.SetActive(false);
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
        forgotPasswordPanel.SetActive(false);
        verificationPanel.SetActive(false);
    }

    private void ShowForgotPasswordPanel(string forgotError = "")
    {
        loadingPanel.SetActive(false);
        signUpPanel.SetActive(false);
        loginPanel.SetActive(false);
        successPanel.SetActive(false);
        forgotPasswordPanel.SetActive(true);
        errorText_forgotPassword.text = forgotError;
        verificationPanel.SetActive(false);
    }

    private void ShowVerificationPanel(string verificationError = "")
    {
        loadingPanel.SetActive(false);
        signUpPanel.SetActive(false);
        loginPanel.SetActive(false);
        successPanel.SetActive(false);
        forgotPasswordPanel.SetActive(false);
        verificationPanel.SetActive(true);
        errorText_verification.text = verificationError;
    }

    private void RegisterButtonClicked()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        signUpButton.onClick.AddListener(OnClickSignUpButton);
        DontHaveAccountButton.onClick.AddListener(OnDontHaveAccountButtonClicked);
        HaveAlreadyAccountButton.onClick.AddListener(OnClickHaveAlreadyAccountButton);
        LogoutButton.onClick.AddListener(OnClickLogoutButton);
        PlayButton.onClick.AddListener(OnClickPlayButton);
        ForgotPasswordButton.onClick.AddListener(OnClickForgotPasswordButton);
        HaveAlreadyAccountFwpButton.onClick.AddListener(OnClickHaveAlreadyAccountFwpButton);
        ResetPasswordButton.onClick.AddListener(OnClickResetPasswordButton);
        verificationButton.onClick.AddListener(OnClickVerificationButton);
    }

    private void OnClickVerificationButton()
    {
        var otpCode = GetOtpCode();
        if (otpCode.Length < 6)
        {
            ShowVerificationPanel("Verification Code must include 6 characters.");
            return;
        }
        NetworkStaticManager.ClientHandle.SendVerifyRegistrationPacket(_tmpUsername, otpCode);
        ShowLoadingPanel("Verifying");
    }

    private void OnInputChanged(int index, string text)
    {
        // Chỉ chuyển focus nếu nhập đúng 1 ký tự
        if (text.Length == 1 && index < digits.Length - 1)
        {
            digits[index + 1].Select(); // Chuyển focus sang ô tiếp theo
        }
        else if (text.Length == 0 && index > 0)
        {
            digits[index - 1].Select(); // Quay lại ô trước đó nếu ô hiện tại bị xóa
        }
    }

    private string GetOtpCode()
    {
        // Lấy mã OTP từ các ô nhập
        string otpCode = "";
        foreach (var field in digits)
        {
            otpCode += field.text;
        }
        return otpCode;
    }

    

    private void OnClickHaveAlreadyAccountFwpButton()
    {
        ShowLoginPanel();
    }

    private void OnClickForgotPasswordButton()
    {
        ShowForgotPasswordPanel();
    }

    private void OnClickPlayButton()
    {
        SceneManager.LoadScene("Main Menu");
    }
}

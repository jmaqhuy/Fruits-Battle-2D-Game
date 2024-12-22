using System;
using System.Collections;
using System.Text.RegularExpressions;
using NetworkThread;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ChangePassword : MonoBehaviour
{
    [Header("Change password")]
    public TMP_InputField oldPassword;
    public TMP_InputField newPassword;
    public TMP_InputField confirmPassword;
    public TextMeshProUGUI errorText;
    public Button changePasswordButton;
    private string username;
    private void Awake()
    {
        Debug.Log($"Scene {SceneManager.GetActiveScene().name}");
        RegisterButtonClicked();
    }

    // Start is called before the first frame update
    void Start()
    {
        NetworkStaticManager.ClientHandle.SetUiScripts(this);
        NetworkStaticManager.ClientHandle.GetScriptNameNow();
    }
    private void RegisterButtonClicked()
    {
        changePasswordButton.onClick.AddListener(OnClickChangePasswordButton);
      
    }

    private void OnClickBackButton()
    {
        SceneManager.LoadScene("Main Menu");
    }
   
   
    private void OnClickChangePasswordButton()
    {
        string username = NetworkStaticManager.ClientHandle.GetUsername();
        string oldPass = oldPassword.text;
        string newPass = newPassword.text;
        string confirm = confirmPassword.text;
        if (string.IsNullOrEmpty(oldPass))
        {
            ShowChangePasswordPanel("Old Password is required");
        } 
        else if (string.IsNullOrEmpty(newPass))
        {
            ShowChangePasswordPanel("New Password is required");
        }
        else if (string.IsNullOrEmpty(confirm))
        {
            ShowChangePasswordPanel("Confirm Password is required");
        }
        else if (newPass != confirm)
        {
            ShowChangePasswordPanel("Passwords do not match");
        }
        else
        {
            NetworkStaticManager.ClientHandle.SendChangePasswordPacket(username,oldPass, newPass, confirm);
            
        }
    }
    
    private void ShowChangePasswordPanel(string forgotError = "")
    {
        errorText.text = forgotError;
    }
   

   

    
}

using System;
using System.Collections;
using System.Collections.Generic;
using DataTransfer;
using NetworkThread;
using NetworkThread.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public UserData userData;
    public CharactersData charactersData;
    [Header("Basic User Information")]
    public TMP_Text displayName;
    public TMP_Text coins;

    public TMP_Text displayNameProfile;

    [Header("Change Display Name Panel")]
    public GameObject changeDisplayNamePanel;

    public Button closeButton;
    public TMP_InputField newDisplayName;
    public Button acceptButton;


   

    // Start is called before the first frame update

    void Awake()
    {
        NetworkStaticManager.ClientHandle.SetUiScripts(this);
        Debug.Log($"Scene {SceneManager.GetActiveScene().name}");
    }
    
    void Start()
    {
        acceptButton.onClick.AddListener(OnChangeDisplayNameButtonClicked);
        SetCoinsTMP(userData.UserInfo.coin);
        if ( string.IsNullOrEmpty(userData.UserInfo.displayName))

        NetworkStaticManager.ClientHandle.GetScriptNameNow();
        NetworkStaticManager.ClientHandle.RequestBasicUserInfo();
    }

    // Update is called once per frame
    void Update()
    {
        // if (firstTime)
        // {
        //     ShowChangeDisplayNamePanel(false);
        // }
        // else
        // {
        //     SetDisplayNameTMP(userData.UserInfo.displayName);
        //     HideChangeDisplayNamePanel();
        // }
    }
    private void OnChangeDisplayNameButtonClicked()
    {
        NetworkStaticManager.ClientHandle.SendChangeDisplayNamePacket(newDisplayName.text);
        SetDisplayNameTMP(newDisplayName.text);
        userData.UserInfo.displayName = newDisplayName.text;
        HideChangeDisplayNamePanel();
    }

    private void SetDisplayNameTMP(string dn)
    {
        displayName.text = dn;
    }

    private void SetCoinsTMP(int coin)
    {
        coins.text = coin.ToString();
    }

    public void GoToShop()
    {
        SceneManager.LoadScene("Shop");
    }
    
    public void SelectPlayMode()
    {
        SceneManager.LoadScene("Select Play Mode");
    }

    public void GoToFriends()
    {
        SceneManager.LoadScene("Friends");
    }

    private void GoToCharacters()
    {
        NetworkStaticManager.ClientHandle.SendCurrentCharacterPacket();
    }
    public void LoadCharacterScene(GetCurrentCharacterPacket packet)
    {
        charactersData.Characters.Add(packet.Character);
        SceneManager.LoadScene("Character Manager");
    }

    public void ShowMyProfile()
    {
        displayNameProfile.text = displayName.text;
    }

    private void ShowChangeDisplayNamePanel(bool closeButtonStatus)
    {
        closeButton.interactable = closeButtonStatus;
        changeDisplayNamePanel.SetActive(true);
    }

    private void HideChangeDisplayNamePanel()
    {
        changeDisplayNamePanel.SetActive(false);
    }

    public GameObject GetChangeDisplayNamePanel()
    {
        return changeDisplayNamePanel;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    public void ExitAcount()
    {
        NetworkStaticManager.ClientHandle.SendLogoutPacket();
        SceneManager.LoadScene("Login");
    }

}
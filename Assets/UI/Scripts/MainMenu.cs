using System;
using System.Collections.Generic;
using DataTransfer;
using NetworkThread;
using NetworkThread.Multiplayer;
using NetworkThread.Multiplayer.Packets;
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
        {
            ShowChangeDisplayNamePanel(false);
        }
        else
        {
            SetDisplayNameTMP(userData.UserInfo.displayName);
            HideChangeDisplayNamePanel();
        }

        if (charactersData.Characters.Count == 0)
        {
            Debug.Log("Characters data is null");
            NetworkStaticManager.ClientHandle.SendCurrentCharacterPacket();
        }
        else
        {
            Debug.Log($"Characters data is not null. Number of characters: {charactersData.Characters.Count}");
        }

        if (userData.UserInfo == null)
        {
            Debug.Log($"User data is null");
        }
        else
        {
            Debug.Log($"User data is not null. Display name: {userData.UserInfo.displayName}");
        }
    }

    public void OnChangeDisplayNameButtonClicked()
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
        SceneManager.LoadScene("Character Manager");
    }

    public void ParseCharacters(GetCurrentCharacterPacket packet)
    {
        charactersData.Characters.Add(packet.Character); 
        Debug.Log("Number of Character: " + charactersData.Characters.Count);
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
        charactersData.Characters.Clear();
        SceneManager.LoadScene("Login");
    }

}
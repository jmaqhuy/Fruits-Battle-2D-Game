using System;
using System.Collections;
using System.Collections.Generic;
using NetworkThread;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Basic User Information")]
    public TMP_Text displayName;
    public TMP_Text coins;

    public TMP_Text displayNameProfile;

    public GameObject changeDisplayNamePanel;
    // Start is called before the first frame update

    void Awake()
    {
        NetworkStaticManager.ClientHandle.SetUiScripts(this);
        Debug.Log($"Scene {SceneManager.GetActiveScene().name}");
    }
    
    void Start()
    {
        NetworkStaticManager.ClientHandle.GetScriptNameNow();
        NetworkStaticManager.ClientHandle.RequestBasicUserInfo();
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (firstTime)
        {
            firstTime = false;
            NetworkStaticManager.ClientHandle.RequestBasicUserInfo();
            if (NetworkStaticManager.ClientHandle.GetUsername() == "")
            {
                ShowChangeDisplayNamePanel();
            }
        }*/
    }

    public void SetDisplayNameTMP(string displayName)
    {
        this.displayName.text = displayName;
    }

    public void SetCoinsTMP(int coin)
    {
        this.coins.text = coin.ToString();
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

    public void ShowMyProfile()
    {
        displayNameProfile.text = displayName.text;
    }

    public void ShowChangeDisplayNamePanel()
    {
        changeDisplayNamePanel.SetActive(true);
    }

    public void HideChangeDisplayNamePanel()
    {
        changeDisplayNamePanel.SetActive(false);
    }

    public GameObject GetChangeDisplayNamePanel()
    {
        return changeDisplayNamePanel;
    }
}

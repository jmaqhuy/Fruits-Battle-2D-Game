using System;
using System.Collections.Generic;
using DataTransfer;
using NetworkThread;
using NetworkThread.Multiplayer;
using NetworkThread.Multiplayer.Packets;
using Resources;
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

    [Header("Profile Panel")]
    public GameObject profilePanel;
    public TextMeshProUGUI characterNumber;
    public TextMeshProUGUI rankName;
    public SpriteRenderer rankIcon;
    public GameObject stars;
    public GameObject starIcon;
    private List<GameObject> starList = new List<GameObject>();
    
    [Header("Change Password")]
    public ChangePassword changePassword;
    
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
    }

    public void ShowChangePasswordDone()
    {
        changePassword.ShowChangePasswordDone();
    }

    public void ShowChangePasswordFailed()
    {
        changePassword.ShowChangePasswordFailed();
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
        Debug.Log($"Rank: {userData.CurrentRank.rankName} {userData.CurrentRank.currentStar}");
        displayNameProfile.text = displayName.text;starIcon.SetActive(true);
        displayNameProfile.text = displayName.text;
        characterNumber.text = charactersData.Characters.Count.ToString();
        Sprite rIcon = ResourceLoader.LoadRankSprite(userData.CurrentRank.rankAssetName);
        Debug.Log(userData.CurrentRank.rankAssetName);
        if (rIcon != null)
        {
            rankIcon.sprite = rIcon;
        }
        else
        {
            Debug.LogError($"Failed to load rank icon");
        }

        foreach (GameObject star in starList)
        {
            Destroy(star);
        }
        starList.Clear();

        var starNum = userData.CurrentRank.currentStar;
        if (starNum != 0)
        {
            for (int i = 0; i < starNum; i++)
            {
                starList.Add(Instantiate(starIcon, stars.transform));
            }
        }
        else
        {
            GameObject starGO = Instantiate(starIcon, stars.transform);
            starGO.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
            starList.Add(starGO);
        }
        rankName.text = userData.CurrentRank.rankName;
        starIcon.SetActive(false);
        profilePanel.SetActive(true);
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
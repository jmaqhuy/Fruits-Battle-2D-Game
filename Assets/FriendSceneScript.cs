using System.Collections;
using System.Collections.Generic;
using NetworkThread;
using NetworkThread.Multiplayer;
using NetworkThread.Multiplayer.Packets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FriendSceneScript : MonoBehaviour
{
    [Header("Tab Buttons")]
    public Button AllFriendsButton;
    public Button FriendRequestButton;
    public Button SentRequestButton;
    public Button SearchFriendsButton;
    public Button BlockFriendsButton;
    
    [Header("All Friend Tab")]
    public GameObject AllFriendTab;
    public RectTransform AllFriendTabRect;
    public GameObject FriendPrefab;
    public TextMeshProUGUI FriendDisplayName;
    public Button ShowFriendInfoButton;
    public Button DeleteFriendButton;
    
    [Header("Friend Request")]
    public GameObject FriendRequestTab;
    
    [Header("Sent Request")]
    public GameObject SentRequestTab;
    
    [Header("Search Tab")]
    public GameObject SearchTab;
    public RectTransform SearchTabRect;
    [FormerlySerializedAs("FriendResultPrefab")] public GameObject SuggestResultPrefab;
    public TextMeshProUGUI FriendResultDisplayName;
    public TMP_InputField SearchInputField;
    public Button SearchButton;
    
    [Header("Block Friend Tab")]
    public GameObject BlockFriendTab;

    private Dictionary<Button, GameObject> buttonTabMap;
    private List<GameObject> _queryResults = new List<GameObject>();
    private Color defaultButtonColor;
    private Color activeButtonColor;

    void Awake()
    {
        NetworkStaticManager.ClientHandle.SetUiScripts(this);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        ColorUtility.TryParseHtmlString("#06A6E7", out defaultButtonColor);
        ColorUtility.TryParseHtmlString("#D46613", out activeButtonColor);
        buttonTabMap = new Dictionary<Button, GameObject>()
        {
            { AllFriendsButton, AllFriendTab },
            { FriendRequestButton, FriendRequestTab },
            { SentRequestButton, SentRequestTab },
            { SearchFriendsButton, SearchTab },
            { BlockFriendsButton, BlockFriendTab }
        };
        
        foreach (var entry in buttonTabMap)
        {
            entry.Key.onClick.AddListener(() => OnButtonClick(entry.Key));
        }
        
    }
    
    private void OnButtonClick(Button clickedButton)
    {
        foreach (var qr in _queryResults)
        {
            Destroy(qr);
        }
        _queryResults.Clear();
        ShowTab(buttonTabMap[clickedButton]);

        UpdateButtonColors(clickedButton);
    }
    
    private void ShowTab(GameObject activeTab)
    {
        foreach (var tab in buttonTabMap.Values)
        {
            tab.SetActive(tab == activeTab);
        }

        if (activeTab == SearchTab)
        {
            Debug.Log("Search Tab Active");
            GetSuggestFriend();
        }
    }
    
    private void UpdateButtonColors(Button activeButton)
    {
        foreach (var button in buttonTabMap.Keys)
        {
            var buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = (button == activeButton) ? activeButtonColor : defaultButtonColor;
            }
        }
    }

    private void GetSuggestFriend()
    {
        NetworkStaticManager.ClientHandle.SendSuggestFriendPacket();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoBack()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void ParseSuggestFriendInfo(SuggestFriendPacket packet)
    {
        List<FriendTabPacket> suggestedFriends = packet.Friends;
        Debug.Log(suggestedFriends.Count + " User Found");
        foreach (var f in packet.Friends)
        {
            GameObject friend = Instantiate(SuggestResultPrefab, SearchTabRect);
            _queryResults.Add(friend);
            FriendInFriendList tabInfo = friend.GetComponent<FriendInFriendList>();
            tabInfo.SetUserName(f.FriendUsername);
            tabInfo.DisplayName.text = f.FriendDisplayName;
        }

        SuggestResultPrefab.SetActive(false);
    }
}

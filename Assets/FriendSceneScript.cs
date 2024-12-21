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
    public GameObject AllFriendPrefab;

    [Header("Friend Request")]
    public GameObject FriendRequestTab;
    public RectTransform FriendRequestTabRect;
    [FormerlySerializedAs("FriendResultPrefab")] public GameObject FriendRequestPrefab;

    [Header("Sent Request")]
    public GameObject SentRequestTab;
    public RectTransform SentRequestTabRect;
    [FormerlySerializedAs("FriendResultPrefab")] public GameObject SentRequestPrefab;


    [Header("Search Tab")]
    public GameObject SearchTab;
    public RectTransform SearchTabRect;
    [FormerlySerializedAs("FriendResultPrefab")] public GameObject SuggestResultPrefab;
    public TMP_InputField SearchInputField;
    public Button SearchButton;

    [Header("Block Friend Tab")]
    public GameObject BlockFriendTab;
    public RectTransform BlockFriendTabRect;
    [FormerlySerializedAs("FriendResultPrefab")] public GameObject BlockFriendPrefab;

    [Header("Prefabs")]
    public GameObject UserProfilePrefab;
    private string NameToRemove;
    private List<GameObject> PreFabs;
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

        PreFabs = new List<GameObject>()
        {
            AllFriendPrefab,
            FriendRequestPrefab,
            SentRequestPrefab,
            SuggestResultPrefab,
            BlockFriendPrefab
        };

        foreach (GameObject PreFab in PreFabs)
        {
            PreFab.SetActive(false);
        }

        SearchButton.onClick.AddListener(() => PlayerSearch());

        foreach (var entry in buttonTabMap)
        {
            entry.Key.onClick.AddListener(() => OnButtonClick(entry.Key));
        }

    }
    private void PlayerSearch()
    {
        foreach (var qr in _queryResults)
        {
            Destroy(qr);
        }
        _queryResults.Clear();
        Debug.Log("Name: " + SearchInputField.text);
        GetSearchPlayer(SearchInputField.text);
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
        if (activeTab == AllFriendTab)
        {
            Debug.Log("AllFriend Tab Active");
            GetAllFriend();
        }
        if (activeTab == FriendRequestTab)
        {
            Debug.Log("FriendRequest Tab Active");
            GetFriendRequestFriend();
        }
        if (activeTab == SentRequestTab)
        {
            Debug.Log("SentRequest Tab Active");
            GetSentRequestFriend();
        }
        if (activeTab == SearchTab)
        {
            Debug.Log("Search Tab Active");
            GetSuggestFriend();
        }
        if (activeTab == BlockFriendTab)
        {
            Debug.Log("BlockFriend Tab Active");
            GetBlockFriend();
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


    private void GetAllFriend()
    {
        NetworkStaticManager.ClientHandle.SendAllFriendPacket();
    }
    private void GetFriendRequestFriend()
    {
        NetworkStaticManager.ClientHandle.SendFriendRequestPacket();
    }
    private void GetSentRequestFriend()
    {
        NetworkStaticManager.ClientHandle.SendSentRequestPacket();
    }
    private void GetSuggestFriend()
    {
        NetworkStaticManager.ClientHandle.SendSuggestFriendPacket();
    }
    private void GetBlockFriend()
    {
        NetworkStaticManager.ClientHandle.SendBlockFriendPacket();
    }
    private void GetSearchPlayer(string PlayerName)
    {
        NetworkStaticManager.ClientHandle.SendSearchPlayerPacket(PlayerName);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GoBack()
    {
        SceneManager.LoadScene("Main Menu");
    }
    public void ParseAllFriendInfo(AllFriendPacket packet)
    {
        List<FriendTabPacket> allFriends = packet.Friends;
        Debug.Log(allFriends.Count + " User Found");
        foreach (var f in packet.Friends)
        {
            GameObject friend = Instantiate(AllFriendPrefab, AllFriendTabRect);
            friend.SetActive(true);
            _queryResults.Add(friend);
            FriendInFriendList tabInfo = friend.GetComponent<FriendInFriendList>();
            tabInfo.SetUserName(f.FriendUsername);
            tabInfo.DisplayName.text = f.FriendDisplayName;
            tabInfo.GetComponent<FriendInFriendList>().ShowInfoButton.onClick.AddListener(() => ShowUserProfile(f.FriendUsername));
            tabInfo.GetComponent<FriendInFriendList>().DeleteFriendButton.onClick.AddListener(() => RemoveFriend(f.FriendUsername));
        }
    }
    public void ParseFriendRequestInfo(FriendRequestPacket packet)
    {
        List<FriendTabPacket> FriendsRequest = packet.Friends;
        Debug.Log(FriendsRequest.Count + " User Found");
        foreach (var f in packet.Friends)
        {
            GameObject friend = Instantiate(FriendRequestPrefab, FriendRequestTabRect);
            friend.SetActive(true);
            _queryResults.Add(friend);
            FriendInFriendList tabInfo = friend.GetComponent<FriendInFriendList>();
            tabInfo.SetUserName(f.FriendUsername);
            tabInfo.DisplayName.text = f.FriendDisplayName;
            tabInfo.GetComponent<FriendInFriendList>().ShowInfoButton.onClick.AddListener(() => ShowUserProfile(f.FriendUsername));
            tabInfo.GetComponent<FriendInFriendList>().ConfirmFriendButton.onClick.AddListener(() => AcceptFriendRequest(f.FriendUsername));
        }
    }
    public void ParseSentFriendInfo(SentRequestPacket packet)
    {
        List<FriendTabPacket> sentFriends = packet.Friends;
        Debug.Log(sentFriends.Count + " User Found");
        foreach (var f in packet.Friends)
        {
            GameObject friend = Instantiate(SentRequestPrefab, SentRequestTabRect);
            friend.SetActive(true);
            _queryResults.Add(friend);
            FriendInFriendList tabInfo = friend.GetComponent<FriendInFriendList>();
            tabInfo.SetUserName(f.FriendUsername);
            tabInfo.DisplayName.text = f.FriendDisplayName;
            tabInfo.GetComponent<FriendInFriendList>().ShowInfoButton.onClick.AddListener(() => ShowUserProfile(f.FriendUsername));
            tabInfo.GetComponent<FriendInFriendList>().CancelRequestButton.onClick.AddListener(() => CancelFriendRequest(f.FriendUsername));
        }
    }
    public void ParseSuggestFriendInfo(SuggestFriendPacket packet)
    {
        List<FriendTabPacket> suggestedFriends = packet.Friends;
        Debug.Log(suggestedFriends.Count + " User Found");
        foreach (var f in packet.Friends)
        {
            GameObject friend = Instantiate(SuggestResultPrefab, SearchTabRect);
            friend.SetActive(true);
            _queryResults.Add(friend);
            FriendInFriendList tabInfo = friend.GetComponent<FriendInFriendList>();
            tabInfo.SetUserName(f.FriendUsername);
            tabInfo.DisplayName.text = f.FriendDisplayName;
            tabInfo.GetComponent<FriendInFriendList>().ShowInfoButton.onClick.AddListener(() => ShowUserProfile(f.FriendUsername));
            tabInfo.GetComponent<FriendInFriendList>().AddFriendButton.onClick.AddListener(() => AddFriend(f.FriendUsername));
        }
    }
    public void ParseSearchedFriendInfo(SearchPlayerPacket packet)
    {
        List<FriendTabPacket> SearchedFriend = packet.Friends;
        Debug.Log(SearchedFriend.Count + " User Found");
        foreach (var f in packet.Friends)
        {
            GameObject friend = Instantiate(SuggestResultPrefab, SearchTabRect);
            friend.SetActive(true);
            _queryResults.Add(friend);
            FriendInFriendList tabInfo = friend.GetComponent<FriendInFriendList>();
            tabInfo.SetUserName(f.FriendUsername);
            tabInfo.DisplayName.text = f.FriendDisplayName;
            tabInfo.GetComponent<FriendInFriendList>().ShowInfoButton.onClick.AddListener(() => ShowUserProfile(f.FriendUsername));
            tabInfo.GetComponent<FriendInFriendList>().AddFriendButton.onClick.AddListener(() => AddFriend(f.FriendUsername));
        }
    }
    public void ParseBlockFriendInfo(BlockFriendPacket packet)
    {
        List<FriendTabPacket> blockFriends = packet.Friends;
        Debug.Log(blockFriends.Count + " User Found");
        foreach (var f in packet.Friends)
        {
            GameObject friend = Instantiate(BlockFriendPrefab, BlockFriendTabRect);
            friend.SetActive(true);
            _queryResults.Add(friend);
            FriendInFriendList tabInfo = friend.GetComponent<FriendInFriendList>();
            tabInfo.SetUserName(f.FriendUsername);
            tabInfo.DisplayName.text = f.FriendDisplayName;
            tabInfo.GetComponent<FriendInFriendList>().ShowInfoButton.onClick.AddListener(() => ShowUserProfile(f.FriendUsername));
            tabInfo.GetComponent<FriendInFriendList>().UnlockFriendButton.onClick.AddListener(() => UnBlockFriend(f.FriendUsername));
        }
    }
    private void AddFriend(string username)
    {
        NetworkStaticManager.ClientHandle.SendAddFriendPacket(username);
        NameToRemove = username;
    }
    private void RemoveFriend(string username)
    {
        NetworkStaticManager.ClientHandle.SendDeleteFriend(username);
        NameToRemove = username;
    }
    private void AcceptFriendRequest(string username)
    {
        NetworkStaticManager.ClientHandle.SendAcceptFriendInvite(username);
        NameToRemove = username;
    }
    private void CancelFriendRequest(string username)
    {
        NetworkStaticManager.ClientHandle.SendCancelFriendRequest(username);
        NameToRemove = username;
    }
    private void BlockFriend(string username)
    {
        NetworkStaticManager.ClientHandle.SendBlockFriend(username);
        NameToRemove = username;
    }
    private void UnBlockFriend(string username)
    {
        NetworkStaticManager.ClientHandle.SendUnBlockFriend(username);
        NameToRemove = username;
    }

    public void ShowUserProfile(string player)
    {
    }
    private void RemoveOnePrefab()
    {
        for (int i = 0;i<_queryResults.Count;i++) 
        {
            if (_queryResults[i].GetComponent<FriendInFriendList>().GetUserName() == NameToRemove)
            {
                Destroy(_queryResults[i]);
                _queryResults.Remove(_queryResults[i]);
            }
        }
    }
    public void AfterDeleteFriend(DeleteFriend packet)
    {
        if (packet.IsSuccess)
        {
            Debug.Log("Delete friend successfull");
            RemoveOnePrefab();
        }
        else
        {
            Debug.Log("Delete friend not successfull");
        }
    }
    public void AfterAcceptFriend(AcceptFriendInvite packet)
    {
        if (packet.IsSuccess)
        {
            Debug.Log("Accept friend successfull");
            RemoveOnePrefab();
        }
        else
        {
            Debug.Log("Accept friend not successfull");

        }
    }
    public void AfterCancelRequest(CancelFriendRequest packet)
    {
        if (packet.IsSuccess)
        {
            Debug.Log("Cancel friend successfull");
            RemoveOnePrefab();
        }
        else
        {
            Debug.Log("Cancel friend not successfull");

        }
    }
    public void AfterAddFriend(AddFriendPacket packet)
    {
        if (packet.IsSuccess)
        {
            Debug.Log("Add friend successfull");
            RemoveOnePrefab();
        }
        else
        {
            Debug.Log("Add friend not successfull");

        }
    }
    public void AfterBlockFriend(BlockFriend packet)
    {
        if (packet.IsSuccess)
        {
            Debug.Log("Block friend successfull");
            RemoveOnePrefab();
        }
        else
        {
            Debug.Log("Block friend not successfull");

        }
    }
    public void AfterUnlockFriend(UnBlockFriend packet)
    {
        if (packet.IsSuccess)
        {
            Debug.Log("Unlock friend successfull"); 
            RemoveOnePrefab();
        }
        else
        {
            Debug.Log("Unlock friend not successfull");

        }
    }
}

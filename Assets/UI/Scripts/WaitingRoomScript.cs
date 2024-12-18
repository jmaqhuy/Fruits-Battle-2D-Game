using System.Collections;
using System.Collections.Generic;
using NetworkThread;
using NetworkThread.Multiplayer;
using NetworkThread.Multiplayer.Packets;
using RoomEnum;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DataTransfer;
using UnityEngine.Serialization;

public class WaitingRoomScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Button buttonBack;
    public RoomData roomData;
    public PositionsData positionsData;
    
    [Header("Room Info")]
    public TextMeshProUGUI RoomId;
    private int _intRoomId;
    public TextMeshProUGUI RoomName;
    public TextMeshProUGUI roomType;
    public RoomModeUI roomModeUI;
    public RoomMode roomMode; 
    
    [Header("Players")]
    public GameObject[] playerList;
    
    [Header("ChatZone")]
    public RectTransform chatPanelZone;
    public GameObject prefab;
    public TextMeshProUGUI MsgLastest;
    public TMP_InputField textBox;

    [Header("Items")] 
    public GameObject SelectItemPopup;
    public Button Item1;
    public Button Item2;
    
    [Header("Host Panel")]
    public GameObject hostPanel;
    public Button StartButton;
    
    [Header("Guest Panel")]
    public GameObject guestPanel;
    public Button ReadyButton;
    
    private bool _isReady = false;
    [Header("Error Panel")]
    public GameObject errorPanel;

    void Awake()
    {
        foreach (var player in playerList)
        {
            player.SetActive(false);
        }
    }
    void Start()
    {
        if (roomData.RoomPacket != null)
        {
            PasteRoomInfo(roomData.RoomPacket);
            SetUIForAll(roomData.PlayersInRoom);
        }
        NetworkStaticManager.ClientHandle.SetUiScripts(this);
        buttonBack.onClick.AddListener(OnButtonBackClick);
        Item1.onClick.AddListener(() => OnItemClick(1));
        Item2.onClick.AddListener(() => OnItemClick(2));
        ReadyButton.onClick.AddListener(PlayerReadyClick);
        StartButton.onClick.AddListener(StartGameButtonClick);
    }

    private void StartGameButtonClick()
    {
        var numPlayerTeam1 = 0;
        var numPlayerTeam2 = 0;
        PlayerInWaitingRoom currentPlayer;
        for (var i = 0; i < 4; i++)
        {
            if (playerList[i].activeSelf)
            {
                numPlayerTeam1++;
                currentPlayer = playerList[i].GetComponent<PlayerInWaitingRoom>();
                if (currentPlayer.isHost.activeSelf) continue;
                if (currentPlayer.isReady.activeSelf == false )
                {
                    Debug.Log("Players Team 1 are not ready");
                    StartCoroutine(ShowErrorPanel("Players are not ready"));
                    return;
                }
                
            }
        }
        for (var i = 4; i < playerList.Length; i++)
        {
            if (playerList[i].activeSelf)
            {
                numPlayerTeam2++;
                currentPlayer = playerList[i].GetComponent<PlayerInWaitingRoom>();
                if (currentPlayer.isHost.activeSelf) continue;
                if (currentPlayer.isReady.activeSelf == false)
                {
                    Debug.Log("Players Team 2 are not ready");
                    StartCoroutine(ShowErrorPanel("Players are not ready"));
                    return;
                }
            }
        }
        if (numPlayerTeam1 != numPlayerTeam2)
        {
            StartCoroutine(ShowErrorPanel("The number of players on both teams is not balanced."));
            return;
        }
        NetworkStaticManager.ClientHandle.SendStartGamePacket(_intRoomId);
    }

    private IEnumerator ShowErrorPanel(string message)
    {
        var seconds = 3;
        while (seconds != 0)
        {
            errorPanel.GetComponentInChildren<TextMeshProUGUI>().text = message;
            errorPanel.SetActive(true);
            yield return new WaitForSeconds(1);
            seconds--;
        }
        errorPanel.SetActive(false);
    }

    private void PlayerReadyClick()
    {
        Debug.Log("Player Ready");
        _isReady = !_isReady;
        NetworkStaticManager.ClientHandle.SendPlayerReadyPacket(_intRoomId, _isReady);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!string.IsNullOrEmpty(textBox.text.Trim()))
            {
                
                SendChatMessage();
            }
        }
    }

    void OnItemClick(int itemNo)
    {
        SelectItemPopup.SetActive(true);
    }
    

    void OnButtonBackClick()
    {
        if (_isReady)
        {
            StartCoroutine(ShowErrorPanel("You can't go out because you are ready!"));
            return;
        }
        NetworkStaticManager.ClientHandle.SendExitRoomPacket(_intRoomId);
        SceneManager.LoadScene("Select Play Mode");
    }

    private void SetRoomID(string roomID)
    {
        RoomId.text = "RoomID: "+roomID;
    }

    private void SetRoomName(string roomName)
    {
        RoomName.text = "Room name: " + roomName;
    }
    
    public void LoadMap() { SceneManager.LoadScene("Normal Mode Map"); }


    public void PasteRoomInfo(RoomPacket room)
    {
        _intRoomId = room.Id;
        SetRoomID(_intRoomId + "");
        SetRoomName(room.Name);
        roomModeUI.SetBannerRoomMode(room.roomMode);
        SetRoomType(room.roomType);
    }

    private void SetRoomType(RoomType rT)
    {
        switch (rT)
        {
            case RoomType.OneVsOne:
                roomType.text = "1 Vs 1";
                break;
            case RoomType.TwoVsTwo:
                roomType.text = "2 Vs 2";
                break;
            case RoomType.FourVsFour:
                roomType.text = "4 Vs 4";
                break;
        }
    }

    private void SendChatMessage()
    {
        Debug.Log($"Username: {NetworkStaticManager.ClientHandle.GetUsername()}, message: {textBox.text}");
        NetworkStaticManager.ClientHandle.SendChatMessagePacket(textBox.text, _intRoomId);
        textBox.text = "";
        textBox.ActivateInputField();
    }

    public void ReceiveChatMessage(string username, string displayName, string msg)
    {
        GameObject newMessage = Instantiate(prefab, chatPanelZone);
        TextMeshProUGUI textComponent = newMessage.GetComponent<TextMeshProUGUI>();

        
        if (string.Equals(username, NetworkStaticManager.ClientHandle.GetUsername()))
        {
            Debug.Log("Set message color");
            textComponent.color = Color.yellow;
            MsgLastest.color = Color.yellow;
        }
        else
        {
            textComponent.color = Color.white;
            MsgLastest.color = Color.white;
        }
        textComponent.text = displayName + ": "+msg;
        MsgLastest.text = displayName + ": "+msg;
        Canvas.ForceUpdateCanvases();
        newMessage.transform.SetAsLastSibling();
        
    }
    public void SetUIForAll(List<PlayerInRoomPacket> players)
    {
        HideAllPlayers(playerList);
        
        foreach (var player in players)
        {
            Debug.Log($"Player Team: {player.team}, Player Position: {player.Position}");
            SetPlayerDetails(playerList[player.Position - 1].GetComponent<PlayerInWaitingRoom>(), player);
            playerList[player.Position - 1 ].SetActive(true);
        }
    }

    private void HideAllPlayers(GameObject[] playerList)
    {
        foreach (var player in playerList)
        {
            player.SetActive(false);
        }
    }

    private void SetPlayerDetails(PlayerInWaitingRoom ui, PlayerInRoomPacket data)
    {
        ui.username = data.username;
        if (ui.username == NetworkStaticManager.ClientHandle.GetUsername())
        {
            ui.PlayerName.color = Color.yellow;
            hostPanel.SetActive(data.isHost);
            guestPanel.SetActive(!data.isHost);
            _isReady = data.isReady;
            if (data.isReady)
            {
                ReadyButton.GetComponent<Image>().color = new Color32(159, 30, 20, 255);
                ReadyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Not Ready";
            }
            else
            {
                ReadyButton.GetComponent<Image>().color = new Color32(0, 135, 10, 255);
                ReadyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
            }
        }
        
        ui.isHost.SetActive(data.isHost);
        ui.PlayerName.text = data.displayname;
        ui.isReady.SetActive(data.isReady);
        
    }
    /*public async void SendStartGameInBattle(GameStartPacket packet)

    {
        GameStartPacket same = packet;

        await Task.Delay(2000);
        if (packet.isHost && packet.username == NetworkStaticManager.ClientHandle.GetUsername()) { NetworkStaticManager.ClientHandle.StartGameInBattle(); }
        else
        {
            Debug.Log(NetworkStaticManager.ClientHandle.GetUsername() + " " + packet.username + " " + packet.isHost);
        }
    }*/
    /*public void SendStartGame()
    {
        NetworkStaticManager.ClientHandle.StartGame();
    }*/
    public void ReceiveStartGame(SpawnPlayerPacketToAll packet)
    {
        positionsData.spawnPlayerPackets = packet.SPPacket;
        LoadMap();
    }
}

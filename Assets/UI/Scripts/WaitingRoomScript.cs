using System.Collections.Generic;
using NetworkThread;
using NetworkThread.Multiplayer;
using NetworkThread.Multiplayer.Packets;
using RoomEnum;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class WaitingRoomScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Button buttonBack;
    [Header("Room Info")]
    public TextMeshProUGUI RoomId;
    private int _intRoomId;
    public TextMeshProUGUI RoomName;
    public TextMeshProUGUI roomType;
    public RoomModeUI roomModeUI;
    public RoomMode roomMode; 
    
    [Header("Team 1")]
    public GameObject Player1;
    public GameObject Player2;
    public GameObject Player3;
    public GameObject Player4;
    
    [Header("Team 2")]
    public GameObject Player5;
    public GameObject Player6;
    public GameObject Player7;
    public GameObject Player8;
    private List<GameObject> playerList = new List<GameObject>();
    
    [Header("ChatZone")]
    public RectTransform chatPanelZone;
    public GameObject prefab;
    public TextMeshProUGUI MsgLastest;
    public TMP_InputField textBox;
    
    private GameObject _Me;

    void Awake()
    {
        playerList.Add(Player1);
        playerList.Add(Player2);
        playerList.Add(Player3);
        playerList.Add(Player4);
        playerList.Add(Player5);
        playerList.Add(Player6);
        playerList.Add(Player7);
        playerList.Add(Player8);
        
        foreach (var player in playerList)
        {
            player.SetActive(false);
        }
    }
    void Start()
    {
        NetworkStaticManager.ClientHandle.SetUiScripts(this);
        NetworkStaticManager.ClientHandle.SendJoinRoomPacket(RoomModeTransfer.RoomMode);
        buttonBack.onClick.AddListener(OnButtonBackClick);
        
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
    

    void OnButtonBackClick()
    {
        NetworkStaticManager.ClientHandle.SendExitRoomPacket(_intRoomId);
        SceneManager.LoadScene("Select Play Mode");
    }

    public void SetRoomID(string roomID)
    {
        RoomId.text = "RoomID: "+roomID;
    }

    public void SetRoomName(string roomName)
    {
        RoomName.text = "Room name: " + roomName;
    }


    public void PasteRoomInfo(JoinRoomPacket packet)
    {
        _intRoomId = packet.room.Id;
        SetRoomID(_intRoomId + "");
        SetRoomName(packet.room.Name);
        roomModeUI.SetBannerRoomMode(packet.room.roomMode);
        SetRoomType(packet.room.roomType);
        
    }

    public void SetRoomType(RoomType rT)
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
        textComponent.text = displayName + ": "+msg;
        MsgLastest.text = displayName + ": "+msg;
        Canvas.ForceUpdateCanvases();
        newMessage.transform.SetAsLastSibling();
        
    }

    /*public void PasteMyChracterInfo(JoinRoomPacket packet)
    {
        if (packet.team == Team.Team1)
        {
            _Me = player1List[packet.position - 1];
        }
        else
        {
            _Me = player2List[packet.position - 1];
        }

        Debug.Log($"Team : {packet.team}, position : {packet.position}");
        _Me.SetActive(true);
        
        UpdateMyChracterUI(_Me.GetComponent<PlayerInWaitingRoom>(), packet);
    }

    private void UpdateMyChracterUI(PlayerInWaitingRoom player, JoinRoomPacket packet)
    {
        player.PlayerName.text = packet.displayName;
        player.isHost.SetActive(packet.isHost);
    }*/
    public void SetUIForAll(JoinRoomPacketToAll packet)
    {
        HideAllPlayers(playerList);
        
        foreach (var player in packet.Players)
        {
            Debug.Log($"Player Team: {player.team}, Player Position: {player.Position}");
            playerList[player.Position - 1 ].SetActive(true);
            SetPlayerDetails(playerList[player.Position - 1].GetComponent<PlayerInWaitingRoom>(), player);
            
        }
    }

    private void HideAllPlayers(List<GameObject> playerList)
    {
        foreach (var player in playerList)
        {
            player.SetActive(false);
        }
    }

    private void SetPlayerDetails(PlayerInWaitingRoom ui, PlayerInRoomPacket data)
    {
        ui.username = data.username;
        if(ui.username == NetworkStaticManager.ClientHandle.GetUsername()) ui.PlayerName.color = Color.yellow;
        ui.isHost.SetActive(data.isHost);
        ui.PlayerName.text = data.displayname;
        ui.isReady.SetActive(data.isReady);
    }

    
}

using System.Collections.Generic;
using NetworkThread;
using NetworkThread.Multiplayer;
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
    private List<GameObject> player1List = new List<GameObject>();
    
    [Header("Team 2")]
    public GameObject Player5;
    public GameObject Player6;
    public GameObject Player7;
    public GameObject Player8;
    private List<GameObject> player2List = new List<GameObject>();
    
    private GameObject _Me;

    void Awake()
    {
        player1List.Add(Player1);
        player1List.Add(Player2);
        player1List.Add(Player3);
        player1List.Add(Player4);
        player2List.Add(Player5);
        player2List.Add(Player6);
        player2List.Add(Player7);
        player2List.Add(Player8);
        
        foreach (var player in player1List)
        {
            player.SetActive(false);
        }

        foreach (var player in player2List)
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
        _intRoomId = packet.roomId;
        SetRoomID(_intRoomId + "");
        SetRoomName(packet.roomName);
        roomModeUI.SetBannerRoomMode(packet.roomMode);
        SetRoomType(packet.roomType);
        
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

    public void PasteMyChracterInfo(JoinRoomPacket packet)
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
    }
}

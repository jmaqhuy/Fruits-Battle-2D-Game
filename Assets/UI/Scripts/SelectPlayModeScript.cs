using System.Collections.Generic;
using DataTransfer;
using NetworkThread;
using NetworkThread.Multiplayer;
using NetworkThread.Multiplayer.Packets;
using RoomEnum;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectPlayModeScript : MonoBehaviour
{
    public RoomData roomData;
    public RoomsData roomsData;
    public Button ShowRoomsButton;
    public Button ReloadRoomListButton;
    
    [Header("Room List Panel")]
    public GameObject RoomListPanel;
    public RectTransform position;
    public GameObject roomInfoPrefab;
    private List<GameObject> _roomInfos = new List<GameObject>();
    
    
    [Header("Filter")]
    public Button FindButton;
    public TMP_InputField roomIdFilter;
    public TMPro.TMP_Dropdown RoomModeDropdown;
    public TMPro.TMP_Dropdown RoomTypeDropdown;

    private bool _inProcess = false;
    private void Awake()
    {
        NetworkStaticManager.ClientHandle.SetUiScripts(this);
        ShowRoomsButton.onClick.AddListener(ShowRoomList);
        ReloadRoomListButton.onClick.AddListener(ShowRoomList);
        RoomTypeDropdown.onValueChanged.AddListener(UpdateRoomList);
        RoomModeDropdown.onValueChanged.AddListener(UpdateRoomList);
        FindButton.onClick.AddListener(FindRoomById);
    }
    
    private void FindRoomById()
    {
        UpdateRoomList(0);
    }
    private void ShowRoomList()
    {
        if (_inProcess) return;
        foreach (var r in _roomInfos)
        {
            Destroy(r);
        }
        _roomInfos.Clear();
        roomIdFilter.text = "";
        NetworkStaticManager.ClientHandle.SendRoomListPacket();
        _inProcess = true;
    }

    public void ParseRoomList(RoomListPacket roomListPacket)
    {
        _inProcess = false;
        foreach (var r in roomListPacket.rooms)
        {
            var newButton = Instantiate(roomInfoPrefab, position);
            var buttonScript = newButton.GetComponent<RoomInfoPrefabScript>();
            buttonScript.InitializeRoom(
                roomIdInt: r.Id,
                roomNameText: r.Name,
                mode: r.roomMode,
                type: r.roomType,
                currentPlayers: r.PlayerNumber,
                rStatus: r.roomStatus);
            
            Button uiButton = newButton.GetComponent<Button>();
            uiButton.onClick.AddListener(buttonScript.SendJoinRoomPacket);
            newButton.SetActive(true);
            _roomInfos.Add(newButton);
        }
        
        UpdateRoomList(0);
        RoomListPanel.SetActive(true);
    }

    private void UpdateRoomList(int value)
    {
        int tmp;
        if (RoomTypeDropdown.value == 0)
        {
            tmp = 0;
        } else if (RoomTypeDropdown.value == 1)
        {
            tmp = 2;
        } else if (RoomTypeDropdown.value == 2)
        {
            tmp = 4;
        }
        else
        {
            tmp = 8;
        }
        foreach (var r in _roomInfos)
        {
            r.SetActive(false);
            var rs = r.GetComponent<RoomInfoPrefabScript>();
            if (!string.IsNullOrEmpty(roomIdFilter.text))
            {
                if (roomIdFilter.text != rs.roomId.text)
                {
                    continue;
                }
            }
            if (RoomModeDropdown.value == 0 && RoomTypeDropdown.value == 0)
            {
                r.SetActive(true);
            }
            else if (RoomModeDropdown.value != 0 && RoomTypeDropdown.value != 0)
            {
                r.SetActive(rs.GetRoomMode() == (RoomMode)RoomModeDropdown.value
                            && (int)rs.GetRoomType() == tmp);
            }
            else if (RoomModeDropdown.value == 0)
            {
                r.SetActive((int)rs.GetRoomType() == tmp);
            }
            else
            {
                r.SetActive(rs.GetRoomMode() == (RoomMode)RoomModeDropdown.value);
            }
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void NormalModeSelected()
    {
        if (_inProcess) return;
        NetworkStaticManager.ClientHandle.SendJoinRoomPacket(RoomMode.Normal, RoomType.TwoVsTwo);
        _inProcess = true;
    }

    public void ParseRoomInfoData(RoomPacket roomPacket)
    {
        Debug.Log("Get roomPacket");
        roomData.RoomPacket = roomPacket;
        try
        {
            if (roomData.RoomPacket != null && roomData.PlayersInRoom.Count > 0)
            {
                if (roomData.RoomPacket.roomMode == RoomMode.Normal)
                {
                    SceneManager.LoadScene("Waiting Room");
                }
                else if (roomData.RoomPacket.roomMode == RoomMode.Rank)
                {
                    SceneManager.LoadScene("Rank");
                }
            }
        }
        finally
        {
            
        }
    }

    public void ParsePlayerInRoomData(JoinRoomPacketToAll packet)
    {
        Debug.Log("Get JoinRoomPacketToAll");
        roomData.PlayersInRoom = packet.Players;
        try
        {
            if (roomData.RoomPacket != null && roomData.PlayersInRoom.Count > 0)
            {
                if (roomData.RoomPacket.roomMode == RoomMode.Normal)
                {
                    SceneManager.LoadScene("Waiting Room");
                }
                else if (roomData.RoomPacket.roomMode == RoomMode.Rank)
                {
                    SceneManager.LoadScene("Rank");
                }
            }
        }
        finally
        {
            
        }
    }
    public void RankModeSelected()
    {
        NetworkStaticManager.ClientHandle.CreateNewRoom(RoomMode.Rank, RoomType.OneVsOne);
    }
}

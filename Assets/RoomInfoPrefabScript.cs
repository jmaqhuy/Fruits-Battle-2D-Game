using System.Collections;
using System.Collections.Generic;
using NetworkThread;
using RoomEnum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomInfoPrefabScript : MonoBehaviour
{
    private int _intRoomId;
    private RoomMode _roomMode;
    private RoomType _roomType;
    private RoomStatus _roomStatus;
    
    public TextMeshProUGUI roomId;
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI roomMode;
    public TextMeshProUGUI roomType;
    public TextMeshProUGUI players;
    public Image status;

    private void SetStatus(bool roomStatus)
    {
        status.color = roomStatus ? Color.green : Color.red;
    }

    public void InitializeRoom(int roomIdInt, string roomNameText, RoomMode mode, RoomType type, int currentPlayers, RoomStatus rStatus)
    {
        _intRoomId = roomIdInt;
        _roomMode = mode;
        _roomType = type;
        _roomStatus = rStatus;

        
        this.roomId.text = _intRoomId.ToString();
        roomName.text = roomNameText;
        roomMode.text = mode.ToString();
        roomType.text = $"{(int)type/2}vs{(int)type/2}";
        players.text = $"{currentPlayers}/{(int)type}";
        SetStatus(rStatus == RoomStatus.InLobby);
    }

    public void SendJoinRoomPacket()
    {
        NetworkStaticManager.ClientHandle.SendJoinRoomPacket(_roomMode, _roomType, _intRoomId);
    }

    public RoomMode GetRoomMode()
    {
        return _roomMode;
    }

    public RoomType GetRoomType()
    {
        return _roomType;
    }
}

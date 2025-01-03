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
    private int _currentPlayers;
    
    public TextMeshProUGUI roomId;
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI roomMode;
    public TextMeshProUGUI roomType;
    public TextMeshProUGUI players;
    public Image status;
    
    private GameObject _errorPanel;
    private TextMeshProUGUI _errorText;

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
        _currentPlayers = currentPlayers;

        
        roomId.text = _intRoomId.ToString();
        roomName.text = roomNameText;
        roomMode.text = mode.ToString();
        roomType.text = $"{(int)type/2}vs{(int)type/2}";
        players.text = _roomMode != RoomMode.Rank ? $"{currentPlayers}/{(int)type}" : $"{currentPlayers}/{(int)type/2}";
        SetStatus(rStatus == RoomStatus.InLobby);
        FindErrorPanelAndText();
    }

    public void SendJoinRoomPacket()
    {
        if (_roomMode == RoomMode.Rank && _currentPlayers == (int)_roomType / 2 ||
            _roomMode != RoomMode.Rank && _currentPlayers == (int)_roomType)
        {
            StartCoroutine(ShowErrorPanel($"Room {_intRoomId} is full"));
            return;
        }
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
    IEnumerator ShowErrorPanel(string errorMessage)
    {
        _errorText.text = errorMessage;
        _errorPanel.SetActive(true);
        var time = 0;
        while (time++ < 3)
        {
            yield return new WaitForSeconds(1);
        }
        _errorPanel.SetActive(false);
    }
    void FindErrorPanelAndText()
    {
        Transform rootTransform = gameObject.transform.root;

        Transform errorPanelTransform = rootTransform.Find("ErrorPanel");
        if (errorPanelTransform != null)
        {
            _errorPanel = errorPanelTransform.gameObject;
            Transform errorTextTransform = errorPanelTransform.Find("ErrorText");
            if (errorTextTransform != null)
            {
                _errorText = errorTextTransform.gameObject.GetComponent<TextMeshProUGUI>();
            }
        }
    }
}

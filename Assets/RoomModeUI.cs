using System;
using System.Collections;
using System.Collections.Generic;
using RoomEnum;
using UnityEngine;

public class RoomModeUI : MonoBehaviour
{
    public GameObject botArea;
    public GameObject normal;
    
    private List<GameObject> _rooms = new List<GameObject>();

    void Awake()
    {
        _rooms.Add(botArea);
        _rooms.Add(normal);
    }

    public void SetBannerRoomMode(RoomMode roomMode)
    {
        switch (roomMode)
        {
            case RoomMode.Normal:
                ShowNormalMode();
                break;
            case RoomMode.BotArea:
                ShowBotAreaMode();
                break;
        }
    }
    public void ShowNormalMode()
    {
        HideAllModes();
        normal.SetActive(true);
    }

    public void ShowBotAreaMode()
    {
        HideAllModes();
        botArea.SetActive(true);
    }

    private void HideAllModes()
    {
        foreach (var room in _rooms)
        {
            room.SetActive(false);
        }
    }
}

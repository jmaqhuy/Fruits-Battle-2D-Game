using System.Collections;
using System.Collections.Generic;
using DataTransfer;
using NetworkThread;
using TMPro;
using UnityEngine;

public class ChangePlayerName : MonoBehaviour
{
    public UserData userData;
    public TMP_Text displayName;
    public TMP_InputField newPlayerName;

    public void changeName()
    {
        NetworkStaticManager.ClientHandle.SendChangeDisplayNamePacket(newPlayerName.text);
        SetDisplayName();
        userData.UserInfo.displayName = newPlayerName.text;
        
    }

    private void SetDisplayName()
    {
        displayName.text = newPlayerName.text;
    }
}

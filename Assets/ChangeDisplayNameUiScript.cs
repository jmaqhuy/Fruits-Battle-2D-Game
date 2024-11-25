using System;
using System.Collections;
using System.Collections.Generic;
using NetworkThread;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeDisplayNameUiScript : MonoBehaviour
{
    public Button closeButton;
    public TMP_InputField newDisplayName;
    public Button acceptButton;

    private void Start()
    {
        acceptButton.onClick.AddListener(OnChangeDisplayNameButtonClicked);
    }

    private void OnChangeDisplayNameButtonClicked()
    {
        NetworkStaticManager.ClientHandle.SendChangeDisplayNamePacket(newDisplayName.text);
    }
}

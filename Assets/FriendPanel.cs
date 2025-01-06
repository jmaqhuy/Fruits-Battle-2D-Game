using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FriendPanel : MonoBehaviour
{
    public Button FriendAvatar;
    public TextMeshProUGUI FriendDisplayName;
    public string FriendUsername;
    public GameObject FriendOnline;
    public GameObject FriendOffline;
    public Button InviteFriendButton;

    public void SetFriendOnline(bool online)
    {
        if (online)
        {
            FriendOnline.SetActive(true);
            FriendOffline.SetActive(false);
            InviteFriendButton.interactable = true;
        }
        else
        {
            FriendOnline.SetActive(false);
            FriendOffline.SetActive(true);
            InviteFriendButton.interactable = false;
        }
    }
}

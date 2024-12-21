using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendInFriendList : MonoBehaviour
{
    public TextMeshProUGUI DisplayName;
    private string username;
    public Button ShowInfoButton;
    public Button DeleteFriendButton;
    public Button ConfirmFriendButton;
    public Button CancelRequestButton;
    public Button AddFriendButton;
    public Button UnlockFriendButton;

    public void SetUserName(string username)
    {
        this.username = username;
    }
    public string GetUserName()
    {
        return this.username;
    }
}

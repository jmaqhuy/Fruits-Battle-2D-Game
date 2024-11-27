using System.Collections;
using NetworkThread;
using NetworkThread.Multiplayer.Packets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendInviteUI : MonoBehaviour
{
    public GameObject friendInviteUI;
    public TextMeshProUGUI FriendName;
    public TextMeshProUGUI ModeandRoomType;
    public RoomPacket roomPacket;
    public Button AcceptButton;
    public Button DeclineButton;

    private bool isButtonClicked = false;

    void Awake()
    {
        NetworkStaticManager.ClientHandle.SetInvitePopupScripts(this);
    }
    void Start()
    {
        AcceptButton.onClick.AddListener(OnAcceptButtonClick);
        DeclineButton.onClick.AddListener(OnDeclineButtonClick);
    }

    private void OnDeclineButtonClick()
    {
        isButtonClicked = true;
        HidePanelDirectly();
    }

    private void OnAcceptButtonClick()
    {
        isButtonClicked = true;
        HidePanelDirectly();
    }
    
    public void ShowInvite()
    {
        friendInviteUI.SetActive(true);
        isButtonClicked = false;
        StartCoroutine(HidePanelAfterDelay(5f));
    }

    private IEnumerator HidePanelAfterDelay(float delay)
    {
        float timer = 0f;

        while (timer < delay)
        {
            if (isButtonClicked)
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }

        HidePanelDirectly();
    }

    private void HidePanelDirectly()
    {
        friendInviteUI.SetActive(false);
    }
}
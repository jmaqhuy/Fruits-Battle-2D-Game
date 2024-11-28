using UnityEngine;

public class ChatPanelWaitingRoom : MonoBehaviour
{
    public GameObject quickChat;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSelf();
        }
    }
    public void ToggleSelf()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        quickChat.SetActive(!gameObject.activeSelf);
    }
}

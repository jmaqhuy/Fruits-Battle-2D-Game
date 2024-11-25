using NetworkThread;
using RoomEnum;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectPlayModeScript : MonoBehaviour
{
    private void Awake()
    {
        NetworkStaticManager.ClientHandle.SetUiScripts(this);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void NormalModeSelected()
    {
        RoomModeTransfer.RoomMode = RoomMode.Normal;
        JoinNormalModeSuccess();
    }

    public void JoinNormalModeSuccess()
    {
        SceneManager.LoadScene("Waiting Room");
    }
}

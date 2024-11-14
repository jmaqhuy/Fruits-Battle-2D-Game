using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectPlayModeScript : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void BotAreaModeSelected()
    {
        SceneManager.LoadScene("Waiting Room");
    }
}

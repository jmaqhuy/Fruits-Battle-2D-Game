using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScript : MonoBehaviour
{
    public void LoginSuccessful()
    {
        SceneManager.LoadScene("Main Menu");
    }
}

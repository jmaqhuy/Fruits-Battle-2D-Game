using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreenScript : MonoBehaviour
{
    [SerializeField] private confirmationWindow myConfirmationWindow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowConfirmationWindow();
        }
        
        myConfirmationWindow.yesButton.onClick.AddListener(Application.Quit);
        myConfirmationWindow.noButton.onClick.AddListener(ShowConfirmationWindow);
        
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Main Menu");
        }
        
        
        
        
    }

    private void ShowConfirmationWindow()
    {
        myConfirmationWindow.gameObject.SetActive(!myConfirmationWindow.gameObject.activeSelf);
    }
    
}

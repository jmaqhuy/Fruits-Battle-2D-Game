using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    [SerializeField] private confirmationWindow myConfirmationWindow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowConfirmationWindow();
        }
        
        myConfirmationWindow.yesButton.onClick.AddListener(Application.Quit);
        myConfirmationWindow.noButton.onClick.AddListener(ShowConfirmationWindow);
    }
    
    private void ShowConfirmationWindow()
    {
        myConfirmationWindow.gameObject.SetActive(!myConfirmationWindow.gameObject.activeSelf);
    }
}
